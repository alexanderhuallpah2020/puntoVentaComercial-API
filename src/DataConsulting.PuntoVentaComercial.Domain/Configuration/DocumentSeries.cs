using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Configuration
{
    public sealed class DocumentSeries : Entity
    {
        public int IdEmpresa { get; private set; }
        public int IdSucursal { get; private set; }
        public int IdEstacion { get; private set; }
        public EDocumento TipoDocumento { get; private set; }
        public string NumSerie { get; private set; } = string.Empty;
        public long UltimoCorrelativo { get; private set; }
        public bool Activo { get; private set; }

        private DocumentSeries() { }
    }
}
