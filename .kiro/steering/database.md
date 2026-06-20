---
inclusion: auto
---

# Base de Datos: Guía de Conexión y Modificación

## Conexión Remota

La base de datos es un **SQL Server remoto** con la siguiente configuración (definida en `appsettings.json`):

- **Servidor:** `lightcontact.sytes.net`
- **Base de datos:** `bddOXXO`
- **Usuario:** `sa`
- **Autenticación:** SQL Server Authentication
- **TrustServerCertificate:** `True`

La conexión se establece desde EF Core usando la cadena `DefaultConnection` en `appsettings.json`. No se requiere VPN ni configuración adicional — el servidor es accesible desde internet.

## Cómo Modificar la Base de Datos (Crear tablas, columnas, etc.)

El proyecto usa **Entity Framework Core Migrations** para gestionar los cambios de esquema. Los pasos son:

### 1. Asegurar que `dotnet-ef` esté disponible

```bash
export PATH="$PATH:/Users/oscar/.dotnet/tools"
```

Si no está instalado:
```bash
dotnet tool install --global dotnet-ef
```

### 2. Crear la migración

```bash
cd cartas_aifa
dotnet ef migrations add NombreDescriptivoDeLaMigracion
```

### 3. Verificar el archivo generado

Revisar el archivo en `Migrations/` para confirmar que solo contiene los cambios esperados. Si la migración sale vacía (porque el snapshot ya tenía el modelo), escribir manualmente el contenido del método `Up()` usando `migrationBuilder`.

### 4. Aplicar la migración a la base de datos remota

```bash
dotnet ef database update
```

Esto se conecta directamente al SQL Server remoto y ejecuta los DDL.

### 5. Revertir una migración (si algo sale mal)

```bash
dotnet ef database update NombreDeMigracionAnterior
dotnet ef migrations remove
```

## Historial de Migraciones

| Migración | Fecha | Descripción |
|-----------|-------|-------------|
| `InitialBaseline` | 2025-06-15 | Snapshot vacío del esquema existente (todas las tablas ya existían en la BD) |
| `AgregarTablaCarreras` | 2025-06-15 | Crea la tabla `Carreras` (Id, IdF FK→Facultades, NombreCarrera) |

## Consideraciones Importantes

- **La base de datos ya existía antes de usar migraciones.** Se creó una migración baseline vacía para que EF Core reconozca el estado actual sin intentar recrear tablas.
- **Todas las tablas existentes se crearon manualmente** antes de integrar EF Migrations. A partir de ahora, cualquier cambio nuevo debe hacerse mediante migraciones.
- **No usar `dotnet ef database update` sin revisar primero la migración generada.** EF puede generar cambios inesperados si el snapshot difiere del estado real de la BD.
- **El proyecto NO tiene carpeta de Migrations hasta que se ejecutó el primer `dotnet ef migrations add`.** La carpeta `Migrations/` ahora contiene el snapshot y las migraciones aplicadas.

## Esquema de Tablas Actual

```
Universidades          → Facultades          → Carreras (NUEVA)
                       → DirectoresEscolares
                       → Estudiantes

DireccionesAifa        → SubdireccionesAifa

DetallesEtapas         → DetallesTipoCarta
                       → EtapasAifa
                       → TramitesAcademicos

AutoridadesAifa        → TramitesAcademicos

ImagenesCatalogo       → ConfiguracionesVisuales

Usuarios (autenticación JWT)
```

## Flujo para Agregar una Nueva Entidad

1. Crear el modelo en `Models/NuevaEntidad.cs`
2. Agregar `DbSet<NuevaEntidad>` en `Data/AppDbContext.cs`
3. (Opcional) Configurar relaciones en `OnModelCreating`
4. Crear migración: `dotnet ef migrations add AgregarNuevaEntidad`
5. Verificar el archivo de migración generado
6. Aplicar: `dotnet ef database update`
7. Crear Controller y Vistas (CRUD)
8. Agregar enlace en `_Layout.cshtml` si corresponde
