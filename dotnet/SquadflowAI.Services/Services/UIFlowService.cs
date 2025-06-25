using Microsoft.AspNetCore.Http;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Infrastructure.Repository;
using SquadflowAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Services
{
    public class UIFlowService : IUIFlowService
    {
        private readonly IUIFlowRepository _uIFlowRepository;
        private readonly IFlowExecutorService _flowExecutorService;
        private readonly IActionRunRepository _actionRunRepository;
        private readonly IFileDocumentsRepository _fileDocumentsRepository;

        public UIFlowService(IUIFlowRepository uIFlowRepository, 
            IFlowExecutorService flowExecutorService,
            IActionRunRepository actionRunRepository,
            IFileDocumentsRepository fileDocumentsRepository)
        {
            _uIFlowRepository = uIFlowRepository;
            _flowExecutorService = flowExecutorService;
            _actionRunRepository = actionRunRepository;
            _fileDocumentsRepository = fileDocumentsRepository;
        }

        public async Task CreateUIFlowAsync(UIFlowDto uiflow)
        {
            //foreach (var node in uiflow.Nodes ?? Enumerable.Empty<UIAgentNodeDto>())
            //{
            //    if (node.ParametersIFormFile != null)
            //    {
            //        foreach (var kvp in node.ParametersIFormFile)
            //        {
            //            var key = kvp.Key;
            //            var file = kvp.Value;

            //            using var memoryStream = new MemoryStream();
            //            await file.CopyToAsync(memoryStream);

            //            var document = new FilesDocuments
            //            {
            //                Name = file.FileName,
            //                Content = memoryStream.ToArray(),
            //                ContentType = file.ContentType
            //            };

            //            var fileId = await _fileDocumentsRepository.CreateFileDocumentAsync(document); // returns Guid

            //            node.ParametersByteIds ??= new Dictionary<string, string>();
            //            node.ParametersByteIds[key] = fileId.ToString();
            //        }
            //    }
            //}

            await _uIFlowRepository.CreateUIFlowAsync(uiflow);
        }

        public async Task InsertFileUIFlowAsync(Guid flowId, string nodeId, string key, IFormFile file)
        {
            var flow = await _uIFlowRepository.GetUIFlowByIdAsync(flowId);

            if (flow == null)
            {
                throw new ArgumentException("Flow not found", nameof(flowId));
            }

            var node = flow?.Nodes?.SingleOrDefault(n => n.Id == nodeId);

            if (node == null)
            {
                throw new ArgumentException("Node not found", nameof(nodeId));
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var document = new FilesDocuments
            {
                Name = file.FileName,
                Content = memoryStream.ToArray(),
                ContentType = file.ContentType
            };

            var fileId = await _fileDocumentsRepository.CreateFileDocumentAsync(document);  

            node.ParametersByteIds ??= new Dictionary<string, string>();
            node.ParametersByteIds[key] = fileId.ToString();

            await _uIFlowRepository.UpdateUIFlowAsync(flow);
        }

        public async Task UpdateUIFlowAsync(UIFlowDto uiflow)
        {
            await _uIFlowRepository.UpdateUIFlowAsync(uiflow);
        }

        public async Task<UIFlowDto> GetUIFlowByNameAsync(string name)
        {
            var flow = await _uIFlowRepository.GetUIFlowByNameAsync(name);

            if (flow == null)
                return null;

            foreach (var node in flow.Nodes ?? Enumerable.Empty<UIAgentNodeDto>())
            {
                if (node.ParametersByteIds != null)
                {
                    foreach (var kvp in node.ParametersByteIds)
                    {
                        var key = kvp.Key;
                        var fileId = kvp.Value;

                        var url = $"documents/download/{fileId}";
                        node.ParametersFileUrls[key] = url;
                    }
                }
            }

            return flow;
        }

        public async Task<UIFlowDto> GetUIFlowByIdAsync(Guid id)
        {
            var flow = await _uIFlowRepository.GetUIFlowByIdAsync(id);

            if (flow == null)
                return null;

            foreach (var node in flow.Nodes ?? Enumerable.Empty<UIAgentNodeDto>())
            {
                if (node.ParametersByteIds != null)
                {
                    foreach (var kvp in node.ParametersByteIds)
                    {
                        var key = kvp.Key;
                        var fileId = kvp.Value;

                        var url = $"documents/download/{fileId}";
                        if(node.ParametersFileUrls == null)
                        {
                            node.ParametersFileUrls = new Dictionary<string, string>();
                        }
                        node.ParametersFileUrls[key] = url;
                    }
                }
            }

            return flow;
        }

        public async Task<IEnumerable<UIFlowDto>> GetUIFlowsAsync()
        {
            var result = await _uIFlowRepository.GetUIFlowsAsync();

            return result;
        }

        public async Task<IEnumerable<UIFlowDto>> GetUIFlowsByProjectIdAsync(Guid projectId)
        {
            var result = await _uIFlowRepository.GetUIFlowsByProjectIdAsync(projectId);

            return result;
        }

        public async Task<string> RunUIFlowByIdAsync(Guid id)
        {
            var flow = await _uIFlowRepository.GetUIFlowByIdAsync(id);

            // GET bytes array if pdf includes

            foreach (var node in flow.Nodes ?? Enumerable.Empty<UIAgentNodeDto>())
            {
                if (node.ParametersByteIds != null)
                {
                    foreach (var kvp in node.ParametersByteIds)
                    {
                        var key = kvp.Key;
                        var fileId = kvp.Value;

                        var file = await _fileDocumentsRepository.GetFileDocumentByIdAsync(new Guid(fileId));
                        // TODO

                        if (node.ParametersFileUrls == null)
                        {
                            node.ParametersFileUrls = new Dictionary<string, string>();
                        }
                        //node.ParametersFileUrls[key] = url; TODO
                    }
                }
            }

            var result = await _flowExecutorService.ExecuteAsync(flow);

            if (result != null)
            {
                await _actionRunRepository.CreateActionRunAsync(null, flow.Id, result);
            }

            return result;
        }

        public async Task DeleteUIFlowByIdAsync(Guid id)
        {
            await _uIFlowRepository.DeleteUIFlowByIdAsync(id);
        }

        #region ActionRuns

        public async Task<IEnumerable<ActionRun>> GetActionRunsByAgentIdAsync(Guid agentId)
        {
            var result = await _actionRunRepository.GetActionRunsByAgentIdAsync(agentId);

            return result;
        }

        public async Task<IEnumerable<ActionRun>> GetActionRunsByFlowIdAsync(Guid flowId)
        {
            var result = await _actionRunRepository.GetActionRunsByFlowIdAsync(flowId);

            return result;
        }

        #endregion
    }
}
