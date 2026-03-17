# Análisis Completo del Repositorio — PuntoVentaComercial

> Generado: 2026-03-17

---

## 1. ESTRUCTURA DE LA SOLUCIÓN

```
D:\Sistemas ERP\api-net
├── PuntoVentaComercial.slnx
├── plan-backend-pos.md
└── src/
    ├── DataConsulting.PuntoVentaComercial.API/
    ├── DataConsulting.PuntoVentaComercial.Application/
    ├── DataConsulting.PuntoVentaComercial.Domain/
    └── DataConsulting.PuntoVentaComercial.Infrastructure/
```

---

## 2. ÁRBOL POR PROYECTO

### API — `DataConsulting.PuntoVentaComercial.API`

```
src/DataConsulting.PuntoVentaComercial.API/
├── Controllers/
│   ├── ClasesSunat/
│   │   ├── ClaseSunatController.cs
│   │   └── CreateClaseSunatRequest.cs
│   └── SegmentosSunat/
│       ├── SegmentoSunatController.cs
│       └── CreateSegmentoSunatRequest.cs
├── Extensions/
│   ├── ApplicationBuilderExtensions.cs
│   └── SwaggerExtensions.cs
├── Middleware/
│   ├── GlobalExceptionHandler.cs
│   └── LogContextTraceLoggingMiddleware.cs
├── Utils/
│   └── ApiVersions.cs
└── Program.cs
```

### Application — `DataConsulting.PuntoVentaComercial.Application`

```
src/DataConsulting.PuntoVentaComercial.Application/
├── Abstractions/
│   ├── Behaviors/
│   │   ├── LoggingDecorator.cs
│   │   └── ValidationDecorator.cs
│   ├── Data/
│   │   ├── IApplicationDbContext.cs
│   │   ├── IDbConnectionFactory.cs
│   │   └── IUnitOfWork.cs
│   └── Messaging/
│       ├── ICommand.cs
│       ├── ICommandHandler.cs
│       ├── IQuery.cs
│       └── IQueryHandler.cs
├── Exceptions/
│   ├── ValidationError.cs
│   └── ValidationException.cs
├── Features/
│   ├── ClasesSunat/
│   │   └── Commands/CreateClaseSunat/
│   │       ├── CreateClaseSunatCommand.cs
│   │       └── CreateClaseSunatCommandHandler.cs
│   └── SegmentosSunat/
│       ├── Commands/CreateSegmentoSunat/
│       │   ├── CreateSegmentoSunatCommand.cs
│       │   ├── CreateSegmentoSunatCommandHandler.cs
│       │   └── CreateSegmentoSunatCommandValidator.cs
│       └── Queries/
│           ├── GetAllSegmentosSunat/
│           │   ├── GetAllSegmentosSunatQuery.cs
│           │   ├── GetAllSegmentosSunatQueryHandler.cs
│           │   └── GetAllSegmentosSunatResponse.cs
│           ├── GetSegmentoSunatById/
│           │   ├── GetSegmentoSunatByIdQuery.cs
│           │   ├── GetSegmentoSunatByIdQueryHandler.cs
│           │   └── GetSegmentoSunatByIdResponse.cs
│           └── GetSegmentoFamiliaClase/
│               ├── GetSegmentoFamiliaClaseQuery.cs
│               ├── GetSegmentoFamiliaClaseResponse.cs
│               ├── V1/  ← LINQ
│               │   ├── GetSegmentoFamiliaClaseV1Query.cs
│               │   └── GetSegmentoFamiliaClaseV1QueryHandler.cs
│               └── V2/  ← Dapper
│                   ├── GetSegmentoFamiliaClaseV2Query.cs
│                   └── GetSegmentoFamiliaClaseV2QueryHandler.cs
├── Services/
│   └── ClasesSunat/
│       ├── IClaseSunatService.cs
│       └── ClaseSunatService.cs
└── DependencyInjection.cs
```

### Domain — `DataConsulting.PuntoVentaComercial.Domain`

```
src/DataConsulting.PuntoVentaComercial.Domain/
├── Abstractions/
│   ├── Entity.cs
│   ├── Error.cs
│   ├── ErrorType.cs
│   └── Result.cs
├── ClasesSunat/
│   ├── ClaseSunat.cs
│   ├── ClaseSunatErrors.cs
│   └── IClaseSunatRepository.cs
├── FamiliasSunat/
│   ├── FamiliaSunat.cs
│   ├── FamiliaSunatErrors.cs
│   └── IFamiliaSunatRepository.cs
├── SegmentosSunat/
│   ├── SegmentoSunat.cs
│   ├── SegmentoSunatError.cs
│   └── ISegmentoSunatRepository.cs
├── Constants/
│   └── FiscalConstants.cs
└── Enums/
    ├── BusinessEnums.cs
    ├── FiscalEnums.cs
    ├── MappingEnums.cs
    └── SharedEnums.cs
```

### Infrastructure — `DataConsulting.PuntoVentaComercial.Infrastructure`

```
src/DataConsulting.PuntoVentaComercial.Infrastructure/
├── Configurations/
│   ├── ClaseSunatConfiguration.cs
│   ├── FamiliaSunatConfiguration.cs
│   └── SegmentoSunatConfiguration.cs
├── Database/
│   ├── ApplicationDbContext.cs
│   └── DbConnectionFactory.cs
├── Repositories/
│   ├── Repository.cs               ← base genérico
│   ├── ClaseSunatRepository.cs
│   ├── FamiliaSunatRepository.cs
│   └── SegmentoSunatRepository.cs
└── DependencyInjection.cs
```

---

## 3. ARCHIVOS CLAVE

### Application/DependencyInjection.cs

**Ruta:** `src/DataConsulting.PuntoVentaComercial.Application/DependencyInjection.cs`
**Propósito:** Registra handlers, validadores, servicios y decoradores usando Scrutor (sin MediatR).
**Patrón:** Auto-registration + Decorator pattern.

```csharp
// Escanea el ensamblado y registra:
// - IQueryHandler<,>            → implementaciones internas
// - ICommandHandler<,>          → implementaciones internas
// - ICommandHandler<>           → implementaciones internas
// - Clases terminadas en "Service"

// Aplica decoradores en cadena: Validation → Logging
services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator<,>.CommandHandler));
services.Decorate(typeof(ICommandHandler<>),  typeof(ValidationDecorator<>.CommandBaseHandler));
services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator<,>.CommandHandler));
services.Decorate(typeof(ICommandHandler<>),  typeof(LoggingDecorator<>.CommandBaseHandler));
services.Decorate(typeof(IQueryHandler<,>),   typeof(LoggingDecorator<,>.QueryHandler));

// Registra FluentValidation para el ensamblado
```

---

### Infrastructure/DependencyInjection.cs

**Ruta:** `src/DataConsulting.PuntoVentaComercial.Infrastructure/DependencyInjection.cs`
**Propósito:** Registra DbContext, repositorios, UnitOfWork, DbConnectionFactory y versionado de API.
**Patrón:** Extension method sobre IServiceCollection.

```csharp
services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(connectionString));

services.AddScoped<IApplicationDbContext>(sp =>
    sp.GetRequiredService<ApplicationDbContext>());
services.AddScoped<IUnitOfWork>(sp =>
    sp.GetRequiredService<ApplicationDbContext>());
services.AddSingleton<IDbConnectionFactory>(_ =>
    new DbConnectionFactory(connectionString));

// Repositorios
services.AddScoped<ISegmentoSunatRepository, SegmentoSunatRepository>();
services.AddScoped<IFamiliaSunatRepository,  FamiliaSunatRepository>();
services.AddScoped<IClaseSunatRepository,    ClaseSunatRepository>();

// API Versioning (Asp.Versioning)
services.AddApiVersioning(opts => { opts.DefaultApiVersion = new ApiVersion(1); ... })
        .AddApiExplorer(...);
```

---

### Handler de ejemplo — CreateSegmentoSunatCommandHandler

**Ruta:** `src/.../Application/Features/SegmentosSunat/Commands/CreateSegmentoSunat/CreateSegmentoSunatCommandHandler.cs`
**Propósito:** Crea un SegmentoSunat validando unicidad de código, calculando el siguiente ID y persistiendo.
**Interfaces usadas:** `ISegmentoSunatRepository`, `IUnitOfWork`
**Patrón:** CQRS Command Handler + Railway-Oriented (Result\<T\>)

```csharp
public sealed record CreateSegmentoSunatCommand(string Codigo, string Descripcion)
    : ICommand<int>;

internal sealed class CreateSegmentoSunatCommandHandler(
    ISegmentoSunatRepository repository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateSegmentoSunatCommand, int>
{
    public async Task<Result<int>> Handle(
        CreateSegmentoSunatCommand request,
        CancellationToken cancellationToken)
    {
        if (await repository.ExistsByCodigoAsync(request.Codigo, cancellationToken))
            return Result.Failure<int>(SegmentoSunatErrors.CodigoDuplicado(request.Codigo));

        int nuevoId = await repository.GetNextIdAsync(cancellationToken);

        var result = SegmentoSunat.Create(nuevoId, request.Codigo, request.Descripcion, 1, 1, DateTime.UtcNow);
        if (result.IsFailure) return Result.Failure<int>(result.Error);

        repository.Add(result.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
```

---

### AppDbContext

**Ruta:** `src/DataConsulting.PuntoVentaComercial.Infrastructure/Database/ApplicationDbContext.cs`
**Propósito:** Contexto principal de EF Core. Implementa `IApplicationDbContext` e `IUnitOfWork`.
**Clases/Interfaces:** `ApplicationDbContext : DbContext, IApplicationDbContext, IUnitOfWork`

```csharp
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext, IUnitOfWork
{
    public DbSet<SegmentoSunat> SegmentosSunat { get; set; }
    public DbSet<FamiliaSunat>  FamiliasSunat  { get; set; }
    public DbSet<ClaseSunat>    ClasesSunat    { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct)
        => await Database.BeginTransactionAsync(ct);
}
```

---

## 4. PATRONES Y TECNOLOGÍAS

| Patrón / Tecnología           | Implementación                                                                                  |
|-------------------------------|--------------------------------------------------------------------------------------------------|
| **Clean Architecture**        | 4 capas: Domain → Application → Infrastructure → API. Dependencias hacia adentro.               |
| **CQRS (sin MediatR)**        | Interfaces propias: `ICommand`, `IQuery`, `ICommandHandler<>`, `IQueryHandler<,>`               |
| **Repository Pattern**        | Base genérica `Repository<T : Entity>` + repositorios específicos por entidad                   |
| **Unit of Work**              | `IUnitOfWork` con `SaveChangesAsync` + `BeginTransactionAsync`, implementado en `ApplicationDbContext` |
| **Decorator Pattern**         | `ValidationDecorator` y `LoggingDecorator` envuelven handlers (Scrutor `Decorate()`)            |
| **Railway-Oriented (Result)** | `Result` y `Result<T>` con `Error`, `ErrorType`. Sin excepciones en flujo normal.               |
| **Auto-registration**         | Scrutor escanea ensamblado para registrar handlers y servicios automáticamente                  |
| **FluentValidation**          | Validadores por comando (ej. `CreateSegmentoSunatCommandValidator`)                             |
| **EF Core Fluent API**        | `IEntityTypeConfiguration<T>` por entidad, `ApplyConfigurationsFromAssembly`                   |
| **Dapper**                    | Vía `IDbConnectionFactory` para queries de solo lectura (handlers V2)                          |
| **API Versioning**            | `Asp.Versioning` con URL segment (`/api/v{version}/...`)                                        |
| **Serilog**                   | Logging estructurado con Seq sink y `LogContext` en decoradores                                 |
| **Concurrency Token**         | `UpdateToken (smallint)` marcado como `IsConcurrencyToken()` en EF                             |
| **ValueGeneratedNever**       | IDs calculados manualmente via `GetNextIdAsync()` (no identity de BD)                          |
| **Target Framework**          | .NET 10.0                                                                                        |
| **Base de datos**             | SQL Server (`dbo` schema)                                                                        |

---

## 5. INTERFACES CLAVE

```csharp
// CQRS Messaging
ICommand                                         // marker sin respuesta
ICommand<TResponse>                              // marker con respuesta
ICommandHandler<TCommand>                        // → Task<Result>
ICommandHandler<TCommand, TResponse>             // → Task<Result<TResponse>>
IQuery<TResponse>                                // marker
IQueryHandler<TQuery, TResponse>                 // → Task<Result<TResponse>>

// Data
IApplicationDbContext                            // DbSets expuestos a Application
IUnitOfWork                                      // SaveChangesAsync + BeginTransactionAsync
IDbConnectionFactory                             // OpenConnectionAsync() → DbConnection (Dapper)

// Repositories (definidos en Domain)
ISegmentoSunatRepository
  GetByIdAsync / GetAllAsync / Add
  ExistsByCodigoAsync(codigo) / GetNextIdAsync

IClaseSunatRepository
  GetByIdAsync / Add / Remove
  ExistsByCodigoAsync(idFamilia, codigo) / GetNextIdAsync

IFamiliaSunatRepository
  (mismo patrón que los anteriores)
```

---

## 6. DOMINIO — Entidades y Errores

```csharp
// Base
abstract class Entity { int Id { get; init; } }

// SegmentoSunat : Entity
//   Código (varchar 10), Descripción (varchar 200), Estado (EEstado), UpdateToken
//   Factory: static Result<SegmentoSunat> Create(...)

// FamiliaSunat : Entity
//   IdSegmentoSunat (FK), mismo patrón

// ClaseSunat : Entity
//   IdFamiliaSunat (FK), mismo patrón

// Errores tipados (record)
//   SegmentoSunatErrors.CodigoDuplicado(codigo) → Error con ErrorType.Conflict
//   Error.NotFound / Error.Failure / Error.Problem / Error.Conflict (factory methods)

// EEstado: Todos = 0, Activo = 1, Inactivo = 2
// Auditoría: IdUsuarioCreador, FechaCreacion, IdUsuarioModificador, FechaModificacion
//            (smallint / smalldatetime)
```

---

## 7. ESQUEMA DE BD (mapeo EF Core)

| Entidad         | Tabla SQL            | PK                | FK                       |
|-----------------|----------------------|-------------------|--------------------------|
| SegmentoSunat   | dbo.SegmentoSunat    | IdSegmentoSunat   | —                        |
| FamiliaSunat    | dbo.FamiliaSunat     | IdFamiliaSunat    | IdSegmentoSunat          |
| ClaseSunat      | dbo.ClaseSunat       | IdClaseSunat      | IdFamiliaSunat           |

Campos comunes en todas las tablas:
- `Codigo` — varchar, único por scope
- `Descripcion` — varchar(200)
- `Estado` — smallint (enum EEstado)
- `UpdateToken` — smallint, token de concurrencia optimista
- `IdUsuarioCreador` / `FechaCreacion` — auditoría de creación
- `IdUsuarioModificador` / `FechaModificacion` — auditoría de modificación

> Los IDs **no son identity** de BD; se calculan con `GetNextIdAsync()` (MAX + 1 por código).
