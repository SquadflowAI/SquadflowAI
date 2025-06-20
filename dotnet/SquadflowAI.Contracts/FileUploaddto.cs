using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Contracts
{
    public class FileUploadDto
    {
        public Guid FlowId { get; set; }
        public string NodeId { get; set; }
        public string Key { get; set; }
        public IFormFile File { get; set; }
    }

}
