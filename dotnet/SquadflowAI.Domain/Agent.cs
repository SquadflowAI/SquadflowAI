using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Domain
{
    public class Agent
    {
        //public string Role { get; set; }
        //public string Goal { get; set; }
        //public string LLM { get; set; }
        //public IEnumerable<Tool> Tools { get; set; }

        // Basic Information
        public string Name { get; set; }
        public string Mission { get; set; }
        public List<Capability> Capabilities { get; set; }
        public List<Action> Actions { get; set; }
        public Configuration Configuration { get; set; }
        public List<string> Limitations { get; set; }
    }

    public class Capability
    {
        public string Task { get; set; }
        public string Description { get; set; }
    }

    public class Action
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ActionToExecute { get; set; }
        public List<string> Inputs { get; set; }
        public List<string> Outputs { get; set; }
        public List<string> Triggers { get; set; }
        public List<Tool> Tools { get; set; }
    }

    public class Tool
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Input { get; set; }

        public string RecipientEmail { get; set; }
        public string RecipientName { get; set; }
    }

    public class Configuration
    {
        public string EmailRecipient { get; set; }
        public string ReportSchedule { get; set; }
        public List<string> TrustedSources { get; set; }
    }
}
