using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.SeedWork;
using DataBrowser.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Entities.Geometry
{
    public class Geometry : Entity, IAggregateRoot
    {
        public int UniqueId { get; set; }

        public string Id { get; set; }

        public string Label { get; set; }

        public string Country { get; set; }

        public int NutsLevel { get; set; }

        public string WKT { get; set; }

        protected Geometry()
        {

        }

        public static Geometry CreateGeometry(GeometryDto dto)
        {
            var geometry = new Geometry()
            {
                UniqueId = dto.UniqueId,
                Id = dto.Id,
                Label = dto.Label,
                Country = dto.Country,
                NutsLevel = dto.NutsLevel,
                WKT = dto.WKT
            };
            return geometry;
        }

        public void Edit(GeometryDto dto)
        {
            UniqueId = dto.UniqueId;
            Id = dto.Id;
            Label = dto.Label;
            Country = dto.Country;
            NutsLevel = dto.NutsLevel;
            WKT = dto.WKT;
        }
    }
}
