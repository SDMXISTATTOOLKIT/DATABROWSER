using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Workers;
using Microsoft.Extensions.Logging;

namespace DataBrowser.AC.Workers
{
    public class ApplicationCleaner : IApplicationCleaner
    {
        private readonly ILogger<ApplicationCleaner> _logger;
        private readonly IRepository<Hub> _repositoryHub;
        private readonly IRepository<Node> _repositoryNode;

        public ApplicationCleaner(ILogger<ApplicationCleaner> logger,
            IRepository<Hub> repositoryHub,
            IRepository<Node> repositoryNode)
        {
            _logger = logger;
            _repositoryHub = repositoryHub;
            _repositoryNode = repositoryNode;
        }

        public async Task DoWorkAsync()
        {
            //TODO REMOVE ALL DATAFLOWDATA CACHE FILE ASSIGN TO NODE ID REMOVED

            _logger.LogDebug("DoWorkAsync for DisckCleanerWorker START");

            var hubs = await _repositoryHub.ListAllAsync();
            var nodes = await _repositoryNode.ListAllAsync();

            var filesPath = new List<string>();
            foreach (var item in hubs)
            {
                if (!string.IsNullOrWhiteSpace(item.BackgroundMediaURL)) filesPath.Add(item.BackgroundMediaURL);
                if (!string.IsNullOrWhiteSpace(item.LogoURL)) filesPath.Add(item.LogoURL);
            }

            foreach (var item in nodes)
            {
                if (!string.IsNullOrWhiteSpace(item.BackgroundMediaURL)) filesPath.Add(item.BackgroundMediaURL);
                if (!string.IsNullOrWhiteSpace(item.Logo)) filesPath.Add(item.Logo);
            }

            var allFileEntries = Directory.GetFiles(DataBrowserDirectory.GetImageDirPath(), "*.*",
                SearchOption.TopDirectoryOnly);
            var allEntries = allFileEntries.Union(Directory.GetFiles(DataBrowserDirectory.GetVideoDirPath(), "*.*",
                SearchOption.TopDirectoryOnly));

            foreach (var item in allEntries)
            {
                var isUsed = filesPath.Any(i => item.Replace("\\", "/").EndsWith(i));
                if (isUsed) continue;
                try
                {
                    File.Delete(item);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Remove unsued file: {item}");
                }
            }


            _logger.LogDebug("DoWorkAsync for DisckCleanerWorker END");
        }
    }
}