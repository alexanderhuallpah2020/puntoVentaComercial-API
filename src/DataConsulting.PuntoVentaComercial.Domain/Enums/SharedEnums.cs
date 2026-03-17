namespace DataConsulting.PuntoVentaComercial.Domain.Enums
{
    public enum EEstado : int
    {
        Todos = 0,
        Activo = 1,
        Inactivo = 2
    }

    public enum EFormatoArchivo : int
    {
        Todos = 0,
        Excel = 1,
        Pdf = 2
    }

    public enum ETipoMoneda : int
    {
        Todos = 0,
        Soles = 1,
        Dolares = 2,
        Euros = 3,
        Pesos = 4
    }

    public enum EFlagOrigen : byte
    {
        Todos = 0,
        Manual = 1,
        Migrado = 2
    }

    public enum ETipoDocIdentidad
    {
        SinDocumento = 0,
        DNI = 1,
        CarnetExtranjeria = 4,
        RUC = 6
    }
}
