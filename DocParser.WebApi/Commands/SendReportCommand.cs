using System.Text;
using DocParser.WebApi.Infrastructure;
using DocParser.WebApi.Integration;
using DocParser.WebApi.Models;
using DocParser.WebApi.Queries;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using FolderAccess = MailKit.FolderAccess;
using MessageSummaryItems = MailKit.MessageSummaryItems;

namespace DocParser.WebApi.Commands;

public class SendReportCommand: CommonCommand<string>
{
    private readonly MainMailBoxSettings mainMailBoxSettings;

    public SendReportCommand(MainMailBoxSettings mainMailBoxSettings, IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        this.mainMailBoxSettings = mainMailBoxSettings;
    }

    public override async Task<CommandResult> Execute(string userEmail)
    {
        if (string.IsNullOrEmpty(userEmail)) throw new Exception("userEmail is null");
        
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse("mytravelhelper@yandex.ru"));
        email.To.Add(MailboxAddress.Parse(userEmail));
        email.Subject = "Отчёт по вашим документам";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<h1>Обработанные документы:</h1>");
        sb.AppendLine("<table border=1>");
        sb.AppendLine("<tr>");
        sb.AppendLine("<th>Дата отправления</th>");
        sb.AppendLine("<th>Дата прибытия</th>");
        sb.AppendLine("<th>Авиакомпания</th>");
        sb.AppendLine("<th>Номер рейса</th>");
        sb.AppendLine("<th>Вылет</th>");
        sb.AppendLine("<th>Прибытие</th>");
        sb.AppendLine("</tr>");


        using (var scope = scopeFactory.CreateScope())
        {
            var applicationDbRepository = scope.ServiceProvider.GetService<ApplicationDbRepository>() ?? throw new ArgumentNullException($"{nameof(ApplicationDbRepository)} can't be undefined.");
            var tickets = await applicationDbRepository.GetAirplaneTickets(userEmail);
            foreach (var ticket in tickets)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{ticket.TimelineStartDateTime}</td>");
                sb.AppendLine($"<td>{ticket.TimelineEndDateTime}</td>");
                sb.AppendLine($"<td>{ticket.AirlineNameByIata}</td>");
                sb.AppendLine($"<td>{ticket.FlightNumber}</td>");
                sb.AppendLine($"<td>{ticket.DepartureCountry} {ticket.DepartureCity} {ticket.DepartureTerminal} ({ticket.DepartureIata})</td>");
                sb.AppendLine($"<td>{ticket.ArrivalCountry} {ticket.ArrivalCity} {ticket.ArrivalTerminal} ({ticket.ArrivalIata})</td>");
                sb.AppendLine("</tr>");
            }
        }
        sb.AppendLine("</table>");

        email.Body = new TextPart(TextFormat.Html)
        {
            Text = sb.ToString()
        };
        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("smtp.yandex.ru", 465, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(mainMailBoxSettings.Username, mainMailBoxSettings.Password);
            await client.SendAsync(email);
            await client.DisconnectAsync(true, CancellationToken.None);
        }
        /*using (var client = new ImapClient())
        {
            await client.ConnectAsync(mainMailBoxSettings.Host, mainMailBoxSettings.Port, mainMailBoxSettings.UseSsl);

            await client.AuthenticateAsync(mainMailBoxSettings.Username, mainMailBoxSettings.Password);

            var dictionaryForSentReport = new Dictionary<string, int>();
            var backupFolder = await client.GetFolderAsync("backups");
            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadWrite);
            var messages = await inbox.SearchAsync(SearchQuery.DeliveredAfter(DateTime.Today.AddDays(-depthInDays)));
            foreach (var messageItem in messages)
            {
                var message = await inbox.GetMessageAsync(messageItem);
                string userIdentity = message.From?.Count > 1 ? message.Sender?.Address : (message.From?.SingleOrDefault() as MailboxAddress).Address;
                if(string.IsNullOrEmpty(userIdentity)) continue;
                
                Console.WriteLine ("Message: date:{0} subject: {1} attachments: {2}", message.Date, message.Subject, message.Attachments.Any());
                dictionaryForSentReport.TryAdd(userIdentity, 0);
                
                if (message.Attachments.Any())
                {
                    using (var scope = scopeFactory.CreateScope())
                    {
                        foreach (var attachment in message.Attachments)
                        {
                            string fileName = String.Empty;
                            if (attachment is MessagePart) {
                                fileName = attachment.ContentDisposition?.FileName ?? String.Empty;
                                if(!fileName.Contains(".pdf")) continue;
                                
                                var rfc822 = (MessagePart) attachment;

                                if (string.IsNullOrEmpty (fileName))
                                    fileName = "attached-message.eml";

                                using (var stream = File.Create(fileName))
                                {
                                    await rfc822.Message.WriteToAsync (stream);
                                }
                            } else {
                                var part = (MimePart) attachment;
                                fileName = part.FileName;
                                if(!fileName.Contains(".pdf")) continue;

                                using (var stream = File.Create(fileName))
                                {
                                    await part.Content.DecodeToAsync (stream);
                                }
                            }
                            
                            QueryResult<DocumentTypes> documentTypeResult;
                            var documentTypeDetector = scope.ServiceProvider.GetService<DocumentTypeDetector>() ?? throw new ArgumentNullException($"{nameof(DocumentTypeDetector)} can't be undefined.");
        
                            documentTypeResult = await documentTypeDetector.DetectTypeAsync(fileName);
                            Console.WriteLine("type detected successed: {0} type:{1}", documentTypeResult.Succeeded, documentTypeResult.Data);
                            
                            switch (documentTypeResult.Data)
                            {
                                case DocumentTypes.AirplaneTicket:
                                {
                                    var addAirplaneTicketCommand = scope.ServiceProvider.GetService<ProcessAirplaneTicketCommand>() ?? throw new ArgumentNullException($"{nameof(ProcessAirplaneTicketCommand)} can't be undefined.");
                                    var result = await addAirplaneTicketCommand.Execute(userIdentity, fileName);
                                    Console.WriteLine("processing result: {0}", result.Error);
                                };break;
                                default:
                                {
                                    Console.WriteLine("Don't work with type: {0}", documentTypeResult.Data);
                                };break;
                            }
                        }
                    }        
                }
                await DeleteMessageAndPurgeFolderAsync(inbox, backupFolder,  new UniqueId(messageItem.Id));

                foreach (var userEmailItem in dictionaryForSentReport)
                {
                    
                }
            }
            await client.DisconnectAsync(true);
        }*/
        return new CommandResult();
    }
}