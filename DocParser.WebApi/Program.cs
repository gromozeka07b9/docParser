using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using DocParser.WebApi;
using DocParser.WebApi.Commands;
using DocParser.WebApi.Infrastructure;
using DocParser.WebApi.Integration;
using DocParser.WebApi.Models;
using DocParser.WebApi.Queries;
using Hangfire;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Hangfire.Storage.SQLite;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

GlobalConfiguration.Configuration
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage();

builder.Services.AddHangfire(options =>
{
    options.UseSQLiteStorage();
});

builder.Services.AddHangfireServer(options =>
{
    options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
});

var gptSettings = builder.Configuration.GetSection("GptSettings").Get<GptSettings>()!;
builder.Services.AddSingleton(gptSettings);

var mainMailBoxSettings = builder.Configuration.GetSection("MainMailBoxSettings").Get<MainMailBoxSettings>()!;
builder.Services.AddSingleton(mainMailBoxSettings);

builder.Services.AddTransient<ProcessAirplaneTicketCommand>();
builder.Services.AddTransient<ProcessMainMailBoxCommand>();
builder.Services.AddTransient<SendReportCommand>();
builder.Services.AddTransient<GetDocumentTypeQuery>();

GptQueries gptQueries = new GptQueries(gptSettings);
builder.Services.AddSingleton(gptQueries);

builder.Services.AddTransient<DocumentTypeDetector>();

PdfParser pdfParser = new PdfParser();
builder.Services.AddSingleton(pdfParser);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddScoped<ApplicationDbRepository>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    IsReadOnlyFunc = (DashboardContext context) => true
});
app.MapHangfireDashboard();
RecurringJob.AddOrUpdate<ProcessMainMailBoxCommand>("processCommonMailbox", x => x.Execute(10), Cron.Minutely);

app.MapPost("/mail", async (HttpRequest request, ApplicationDbContext dbContext, IServiceScopeFactory scopeFactory) =>
{
    await ApplyMigrationsIfNeededAsync(dbContext).ConfigureAwait(true);
    var processMainMailBoxCommand = app.Services.GetRequiredService<ProcessMainMailBoxCommand>();
    var result = processMainMailBoxCommand.Execute(1);
    return Results.Text(result.Result.Successed ? "processed" : result.Result.Error);

}).WithName("Prompts").WithOpenApi().DisableAntiforgery();


app.MapPost("/parse", async (HttpRequest request, ApplicationDbContext dbContext, IServiceScopeFactory scopeFactory) =>
{
    await ApplyMigrationsIfNeededAsync(dbContext).ConfigureAwait(true);

    if (!request.HasFormContentType)
        return Results.BadRequest();
    var form = await request.ReadFormAsync();
    if (!form.Files.Any()) 
        return Results.BadRequest();
    var rawFile = form.Files.SingleOrDefault();
    
    var filePath = await SaveToTempFile(rawFile);

    QueryResult<DocumentTypes> documentTypeResult;
    using (var scope = scopeFactory.CreateScope())
    {
        var documentTypeDetector = scope.ServiceProvider.GetService<DocumentTypeDetector>() ?? throw new ArgumentNullException($"{nameof(DocumentTypeDetector)} can't be undefined.");
        
        documentTypeResult = await documentTypeDetector.DetectTypeAsync(filePath);
        if (!documentTypeResult.Succeeded)
        {
            return Results.Text(documentTypeResult.Error);
        }
        
    }        
    
    switch (documentTypeResult.Data)
    {
        case DocumentTypes.AirplaneTicket:
        {
            var addAirplaneTicketCommand = app.Services.GetRequiredService<ProcessAirplaneTicketCommand>();
            var result = await addAirplaneTicketCommand.Execute("default", filePath);
            return Results.Text(result.Successed ? "successfully processed" : result.Error);
        };
        default:
        {
            return Results.Text("Don't work with type:" + documentTypeResult.Data);
        }
    }
}).WithName("Parse").WithOpenApi().DisableAntiforgery().Accepts<IFormFile>("multipart/form-data");

app.Run();

async Task<string> SaveToTempFile(IFormFile? formFile)
{
    var tempFileName = Path.GetTempFileName();
    await using (var stream = File.Create(tempFileName))
    {
        await formFile!.CopyToAsync(stream);
    }

    return tempFileName;
}

async Task ApplyMigrationsIfNeededAsync(ApplicationDbContext dbContext)
{
    var pendingMigrations = (await dbContext!.Database.GetPendingMigrationsAsync().ConfigureAwait(false)).ToList();
    if(pendingMigrations.Count > 0)
    {
        Console.WriteLine($"Have a {pendingMigrations.Count} migrations to apply: {string.Join(", ", pendingMigrations)}.");
        Console.WriteLine($"Apply migrations...");
        await dbContext.Database.MigrateAsync().ConfigureAwait(false);
        Console.WriteLine($"Migrations applied.");
    }
}