using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Sunat
{
    /// <summary>
    /// Registro del estado de envío de un comprobante electrónico a SUNAT.
    /// Un IdVenta tiene como máximo un SunatSubmission; los reintentos actualizan el mismo registro.
    /// </summary>
    public sealed class SunatSubmission : Entity
    {
        // ORM constructor
        private SunatSubmission() { }

        public int IdVenta { get; private set; }
        public DateTime FechaEnvio { get; private set; }

        /// <summary>Estado según <see cref="ETipoEstadoSunat"/>.</summary>
        public int Estado { get; private set; }

        /// <summary>Código de respuesta SUNAT (ej. "0", "2000", "4000").</summary>
        public string? CodigoRespuesta { get; private set; }

        /// <summary>Descripción textual de la respuesta SUNAT.</summary>
        public string? MensajeRespuesta { get; private set; }

        /// <summary>SHA-256 del XML firmado enviado.</summary>
        public string? XmlHash { get; private set; }

        /// <summary>Contenido del CDR devuelto por SUNAT (base64).</summary>
        public string? CdrXml { get; private set; }

        /// <summary>Número de ticket para envíos asíncronos (sendSummary / sendPack).</summary>
        public string? NumTicket { get; private set; }

        /// <summary>Cantidad de intentos de envío acumulados.</summary>
        public int Intentos { get; private set; }

        public DateTime FechaCreacion { get; private set; }
        public DateTime? FechaModificacion { get; private set; }

        public static SunatSubmission Create(int idVenta, DateTime fechaCreacion)
        {
            return new SunatSubmission
            {
                IdVenta = idVenta,
                FechaEnvio = fechaCreacion,
                Estado = (int)ETipoEstadoSunat.NoEnviado,
                Intentos = 0,
                FechaCreacion = fechaCreacion
            };
        }

        public void MarkAsSent(string? xmlHash, string? numTicket, DateTime fechaModificacion)
        {
            Estado = (int)ETipoEstadoSunat.Enviado;
            XmlHash = xmlHash;
            NumTicket = numTicket;
            Intentos++;
            FechaModificacion = fechaModificacion;
        }

        public void MarkAsAccepted(string codigoRespuesta, string mensajeRespuesta,
            string? cdrXml, DateTime fechaModificacion)
        {
            Estado = (int)ETipoEstadoSunat.Aceptado;
            CodigoRespuesta = codigoRespuesta;
            MensajeRespuesta = mensajeRespuesta;
            CdrXml = cdrXml;
            FechaModificacion = fechaModificacion;
        }

        public void MarkAsRejected(string codigoRespuesta, string mensajeRespuesta,
            DateTime fechaModificacion)
        {
            Estado = (int)ETipoEstadoSunat.Rechazado;
            CodigoRespuesta = codigoRespuesta;
            MensajeRespuesta = mensajeRespuesta;
            FechaModificacion = fechaModificacion;
        }
    }
}
