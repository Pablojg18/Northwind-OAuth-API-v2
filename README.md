# API Web Northwind (OAuth Google + CRUD completo)

API REST en ASP.NET Core 10 con autenticación JWT, inicio de sesión con Google Identity Services, Identity para registro local y CRUD completo de las tablas principales de Northwind. Incluye un dashboard web responsive con Bootstrap 5 + DataTables y Swagger UI para probar la API.

## Tecnologías

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 + MySQL (`MySql.EntityFrameworkCore` / Oracle)
- ASP.NET Core Identity
- JWT Bearer Authentication
- Google OAuth 2.0 (Google Identity Services, flujo `id_token`)
- OpenAPI + Swagger UI
- Bootstrap 5, Bootstrap Icons, jQuery, DataTables

> **Nota:** Pomelo no tiene todavía una versión estable para EF Core 10, por eso el proveedor es `MySql.EntityFrameworkCore` 10.x.

## Estructura

```text
APIWeb10/
  APIWeb.slnx                    # Solución
  APIWeb/
    APIWeb.csproj
    Program.cs
    Controllers/                 # Auth + CRUD de Northwind
    Data/
      NorthwindContext.cs        # Contexto de Northwind
      AppIdentityDbContext.cs    # Contexto de Identity (tablas AspNet*)
      DbSeeder.cs                # Migraciones + datos de ejemplo
      Migrations/
        Identity/                # Migraciones de Identity
        Northwind/               # Migraciones de Northwind (solo si la BD está vacía)
    Models/                      # Entidades y DTOs
    Services/                    # JWT (TokenService, JwtSettings)
    wwwroot/                     # login.html, dashboard.html, productos.html, swagger/
dashboard.html                   # Copia de referencia (GitHub)
login.html                       # Copia de referencia (GitHub)
```

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [MySQL 8](https://dev.mysql.com/downloads/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)
- Cuenta en [Google Cloud Console](https://console.cloud.google.com/) (opcional, para login con Google)
- [ngrok](https://ngrok.com/) (opcional para exponer a internet)

## Configuración

### 1) Base de datos

Solo necesitas MySQL en ejecución. La API crea la base de datos `nwind` y sus tablas automáticamente al arrancar (migraciones de EF Core).

La cadena de conexión está en `APIWeb10/APIWeb/appsettings.json`:

```json
"ConnectionStrings": {
  "ConexionMySQL": "Server=localhost;Port=3306;Database=nwind;User Id=root;Password=root;"
}
```

Ajusta usuario y contraseña a tu instalación de MySQL.

- Si `nwind` **no existe**, se crea con las tablas de Northwind y datos de ejemplo.
- Si `nwind` **ya existe** con el esquema clásico de Northwind, la API reutiliza esas tablas (incluida `Order Details`) y no las modifica.
- Las tablas de Identity (`AspNetUsers`, `AspNetRoles`, etc.) se crean siempre que falten.

### 2) JWT

En `appsettings.json` hay una clave de ejemplo. Para producción cámbiala por una larga y aleatoria:

```json
"Jwt": {
  "Key": "CAMBIA_ESTA_CLAVE_POR_UNA_MUY_LARGA_Y_ALEATORIA",
  "Issuer": "APIWeb10",
  "Audience": "APIWeb10Users",
  "DurationInMinutes": 60
}
```

También puedes usar variables de entorno: `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`.

### 3) Google OAuth (opcional)

Sin Client ID la API funciona igual con registro/login local; el botón de Google se oculta.

1. Ir a [Google Cloud Console](https://console.cloud.google.com/) → APIs y servicios → Credenciales.
2. Crear **ID de cliente de OAuth** tipo **Aplicación web**.
3. Agregar **Orígenes de JavaScript autorizados**:
   - `http://localhost:5080`
   - `https://localhost:7080`
   - `http://localhost:5500`
   - `https://127.0.0.1:5500`
   - `https://TU-DOMINIO.ngrok-free.app` (si lo vas a exponer)
4. Copiar el **Client ID** (termina en `.apps.googleusercontent.com`).
5. Pegarlo en `APIWeb10/APIWeb/appsettings.json`:

```json
"Authentication": {
  "Google": {
    "ClientId": "TU_CLIENT_ID.apps.googleusercontent.com"
  }
}
```

> No se necesita el `Client Secret` para este flujo con Google Identity Services (`id_token`).

Si la app está en modo **Testing**, solo los usuarios agregados en *Test users* podrán iniciar sesión.

### 4) CORS

Los orígenes permitidos están en `appsettings.json`:

```json
"Cors": {
  "AllowedOrigins": [
    "http://localhost:5080",
    "https://localhost:7080",
    "http://localhost:5500",
    "http://127.0.0.1:5500"
  ]
}
```

Agrega ahí tu dominio de ngrok si vas a consumir la API desde otro origen. Si usas `"*"` se permite cualquier origen.

## Ejecutar localmente

### Inicio rápido en Windows

Doble clic en `iniciar-api.cmd` (o ejecuta `.\iniciar-api.ps1` en PowerShell). El script:

- cierra una instancia anterior de `APIWeb` si quedó ocupando el puerto,
- verifica que el puerto 5080 esté libre,
- arranca la API y muestra las URLs.

La reconstrucción usa los puertos **5080 (HTTP)** y **7080 (HTTPS)**, distintos de los del proyecto original en `Oauth\API\API` (5023/7214), para que ambos puedan convivir sin conflictos.

### Manual

Desde la raíz del repositorio:

```bash
dotnet run --project APIWeb10/APIWeb
```

O desde la carpeta del proyecto:

```bash
cd APIWeb10/APIWeb
dotnet restore
dotnet run --urls http://localhost:5080
```

También puedes abrir `APIWeb10/APIWeb.slnx` en Visual Studio y ejecutar con F5.

URLs:

- Raíz (login): `http://localhost:5080/`
- Dashboard: `http://localhost:5080/dashboard.html`
- Swagger UI: `http://localhost:5080/swagger`
- Documento OpenAPI: `http://localhost:5080/openapi/v1.json`

Al arrancar, la consola muestra las migraciones aplicadas y si se sembraron datos de ejemplo.

### Migraciones manuales (opcional)

Normalmente no hace falta: la API las aplica sola al iniciar. Para hacerlo a mano:

```bash
cd APIWeb10/APIWeb
dotnet ef database update --context AppIdentityDbContext
dotnet ef database update --context NorthwindContext
```

Si no tienes `dotnet-ef`:

```bash
dotnet tool install --global dotnet-ef
```

## Swagger

La UI de Swagger se sirve desde `wwwroot/swagger/` usando los archivos estáticos de `swagger-ui-dist` (CDN). El documento OpenAPI lo genera `Microsoft.AspNetCore.OpenApi` en `/openapi/v1.json`.

Para probar endpoints protegidos:

1. Ejecuta `POST /api/Auth/register` y luego `POST /api/Auth/login`.
2. Copia el `token` de la respuesta.
3. Pulsa **Authorize** en Swagger y pega el token (sin la palabra `Bearer`, Swagger la agrega).
4. Ejecuta cualquier endpoint CRUD.

## Exponer con ngrok

```bash
ngrok http 5080
```

Agrega la URL de ngrok a los *Authorized JavaScript origins* de Google y a `Cors:AllowedOrigins`. Con dominio estático:

```bash
ngrok http --url=tu-dominio.ngrok-free.app 5080
```

La API sirve los archivos estáticos desde `wwwroot/`, por lo que login y dashboard funcionan desde el mismo origen.

## Endpoints principales

### Autenticación

| Método | Ruta | Auth | Descripción |
| --- | --- | --- | --- |
| POST | `/api/Auth/register` | No | Crear usuario con email/contraseña |
| POST | `/api/Auth/login` | No | Login con email/contraseña → devuelve JWT |
| POST | `/api/Auth/google` | No | Login con Google (`idToken`) → crea usuario si no existe |
| GET | `/api/Auth/google-config` | No | Devuelve el Client ID configurado |

La contraseña debe tener mínimo 6 caracteres, una mayúscula, una minúscula, un número y un símbolo.

### CRUD (requieren JWT `Authorization: Bearer <token>`)

| Recurso | Ruta | Operaciones |
| --- | --- | --- |
| Productos | `/api/Products` | GET, POST, PUT, DELETE |
| Categorías | `/api/Categories` | GET, POST, PUT, DELETE |
| Clientes | `/api/Customers` | GET, POST, PUT, DELETE |
| Empleados | `/api/Employees` | GET, POST, PUT, DELETE |
| Pedidos | `/api/Orders` | GET, POST, PUT, DELETE |
| Detalle pedidos | `/api/OrderDetails` | GET, POST, PUT, DELETE (clave compuesta `orderId` + `productId`) |
| Transportistas | `/api/Shippers` | GET, POST, PUT, DELETE |
| Proveedores | `/api/Suppliers` | GET, POST, PUT, DELETE |

## Uso desde cliente externo

1. Obtener token con `/api/Auth/register` + `/api/Auth/login` o Google.
2. Enviar `Authorization: Bearer <token>` en todas las peticiones a los endpoints CRUD.
3. Agregar el origen del cliente en `Cors:AllowedOrigins`.

## Seguridad

- No subir `appsettings.json` con credenciales reales; usa variables de entorno o *user secrets*.
- El `Google Client Secret` no se usa en este flujo.
- Las rutas CRUD están protegidas con `[Authorize]`.
- Usa claves JWT largas y únicas en despliegues reales.

## Problemas conocidos

- **Smart App Control de Windows 11**: si está en modo *Enforced* puede bloquear DLLs de NuGet sin firma/reputación. En este proyecto se detectó bloqueando `Swashbuckle.AspNetCore.SwaggerGen.dll`; por eso Swagger se sirve con `Microsoft.AspNetCore.OpenApi` (Microsoft) y la UI estática desde CDN. Síntoma: `Una directiva de Control de aplicaciones bloqueó este archivo (0x800711C7)` al arrancar `APIWeb.dll`.

Solución aplicada en este equipo: desactivar Smart App Control. Desde **PowerShell como administrador**:

```powershell
reg add "HKLM\SYSTEM\CurrentControlSet\Control\CI\Policy" /v VerifiedAndReputablePolicyState /t REG_DWORD /d 0 /f
```

Luego **reiniciar Windows** (el cambio no surte efecto hasta el reinicio). También se puede hacer desde **Seguridad de Windows → Control de aplicaciones y navegador → Smart App Control**.

> Esta configuración no se puede volver a activar sin reinstalar Windows. Si prefieres mantenerla activa, la alternativa es ejecutar la API en un contenedor o en otra máquina.
- Si MySQL no está disponible al arrancar, la API inicia igual y muestra el error en consola; revisa la cadena de conexión.

## Licencia

Este proyecto es con fines académicos/prácticos.
