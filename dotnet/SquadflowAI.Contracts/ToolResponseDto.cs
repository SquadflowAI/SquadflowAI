using SquadflowAI.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Contracts
{
    public class ToolResponseDto
    {
        public string Data { get; set; }
        public byte[] ByteData { get; set; }
        public ToolDataTypeEnum DataType {  get; set; }
    }
}
