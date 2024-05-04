using DocParser.WebApi.Integration;
using DocParser.WebApi.Models;
using DocParser.WebApi.Queries;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using FolderAccess = MailKit.FolderAccess;
using MessageSummaryItems = MailKit.MessageSummaryItems;

namespace DocParser.WebApi.Commands;

public class ProcessMainMailBoxCommand: CommonCommand<int>
{
    private readonly MainMailBoxSettings mainMailBoxSettings;

    public ProcessMainMailBoxCommand(MainMailBoxSettings mainMailBoxSettings, IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        this.mainMailBoxSettings = mainMailBoxSettings;
    }

    public override async Task<CommandResult> Execute(int depthInDays)
    {
        using (var client = new ImapClient())
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

            }
            await client.DisconnectAsync(true);

            using (var scope = scopeFactory.CreateScope())
            {
                foreach (var userEmailItem in dictionaryForSentReport)
                {
                    var sendReportCommand = scope.ServiceProvider.GetService<SendReportCommand>() ?? throw new ArgumentNullException($"{nameof(SendReportCommand)} can't be undefined.");
                    var result = await sendReportCommand.Execute(userEmailItem.Key);
                    Console.WriteLine("result send mail:" + result);
                }
            }
        }
        return new CommandResult();
    }

    async Task DeleteMessageAndPurgeFolderAsync(IMailFolder inputFolder, IMailFolder backupFolder, UniqueId messageId)
    {
        var listIds = new List<UniqueId>();
        listIds.Add(messageId);
        await inputFolder.CopyToAsync(messageId, backupFolder, CancellationToken.None);
        await inputFolder.AddFlagsAsync(messageId, MessageFlags.Deleted, true);
        await inputFolder.ExpungeAsync(listIds);
    }
    
    void SaveAttachments (MimeMessage message)
    {
        #region SaveAttachments
        foreach (var attachment in message.Attachments) {
            if (attachment is MessagePart) {
                var fileName = attachment.ContentDisposition?.FileName;
                var rfc822 = (MessagePart) attachment;

                if (string.IsNullOrEmpty (fileName))
                    fileName = "attached-message.eml";

                using (var stream = File.Create(fileName))
                {
                    rfc822.Message.WriteTo (stream);
                }
            } else {
                var part = (MimePart) attachment;
                var fileName = part.FileName;

                using (var stream = File.Create(fileName))
                {
                    part.Content.DecodeTo (stream);
                }
            }
        }
        #endregion SaveAttachments
    }

    async Task<string> SaveToTempFile(IFormFile? formFile)
    {
        var tempFileName = Path.GetTempFileName();
        await using (var stream = File.Create(tempFileName))
        {
            await formFile!.CopyToAsync(stream);
        }

        return tempFileName;
    }
}