using System.Data;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Database;

namespace DataBrowser.AC
{
    public class GeometryDbConnection : IGeometryDbConnection
    {
        public IDbConnection IDbConnection { get; set; }
    }
}