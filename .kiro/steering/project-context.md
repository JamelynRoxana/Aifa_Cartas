---
inclusion: auto
---

# Contexto del Proyecto: Cartas AIFA

## Descripción General
Sistema de gestión de cartas académicas para el Aeropuerto Internacional Felipe Ángeles (AIFA). Permite registrar trámites académicos, generar cartas de aceptación/terminación en PDF, administrar estudiantes, universidades, facultades, etapas y autoridades firmantes.

## Stack Tecnológico
- **Backend:** ASP.NET Core 10 MVC + Web API (C#)
- **Frontend:** Razor Views (.cshtml) con Tailwind CSS v4
- **Base de datos:** SQL Server remoto (Entity Framework Core)
- **Autenticación:** JWT propio (BCrypt para passwords, tokens firmados con HMAC-SHA256)
- **Arquitectura:** MVC con controllers que renderizan vistas + API controllers separados en `/Controllers/Api/`

## Estructura del Proyecto
```
cartas_aifa/
├── Controllers/          # MVC controllers (renderizan vistas)
│   └── Api/              # API controllers (JSON endpoints)
├── Data/                 # DbContext (AppDbContext.cs)
├── Models/               # Entidades EF Core mapeadas a SQL Server
├── Views/                # Razor views organizadas por controller
│   ├── Shared/           # _Layout.cshtml (navbar, footer, scripts globales)
│   └── [Controller]/     # Vistas CRUD por entidad
├── Styles/               # tailwind-input.css (source de Tailwind)
├── wwwroot/
│   ├── css/              # tailwind.css (output compilado)
│   └── images/           # Assets estáticos (Logo-AIFA.png)
└── package.json          # Tailwind CLI + bootstrap-icons
```

## Autenticación
- El login se maneja con JWT almacenado en `localStorage` del navegador.
- El endpoint de login es `POST /api/auth/Login` que retorna un token.
- Todas las peticiones AJAX incluyen el header `Authorization: Bearer {token}` via `$.ajaxSetup`.
- Si el token no existe o expira, el usuario es redirigido a `/Auth/Login`.
- La verificación de auth se hace del lado del cliente con JavaScript en el `_Layout.cshtml`.

## Reglas de Desarrollo

### CSS y Estilos
- **SIEMPRE usar Tailwind CSS** para todo el diseño visual.
- **Todo diseño debe ser FULL RESPONSIVE.** Siempre pensar en mobile-first y desktop. Usar breakpoints de Tailwind (`sm:`, `md:`, `lg:`, `xl:`) para adaptar layouts, tipografía, spacing y visibilidad de elementos. Ninguna vista debe verse rota o inutilizable en móvil.
- **NUNCA usar CSS vanilla** (no crear archivos .css custom, no usar `<style>` tags salvo casos extremadamente justificados como variables CSS del tema).
- Animaciones, transiciones, hover states, responsive design: todo con clases de Tailwind.
- Si una interacción visual se puede lograr con Tailwind (hover, focus, group-hover, transitions, animations), hacerlo con Tailwind. NO usar JavaScript para eso.
- Los colores custom del tema están definidos en `Styles/tailwind-input.css`: `base-blue`, `grey-olive`, `ash-grey`, `mint-cream`, `celadon`.
- Iconos: usar Bootstrap Icons via CDN (`bi bi-*`).

### JavaScript
- **Uso MUY restrictivo de JavaScript.** Solo cuando sea absolutamente necesario para funcionalidad que NO se puede lograr con Tailwind/HTML.
- Casos válidos para JS: llamadas AJAX, manipulación de localStorage (JWT), validaciones dinámicas complejas, lógica de negocio del lado del cliente.
- Casos INVÁLIDOS para JS: animaciones, toggles de visibilidad simples, hover effects, transiciones (usar Tailwind).
- Si el JS es pequeño y específico de una vista (< 20 líneas), puede ir en un `@section Scripts {}` al final de la vista.
- Si el JS es grande o reutilizable, crear un archivo separado en `wwwroot/js/[modulo].js` y referenciarlo.
- Usar JavaScript vanilla moderno (ES6+). No agregar librerías JS nuevas sin justificación fuerte.
- jQuery ya está incluido globalmente (para AJAX con JWT). Usarlo solo para AJAX, no para manipulación DOM trivial.

### Buenas Prácticas
- **Separación de responsabilidades:** Controllers solo orquestan (no lógica de negocio pesada). Models son entidades puras. Vistas solo presentan datos.
- **Naming:** Controllers en PascalCase, vistas organizadas por controller, modelos con nombres descriptivos en español.
- **Formularios:** Usar Tag Helpers de ASP.NET Core (`asp-for`, `asp-action`, `asp-controller`, `asp-validation-for`).
- **Seguridad:** Siempre `[ValidateAntiForgeryToken]` en POST. Nunca exponer connection strings o secrets en el frontend.
- **EF Core:** Usar `Include()` para eager loading. No hacer queries N+1. Usar `async/await` en todos los accesos a BD.
- **Vistas:** Mantener consistencia visual con el `_Layout.cshtml` existente (cards con rounded-2xl, shadows suaves, tipografía Inter).

### Compilar Tailwind
Después de agregar nuevas clases de Tailwind en las vistas, compilar con:
```bash
npm run build
```
O en modo watch durante desarrollo:
```bash
npm run watch
```
