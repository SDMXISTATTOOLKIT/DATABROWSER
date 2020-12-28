using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using DataBrowser.Domain.Dtos;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Database;
using DataBrowser.Interfaces.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Util.Extensions;

namespace DataBrowser.Query.Geometries
{
    public class GetGeometriesQuery : IQuery<List<GeometryDto>>
    {
        public GetGeometriesQuery(List<string> idList = null)
        {
            IdList = idList;
        }

        public List<string> IdList { get; set; }

        public class GetGeometriesHandler : IRequestHandler<GetGeometriesQuery, List<GeometryDto>>
        {
            private readonly IGeometryDbConnection _dbConnection;
            private readonly ILogger<GetGeometriesHandler> _logger;
            private readonly IMapper _mapper;

            public GetGeometriesHandler(ILogger<GetGeometriesHandler> logger,
                IMapper mapper,
                IGeometryDbConnection dbConnection)
            {
                _logger = logger;
                _mapper = mapper;
                _dbConnection = dbConnection;
            }

            public async Task<List<GeometryDto>> Handle(GetGeometriesQuery request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var query = "SELECT * FROM geometry";
                Dictionary<string, object> parameters = null;
                /*if (request.IdList != null)
                {
                    query += " WHERE Id IN @Ids";
                    parameters = new Dictionary<string, object> { { "@Ids", request.IdList } };
                }*/

                var wantedGeometryIdSet = new HashSet<string>();
                if (request.IdList?.Count > 0) wantedGeometryIdSet.AddAll(request.IdList.Select(id => id.ToUpper()));

                using (var reader = await _dbConnection.IDbConnection.ExecuteReaderAsync(query, parameters))
                {
                    var idToGeometryMap = new Dictionary<string, GeometryDto>();
                    var aliasToGeometryIdMap = new Dictionary<string, string>();
                    while (reader.Read())
                    {
                        var toUpperGeomId = (reader["Id"] as string).ToUpper();

                        var geom = new GeometryDto
                        {
                            UniqueId = (int) (long) reader["UniqueId"],
                            Id = toUpperGeomId,
                            Label = reader["Label"] as string,
                            Country = reader["Country"] as string,
                            NutsLevel = (int) (long) reader["NutsLevel"],
                            WKT = reader["WKT"] as string
                        };
                        idToGeometryMap[toUpperGeomId] = geom;

                        var aliasList = (reader["AlternativeIds"] as string)?.ToUpper().Split("|")
                            .Where(v => !string.IsNullOrEmpty(v)).ToList();
                        if (aliasList != null)
                            foreach (var alias in aliasList)
                                aliasToGeometryIdMap[alias] = toUpperGeomId;
                    }

                    if (request.IdList?.Count == 0)
                    {
                        _logger.LogDebug("END");
                        return idToGeometryMap.Select(e => e.Value).ToList();
                    }

                    var result = new List<GeometryDto>();
                    var missingIds = new List<string>();
                    foreach (var wantedId in request.IdList)
                    {
                        var wantedIdToUpper = wantedId.ToUpper();
                        if (idToGeometryMap.ContainsKey(wantedIdToUpper))
                        {
                            var referencedGeometry = idToGeometryMap[wantedIdToUpper];
                            referencedGeometry.Id = wantedId;
                            result.Add(idToGeometryMap[wantedIdToUpper]);
                        }
                        else if (aliasToGeometryIdMap.ContainsKey(wantedIdToUpper))
                        {
                            var referencedGeometryId = aliasToGeometryIdMap[wantedIdToUpper];
                            var geom = idToGeometryMap[referencedGeometryId];
                            var geometryCopy = GeometryDto.DeepCopy(geom);
                            geometryCopy.Id = wantedId;
                            result.Add(geometryCopy);
                        }
                    }

                    _logger.LogDebug("END");
                    return result;
                }
            }
        }
    }
}