﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Interfaces
{
    public interface IOpenAILLMExecutorServiceOLD
    {
        Task ExecuteAsync(Domain.Agent agent, int maxIterations = 10);
    }
}
