# Contexto de sesión — B5 Ventas

> Archivo para continuar la conversación en una nueva máquina.
> Al iniciar sesión en Claude Code, di: **"lee CONTEXTO_SESION.md y continúa"**

---

## Estado actual (2026-03-17)

Módulo B5 Ventas **funcional**. Los 4 endpoints están operativos contra la BD real:
- `POST   /api/v1/ventas` — crear venta
- `GET    /api/v1/ventas/{id}` — obtener por ID
- `GET    /api/v1/ventas` — buscar con filtros y paginación
- `PUT    /api/v1/ventas/{id}/anular` — anular venta

---

## Stack y patrones

- .NET 10 / EF Core 10.0.3 / SQL Server
- CQRS sin MediatR: `ICommand<T>`, `ICommandHandler<,>`, `IQuery<T>`, `IQueryHandler<,>`
- Railway-Oriented: `Result<T>` — sin excepciones en flujo normal
- Scrutor para auto-registro de repositorios
- BD del schema: `D:\Sistemas ERP\BD_dbscopDC\dbscopDC\dbscopDC\dbo\Tables\*.sql`

---

## Archivos clave del módulo

### Domain
- `src/.../Domain/Ventas/Venta.cs` — aggregate root
- `src/.../Domain/Ventas/VentaDetalle.cs` — ítem de venta
- `src/.../Domain/Ventas/VentaPago.cs` — pago
- `src/.../Domain/Ventas/IVentaRepository.cs`
- `src/.../Domain/Ventas/VentaErrors.cs`

### Infrastructure
- `src/.../Infrastructure/Configurations/VentaConfiguration.cs`
- `src/.../Infrastructure/Configurations/VentaDetalleConfiguration.cs`
- `src/.../Infrastructure/Configurations/VentaPagoConfiguration.cs`
- `src/.../Infrastructure/Repositories/VentaRepository.cs`
- `src/.../Infrastructure/Database/ApplicationDbContext.cs`

### Application
- `src/.../Application/Features/Ventas/Commands/CreateVenta/`
- `src/.../Application/Features/Ventas/Commands/AnularVenta/`
- `src/.../Application/Features/Ventas/Queries/GetVentaById/`
- `src/.../Application/Features/Ventas/Queries/SearchVentas/`

### API
- `src/.../API/Controllers/Ventas/VentasController.cs`
- `src/.../API/Controllers/Ventas/CreateVentaRequest.cs`

---

## Decisiones de diseño críticas

### Semántica de Estado (legacy)
`EEstadoVenta`: **"A" = Aprobado** (activo), **"E" = Anulado**
- `Venta.Create()` → `Estado = "A"`
- `Venta.Anular()` → guard `!= "A"`, setea `"E"`

### Series de documento
- `NumSerie short?` = serie numérica (legacy, puede ser null)
- `NumSerieA string?` = serie alfanumérica como `'F001'`, `'B001'`
- El SP `GetNuevoCorrelativoDocumento` **siempre recibe `@NumSerie=0`** y usa `@NumSerieA`

### EF Core — Shadow Properties
Regla para columnas NOT NULL sin DEFAULT constraint en la BD:
```csharp
// NO funciona — EF omite la columna en INSERT cuando valor == sentinel(0)
builder.Property<byte>("Col").HasDefaultValue((byte)0);

// CORRECTO — ValueGeneratedNever() fuerza EF a siempre incluir el valor
builder.Property<byte>("Col").HasDefaultValue((byte)0).ValueGeneratedNever();
```
- Tipos nullable (`short?`, `decimal?`) → manejados por `ApplyShadowPropertyDefaults()` en ApplicationDbContext
- Tipos no-nullable (`byte`, `bool`, `decimal`) → requieren `.ValueGeneratedNever()`
- Columnas CON DEFAULT en BD (ej. `EstadoMigracion DEFAULT((1))`) → solo `HasDefaultValue`, EF deja que BD lo ponga

### EF Core — Nullability
- Siempre verificar `*.sql` en BD_dbscopDC antes de poner `.IsRequired()` o tipos no-nullable
- Si columna es NULL en BD → propiedad C# debe ser nullable (`short?`, `decimal?`, etc.)
- `IsRequired()` en config fuerza lector no-nullable → `SqlNullValueException` si hay NULLs reales

### Otras
- `UseSqlOutputClause(false)` en `dbo.Venta` por trigger `TR_Venta_Documento`
- Columna `Valorventa` en dbo.Venta tiene **'v' minúscula** → `HasColumnName("Valorventa")`
- `VentaDetalle.Igv` = `bool` (BIT en DB, flag de afectación IGV)
- `Venta.Igv` = `decimal` (MONEY en DB, monto de IGV)
- Importes pre-calculados por el cliente POS — el handler NO recalcula
- `usuarioCreador` = `"SISTEMA"` hasta implementar Auth

---

## Errores ya resueltos — no repetir

| Error | Causa | Fix aplicado |
|-------|-------|-------------|
| `Invalid column name 'Redondeo'` | Columna real es `RedondeoTotal` | Renombrado en entidad y config |
| `SqlNullValueException GetInt16` en Search | SMALLINT NULL mapeados como no-nullable | Propiedades → `short?`, quitar `IsRequired()` |
| `SqlNullValueException GetInt16` en Search (2) | Shadow `short IGVFactor`/`ServicioFactor` | Cambiados a `short?` |
| `Cannot insert NULL into 'ImpresionCuenta'` | `HasDefaultValue(0)` → EF omite columna | `.ValueGeneratedNever()` en shadow props byte/bool |
| Estado invertido al crear/anular | "E"=Anulado, no Emitido | `Create→"A"`, `Anular→"E"` |
| SP recibía numSerie del request | Legacy siempre pasa `@NumSerie=0` | Hardcodeado a 0, usa `@NumSerieA` |

---

## Pendiente para próximas fases

- Enviar a SUNAT (ISunatClientLookupService existe como stub)
- Revertir stock al anular venta
- Comunicación baja SUNAT al anular documentos electrónicos
- Auth: reemplazar `"SISTEMA"` por usuario real de sesión
