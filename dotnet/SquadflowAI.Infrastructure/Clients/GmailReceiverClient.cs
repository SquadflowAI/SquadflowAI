using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SquadflowAI.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Clients
{
    public class GmailReceiverClient : IGmailReceiverClient
    {
        private readonly string _email;
        private readonly string _appPassword;

        public GmailReceiverClient(IConfiguration configuration)
        {
            _email = configuration.GetValue<string>("GMAIL_EMAIL");
            _appPassword = configuration.GetValue<string>("GMAIL_APPPASWORD");
        }

        //public async Task<List<MimeMessage>> GetUnreadEmailsBySenderAsync(string senderEmail, int maxCount = 5)
        //{
        //    using var client = new ImapClient();

        //    await client.ConnectAsync("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
        //    await client.AuthenticateAsync(_email, _appPassword);

        //    var inbox = client.Inbox;
        //    await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

        //    // Search for UNREAD emails from the specified sender
        //    var query = SearchQuery.NotSeen.And(SearchQuery.FromContains(senderEmail));
        //    var uids = await inbox.SearchAsync(query);

        //    // Take the most recent maxCount emails
        //    var recentUids = uids.Count > maxCount
        //        ? uids.Skip(uids.Count - maxCount).ToList()
        //        : uids;

        //    var emails = new List<MimeMessage>();

        //    foreach (var uid in recentUids)
        //    {
        //        var message = await inbox.GetMessageAsync(uid);
        //        emails.Add(message);
        //    }

        //    await client.DisconnectAsync(true);
        //    return emails;
        //}


        public async Task<List<MimeMessage>> GetRecentEmailsBySenderAsync(string senderEmail, int maxCount = 5)
        {
            using var client = new ImapClient();

            await client.ConnectAsync("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_email, _appPassword);

            var inbox = client.Inbox;
            await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

            // Search messages from the specified sender
            var query = SearchQuery.NotSeen.And(SearchQuery.FromContains(senderEmail));
            var uids = await inbox.SearchAsync(query);
            //var uids = await inbox.SearchAsync(SearchQuery.FromContains(senderEmail));

            // Take the most recent maxCount emails
            var recentUids = uids.Count > maxCount
                ? uids.Skip(uids.Count - maxCount).ToList()
                : uids;

            var emails = new List<MimeMessage>();

            foreach (var uid in recentUids)
            {
                var message = await inbox.GetMessageAsync(uid);
                emails.Add(message);
            }

            await client.DisconnectAsync(true);
            return emails;
        }


        public async Task<List<MimeMessage>> GetRecentEmailsAsync(int count = 5)
        {
            using var client = new ImapClient();

            await client.ConnectAsync("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_email, _appPassword);

            var inbox = client.Inbox;
            await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

            var emails = new List<MimeMessage>();
            int messagesToRead = Math.Min(inbox.Count, count);

            for (int i = inbox.Count - messagesToRead; i < inbox.Count; i++)
            {
                var message = await inbox.GetMessageAsync(i);
                emails.Add(message);
            }

            await client.DisconnectAsync(true);
            return emails;
        }
    }
}
