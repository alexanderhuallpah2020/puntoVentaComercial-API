using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Database
{
    internal sealed class DbConnectionFactory(string connectionString) : IDbConnectionFactory
    {
        public async ValueTask<DbConnection> OpenConnectionAsync()
        {
            // Creamos la conexión para SQL Server
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
