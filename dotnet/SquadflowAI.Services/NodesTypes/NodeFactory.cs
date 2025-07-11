﻿using Microsoft.Extensions.DependencyInjection;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Services.NodesTypes.Apps;
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
        //private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _scopeFactory;

        public NodeFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public INode CreateNode(UIAgentNodeDto node)
        {
            // to check the dispose of the scope or move the scope inside nodes
            var scope = _scopeFactory.CreateScope();
            var provider = scope.ServiceProvider;


            var instance = node.Type switch
            {
               
                "llm-promt" => (INode)provider.GetRequiredService<LLMPromptNode>(),
                "text-summarizer" => (INode)provider.GetRequiredService<AISummarizeTextNode>(),
                "web-research" => (INode)provider.GetRequiredService<WebResearchNode>(),
                "text-input" => (INode)provider.GetRequiredService<TextInputNode>(),
                "pdf-input" => (INode)provider.GetRequiredService<PdfInputNode>(),
                "text-output" => (INode)provider.GetRequiredService<TextOutputNode>(),
                "gmail-app" => (INode)provider.GetRequiredService<AppGmailReceiverNode>(),
                _ => throw new NotSupportedException($"Node type {node.Type} not supported")
            };


            if (instance is IInitializableNode initializable)
            {
                initializable.Initialize(node.Id, node.Parameters);
            }

            return instance;
        }
    }


    public interface IInitializableNode
    {
        void Initialize(string id, IDictionary<string, string> parameters);
    }
}
