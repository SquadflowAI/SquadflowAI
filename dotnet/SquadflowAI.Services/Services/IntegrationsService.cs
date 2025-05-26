using Konscious.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using SquadflowAI.Contracts;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Contracts.User;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Infrastructure.Repository;
using SquadflowAI.Services.Helpers;
using SquadflowAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Services
{
    public class IntegrationsService : IIntegrationsService
    {
       
        private readonly IIntegrationsRepository _integrationsRepository;
        private IConfiguration _configuration;
        public IntegrationsService(IIntegrationsRepository integrationsRepository, IConfiguration configuration)
        {
            _integrationsRepository = integrationsRepository;
            _configuration = configuration;
        }

        public async Task CreateIntegrationAsync(IntegrationsDto integration)
        {
            var hashedOpenAIKey = EncryptionHelper.EncryptApiKey(integration.OpenAIKey, _configuration);
            integration.OpenAIKey = hashedOpenAIKey;

            await _integrationsRepository.CreateIntegrationAsync(integration);
        }

        public async Task<IntegrationsDto> GetIntegrationByUserIdAsync(Guid id)
        {
            var result = await _integrationsRepository.GetIntegrationByUserIdAsync(id);

            result.OpenAIKey = EncryptionHelper.DecryptApiKey(result.OpenAIKey, _configuration);

            return result;
        }

      
    }
}
