using MimeKit;
using SquadflowAI.Contracts;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Services.NodesTypes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.NodesTypes.Apps
{
    public class AppGmailReceiverNode : INode
    {
        private readonly IGmailReceiverClient _gmailReceiverClient;
        public AppGmailReceiverNode(IGmailReceiverClient gmailReceiverClient)
        {
            _gmailReceiverClient = gmailReceiverClient;
        }
        public string Id { get; private set; }

        public void Initialize(string id, IDictionary<string, string> parameters)
        {
            Id = id;
        }

        public async Task<ExecutionInputOutputDto> ExecuteAsync(ExecutionInputOutputDto input, IDictionary<string, string> parameters, UIFlowDto uIFlow, IDictionary<string, byte[]> parametersByte)
        {
            var output = new ExecutionInputOutputDto();

            var sender = parameters["appGmailReceiverSender"];
            bool includeAttachments = false;

            if (parameters.TryGetValue("appGmailReceiverIncludeAttachments", out var value))
            {
                if (bool.TryParse(value, out var parsed))
                {
                    includeAttachments = parsed;
                }
            }
            var numberOfReadedEmails = !string.IsNullOrEmpty(parameters["appGmailNumberOfReadedEmails"]) ? int.Parse(parameters["appGmailNumberOfReadedEmails"]) : 5;

            if (includeAttachments)
            {
                output.ByteInputs = new List<ByteDocument>();
            }


            var emails = new List<MimeKit.MimeMessage>();
            if (!string.IsNullOrEmpty(sender))
            {
                emails = await _gmailReceiverClient.GetRecentEmailsBySenderAsync(sender, numberOfReadedEmails);
            } else
            {
                emails = await _gmailReceiverClient.GetRecentEmailsAsync(numberOfReadedEmails);
            }

            if (emails == null || !emails.Any())
            {
                output.Input = "No emails found.";
                return output;
            }

            //var filteredEmailsBySender = emails.Where(email => email.From.Mailboxes.Any(mb => mb.Address.Equals(sender, StringComparison.OrdinalIgnoreCase))).ToList();

            var emailContents = new StringBuilder();

            foreach (var email in emails)
            {
                // Prefer text body, fallback to HTML body stripped to text (optional)
                string content = email.TextBody;

                if (string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(email.HtmlBody))
                {
                    
                    content = HtmlToPlainText(email.HtmlBody);
                }

                emailContents.AppendLine($"From: {email.From}");
                emailContents.AppendLine($"Date: {email.Date}");
                emailContents.AppendLine($"Subject: {email.Subject}");
                emailContents.AppendLine("Content:");
                emailContents.AppendLine(content);
                emailContents.AppendLine(new string('-', 50));
            }

            output.Input = emailContents.ToString();

            foreach (var email in emails)
            {
                var pdfFiles = GetPdfAttachments(email);

                foreach (var attachment in pdfFiles)
                {
                    var doc = new ByteDocument
                    {
                        FileName = attachment.FileName,
                        ByteInput = attachment.Bytes
                    };

                    output.ByteInputs.Add(doc);
                }
            }

            return output;
        }

        public List<FileAttachment> GetPdfAttachments(MimeMessage message)
        {
            var pdfAttachments = new List<FileAttachment>();

            foreach (var attachment in message.Attachments)
            {
                if (attachment is MimePart part)
                {
                    var isPdf = part.ContentType.MediaType.Equals("application", StringComparison.OrdinalIgnoreCase) &&
                                part.ContentType.MediaSubtype.Equals("pdf", StringComparison.OrdinalIgnoreCase);

                    var hasPdfExtension = part.FileName != null && part.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);

                    if (isPdf || hasPdfExtension)
                    {
                        using var ms = new MemoryStream();
                        part.Content.DecodeTo(ms);

                        pdfAttachments.Add(new FileAttachment
                        {
                            FileName = part.FileName ?? "attachment.pdf",
                            Bytes = ms.ToArray()
                        });
                    }
                }
            }

            return pdfAttachments;
        }


        private string HtmlToPlainText(string html)
        {
            // Basic stripping of tags (for a more robust solution use HtmlAgilityPack or similar)
            return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
        }

        public class FileAttachment
        {
            public string FileName { get; set; }
            public byte[] Bytes { get; set; }
        }

    }
}
