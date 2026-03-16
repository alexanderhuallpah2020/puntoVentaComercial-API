using System.Data.Common;

namespace DataConsulting.PuntoVentaComercial.Application.Abstractions.Data
{
    public interface IDbConnectionFactory
    {
        ValueTask<DbConnection> OpenConnectionAsync();
    }

}
