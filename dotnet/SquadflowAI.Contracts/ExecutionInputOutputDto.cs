using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Contracts
{
    public class ExecutionInputOutputDto
    {
        public string Type { get; set; }
        public string InputName { get; set; }
        public string Input { get; set; }
        public List<ByteDocument> ByteInputs {  get; set; }
    }

    public class ByteDocument
    {
        public string FileName { get; set; }
        public byte[] ByteInput { get; set; }
    }
}
