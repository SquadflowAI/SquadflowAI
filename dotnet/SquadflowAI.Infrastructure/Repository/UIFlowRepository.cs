using Dapper;
using Newtonsoft.Json;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquadflowAI.Infrastructure.Repository
{
    public class UIFlowRepository : IUIFlowRepository
    {
        private readonly DbContext _dbContext;
        public UIFlowRepository(DbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task CreateUIFlowAsync(UIFlowDto flow)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            if(flow.Nodes != null && flow.Connections != null )
            {
                AssignOrderSequences(flow);
            }
            

            var data = JsonConvert.SerializeObject(flow);
            var uiflowQuery = "INSERT INTO uiflows (name, userId, projectId, data) VALUES (@name, @userId, @projectId, @data::jsonb)";

            await connection.ExecuteAsync(uiflowQuery, new { flow.Name, flow.UserId, flow.ProjectId, data });
        }

        public async Task UpdateUIFlowAsync(UIFlowDto flow)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            if (flow.Nodes != null && flow.Connections != null)
            {
                AssignOrderSequences(flow);
            }

            var data = JsonConvert.SerializeObject(flow);
            var uiflowQuery = @"UPDATE uiflows
                                    SET name = @name,
                                        data = @data::jsonb
                                    WHERE id = @id;";

            await connection.ExecuteAsync(uiflowQuery, new { name = flow.Name, data = data, id = flow.Id });
        }

        public async Task<UIFlowDto> GetUIFlowByNameAsync(string inputName)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var uiflowQuery = @"
            SELECT a.data
            FROM uiflows a
            WHERE a.name = @inputName";

            string uiflowQueryResult = await connection.QuerySingleOrDefaultAsync<string>(uiflowQuery, new { inputName = inputName });

            if (uiflowQueryResult == null)
                return null;

            var result = JsonConvert.DeserializeObject<UIFlowDto>(uiflowQueryResult);

            return result;
        }

        public async Task<UIFlowDto> GetUIFlowByIdAsync(Guid id)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var uiflowQuery = @"
            SELECT a.data
            FROM uiflows a
            WHERE a.id = @id";

            string uiflowQueryResult = await connection.QuerySingleOrDefaultAsync<string>(uiflowQuery, new { id = id });

            if (uiflowQueryResult == null)
                return null;

            var result = JsonConvert.DeserializeObject<UIFlowDto>(uiflowQueryResult);

            return result;
        }

        public async Task DeleteUIFlowByIdAsync(Guid id)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var uiflowSql = @"
            DELETE
            FROM uiflows a
            WHERE a.id = @id";

            await connection.ExecuteAsync(uiflowSql, new { id = id });
        }

        public async Task<IEnumerable<UIFlowDto>> GetUIFlowsAsync()
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var uiflowQuery = @"
            SELECT a.data
            FROM uiflows a";

            IEnumerable<UIFlowDto> uiflowQueryResult = await connection.QueryAsync<UIFlowDto>(uiflowQuery);

            if (uiflowQueryResult == null)
                return null;

            return uiflowQueryResult.ToList();
        }

        public async Task<IEnumerable<UIFlowDto>> GetUIFlowsByProjectIdAsync(Guid projectId)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var uiflowQuery = @"
            SELECT *
            FROM uiflows a WHERE a.projectId = @projectId";

            IEnumerable<UIFlowDto> projectsQueryResult = await connection.QueryAsync<UIFlowDto>(uiflowQuery, new { projectId = projectId });

            if (projectsQueryResult == null)
                return null;

            return projectsQueryResult.ToList();
        }

        public void AssignOrderSequences(UIFlowDto uIFlow)
        {
            var nodeDict = uIFlow.Nodes.ToDictionary(n => n.Id);
            var inDegree = new Dictionary<string, int>();
            var adjacencyList = new Dictionary<string, List<string>>();

            // Initialize in-degree and adjacency list
            foreach (var node in uIFlow.Nodes)
            {
                inDegree[node.Id] = 0;
                adjacencyList[node.Id] = new List<string>();
                node.NextNodeIds = new List<int>(); // Ensure it's initialized
            }

            foreach (var conn in uIFlow.Connections)
            {
                adjacencyList[conn.SourceNodeId].Add(conn.TargetNodeId);
                inDegree[conn.TargetNodeId]++;
            }

            // Topological sort using BFS (Kahn's algorithm)
            var queue = new Queue<string>();
            int sequence = 1;

            // Start from nodes with 0 in-degree
            foreach (var nodeId in inDegree.Where(x => x.Value == 0).Select(x => x.Key))
            {
                queue.Enqueue(nodeId);
            }

            // Track order sequence per node ID
            var orderById = new Dictionary<string, int>();

            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                var currentNode = nodeDict[currentId];
                currentNode.OrderSequence = sequence;
                orderById[currentId] = sequence;
                sequence++;

                foreach (var neighborId in adjacencyList[currentId])
                {
                    inDegree[neighborId]--;
                    if (inDegree[neighborId] == 0)
                    {
                        queue.Enqueue(neighborId);
                    }
                }
            }

            // After all OrderSequences are assigned, populate NextNodeIds using OrderSequence
            foreach (var conn in uIFlow.Connections)
            {
                if (orderById.TryGetValue(conn.SourceNodeId, out var sourceOrder) &&
                    orderById.TryGetValue(conn.TargetNodeId, out var targetOrder) &&
                    nodeDict.TryGetValue(conn.SourceNodeId, out var sourceNode))
                {
                    if (!sourceNode.NextNodeIds.Contains(targetOrder))
                    {
                        sourceNode.NextNodeIds.Add(targetOrder);
                    }
                }
            }
        }


        //public void AssignOrderSequences(UIFlowDto uIFlow)
        //{
        //    var nodeDict = uIFlow.Nodes.ToDictionary(n => n.Id);
        //    var inDegree = new Dictionary<string, int>();
        //    var adjacencyList = new Dictionary<string, List<string>>();

        //    // Initialize in-degree and adjacency list
        //    foreach (var node in uIFlow.Nodes)
        //    {
        //        inDegree[node.Id] = 0;
        //        adjacencyList[node.Id] = new List<string>();
        //    }

        //    foreach (var conn in uIFlow.Connections)
        //    {
        //        adjacencyList[conn.SourceNodeId].Add(conn.TargetNodeId);
        //        inDegree[conn.TargetNodeId]++;
        //    }

        //    // Topological sort using BFS (Kahn's algorithm)
        //    var queue = new Queue<string>();
        //    int sequence = 1;

        //    // Start from nodes with 0 in-degree
        //    foreach (var nodeId in inDegree.Where(x => x.Value == 0).Select(x => x.Key))
        //    {
        //        queue.Enqueue(nodeId);
        //    }

        //    while (queue.Count > 0)
        //    {
        //        var currentId = queue.Dequeue();
        //        var currentNode = nodeDict[currentId];
        //        currentNode.OrderSequence = sequence++;

        //        foreach (var neighbor in adjacencyList[currentId])
        //        {
        //            inDegree[neighbor]--;
        //            if (inDegree[neighbor] == 0)
        //            {
        //                queue.Enqueue(neighbor);
        //            }
        //        }
        //    }
        //}
    }
}
