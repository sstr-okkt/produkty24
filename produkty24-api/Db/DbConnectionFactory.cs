using Microsoft.Data.Sqlite;
using System.Data;

namespace Produkty24_API.Db
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }

    public class SqliteConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}
