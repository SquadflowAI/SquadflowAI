using MimeKit;
using SquadflowAI.Contracts;
using SquadflowAI.Tools.Interfaces;
using System;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration;


namespace SquadflowAI.Tools.GmailClient
{
    public class GmailClient : ITool
    {
        public string Key => "gmail-client";
        private readonly string _email;
        private readonly string _appPassword;

        public GmailClient(IConfiguration configuration)
        {
            _email = configuration.GetValue<string>("GMAIL_EMAIL");
            _appPassword = configuration.GetValue<string>("GMAIL_APPPASWORD");
        }

        public async Task<ToolResponseDto> ExecuteAsync(ToolConfigDto config)
        {
            config.Inputs.TryGetValue("Pdf", out var pdfBytes);
            config.Inputs.TryGetValue("PdfName", out var pdfName);
            config.Inputs.TryGetValue("RecipientEmail", out var recipientEmail);
            config.Inputs.TryGetValue("RecipientName", out var recipientName);

            try
            {
                // Fetch the byte array from your database (simulated here)
                //byte[] pdfBytes = GetPdfFromDatabase();

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Your Name", _email));
                message.To.Add(new MailboxAddress(recipientName, recipientEmail));
                message.Subject = "Test Email with PDF Attachment";

                // Create the body of the email
                var builder = new BodyBuilder
                {
                    TextBody = "This email contains a PDF attachment."
                };

                // Add the PDF attachment
                builder.Attachments.Add($"{pdfName}.pdf", pdfBytes, new ContentType("application", "pdf"));

                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(_email, _appPassword);

                    client.Send(message);
                    client.Disconnect(true);
                }

                Console.WriteLine("Email sent successfully with PDF attachment.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            var result = new ToolResponseDto()
            {
                Data = "",
                DataType = Contracts.Enums.ToolDataTypeEnum.String
            };
            
            return result;
        }
    }
}
