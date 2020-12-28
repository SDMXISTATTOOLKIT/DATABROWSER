using AutoMapper;
using DataBrowser.AC;

namespace DataBrowser.UnitTests.Mapper
{
    public class HubMappingProfileConfigurationTest
    {
        private readonly IMapper _mapper;

        public HubMappingProfileConfigurationTest()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfileConfiguration()); });
            _mapper = config.CreateMapper();
        }
    }
}