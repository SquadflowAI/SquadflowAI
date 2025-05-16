using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Services.NodesTypes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.NodesTypes
{
    public class NodeFactory
    {
        public static INode CreateNode(UIAgentNodeDto node)
        {
            return node.Type switch
            {
                "text_input" => new TextInputNode(node.Id, node.Parameters["text"]),
                "llm_prompt" => new LLMPromptNode(node.Id),
                "ai_summarize_text" => new AISummarizeTextNode(node.Id),
                "text_output" => new TextOutputNode(node.Id),
                _ => throw new NotSupportedException($"Node type {node.Type} not supported")
            };
        }
    }
}
