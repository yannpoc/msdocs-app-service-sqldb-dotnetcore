using DotNetCoreSqlDb.Data;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreSqlDb.Services
{
    public class DataService
    {
        private readonly MyDatabaseContext _databaseContext;

        public DataService(MyDatabaseContext databaseContext) => _databaseContext = databaseContext;

        /// <summary>
        /// Initializes the database.
        /// </summary>
        public Task InitDatabase()
        {
            return Task.Run(_databaseContext.Database.Migrate);
        }
    }
}