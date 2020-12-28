using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataBrowser.Interfaces.Workers
{
    public interface IDataflowCacheGeneratorWorker
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
