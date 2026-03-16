# Blueprint: API Backend — SCOP Punto de Venta Comercial

**Versión:** 1.0
**Fecha:** 2026-03-16
**Fuente funcional:** análisis del legacy `D:\Sistemas ERP\PtoVentaComercial`
**Proyecto destino:** `D:\Sistemas ERP\api-net` — solución `PuntoVentaComercial.slnx`

---

## 1. Visión General de la Solución

El backend es una **Web API REST en .NET 10**, con arquitectura Clean Architecture + CQRS, que reemplaza la capa de lógica actualmente distribuida entre:

- `FormMainPtoVenta.cs` (133 KB) — formulario monolítico que mezcla UI, cálculo tributario, stock, políticas y SUNAT
- DLLs propietarias de DataConsulting (`BussinessRules`, `BussinessEntities`) — lógica de negocio opaca
- Integración WCF directa desde la UI hacia SUNAT (`WebfacturaDCPSoapClient`)
- Base de datos SQL Server accedida via Enterprise Library (tecnología obsoleta)

El objetivo es exponer toda esa lógica como una API REST limpia, desacoplada y testeable, consumida por el nuevo frontend Angular.

### Principios de diseño

- **Domain es agnóstico** — sin dependencias externas, sin EF, sin frameworks
- **Application orquesta** — CQRS puro, sin acceso directo a DB
- **Infrastructure implementa** — EF Core + Dapper, repositorios, servicios externos (SUNAT)
- **API delega** — los controllers solo traducen HTTP → Command/Query → HTTP
- **Cálculo tributario en Domain** — la lógica IGV/ISC/ICBPER vive en el dominio, nunca en el controller ni en la UI

---

## 2. Estructura de Proyectos y Capas

Sin cambios en la estructura existente. Los 9 módulos del POS se expresan como subcarpetas dentro de cada capa.

```
src/
├── DataConsulting.PuntoVentaComercial.Domain
│   ├── Abstractions/          # Entity, Result, Error, ErrorType (ya existen)
│   ├── Constants/             # TaxConstants (IGV, ICBPER), FiscalConstants (ya existe)
│   ├── Enums/                 # FiscalEnums (ya existen) + SaleEnums (nuevos)
│   ├── Sales/                 # SaleItem, SaleTotals, SaleErrors
│   ├── Orders/                # Order, OrderItem, OrderErrors
│   ├── Clients/               # Client, ClientLocal, ClientErrors
│   ├── Products/              # Product (proyección POS), ProductErrors
│   ├── Payments/              # Payment, PaymentDetail, PaymentErrors
│   ├── Cash/                  # VaultDeposit, CashErrors
│   ├── Identity/              # User, Policy, Workstation
│   └── Configuration/         # DocumentSeries, Currency, ExchangeRate, Shift
│
├── DataConsulting.PuntoVentaComercial.Application
│   ├── Abstractions/          # ICommand, IQuery, handlers, behaviors (ya existen)
│   ├── Exceptions/            # ValidationException (ya existe)
│   ├── Features/
│   │   ├── Auth/
│   │   ├── Sales/
│   │   ├── Orders/
│   │   ├── Clients/
│   │   ├── Products/
│   │   ├── Payments/
│   │   ├── Sunat/
│   │   ├── Cash/
│   │   └── Configuration/
│   └── Services/
│       ├── Sales/             # ISaleCalculationService, SaleCalculationService
│       ├── Sunat/             # ISunatService
│       └── Print/             # IPrintService
│
├── DataConsulting.PuntoVentaComercial.Infrastructure
│   ├── Database/              # ApplicationDbContext, DbConnectionFactory (ya existen)
│   ├── Configurations/        # EF entity configs (ya existen + nuevas para POS)
│   ├── Repositories/          # Repository<T> base (ya existe) + repos POS
│   └── ExternalServices/
│       ├── Sunat/             # SunatHttpService, SunatWcfService
│       └── Print/             # PdfPrintService (QuestPDF)
│
└── DataConsulting.PuntoVentaComercial.API
    ├── Controllers/
    │   ├── Auth/
    │   ├── Sales/
    │   ├── Orders/
    │   ├── Clients/
    │   ├── Products/
    │   ├── Payments/
    │   ├── Sunat/
    │   ├── Cash/
    │   └── Configuration/
    ├── Middleware/             # GlobalExceptionHandler (ya existe)
    ├── Extensions/            # SwaggerExtensions (ya existe) + AuthExtensions
    └── Utils/                 # ApiVersions (ya existe)
```

---

## 3. Módulos / Bounded Contexts

Del análisis del legacy se identifican **9 dominios funcionales**:

| # | Módulo | Origen en legacy | Complejidad |
|---|--------|-----------------|-------------|
| 1 | **Auth & Session** | `Program.cs`: login, licencia, estación, constantes | Alta |
| 2 | **Sales** (Ventas) | `FormMainPtoVenta`, `FormSearchVoucher`, `FormPayment` | Muy Alta |
| 3 | **Orders** (Pedidos) | `FormMainPtoVenta` (modo pedido), `FormSearchVoucherPedido` | Alta |
| 4 | **Clients** (Clientes) | `FormClient`, búsqueda en SUNAT, validación DNI | Media |
| 5 | **Products** (Artículos/Catálogo) | `PopUpArticuloPtoVenta`, top vendidos, composición | Media |
| 6 | **Payments** (Cobranzas) | `FormPayment`, `PopupPayment`, cuotas, formas de pago | Alta |
| 7 | **SUNAT** (Facturación Electrónica) | `FormEnviarVentaSunat`, `WebfacturaDCPSoapClient` | Alta |
| 8 | **Cash** (Caja/Depósitos) | `FormDepositoBoveda`, `FormAnularDeposito`, `FormCashIncome` | Media |
| 9 | **Configuration** (Configuración) | Series, monedas, turnos, vendedores, constantes | Media |

---

## 4. Entidades, Value Objects, Enums y Reglas de Negocio

### 4.1 Entidades del Dominio

#### `Sale` (Venta)

```
IdVenta
FechaEmision
TipoDocumento         : EDocumento
Serie                 : string
Correlativo           : int
Cliente               : ClientInfo (VO)
Vendedor              : int (IdTrabajador)
Vendedor2             : int?
Estacion              : int
Turno                 : int
TipoMoneda            : ETipoMoneda
TipoCambio            : decimal
FormaPago             : EFormaPagoRes
FlagIGV               : bool
DescuentoGlobal       : DescuentoGlobal (VO)
Items                 : List<SaleItem>
Totales               : SaleTotals (VO)
FlagDetraccion        : bool
IdSubdiario           : int?
Estado                : EEstadoVenta
FechaCreacion, IdUsuarioCreador, etc.
```

#### `SaleItem` (ítem de detalle — mutable desde UI, no VO)

```
IdArticulo
Codigo
Descripcion
SiglaUnidad
IdUnidad
Cantidad              : decimal
StockDisponible       : decimal
PrecioUnitario        : decimal        # precio que ve el cajero (con o sin IGV)
PrecioIncluyeIgv      : bool
ValorFacial           : decimal        # precio unitario "cara" (siempre con IGV si aplica)
TipoAfectacionIgv     : ETipoAfectacionIgv
TasaIsc               : decimal
TipoIsc               : ETipoIsc
FlagIcbper            : bool
ImporteDescuento      : decimal
TipoDescuento         : ETipoDescuento
IdClaseProducto       : int
IdTipoCliente         : int
# Calculados (devueltos por SaleCalculationService)
ValorVenta            : decimal        # sin IGV
ValorVentaNeto        : decimal        # ValorVenta - ImporteDescuento
IGV                   : decimal
ISC                   : decimal
ICBPER                : decimal
Subtotal              : decimal        # lo que paga el cliente por esta línea
```

#### `Order` (Pedido)

Mismo modelo que Sale pero:
- Sin `FormaPago`, sin `OperacionPago`
- Estado propio: `EEstadoPedido` (Pendiente, Facturado, Anulado)
- Los ítems son `OrderItem` con la misma estructura que `SaleItem`

#### `Client` (Cliente)

```
IdCliente
Nombre
NombreComercial
IdDocumentoIdentidad  : EDocumentoIdentidad
NumDocumento
CodValidadorDoc       # para DNI peruano
IdPais
FlagIGV               : bool          # si el cliente aplica IGV
CreditoMaximo         : decimal
EstadoCliente         : EEstado
Locals                : List<ClientLocal>
FechaAlta, FechaBaja, etc.
```

#### `ClientLocal`

```
IdLocal, IdCliente, IdSucursal,
DireccionLocal, Telefono1, Estado
```

#### `Product` *(proyección de solo lectura para el POS)*

```
IdArticulo
Codigo, CodBarra
Descripcion
SiglaUnidad, IdUnidad, FactorUnd
FlagCompuesto         : int           # 2 = compuesto
PrecioVenta           : decimal       # precio con IGV
ValorVenta            : decimal       # precio sin IGV
StockDisponible       : decimal
TipoAfectacionIgv     : ETipoAfectacionIgv
TasaIsc               : decimal
TipoIsc               : ETipoIsc
FlagIcbper            : bool
IdClaseProducto       : int
Composicion           : List<ProductComponent>
Foto                  : byte[]?
```

#### `VaultDeposit` (DepositoBoveda)

```
IdDepositoBoveda
IdTrabajador, IdIsla, IdTurnoAsistencia
FechaEmision
TipoDocumento, NumSerie, NumDocumento
TipoMoneda, TipoCambio, Importe
Glosa, FormaPago
Estado                : EEstado
UpdateToken           : short
```

#### `Payment` (OperacionPago)

```
IdOperacion
IdVenta
FechaRegistro
Cliente               : ClientInfo (VO)
Detalles              : List<PaymentDetail>
ImporteTotal, ImportePagado, Vuelto, Credito
Cuotas                : List<SaleInstallment>
```

#### `DocumentSeries` (DocumentoSerie)

```
IdEmpresa, IdSucursal, IdEstacion
TipoDocumento         : EDocumento
NumSerie              : string
UltimoCorrelativo     : long
```

---

### 4.2 Value Objects

| Value Object | Propiedades clave |
|---|---|
| `SaleTotals` | ValorAfecto, ValorExonerado, ValorInafecto, ValorRegalo, TotalISC, TotalIGV, TotalICBPER, TotalDescuentos, ImporteTotal, Redondeo, ImporteTotalRedondeado |
| `DiscountGlobal` | EsPorcentaje (bool), Valor (decimal), ImporteCalculado (decimal) |
| `ClientInfo` | IdCliente, Nombre, NumDocumento, TipoDocumento, FlagIGV |
| `Money` | Importe, TipoMoneda, TipoCambio, ImporteEnSoles |
| `DocumentReference` | TipoDocumento, Serie, Correlativo, NumeroFormateado (ej. "B001-000123") |
| `PaymentDetail` | IdFormaPago, Descripcion, Importe, TipoMoneda |
| `SaleInstallment` | NumeroCuota, FechaVencimiento, Importe |

---

### 4.3 Enums Nuevos (a crear en Domain/Enums/SaleEnums.cs)

```csharp
// Catálogo 07 SUNAT – tipo de afectación IGV
ETipoAfectacionIgv : Gravado(10), GravadoRetiro(11), GravadoGratuito(12),
                     Exonerado(20), ExoneradoRetiro(21),
                     Inafecto(30), InafectoRetiro(31),
                     Exportacion(40), Regalo(50)

// Sistema de cálculo ISC
ETipoIsc           : NoAplica(0), SistemaValor(1), SistemaEspecifico(2), SistemaPrecioVenta(3)

// Tipo de descuento por ítem
ETipoDescuento     : Ninguno(0), Porcentaje(1), MontoFijo(2), Regalo(3)

// Estado de venta/pedido
EEstadoVenta       : Aprobado, Anulado
EEstadoPedido      : Pendiente, Facturado, Anulado
EEstadoDeposito    : Activo, Anulado

// Tipo de cliente (para lista de precios)
ETipoCliente       : Varios(1), Regular(2), Empresa(3)

// Tipo de venta
ETipoVenta         : Directa(1), Credito(2), Consignacion(3)

// Forma de pago (resolución)
EFormaPagoRes      : Efectivo(1), Credito(2), Cheque(3), Deposito(4), TarjetaCredito(5),
                     NotaCredito(6), ValeConsumo(7), ValeExterno(8)
```

---

### 4.4 Constantes de Negocio (a crear en Domain/Constants/TaxConstants.cs)

```csharp
TaxConstants:
  TasaIgv           = 0.18m        // 18% — Perú
  FactorIgv         = 1.18m        // divisor para extraer base de precio con IGV
  TarifaIcbper      = 0.50m        // S/ 0.50 por bolsa plástica (Ley 30884)
  MaxDecimalesCalc  = 4            // decimales en cálculos intermedios
  MaxDecimalesTotal = 2            // decimales en totales finales

FiscalLimits:
  LongitudRuc       = 11
  LongitudDni       = 8
  // MaximoMontoBoleta: dinámico, viene de DB/constantes por empresa
```

---

### 4.5 Reglas de Negocio Críticas

Estas reglas deben vivir en **Domain**, no en Application ni en la API:

| # | Regla | Origen en legacy |
|---|-------|-----------------|
| 1 | **RUC → Factura / DNI → Boleta** — tipo de documento determinado por documento del cliente | `DeterminarTipoDocumento()`, `PopupTipoComprobante` |
| 2 | **Monto máximo boleta sin documento** — si cliente sin DNI y monto > tope legal, venta inválida | `CamposValidos()`: `Constantes.MaximoMontoBoleta` |
| 3 | **Validación DNI peruano** — check digit (módulo 11 con pesos específicos) | `FunctionsBR.ValidarDNIPeru` |
| 4 | **Validación RUC peruano** — módulo 11 con pesos específicos | `UIFunctions.ValidarRUC` |
| 5 | **Descuento máximo por trabajador** — no puede superar el porcentaje asignado al worker | `PopUpDescuento`, `TrabajadorBR.GetPermisosDescuentoTrabajador` |
| 6 | **Regalo requiere autorización de supervisor** — segundo login con política `AutorizarRegalo` | `btnAutorizarVentaAlmacenes_Click`, `PopUpDescuento` |
| 7 | **Stock obligatorio** — si `HabilitarVentaSinStock` inactivo, cantidad no puede superar stock | `CamposValidos()`: `moVenta.ValidarStock` |
| 8 | **Subdiario requerido** — si política `RegistrarSubdiarioVenta` activa, venta necesita IdSubdiario | `CamposValidos()` |
| 9 | **Producto compuesto** — al vender, desagregar componentes en el movimiento de almacén | `LlenarEntidad()`: loop sobre `ComposicionArticulo` |
| 10 | **Anulación en cascada** — anular venta/pedido anula también la `GuiaRemision` asociada | `FormSearchVoucher.Annul()`: `GuiaRemisionBR.Anular` |
| 11 | **Redondeo configurable** — el total puede redondearse al centavo más cercano o a 0.10 soles | `VentaBR.RedondeoTotal` |
| 12 | **Factura requiere RUC** — no se puede emitir factura a cliente con DNI | `CamposValidos()` |
| 13 | **Subvención diaria** — ciertos artículos subvencionados solo pueden venderse una vez por cliente/día | `ValidarArticuloSubvencionado()` |
| 14 | **Agrupación automática de detalles** — controlada por constante, agrupa ítems duplicados | `Constantes.AgrupamientoAutomaticoDetalle` |

---

## 5. Casos de Uso / Commands / Queries

### Auth & Session

| Tipo | Nombre | Descripción |
|------|--------|-------------|
| Command | `LoginCommand` | Autenticar usuario. Retorna JWT + contexto de sesión completo |
| Command | `ValidateLicenseCommand` | Validar clave de activación del RUC empresa contra WS SUNAT |
| Query | `GetUserPoliciesQuery` | Políticas activas del usuario autenticado (por nombre de política) |
| Query | `GetSessionEnvironmentQuery` | Empresa, sucursal, estación, trabajador, constantes del sistema |
| Query | `ValidateWorkstationQuery` | Verifica que la estación tiene permiso de acceso |

### Sales

| Tipo | Nombre | Descripción |
|------|--------|-------------|
| Command | `CalculateSaleCommand` | **Recalcular totales en tiempo real.** Sin persistencia. Retorna SaleTotals |
| Command | `CreateSaleCommand` | Registrar venta completa con cobro. Genera correlativo, persiste en DB |
| Command | `AnnulSaleCommand` | Anular venta. Requiere política `PreventaAnularVenta`. Anula GuiaRemision |
| Query | `GetSalesQuery` | Búsqueda paginada: fecha, tipo doc, serie, número, cliente, estado |
| Query | `GetSaleByIdQuery` | Detalle completo de una venta (cabecera + ítems + cobro) |
| Query | `GetNextCorrelativeQuery` | Próximo número disponible para tipo doc + serie + sucursal |
| Query | `GetPrintableSaleQuery` | Datos completos del ticket para generación de PDF |

### Orders

| Tipo | Nombre | Descripción |
|------|--------|-------------|
| Command | `CreateOrderCommand` | Guardar pedido. Genera correlativo, persiste en DB |
| Command | `AnnulOrderCommand` | Anular pedido. Requiere política `PreventaAnularPedido`. Anula GuiaRemision |
| Query | `GetOrdersQuery` | Búsqueda de pedidos por fecha, cliente, estado |
| Query | `GetOrderByIdQuery` | Detalle completo del pedido |
| Query | `GetPrintableOrderQuery` | Datos para generación de PDF del pedido |

### Clients

| Tipo | Nombre | Descripción |
|------|--------|-------------|
| Command | `CreateClientCommand` | Crear nuevo cliente. Valida DNI/RUC. Requiere política `ClienteCrearNuevo` |
| Command | `UpdateClientCommand` | Modificar cliente. Requiere política `ClienteModificar` |
| Query | `SearchClientsQuery` | Búsqueda por nombre, RUC, DNI |
| Query | `GetClientByIdQuery` | Detalle completo del cliente |
| Query | `LookupRucSunatQuery` | Consultar datos de empresa en SUNAT por RUC (nombre, dirección) |
| Query | `GetClientAddressesQuery` | Direcciones locales del cliente (para cboDireccion) |

### Products

| Tipo | Nombre | Descripción |
|------|--------|-------------|
| Query | `GetPriceListQuery` | Lista de precios para POS. Filtros: barcode, código, descripción, sucursal, tipoCliente |
| Query | `GetTopSellersQuery` | Top artículos más vendidos en la sucursal (para panel rápido del POS) |
| Query | `GetProductDetailQuery` | Detalle + composición del artículo compuesto |
| Query | `GetProductStockQuery` | Stock disponible por almacén/sucursal para un artículo |

### Payments

| Tipo | Nombre | Descripción |
|------|--------|-------------|
| Command | `RegisterPaymentCommand` | Registrar cobro de una venta (OperacionPago + detalles) |
| Query | `GetPaymentMethodsQuery` | Métodos de pago disponibles para el tipo de venta |
| Query | `GetInstallmentOptionsQuery` | Opciones de cuotas de crédito disponibles |

### SUNAT

| Tipo | Nombre | Descripción |
|------|--------|-------------|
| Command | `SubmitSaleToSunatCommand` | Generar XML UBL 2.1 + firmar + enviar a SUNAT. Actualiza estado |
| Command | `ValidateLicenseKeyCommand` | Generar y validar clave de licencia para un RUC |
| Query | `GetPendingSubmissionsQuery` | Ventas pendientes de envío a SUNAT (estados: NoEnviado, Rechazado) |
| Query | `GetSubmissionStatusQuery` | Estado de envío de una venta específica en SUNAT |
| Query | `LookupRucSunatQuery` | Compartido con Clients — búsqueda pública de RUC |

### Cash

| Tipo | Nombre | Descripción |
|------|--------|-------------|
| Command | `CreateVaultDepositCommand` | Registrar depósito en bóveda. Genera documento con correlativo |
| Command | `AnnulVaultDepositCommand` | Anular depósito. Valida UpdateToken (concurrencia) |
| Command | `RegisterCashIncomeCommand` | Registrar ingreso de efectivo (feature pendiente en legacy — stub) |
| Query | `GetAvailableCashQuery` | Efectivo disponible por vendedor/isla/moneda/fecha |
| Query | `GetVaultDepositsQuery` | Búsqueda de depósitos por isla, documento, serie, número, moneda, fecha |

### Configuration

| Tipo | Nombre | Descripción |
|------|--------|-------------|
| Query | `GetDocumentSeriesQuery` | Series asignadas a la estación/sucursal/empresa por tipo de documento |
| Query | `GetNextCorrelativeQuery` | Compartido con Sales — próximo número disponible |
| Query | `GetExchangeRatesQuery` | Tipos de cambio por fecha y tipo de moneda |
| Query | `GetCurrenciesQuery` | Catálogo de monedas activas |
| Query | `GetActiveShiftsQuery` | Turnos activos para el día/horario actual |
| Query | `GetSellersQuery` | Trabajadores/vendedores activos en la empresa |
| Query | `GetConstantsQuery` | Constantes del sistema por empresa y sucursal (monto máximo boleta, etc.) |
| Query | `GetIdentityDocumentsQuery` | Tipos de documento de identidad con longitud y máscara |

---

## 6. Endpoints Propuestos por Módulo

Todos bajo versión `v1`. Prefijo: `/api/v1/`.

### AUTH & SESSION

```
POST   /auth/login
POST   /auth/validate-license
GET    /session/environment
GET    /session/policies
```

### SALES

```
POST   /sales/calculate                     ← sin persistencia, en tiempo real
POST   /sales                               ← crear venta completa
GET    /sales                               ← búsqueda ?fecha=&tipoDoc=&serie=&num=&cliente=&estado=
GET    /sales/{id}
PUT    /sales/{id}/annul
GET    /sales/{id}/print                    ← datos para generar PDF
GET    /sales/next-correlative              ← ?tipoDoc=&serie=&sucursal=&estacion=
```

### ORDERS

```
POST   /orders
GET    /orders                              ← ?fecha=&cliente=&trabajador=&estado=
GET    /orders/{id}
PUT    /orders/{id}/annul
GET    /orders/{id}/print
```

### CLIENTS

```
GET    /clients                             ← ?nombre=&ruc=&doc=&tipoDoc=
POST   /clients
GET    /clients/{id}
PUT    /clients/{id}
GET    /clients/{id}/addresses
GET    /clients/sunat/{ruc}                 ← lookup SUNAT
```

### PRODUCTS

```
GET    /products/price-list                 ← ?codigo=&descripcion=&sucursal=&tipoCliente=&stock=
GET    /products/top-sellers                ← ?sucursal=
GET    /products/{id}
GET    /products/{id}/stock                 ← ?sucursal=
```

### PAYMENTS

```
GET    /payment-methods                     ← ?tipoVenta=&formaRes=
POST   /payments
GET    /payments/{idVenta}
```

### SUNAT

```
GET    /sunat/pending                       ← ?fecha=&tipoDoc=&serie=&num=&cliente=
POST   /sunat/submissions/{idVenta}         ← enviar a SUNAT
GET    /sunat/submissions/{idVenta}/status
GET    /sunat/ruc/{ruc}                     ← lookup público
```

### CASH

```
GET    /vault/available-cash                ← ?worker=&isla=&moneda=&fecha=
POST   /vault/deposits
GET    /vault/deposits                      ← ?isla=&doc=&serie=&num=&moneda=&fecha=
PUT    /vault/deposits/{id}/annul
POST   /cash/income
```

### CONFIGURATION

```
GET    /document-series                     ← ?empresa=&sucursal=&estacion=
GET    /document-series/next-correlative    ← ?tipoDoc=&serie=&sucursal=
GET    /currencies
GET    /exchange-rates                      ← ?fecha=
GET    /shifts                              ← ?dia=&hora=
GET    /sellers                             ← ?empresa=&estado=
GET    /constants                           ← ?empresa=&sucursal=
GET    /identity-documents                  ← ?pais=&activo=
```

**Total: ~40 endpoints en v1.**

---

## 7. Prioridad de Implementación

### 🔴 Crítico — sin esto el POS no puede operar

| # | Endpoint / Feature | Por qué es bloqueante |
|---|---|---|
| 1 | `GET /session/environment` | Angular necesita empresa, sucursal, estación y constantes al iniciar |
| 2 | `POST /auth/login` | Acceso al sistema |
| 3 | `GET /session/policies` | Controla qué puede hacer cada rol en el POS |
| 4 | `POST /sales/calculate` | Se llama en **cada cambio** de ítem, cantidad o precio. Es el corazón del POS |
| 5 | `GET /document-series` + `next-correlative` | Sin series no se puede crear ningún comprobante |
| 6 | `GET /products/price-list` | Sin esto no se pueden agregar productos al carrito |
| 7 | `GET /clients` (búsqueda) | El POS necesita cliente antes de facturar |
| 8 | `GET /payment-methods` | Modal de cobro no puede cargar |
| 9 | `POST /sales` | Persistir la venta |
| 10 | `POST /payments` | Registrar el cobro |

### 🟠 Alta prioridad — flujo completo funcional

| # | Endpoint / Feature |
|---|---|
| 11 | `GET /products/top-sellers` (panel POS) |
| 12 | `POST /clients` + `PUT /clients/{id}` |
| 13 | `GET /clients/sunat/{ruc}` (búsqueda automática en SUNAT) |
| 14 | `POST /orders` + `GET /orders` + `PUT /orders/{id}/annul` |
| 15 | `PUT /sales/{id}/annul` |
| 16 | `GET /exchange-rates` + `GET /currencies` |
| 17 | `GET /shifts` + `GET /sellers` |
| 18 | `GET /constants` (máximo boleta, redondeo, etc.) |

### 🟡 Media prioridad — operaciones post-venta

| # | Endpoint / Feature |
|---|---|
| 19 | `GET /sales` (historial/búsqueda de ventas) |
| 20 | `GET /sales/{id}/print` + `GET /orders/{id}/print` (PDF) |
| 21 | `POST /vault/deposits` + `PUT /vault/deposits/{id}/annul` |
| 22 | `GET /vault/available-cash` |
| 23 | `GET /products/{id}` (detalle + composición) |
| 24 | `GET /products/{id}/stock` |
| 25 | `GET /vault/deposits` (búsqueda) |

### 🟢 Puede esperar — post-MVP

| # | Endpoint / Feature | Nota |
|---|---|---|
| 26 | Todo el módulo SUNAT (`/sunat/submissions`) | Requiere infraestructura de firma XML UBL 2.1 |
| 27 | `POST /auth/validate-license` | Solo necesario en activación inicial |
| 28 | `POST /cash/income` | Era un stub vacío en el legacy |
| 29 | `GET /constants` completo (todas las constantes) | Puede hardcodearse parcialmente al inicio |
| 30 | Cuotas de crédito (`GetInstallmentOptionsQuery`) | Caso de uso menos frecuente |

---

## 8. Qué Conservar del Legacy y Qué Rediseñar

### ✅ Conservar conceptualmente

| Concepto | Por qué es inamovible |
|----------|----------------------|
| Lógica tributaria (IGV 18%, tipos ISC, ICBPER) | Ley SUNAT peruana |
| Regla RUC → Factura / DNI → Boleta | Obligación tributaria |
| Algoritmo check digit DNI peruano | Validación legal |
| Algoritmo módulo 11 RUC peruano | Validación legal |
| Sistema de series y correlativos por estación | Requerimiento SUNAT |
| Jerarquía de descuentos (global % > por ítem > regalo con autorizacion) | Regla de negocio real |
| Políticas de permisos por trabajador | Lógica operativa de negocio |
| Composición de artículos en detalle de almacén | Afecta contabilidad de inventario |
| Anulación en cascada Venta → GuiaRemision | Integridad contable |
| Validación de monto máximo boleta sin documento | Obligación tributaria |
| Dos vendedores por venta (principal + secundario) | Regla de comisiones real |
| Cliente "Varios" como cliente genérico por defecto | Flujo operativo de POS |

### ❌ Rediseñar completamente

| Componente legacy | Problema | Propuesta nueva |
|---|---|---|
| `FormMainPtoVenta.cs` (133 KB) | Mezcla UI + lógica + estado + SUNAT | Endpoints separados por responsabilidad |
| Crystal Reports / `ReporteGestorOpenXmlBR` | DLL binaria local, no portable a web | PDF vía **QuestPDF** en el backend |
| WCF SOAP directo a SUNAT | Tecnología obsoleta, acoplado a UI | `ISunatService` en Infrastructure con `HttpClient` o SDK |
| Registro Windows para rutas y config | No funciona fuera del cliente Windows | `appsettings.json` + variables de entorno |
| Políticas `EPolitica` chequeadas con `if` inline | No escalable, no declarativo | JWT claims-based authorization con políticas ASP.NET Core |
| `Globals.cs` — estado global estático | Thread-unsafe en contexto web | Scoped session por request usando JWT claims |
| Enterprise Library DAL | Obsoleto desde .NET 5 | EF Core + Dapper (ya en el proyecto) |
| Licencia validada en XML local | Fácil de evadir, no auditado | Validación en backend al login, token con expiración |
| `Application.DoEvents()` — bloqueo UI | Patrón UI monolítico | `async/await` en toda la cadena de la API |
| `SendKeys.Send("{F4}")` para expandir combos | Hack de UI | No aplica en API |
| Impresión local via DLL | Solo funciona en el PC de caja | PDF generado en backend, descargado por Angular |

---

## 9. Dependencias Externas e Integraciones

### Integraciones existentes en el legacy

| Integración | Rol en legacy | Estrategia en el nuevo backend |
|-------------|--------------|-------------------------------|
| **SUNAT SOAP** (`WebfacturaDCPSoapClient`) | Validación de licencia, búsqueda de RUC, envío de facturas electrónicas | Encapsular en `ISunatService` (Infrastructure). Mantener WCF si el contrato SUNAT no cambia, o migrar a REST con SDK |
| **SQL Server** | Base de datos principal (via Enterprise Library) | EF Core (escritura/lecturas simples) + Dapper (consultas complejas del POS). Ya configurado en el proyecto |
| **SUNAT RUC lookup** | `trans.BuscarRuc(ruc, usuario, pass)` — devuelve nombre y dirección | `GET /clients/sunat/{ruc}` que llama internamente al WS o API pública SRI/SUNAT |
| **Crystal Reports** | Impresión de tickets y reportes | Reemplazar por QuestPDF. Templates en código C# |
| **ReporteGestorOpenXmlBR** | Impresión de tickets POS via DLL | Misma estrategia: PDF en backend |

### NuGet adicionales a evaluar (no instalar aún)

| Paquete | Para qué |
|---------|---------|
| `QuestPDF` | Generación de tickets PDF sin Crystal Reports |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT Auth |
| `System.ServiceModel.Http` | Compatibilidad WCF para SUNAT SOAP si se necesita |
| `System.ServiceModel.Duplex` | Idem |

### Dependencias ya presentes y aprovechables

| Paquete | Ya en proyecto | Cómo se usa |
|---------|--------------|-------------|
| `FluentValidation` | ✅ Application | Validar commands (CreateClient, CreateSale, etc.) |
| `Dapper` | ✅ Application/Infrastructure | Consultas complejas de POS (price list, búsqueda de ventas) |
| `EF Core + SQL Server` | ✅ Infrastructure | Persistencia de ventas, clientes, depósitos |
| `Scrutor` | ✅ Application | Auto-registro de handlers y services |
| `Serilog` | ✅ API | Logging estructurado |
| `Asp.Versioning` | ✅ Infrastructure/API | Versionado de endpoints |

---

## 10. Roadmap de Implementación del Backend por Fases

### Fase B1 — Núcleo de Cálculo (sin DB)

**Objetivo:** Angular puede calcular ventas sin base de datos. Primer endpoint funcional.

**Archivos a crear:**
```
Domain/
  Enums/SaleEnums.cs                     # ETipoAfectacionIgv, ETipoIsc, ETipoDescuento, EFormaPagoRes
  Constants/TaxConstants.cs              # TasaIgv, FactorIgv, TarifaIcbper
  Sales/SaleErrors.cs                    # Errores de dominio de ventas

Application/
  Services/Sales/ISaleCalculationService.cs
  Services/Sales/SaleCalculationService.cs   # Lógica IGV/ISC/ICBPER/descuentos
  Features/Sales/Commands/CalculateSale/
    CalculateSaleCommand.cs
    CalculateSaleResponse.cs
    CalculateSaleCommandValidator.cs
    CalculateSaleCommandHandler.cs

API/
  Controllers/Sales/SalesController.cs
  Controllers/Sales/CalculateSaleRequest.cs
```

**Sin DB. Sin auth. Entrada/salida pura.**

**Entregable:** `POST /api/v1/sales/calculate` retorna totales correctos con IGV/ISC/ICBPER.

---

### Fase B2 — Auth & Session

**Objetivo:** Login, JWT, políticas y contexto de sesión.

**Archivos a crear:**
```
Domain/
  Identity/User.cs
  Identity/Policy.cs
  Identity/Workstation.cs
  Identity/IdentityErrors.cs
  Identity/IUserRepository.cs
  Identity/IWorkstationRepository.cs

Application/
  Features/Auth/Commands/Login/LoginCommand.cs
  Features/Auth/Commands/Login/LoginCommandHandler.cs
  Features/Auth/Commands/Login/LoginCommandValidator.cs
  Features/Auth/Commands/Login/LoginResponse.cs
  Features/Auth/Queries/GetUserPolicies/...
  Features/Auth/Queries/GetSessionEnvironment/...

Infrastructure/
  Repositories/UserRepository.cs
  Repositories/WorkstationRepository.cs
  Configurations/UserConfiguration.cs
  Auth/JwtTokenService.cs

API/
  Controllers/Auth/AuthController.cs
  Extensions/AuthExtensions.cs          # AddJwtBearer, AddAuthorization con políticas
```

**Entregable:** Login retorna JWT con claims (empresa, sucursal, estación, usuario, políticas). Guards de políticas funcionando.

---

### Fase B3 — Catálogo y Configuración

**Objetivo:** El POS puede buscar productos, cargar series y obtener configuración.

**Archivos a crear:**
```
Domain/
  Products/Product.cs
  Products/ProductComponent.cs
  Products/IProductRepository.cs
  Configuration/DocumentSeries.cs
  Configuration/ExchangeRate.cs
  Configuration/IDocumentSeriesRepository.cs
  Configuration/IExchangeRateRepository.cs

Application/
  Features/Products/Queries/GetPriceList/...
  Features/Products/Queries/GetTopSellers/...
  Features/Products/Queries/GetProductDetail/...
  Features/Configuration/Queries/GetDocumentSeries/...
  Features/Configuration/Queries/GetNextCorrelative/...
  Features/Configuration/Queries/GetExchangeRates/...
  Features/Configuration/Queries/GetShifts/...
  Features/Configuration/Queries/GetSellers/...
  Features/Configuration/Queries/GetConstants/...

Infrastructure/
  Repositories/ProductRepository.cs      # Dapper para price-list y top-sellers
  Repositories/DocumentSeriesRepository.cs
  Repositories/ExchangeRateRepository.cs
  Configurations/ProductConfiguration.cs

API/
  Controllers/Products/ProductsController.cs
  Controllers/Configuration/DocumentSeriesController.cs
  Controllers/Configuration/ConfigurationController.cs
```

**Entregable:** El POS puede buscar productos, cargar turnos/series/tipo de cambio.

---

### Fase B4 — Clientes

**Objetivo:** El POS puede buscar, crear y editar clientes. Validar DNI/RUC peruano.

**Archivos a crear:**
```
Domain/
  Clients/Client.cs
  Clients/ClientLocal.cs
  Clients/ClientErrors.cs
  Clients/IClientRepository.cs
  Clients/DniValidator.cs               # Check digit DNI peruano
  Clients/RucValidator.cs               # Módulo 11 RUC peruano

Application/
  Features/Clients/Commands/CreateClient/...
  Features/Clients/Commands/UpdateClient/...
  Features/Clients/Queries/SearchClients/...
  Features/Clients/Queries/GetClientById/...
  Features/Clients/Queries/GetClientAddresses/...
  Features/Clients/Queries/LookupRucSunat/...
  Services/Sunat/ISunatClientLookupService.cs

Infrastructure/
  Repositories/ClientRepository.cs
  Configurations/ClientConfiguration.cs
  ExternalServices/Sunat/SunatClientLookupService.cs

API/
  Controllers/Clients/ClientsController.cs
```

**Entregable:** Clientes buscables, creables, editables. Validación DNI/RUC. Lookup SUNAT por RUC.

---

### Fase B5 — Ventas y Pagos (persistencia completa)

**Objetivo:** El POS puede registrar ventas y cobros. Flujo completo de caja.

**Archivos a crear:**
```
Domain/
  Sales/Sale.cs
  Sales/SaleItem.cs
  Sales/SaleTotals.cs (VO)
  Sales/DiscountGlobal.cs (VO)
  Sales/DocumentReference.cs (VO)
  Sales/ISaleRepository.cs
  Payments/Payment.cs
  Payments/PaymentDetail.cs
  Payments/SaleInstallment.cs
  Payments/IPaymentRepository.cs

Application/
  Features/Sales/Commands/CreateSale/...
  Features/Sales/Commands/AnnulSale/...
  Features/Sales/Queries/GetSales/...
  Features/Sales/Queries/GetSaleById/...
  Features/Payments/Commands/RegisterPayment/...
  Features/Payments/Queries/GetPaymentMethods/...

Infrastructure/
  Repositories/SaleRepository.cs
  Repositories/PaymentRepository.cs
  Configurations/SaleConfiguration.cs
  Configurations/SaleItemConfiguration.cs
  Configurations/PaymentConfiguration.cs

API/
  Controllers/Sales/SalesController.cs   # Agregar Create, Annul, GetAll, GetById
  Controllers/Payments/PaymentsController.cs
```

**Entregable:** Flujo completo: calcular → crear venta → cobrar → confirmar.

---

### Fase B6 — Pedidos

**Objetivo:** El POS puede crear, buscar y anular pedidos.

**Archivos a crear:**
```
Domain/
  Orders/Order.cs
  Orders/OrderItem.cs
  Orders/OrderErrors.cs
  Orders/IOrderRepository.cs

Application/
  Features/Orders/Commands/CreateOrder/...
  Features/Orders/Commands/AnnulOrder/...
  Features/Orders/Queries/GetOrders/...
  Features/Orders/Queries/GetOrderById/...

Infrastructure/
  Repositories/OrderRepository.cs
  Configurations/OrderConfiguration.cs

API/
  Controllers/Orders/OrdersController.cs
```

**Entregable:** Pedidos funcionales con mismas reglas que ventas (sin cobro).

---

### Fase B7 — Caja y Depósitos

**Objetivo:** Registrar, buscar y anular depósitos en bóveda. Control de efectivo.

**Archivos a crear:**
```
Domain/
  Cash/VaultDeposit.cs
  Cash/CashErrors.cs
  Cash/IVaultDepositRepository.cs

Application/
  Features/Cash/Commands/CreateVaultDeposit/...
  Features/Cash/Commands/AnnulVaultDeposit/...
  Features/Cash/Queries/GetVaultDeposits/...
  Features/Cash/Queries/GetAvailableCash/...

Infrastructure/
  Repositories/VaultDepositRepository.cs
  Configurations/VaultDepositConfiguration.cs

API/
  Controllers/Cash/CashController.cs
```

**Entregable:** Depósitos en bóveda operacionales.

---

### Fase B8 — Impresión (PDF)

**Objetivo:** El backend genera PDFs de tickets para ventas y pedidos.

**Archivos a crear:**
```
Application/
  Services/Print/IPrintService.cs
  Features/Sales/Queries/GetPrintableSale/...
  Features/Orders/Queries/GetPrintableOrder/...

Infrastructure/
  ExternalServices/Print/PdfPrintService.cs   # QuestPDF
  Templates/TicketVentaTemplate.cs
  Templates/TicketPedidoTemplate.cs

API/
  Controllers/Sales/SalesController.cs        # GET /sales/{id}/print → File(pdf)
  Controllers/Orders/OrdersController.cs      # GET /orders/{id}/print
```

**NuGet nuevo:** `QuestPDF`

**Entregable:** Angular puede descargar PDF de cualquier venta o pedido.

---

### Fase B9 — SUNAT (Facturación Electrónica)

**Objetivo:** Enviar comprobantes electrónicos a SUNAT. Post-MVP.

**Archivos a crear:**
```
Domain/
  Sunat/SunatSubmission.cs
  Sunat/SunatErrors.cs
  Sunat/ISunatSubmissionRepository.cs

Application/
  Features/Sunat/Commands/SubmitSaleToSunat/...
  Features/Sunat/Queries/GetPendingSubmissions/...
  Features/Sunat/Queries/GetSubmissionStatus/...
  Services/Sunat/ISunatService.cs

Infrastructure/
  ExternalServices/Sunat/SunatService.cs      # WCF o HttpClient según endpoint SUNAT
  Configurations/SunatSubmissionConfiguration.cs

API/
  Controllers/Sunat/SunatController.cs
```

**Complejidad alta.** Requiere: generación XML UBL 2.1, firma digital con certificado, envío al endpoint SUNAT, manejo de errores SUNAT (códigos 0000, 2xxx, 3xxx, 4xxx), reintentos y estado asincrónico.

**Entregable:** Ventas electrónicas enviadas y aceptadas por SUNAT.

---

## Resumen del Roadmap

| Fase | Objetivo principal | Sin DB | Con DB | Endpoints |
|------|-------------------|--------|--------|-----------|
| B1 | Cálculo de venta | ✅ | — | 1 (`/sales/calculate`) |
| B2 | Auth y sesión | — | ✅ | 4 (`/auth`, `/session`) |
| B3 | Catálogo y configuración | — | ✅ | 10 (`/products`, `/config`) |
| B4 | Clientes | — | ✅ | 6 (`/clients`) |
| B5 | Ventas + pagos completos | — | ✅ | 8 (`/sales`, `/payments`) |
| B6 | Pedidos | — | ✅ | 5 (`/orders`) |
| B7 | Caja y depósitos | — | ✅ | 5 (`/vault`, `/cash`) |
| B8 | Impresión PDF | — | ✅ | +2 (`/print`) |
| B9 | SUNAT electrónica | — | ✅ | 4 (`/sunat`) |

**Total estimado al completar:** ~45 endpoints, 9 módulos, ~80-100 archivos nuevos.

---

*Documento generado el 2026-03-16 como base para el desarrollo del backend del POS.*
*Fuente funcional: análisis de código fuente `D:\Sistemas ERP\PtoVentaComercial`.*
