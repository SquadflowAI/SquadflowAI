﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Domain
{
    public class UIAgentNode
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public IDictionary<string, string>? Parameters { get; set; }
        public IDictionary<string, byte[]>? ParametersByte { get; set; }
        public IDictionary<string, IFormFile>? ParametersIFormFile { get; set; }
        public IDictionary<string, string>? ParametersByteIds { get; set; }
        public IDictionary<string, string>? ParametersFileUrls { get; set; }

        //public IDictionary<string, string>? ParametersFileTextContent { get; set; }
        public string? Input { get; set; }
        public string? Output { get; set; }
        public List<int>? NextNodeIds { get; set; } = new();
        public int OrderSequence { get; set; }
        public decimal PositionX { get; set; }
        public decimal PositionY { get; set; }
    }
}
