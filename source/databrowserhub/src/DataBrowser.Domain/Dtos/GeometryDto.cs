using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Dtos
{
    public class GeometryDto
    {
        public int UniqueId { get; set; }

        public string Id { get; set; }

        public string Label { get; set; }

        public string Country { get; set; }

        public int NutsLevel { get; set; }

        public string WKT { get; set; }


        public static GeometryDto DeepCopy(GeometryDto dto)
        {
            var result = new GeometryDto() {
                UniqueId = dto.UniqueId,
                Id = dto.Id,
                Label = dto.Label,
                Country = dto.Country,
                NutsLevel = dto.NutsLevel,
                WKT = dto.WKT,
            };
            return result;
        }
    }
}
