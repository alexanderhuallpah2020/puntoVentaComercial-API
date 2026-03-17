# Plan de Implementación — Módulo Clientes
> Ruta del proyecto: `D:\Sistemas ERP\api-net`
> Fecha: 2026-03-17
> Stack: .NET 10 | EF Core 10.0.3 | Sin MediatR | Sin Auth (se agrega al final)

---

## CONTEXTO OBLIGATORIO

### Solución existente
```
D:\Sistemas ERP\api-net\src\
├── DataConsulting.PuntoVentaComercial.API
├── DataConsulting.PuntoVentaComercial.Application
├── DataConsulting.PuntoVentaComercial.Domain
└── DataConsulting.PuntoVentaComercial.Infrastructure
```

### Patrones que DEBES respetar siempre

- **CQRS sin MediatR**: usar `ICommand<T>`, `ICommandHandler<,>`, `IQuery<T>`, `IQueryHandler<,>` del proyecto
- **Railway-Oriented**: todo retorna `Result<T>` o `Result`. NUNCA lanzar excepciones en flujo normal
- **Factory method estático** en entidades: `public static Result<Cliente> Create(...)`
- **IDs manuales**: `GetNextIdAsync()` — no identity de BD
- **Auditoría** en todas las entidades: `IdUsuarioCreador (smallint)`, `FechaCreacion (datetime)`, `IdUsuarioModificador (smallint)`, `FechaModificacion (smalldatetime)`
- **Scrutor auto-registro**: los handlers se registran solos. Solo registrar manualmente repositorios y servicios
- **EF Core Fluent API**: `IEntityTypeConfiguration<T>` por entidad en `Infrastructure/Configurations/`
- **Concurrency token**: `UpdateToken (smallint)` con `.IsConcurrencyToken()`
- **Sin `[Authorize]`** en ningún endpoint de esta fase

### Namespaces a usar
```
DataConsulting.PuntoVentaComercial.Domain.Clientes
DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Commands.CreateCliente
DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Commands.UpdateCliente
DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.GetClienteById
DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.SearchClientes
DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.GetClienteAddresses
DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.LookupRucSunat
DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
DataConsulting.PuntoVentaComercial.Infrastructure.Services
DataConsulting.PuntoVentaComercial.API.Controllers.Clientes
```

---

## PASO 1 — DOMAIN: Enums

### `Domain/Enums/BusinessEnums.cs`
**AGREGAR** al archivo existente (no reemplazar):

```csharp
public enum ETipoCliente
{
    ClienteLocal = 1,
    ClienteNacional = 2,
    ClienteExtranjero = 3
}

public enum ECliente
{
    Varios = 1
}

public enum EFlagSexo : byte
{
    Empresa = 0,
    Masculino = 1,
    Femenino = 2
}
```

### `Domain/Enums/SharedEnums.cs`
**AGREGAR** al archivo existente (no reemplazar):

```csharp
public enum ETipoDocIdentidad
{
    SinDocumento = 0,
    DNI = 1,
    CarnetExtranjeria = 4,
    RUC = 6
}
```

---

## PASO 2 — DOMAIN: Entidades

### `Domain/Clientes/Cliente.cs`

```csharp
namespace DataConsulting.PuntoVentaComercial.Domain.Clientes;

public sealed class Cliente : Entity
{
    // Datos principales
    public int IdTipoCliente { get; private set; }
    public string Nombre { get; private set; }
    public string NombreComercial { get; private set; }
    public int? IdDocumentoIdentidad { get; private set; }
    public string? NumDocumento { get; private set; }
    public string? CodValidadorDoc { get; private set; }   // dígito validador DNI (varchar 3)
    public string EstadoCliente { get; private set; }      // 'A' / 'I'
    public string? Observaciones { get; private set; }

    // Flags del POS
    public int FlagTipoCliente { get; private set; }       // siempre 1 (ClienteLocal) desde POS
    public byte FlagSexo { get; private set; }             // 0=Empresa, 1=Masc, 2=Fem
    public int FlagOperacion { get; private set; }
    public byte FlagCertCalidad { get; private set; }
    public short FlagCredito { get; private set; }
    public short FlagTipoCalificacion { get; private set; }

    // Financiero
    public decimal CreditoMaximo { get; private set; }
    public int? IdTipoMoneda { get; private set; }
    public int? IdFormaPago { get; private set; }
    public int? IdGiroNegocio { get; private set; }

    // Referencia — si IdTrabajadorRef > 0, NO editable desde POS
    public short? IdTrabajadorRef { get; private set; }
    public int? IdClienteRef { get; private set; }
    public int? IdEntidadRef { get; private set; }

    // País
    public short IdPais { get; private set; }

    // Owner (persona natural)
    public string? NombreOwner { get; private set; }
    public int? IdDocumentoIdentidadOwner { get; private set; }
    public string? NumDocumentoOwner { get; private set; }
    public DateTime? FechaNacimientoOwner { get; private set; }

    // Fechas
    public DateTime FechaAlta { get; private set; }
    public DateTime? FechaBaja { get; private set; }

    // Auditoría (heredada de Entity + campos adicionales)
    public string UsuarioCreador { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public string? UsuarioModificador { get; private set; }
    public DateTime? FechaModificacion { get; private set; }

    // Navegación
    private readonly List<ClienteLocal> _clienteLocales = [];
    public IReadOnlyCollection<ClienteLocal> ClienteLocales => _clienteLocales.AsReadOnly();

    private Cliente() { }  // EF Core

    public static Result<Cliente> Create(
        string nombre,
        int? idDocumentoIdentidad,
        string? numDocumento,
        string? codValidadorDoc,
        short idPais,
        string direccionLocal,
        string? telefono1,
        short idSucursal,
        string usuarioCreador)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return Result.Failure<Cliente>(ClienteErrors.NombreRequerido);

        if (string.IsNullOrWhiteSpace(direccionLocal))
            return Result.Failure<Cliente>(ClienteErrors.DireccionRequerida);

        if (idPais <= 0)
            return Result.Failure<Cliente>(ClienteErrors.PaisRequerido);

        var cliente = new Cliente
        {
            IdTipoCliente     = (int)ECliente.Varios,
            Nombre            = nombre.Trim(),
            NombreComercial   = nombre.Trim(),
            IdDocumentoIdentidad = idDocumentoIdentidad,
            NumDocumento      = numDocumento?.Trim(),
            CodValidadorDoc   = codValidadorDoc?.Trim(),
            EstadoCliente     = "A",
            FlagTipoCliente   = (int)ETipoCliente.ClienteLocal,
            FlagSexo          = (byte)EFlagSexo.Empresa,
            FlagOperacion     = 1,
            FlagCertCalidad   = 0,
            FlagCredito       = 0,
            FlagTipoCalificacion = 0,
            CreditoMaximo     = 0,
            IdPais            = idPais,
            FechaAlta         = DateTime.Now,
            UsuarioCreador    = usuarioCreador,
            FechaCreacion     = DateTime.Now
        };

        var local = ClienteLocal.Create(direccionLocal, telefono1, idSucursal);
        cliente._clienteLocales.Add(local);

        return Result.Success(cliente);
    }

    public Result Update(
        string nombre,
        int? idDocumentoIdentidad,
        string? numDocumento,
        string? codValidadorDoc,
        short idPais,
        string direccionLocal,
        string? telefono1,
        string usuarioModificador)
    {
        // Regla del legacy: si fue creado desde RRHH, no se puede editar desde POS
        if (IdTrabajadorRef.HasValue && IdTrabajadorRef > 0)
            return Result.Failure(ClienteErrors.NoEditableDesdePos);

        if (string.IsNullOrWhiteSpace(nombre))
            return Result.Failure(ClienteErrors.NombreRequerido);

        if (string.IsNullOrWhiteSpace(direccionLocal))
            return Result.Failure(ClienteErrors.DireccionRequerida);

        Nombre               = nombre.Trim();
        NombreComercial      = nombre.Trim();
        IdDocumentoIdentidad = idDocumentoIdentidad;
        NumDocumento         = numDocumento?.Trim();
        CodValidadorDoc      = codValidadorDoc?.Trim();
        IdPais               = idPais;
        UsuarioModificador   = usuarioModificador;
        FechaModificacion    = DateTime.Now;

        var local = _clienteLocales.FirstOrDefault();
        if (local is not null)
            local.Update(direccionLocal, telefono1);

        return Result.Success();
    }
}
```

### `Domain/Clientes/ClienteLocal.cs`

```csharp
namespace DataConsulting.PuntoVentaComercial.Domain.Clientes;

public sealed class ClienteLocal : Entity
{
    public int IdCliente { get; private set; }
    public short IdSucursal { get; private set; }
    public string DireccionLocal { get; private set; }
    public string? Telefono1 { get; private set; }
    public int IdTipoCliente { get; private set; }
    public string Estado { get; private set; }   // 'A' / 'I'

    private ClienteLocal() { }  // EF Core

    internal static ClienteLocal Create(
        string direccionLocal,
        string? telefono1,
        short idSucursal) => new()
    {
        DireccionLocal = direccionLocal.Trim(),
        Telefono1      = telefono1?.Trim(),
        IdSucursal     = idSucursal,
        IdTipoCliente  = (int)ECliente.Varios,
        Estado         = "A"
    };

    internal void Update(string direccionLocal, string? telefono1)
    {
        DireccionLocal = direccionLocal.Trim();
        Telefono1      = telefono1?.Trim();
    }
}
```

---

## PASO 3 — DOMAIN: Errores

### `Domain/Clientes/ClienteErrors.cs`

```csharp
namespace DataConsulting.PuntoVentaComercial.Domain.Clientes;

public static class ClienteErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Cliente.NotFound", $"El cliente con ID {id} no existe.");

    public static Error DocumentoDuplicado(string numDoc) =>
        Error.Conflict("Cliente.DocumentoDuplicado", $"Ya existe un cliente con el documento {numDoc}.");

    public static Error DniInvalido(string numDoc) =>
        Error.Problem("Cliente.DniInvalido", $"El DNI {numDoc} con su código validador no es válido.");

    public static Error RucInvalido(string ruc) =>
        Error.Problem("Cliente.RucInvalido", $"El RUC {ruc} no es válido.");

    public static readonly Error NoEditableDesdePos =
        Error.Problem("Cliente.NoEditableDesdePos", "Este cliente fue creado desde mantenimiento de trabajadores y no puede modificarse desde el POS.");

    public static readonly Error PaisRequerido =
        Error.Problem("Cliente.PaisRequerido", "Debe especificar el país de origen.");

    public static readonly Error NombreRequerido =
        Error.Problem("Cliente.NombreRequerido", "Debe ingresar el nombre del cliente.");

    public static readonly Error DireccionRequerida =
        Error.Problem("Cliente.DireccionRequerida", "Debe ingresar la dirección del cliente.");

    public static readonly Error SunatNoDisponible =
        Error.Problem("Cliente.SunatNoDisponible", "El servicio de consulta SUNAT no está disponible en este momento.");
}
```

---

## PASO 4 — DOMAIN: Repositorio

### `Domain/Clientes/IClienteRepository.cs`

```csharp
namespace DataConsulting.PuntoVentaComercial.Domain.Clientes;

public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(int idCliente, CancellationToken ct);
    Task<bool> ExistsByDocumentoAsync(int idDocIdentidad, string numDocumento, CancellationToken ct);
    Task<(IList<Cliente> Items, int Total)> SearchAsync(
        string? nombre,
        string? numDocumento,
        short? idPais,
        int? idDocIdentidad,
        int page,
        int pageSize,
        CancellationToken ct);
    Task<IList<ClienteLocal>> GetAddressesByClienteIdAsync(int idCliente, CancellationToken ct);
    Task<int> GetNextIdAsync(CancellationToken ct);
    void Add(Cliente cliente);
}
```

---

## PASO 5 — APPLICATION: Abstracción SUNAT

### `Application/Abstractions/Services/ISunatClientLookupService.cs`

```csharp
namespace DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;

public sealed record SunatClienteInfo(
    string Ruc,
    string RazonSocial,
    string Direccion,
    string Estado,      // "ACTIVO" / "BAJA"
    string Condicion);  // "HABIDO" / "NO HABIDO"

public interface ISunatClientLookupService
{
    Task<Result<SunatClienteInfo>> LookupAsync(string ruc, CancellationToken ct);
}
```

---

## PASO 6 — APPLICATION: Queries

### `Application/Features/Clientes/Queries/GetClienteById/GetClienteByIdQuery.cs`
```csharp
public sealed record GetClienteByIdQuery(int IdCliente) : IQuery<GetClienteByIdResponse>;
```

### `Application/Features/Clientes/Queries/GetClienteById/GetClienteByIdResponse.cs`
```csharp
public sealed record GetClienteByIdResponse(
    int IdCliente,
    string Nombre,
    string NombreComercial,
    int? IdDocumentoIdentidad,
    string? NumDocumento,
    string? CodValidadorDoc,
    short IdPais,
    string EstadoCliente,
    bool EsEditableDesdePos,   // IdTrabajadorRef == null || IdTrabajadorRef == 0
    IList<ClienteLocalResponse> ClienteLocales);

public sealed record ClienteLocalResponse(
    int IdClienteLocal,
    short IdSucursal,
    string DireccionLocal,
    string? Telefono1,
    string Estado);
```

### `Application/Features/Clientes/Queries/GetClienteById/GetClienteByIdQueryHandler.cs`
```csharp
internal sealed class GetClienteByIdQueryHandler(IClienteRepository repository)
    : IQueryHandler<GetClienteByIdQuery, GetClienteByIdResponse>
{
    public async Task<Result<GetClienteByIdResponse>> Handle(
        GetClienteByIdQuery request, CancellationToken ct)
    {
        var cliente = await repository.GetByIdAsync(request.IdCliente, ct);
        if (cliente is null)
            return Result.Failure<GetClienteByIdResponse>(ClienteErrors.NotFound(request.IdCliente));

        return Result.Success(MapToResponse(cliente));
    }

    private static GetClienteByIdResponse MapToResponse(Cliente c) => new(
        c.Id,
        c.Nombre,
        c.NombreComercial,
        c.IdDocumentoIdentidad,
        c.NumDocumento,
        c.CodValidadorDoc,
        c.IdPais,
        c.EstadoCliente,
        c.IdTrabajadorRef is null or 0,
        c.ClienteLocales.Select(l => new ClienteLocalResponse(
            l.Id, l.IdSucursal, l.DireccionLocal, l.Telefono1, l.Estado)).ToList());
}
```

---

### `Application/Features/Clientes/Queries/SearchClientes/SearchClientesQuery.cs`
```csharp
public sealed record SearchClientesQuery(
    string? Nombre,
    string? NumDocumento,
    short? IdPais,
    int? IdDocIdentidad,
    int Page = 1,
    int PageSize = 20) : IQuery<SearchClientesResponse>;
```

### `Application/Features/Clientes/Queries/SearchClientes/SearchClientesResponse.cs`
```csharp
public sealed record SearchClientesResponse(
    IList<ClienteResumenResponse> Items,
    int Total,
    int Page,
    int PageSize);

public sealed record ClienteResumenResponse(
    int IdCliente,
    string Nombre,
    string? NumDocumento,
    string? TipoDocumento,
    string? DireccionLocal,
    string? Telefono1,
    string EstadoCliente);
```

### `Application/Features/Clientes/Queries/SearchClientes/SearchClientesQueryHandler.cs`
```csharp
internal sealed class SearchClientesQueryHandler(IClienteRepository repository)
    : IQueryHandler<SearchClientesQuery, SearchClientesResponse>
{
    public async Task<Result<SearchClientesResponse>> Handle(
        SearchClientesQuery request, CancellationToken ct)
    {
        var (items, total) = await repository.SearchAsync(
            request.Nombre, request.NumDocumento,
            request.IdPais, request.IdDocIdentidad,
            request.Page, request.PageSize, ct);

        var response = new SearchClientesResponse(
            items.Select(MapToResumen).ToList(),
            total, request.Page, request.PageSize);

        return Result.Success(response);
    }

    private static ClienteResumenResponse MapToResumen(Cliente c)
    {
        var local = c.ClienteLocales.FirstOrDefault();
        return new(c.Id, c.Nombre, c.NumDocumento, null,
            local?.DireccionLocal, local?.Telefono1, c.EstadoCliente);
    }
}
```

---

### `Application/Features/Clientes/Queries/GetClienteAddresses/GetClienteAddressesQuery.cs`
```csharp
public sealed record GetClienteAddressesQuery(int IdCliente)
    : IQuery<IList<ClienteLocalResponse>>;
```

### `Application/Features/Clientes/Queries/GetClienteAddresses/GetClienteAddressesQueryHandler.cs`
```csharp
internal sealed class GetClienteAddressesQueryHandler(IClienteRepository repository)
    : IQueryHandler<GetClienteAddressesQuery, IList<ClienteLocalResponse>>
{
    public async Task<Result<IList<ClienteLocalResponse>>> Handle(
        GetClienteAddressesQuery request, CancellationToken ct)
    {
        var addresses = await repository.GetAddressesByClienteIdAsync(request.IdCliente, ct);

        var response = addresses.Select(l => new ClienteLocalResponse(
            l.Id, l.IdSucursal, l.DireccionLocal, l.Telefono1, l.Estado)).ToList();

        return Result.Success<IList<ClienteLocalResponse>>(response);
    }
}
```

---

### `Application/Features/Clientes/Queries/LookupRucSunat/LookupRucSunatQuery.cs`
```csharp
public sealed record LookupRucSunatQuery(string Ruc) : IQuery<LookupRucSunatResponse>;
```

### `Application/Features/Clientes/Queries/LookupRucSunat/LookupRucSunatResponse.cs`
```csharp
public sealed record LookupRucSunatResponse(
    string Ruc,
    string RazonSocial,
    string Direccion,
    string Estado,
    string Condicion);
```

### `Application/Features/Clientes/Queries/LookupRucSunat/LookupRucSunatQueryHandler.cs`
```csharp
internal sealed class LookupRucSunatQueryHandler(ISunatClientLookupService sunatService)
    : IQueryHandler<LookupRucSunatQuery, LookupRucSunatResponse>
{
    public async Task<Result<LookupRucSunatResponse>> Handle(
        LookupRucSunatQuery request, CancellationToken ct)
    {
        if (!RucValidator.EsValido(request.Ruc))
            return Result.Failure<LookupRucSunatResponse>(ClienteErrors.RucInvalido(request.Ruc));

        var result = await sunatService.LookupAsync(request.Ruc, ct);
        if (result.IsFailure)
            return Result.Failure<LookupRucSunatResponse>(result.Error);

        var info = result.Value;
        return Result.Success(new LookupRucSunatResponse(
            info.Ruc, info.RazonSocial, info.Direccion, info.Estado, info.Condicion));
    }
}
```

---

## PASO 7 — APPLICATION: Commands

### `Application/Features/Clientes/Commands/CreateCliente/CreateClienteCommand.cs`
```csharp
public sealed record CreateClienteCommand(
    string Nombre,
    int? IdDocumentoIdentidad,
    string? NumDocumento,
    string? CodValidadorDoc,
    short IdPais,
    string DireccionLocal,
    string? Telefono1,
    short IdSucursal,
    string? NombreComercial = null) : ICommand<int>;
```

### `Application/Features/Clientes/Commands/CreateCliente/CreateClienteCommandValidator.cs`
```csharp
public sealed class CreateClienteCommandValidator : AbstractValidator<CreateClienteCommand>
{
    public CreateClienteCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del cliente es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.IdPais)
            .GreaterThan((short)0).WithMessage("Debe especificar el país.");

        RuleFor(x => x.DireccionLocal)
            .NotEmpty().WithMessage("La dirección es requerida.");

        RuleFor(x => x.IdSucursal)
            .GreaterThan((short)0).WithMessage("Debe especificar la sucursal.");

        // Validación DNI
        When(x => x.IdDocumentoIdentidad == (int)ETipoDocIdentidad.DNI, () =>
        {
            RuleFor(x => x.NumDocumento)
                .NotEmpty().WithMessage("El número de DNI es requerido.")
                .Length(8).WithMessage("El DNI debe tener 8 dígitos.");

            RuleFor(x => x.CodValidadorDoc)
                .NotEmpty().WithMessage("El código validador del DNI es requerido.");

            RuleFor(x => x)
                .Must(x => DniValidator.EsValido(x.NumDocumento!, x.CodValidadorDoc!))
                .WithMessage(x => $"El DNI {x.NumDocumento} con código validador {x.CodValidadorDoc} no es válido.");
        });

        // Validación RUC
        When(x => x.IdDocumentoIdentidad == (int)ETipoDocIdentidad.RUC, () =>
        {
            RuleFor(x => x.NumDocumento)
                .NotEmpty().WithMessage("El número de RUC es requerido.")
                .Must(ruc => RucValidator.EsValido(ruc!))
                .WithMessage(x => $"El RUC {x.NumDocumento} no es válido.");
        });

        // Si se elige tipo de documento, el número es obligatorio
        When(x => x.IdDocumentoIdentidad.HasValue && x.IdDocumentoIdentidad > 0, () =>
        {
            RuleFor(x => x.NumDocumento)
                .NotEmpty().WithMessage("Debe ingresar el número de documento.");
        });
    }
}
```

### `Application/Features/Clientes/Commands/CreateCliente/CreateClienteCommandHandler.cs`
```csharp
internal sealed class CreateClienteCommandHandler(
    IClienteRepository repository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateClienteCommand, int>
{
    public async Task<Result<int>> Handle(
        CreateClienteCommand request, CancellationToken ct)
    {
        // Verificar duplicado por documento
        if (request.IdDocumentoIdentidad.HasValue && !string.IsNullOrWhiteSpace(request.NumDocumento))
        {
            bool existe = await repository.ExistsByDocumentoAsync(
                request.IdDocumentoIdentidad.Value, request.NumDocumento, ct);
            if (existe)
                return Result.Failure<int>(ClienteErrors.DocumentoDuplicado(request.NumDocumento));
        }

        int nuevoId = await repository.GetNextIdAsync(ct);

        var result = Cliente.Create(
            request.Nombre,
            request.IdDocumentoIdentidad,
            request.NumDocumento,
            request.CodValidadorDoc,
            request.IdPais,
            request.DireccionLocal,
            request.Telefono1,
            request.IdSucursal,
            usuarioCreador: "SISTEMA"); // TODO: reemplazar con usuario de sesión cuando se implemente Auth

        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        repository.Add(result.Value);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(result.Value.Id);
    }
}
```

---

### `Application/Features/Clientes/Commands/UpdateCliente/UpdateClienteCommand.cs`
```csharp
public sealed record UpdateClienteCommand(
    int IdCliente,
    string Nombre,
    int? IdDocumentoIdentidad,
    string? NumDocumento,
    string? CodValidadorDoc,
    short IdPais,
    string DireccionLocal,
    string? Telefono1) : ICommand;
```

### `Application/Features/Clientes/Commands/UpdateCliente/UpdateClienteCommandValidator.cs`
```csharp
public sealed class UpdateClienteCommandValidator : AbstractValidator<UpdateClienteCommand>
{
    public UpdateClienteCommandValidator()
    {
        RuleFor(x => x.IdCliente).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.IdPais).GreaterThan((short)0);
        RuleFor(x => x.DireccionLocal).NotEmpty();

        When(x => x.IdDocumentoIdentidad == (int)ETipoDocIdentidad.DNI, () =>
        {
            RuleFor(x => x.NumDocumento).NotEmpty().Length(8);
            RuleFor(x => x.CodValidadorDoc).NotEmpty();
            RuleFor(x => x)
                .Must(x => DniValidator.EsValido(x.NumDocumento!, x.CodValidadorDoc!))
                .WithMessage(x => $"El DNI {x.NumDocumento} con código validador {x.CodValidadorDoc} no es válido.");
        });

        When(x => x.IdDocumentoIdentidad == (int)ETipoDocIdentidad.RUC, () =>
        {
            RuleFor(x => x.NumDocumento)
                .NotEmpty()
                .Must(ruc => RucValidator.EsValido(ruc!))
                .WithMessage(x => $"El RUC {x.NumDocumento} no es válido.");
        });
    }
}
```

### `Application/Features/Clientes/Commands/UpdateCliente/UpdateClienteCommandHandler.cs`
```csharp
internal sealed class UpdateClienteCommandHandler(
    IClienteRepository repository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateClienteCommand>
{
    public async Task<Result> Handle(
        UpdateClienteCommand request, CancellationToken ct)
    {
        var cliente = await repository.GetByIdAsync(request.IdCliente, ct);
        if (cliente is null)
            return Result.Failure(ClienteErrors.NotFound(request.IdCliente));

        var result = cliente.Update(
            request.Nombre,
            request.IdDocumentoIdentidad,
            request.NumDocumento,
            request.CodValidadorDoc,
            request.IdPais,
            request.DireccionLocal,
            request.Telefono1,
            usuarioModificador: "SISTEMA"); // TODO: reemplazar con usuario de sesión cuando se implemente Auth

        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
```

---

## PASO 8 — DOMAIN: Validadores

### `Domain/Clientes/DniValidator.cs`
```csharp
namespace DataConsulting.PuntoVentaComercial.Domain.Clientes;

/// <summary>
/// Validador de DNI peruano.
/// Fuente: FunctionsBR.ValidarDNIPeru del sistema legacy.
/// El DNI peruano tiene 8 dígitos + 1 código validador (letra o dígito).
/// </summary>
public static class DniValidator
{
    private static readonly int[] Factores = [3, 2, 7, 6, 5, 4, 3, 2];
    private static readonly char[] Resultados = ['6', '7', '8', '9', '0', '1', '1', '2', '3', '4', '5'];

    public static bool EsValido(string numDni, string codValidador)
    {
        if (string.IsNullOrWhiteSpace(numDni) || numDni.Length != 8)
            return false;
        if (string.IsNullOrWhiteSpace(codValidador))
            return false;
        if (!numDni.All(char.IsDigit))
            return false;

        int suma = 0;
        for (int i = 0; i < 8; i++)
            suma += (numDni[i] - '0') * Factores[i];

        int residuo = suma % 11;
        return Resultados[residuo] == char.ToUpper(codValidador[0]);
    }
}
```

### `Domain/Clientes/RucValidator.cs`
```csharp
namespace DataConsulting.PuntoVentaComercial.Domain.Clientes;

/// <summary>
/// Validador de RUC peruano.
/// Fuente: UIFunctions.ValidarRUC del sistema legacy.
/// </summary>
public static class RucValidator
{
    private static readonly int[] Factores = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2];

    public static bool EsValido(string ruc)
    {
        if (string.IsNullOrWhiteSpace(ruc) || ruc.Length != 11)
            return false;
        if (!ruc.All(char.IsDigit))
            return false;

        string prefijo = ruc[..2];
        if (prefijo != "10" && prefijo != "20")
            return false;

        int suma = 0;
        for (int i = 0; i < 10; i++)
            suma += (ruc[i] - '0') * Factores[i];

        int digitoVerificador = 11 - (suma % 11);
        if (digitoVerificador >= 10) digitoVerificador -= 10;

        return digitoVerificador == (ruc[10] - '0');
    }
}
```

---

## PASO 9 — INFRASTRUCTURE: Configuraciones EF Core

### `Infrastructure/Configurations/ClienteConfiguration.cs`
```csharp
public sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Cliente", "dbo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("IdCliente").ValueGeneratedNever();

        builder.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(x => x.NombreComercial).HasMaxLength(100);
        builder.Property(x => x.NumDocumento).HasMaxLength(20);
        builder.Property(x => x.CodValidadorDoc).HasMaxLength(3);
        builder.Property(x => x.EstadoCliente).HasMaxLength(1).IsFixedLength().IsRequired();
        builder.Property(x => x.Observaciones).HasMaxLength(200);
        builder.Property(x => x.UsuarioCreador).HasMaxLength(20).IsRequired();
        builder.Property(x => x.UsuarioModificador).HasMaxLength(20);
        builder.Property(x => x.CreditoMaximo).HasColumnType("decimal(18,2)");

        builder.HasMany(x => x.ClienteLocales)
               .WithOne()
               .HasForeignKey(x => x.IdCliente);
    }
}
```

### `Infrastructure/Configurations/ClienteLocalConfiguration.cs`
```csharp
public sealed class ClienteLocalConfiguration : IEntityTypeConfiguration<ClienteLocal>
{
    public void Configure(EntityTypeBuilder<ClienteLocal> builder)
    {
        // ⚠️ VERIFICAR nombre exacto de la tabla en BD antes de ejecutar
        builder.ToTable("ClienteLocal", "dbo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("IdClienteLocal").ValueGeneratedNever();

        builder.Property(x => x.DireccionLocal).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Telefono1).HasMaxLength(20);
        builder.Property(x => x.Estado).HasMaxLength(1).IsFixedLength().IsRequired();
        builder.Property(x => x.IdCliente).IsRequired();
    }
}
```

---

## PASO 10 — INFRASTRUCTURE: Repositorio

### `Infrastructure/Repositories/ClienteRepository.cs`
```csharp
internal sealed class ClienteRepository(ApplicationDbContext context)
    : Repository<Cliente>(context), IClienteRepository
{
    public async Task<Cliente?> GetByIdAsync(int idCliente, CancellationToken ct) =>
        await context.Clientes
            .Include(x => x.ClienteLocales)
            .FirstOrDefaultAsync(x => x.Id == idCliente, ct);

    public async Task<bool> ExistsByDocumentoAsync(
        int idDocIdentidad, string numDocumento, CancellationToken ct) =>
        await context.Clientes.AnyAsync(
            x => x.IdDocumentoIdentidad == idDocIdentidad &&
                 x.NumDocumento == numDocumento, ct);

    public async Task<(IList<Cliente> Items, int Total)> SearchAsync(
        string? nombre, string? numDocumento,
        short? idPais, int? idDocIdentidad,
        int page, int pageSize, CancellationToken ct)
    {
        var query = context.Clientes
            .Include(x => x.ClienteLocales)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(x => x.Nombre.Contains(nombre));

        if (!string.IsNullOrWhiteSpace(numDocumento))
            query = query.Where(x => x.NumDocumento == numDocumento);

        if (idPais.HasValue)
            query = query.Where(x => x.IdPais == idPais.Value);

        if (idDocIdentidad.HasValue)
            query = query.Where(x => x.IdDocumentoIdentidad == idDocIdentidad.Value);

        int total = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IList<ClienteLocal>> GetAddressesByClienteIdAsync(
        int idCliente, CancellationToken ct) =>
        await context.ClienteLocales
            .AsNoTracking()
            .Where(x => x.IdCliente == idCliente)
            .ToListAsync(ct);

    public async Task<int> GetNextIdAsync(CancellationToken ct)
    {
        int max = await context.Clientes.MaxAsync(x => (int?)x.Id, ct) ?? 0;
        return max + 1;
    }
}
```

---

## PASO 11 — INFRASTRUCTURE: Stub SUNAT

### `Infrastructure/Services/SunatClientLookupStub.cs`
```csharp
internal sealed class SunatClientLookupStub : ISunatClientLookupService
{
    public Task<Result<SunatClienteInfo>> LookupAsync(string ruc, CancellationToken ct) =>
        Task.FromResult(Result.Failure<SunatClienteInfo>(ClienteErrors.SunatNoDisponible));
}
```

---

## PASO 12 — INFRASTRUCTURE: Actualizar archivos existentes

### `Infrastructure/Database/ApplicationDbContext.cs`
**AGREGAR** los siguientes DbSets:
```csharp
public DbSet<Cliente>      Clientes       { get; set; }
public DbSet<ClienteLocal> ClienteLocales { get; set; }
```

### `Infrastructure/DependencyInjection.cs`
**AGREGAR** en el método `AddInfrastructure`:
```csharp
services.AddScoped<IClienteRepository, ClienteRepository>();
services.AddScoped<ISunatClientLookupService, SunatClientLookupStub>();
```

---

## PASO 13 — API: Controller y Request DTOs

### `API/Controllers/Clientes/CreateClienteRequest.cs`
```csharp
public sealed record CreateClienteRequest(
    string Nombre,
    int? IdDocumentoIdentidad,
    string? NumDocumento,
    string? CodValidadorDoc,
    short IdPais,
    string DireccionLocal,
    string? Telefono1,
    short IdSucursal,
    string? NombreComercial = null);
```

### `API/Controllers/Clientes/UpdateClienteRequest.cs`
```csharp
public sealed record UpdateClienteRequest(
    string Nombre,
    int? IdDocumentoIdentidad,
    string? NumDocumento,
    string? CodValidadorDoc,
    short IdPais,
    string DireccionLocal,
    string? Telefono1);
```

### `API/Controllers/Clientes/ClientesController.cs`
```csharp
[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/clientes")]
public sealed class ClientesController(
    ICommandHandler<CreateClienteCommand, int> createHandler,
    ICommandHandler<UpdateClienteCommand> updateHandler,
    IQueryHandler<GetClienteByIdQuery, GetClienteByIdResponse> getByIdHandler,
    IQueryHandler<SearchClientesQuery, SearchClientesResponse> searchHandler,
    IQueryHandler<GetClienteAddressesQuery, IList<ClienteLocalResponse>> addressesHandler,
    IQueryHandler<LookupRucSunatQuery, LookupRucSunatResponse> sunatHandler)
    : ControllerBase
{
    // GET /api/v1/clientes?nombre=&numDocumento=&idPais=&idDocIdentidad=&page=1&pageSize=20
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? nombre,
        [FromQuery] string? numDocumento,
        [FromQuery] short? idPais,
        [FromQuery] int? idDocIdentidad,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await searchHandler.Handle(
            new SearchClientesQuery(nombre, numDocumento, idPais, idDocIdentidad, page, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    // POST /api/v1/clientes
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateClienteRequest request,
        CancellationToken ct)
    {
        var command = new CreateClienteCommand(
            request.Nombre, request.IdDocumentoIdentidad, request.NumDocumento,
            request.CodValidadorDoc, request.IdPais, request.DireccionLocal,
            request.Telefono1, request.IdSucursal, request.NombreComercial);

        var result = await createHandler.Handle(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    // GET /api/v1/clientes/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await getByIdHandler.Handle(new GetClienteByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    // PUT /api/v1/clientes/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateClienteRequest request,
        CancellationToken ct)
    {
        var command = new UpdateClienteCommand(
            id, request.Nombre, request.IdDocumentoIdentidad, request.NumDocumento,
            request.CodValidadorDoc, request.IdPais, request.DireccionLocal, request.Telefono1);

        var result = await updateHandler.Handle(command, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    // GET /api/v1/clientes/{id}/addresses
    [HttpGet("{id:int}/addresses")]
    public async Task<IActionResult> GetAddresses(int id, CancellationToken ct)
    {
        var result = await addressesHandler.Handle(new GetClienteAddressesQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    // GET /api/v1/clientes/sunat/{ruc}
    [HttpGet("sunat/{ruc}")]
    public async Task<IActionResult> LookupSunat(string ruc, CancellationToken ct)
    {
        var result = await sunatHandler.Handle(new LookupRucSunatQuery(ruc), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
```

---

## RESUMEN DE ARCHIVOS

| # | Acción | Archivo |
|---|---|---|
| 1 | MODIFICAR | `Domain/Enums/BusinessEnums.cs` — agregar 3 enums |
| 2 | MODIFICAR | `Domain/Enums/SharedEnums.cs` — agregar ETipoDocIdentidad |
| 3 | CREAR | `Domain/Clientes/Cliente.cs` |
| 4 | CREAR | `Domain/Clientes/ClienteLocal.cs` |
| 5 | CREAR | `Domain/Clientes/ClienteErrors.cs` |
| 6 | CREAR | `Domain/Clientes/IClienteRepository.cs` |
| 7 | CREAR | `Domain/Clientes/DniValidator.cs` |
| 8 | CREAR | `Domain/Clientes/RucValidator.cs` |
| 9 | CREAR | `Application/Abstractions/Services/ISunatClientLookupService.cs` |
| 10 | CREAR | `Application/Features/Clientes/Queries/GetClienteById/` (3 archivos) |
| 11 | CREAR | `Application/Features/Clientes/Queries/SearchClientes/` (3 archivos) |
| 12 | CREAR | `Application/Features/Clientes/Queries/GetClienteAddresses/` (2 archivos) |
| 13 | CREAR | `Application/Features/Clientes/Queries/LookupRucSunat/` (3 archivos) |
| 14 | CREAR | `Application/Features/Clientes/Commands/CreateCliente/` (3 archivos) |
| 15 | CREAR | `Application/Features/Clientes/Commands/UpdateCliente/` (3 archivos) |
| 16 | CREAR | `Infrastructure/Configurations/ClienteConfiguration.cs` |
| 17 | CREAR | `Infrastructure/Configurations/ClienteLocalConfiguration.cs` |
| 18 | CREAR | `Infrastructure/Repositories/ClienteRepository.cs` |
| 19 | CREAR | `Infrastructure/Services/SunatClientLookupStub.cs` |
| 20 | MODIFICAR | `Infrastructure/Database/ApplicationDbContext.cs` — agregar 2 DbSets |
| 21 | MODIFICAR | `Infrastructure/DependencyInjection.cs` — registrar repo y stub |
| 22 | CREAR | `API/Controllers/Clientes/CreateClienteRequest.cs` |
| 23 | CREAR | `API/Controllers/Clientes/UpdateClienteRequest.cs` |
| 24 | CREAR | `API/Controllers/Clientes/ClientesController.cs` |

**Total: 24 archivos — 8 modificaciones, 16 creaciones**