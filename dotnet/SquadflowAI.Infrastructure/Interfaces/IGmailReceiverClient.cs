using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Interfaces
{
    public interface IGmailReceiverClient
    {
        Task<List<MimeMessage>> GetRecentEmailsBySenderAsync(string senderEmail, int maxCount = 5);
        Task<List<MimeMessage>> GetRecentEmailsAsync(int count = 5);
    }
}
