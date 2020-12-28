using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Database;
using DataBrowser.Interfaces.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Query.Geometries
{
    public class GetCountItemsQuery : IQuery<int>
    {
        public class CountGeometriesHandler : IRequestHandler<GetCountItemsQuery, int>
        {
            private readonly IGeometryDbConnection _dbConnection;
            private readonly ILogger<CountGeometriesHandler> _logger;
            private readonly IMapper _mapper;

            public CountGeometriesHandler(ILogger<CountGeometriesHandler> logger,
                IMapper mapper,
                IGeometryDbConnection dbConnection)
            {
                _logger = logger;
                _mapper = mapper;
                _dbConnection = dbConnection;
            }

            public async Task<int> Handle(GetCountItemsQuery request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");


                var result = await _dbConnection.IDbConnection.QuerySingleAsync<int>("SELECT COUNT(*) FROM geometry");

                //await _dbConnection.IDbConnection.ExecuteReader();

                _logger.LogDebug("END");
                return result;
            }
        }
    }
}