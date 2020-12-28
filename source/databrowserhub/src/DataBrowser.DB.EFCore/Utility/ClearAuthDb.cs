using System;
using DataBrowser.Domain.Interfaces.Entities;
using DataBrowser.Entities.SQLite;
using DataBrowser.Interfaces.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataBrowser.DB.EFCore.Utility
{
    public class ClearAuthDb : IClearAuthDb
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<ClearAuthDb> _logger;

        public ClearAuthDb(ILogger<ClearAuthDb> logger,
            DatabaseContext databaseContext)
        {
            _logger = logger;
            _databaseContext = databaseContext;
        }

        public bool ClearAfterNodeRemove(int nodeId)
        {
            _logger.LogDebug("START  ClearAfterNodeRemove");
            try
            {
                var query = $"DELETE FROM [{TableNames.UserClaims}] WHERE ClaimType='Permission_SingleNode_{nodeId}'";
                _databaseContext.Database.ExecuteSqlCommandAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in remove claims clearAuthDb {nodeId}", ex);

                return false;
            }

            return true;
        }
    }
}