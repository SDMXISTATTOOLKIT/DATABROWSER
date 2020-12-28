using AutoMapper;
using DataBrowser.Domain.Entities.Geometry;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Entities.SQLite
{
    public class GeometryRepository : GenericRepository<Geometry>, IGeometryRepository
    {
        private readonly ILogger<GeometryRepository> _logger;
        private readonly IMapper _mapper;

        public GeometryRepository(ILogger<GeometryRepository> logger,
            DatabaseContext dbContext,
            IMapper mapper,
            IRequestContext requestContext) :
            base(dbContext, requestContext)
        {
            _logger = logger;
            _mapper = mapper;
        }
    }
}