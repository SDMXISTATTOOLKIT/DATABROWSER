using System.Data;

namespace DataBrowser.Interfaces.Database
{
    public interface IGeometryDbConnection
    {
        IDbConnection IDbConnection { get; set; }
    }
}