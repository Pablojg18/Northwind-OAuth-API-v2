# API Web Northwind (OAuth Google + CRUD completo)

API REST en ASP.NET Core 10 con autenticación JWT, inicio de sesión con Google Identity Services, Identity para registro local y CRUD completo de todas las tablas de Northwind. Incluye un dashboard web responsive con Bootstrap 5 + DataTables.

## Tecnologías

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 + MySQL (Pomelo/MySql.EntityFrameworkCore)
- ASP.NET Core Identity
- JWT Bearer Authentication
- Google OAuth 2.0 (Google Identity Services, flujo `id_token`)
- Swagger / OpenAPI
- Bootstrap 5, Bootstrap Icons, jQuery, DataTables

## Estructura

```text
APIWeb10/                # API principal (Auth + CRUD protegidos)
APIWeb.slnx
NorthWind/               # Proyecto MVC (referencia/lecturas)
dashboard.html           # Dashboard admin (copia para GitHub)
login.html               # Login con Google + registro local
wwwroot/                 # Archivos servidos por la API en producción/ngrok
nwind (1).sql            # Script SQL de la base de datos Northwind
```

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [MySQL 8](https://dev.mysql.com/downloads/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)
- Cuenta en [Google Cloud Console](https://console.cloud.google.com/)
- [ngrok](https://ngrok.com/) (opcional para exponer a internet)

## Configuración

### 1) Base de datos

Crear la base de datos e importar el script:

```sql
CREATE DATABASE nwind;
USE nwind;
SOURCE "nwind (1).sql";
```

O desde MySQL Workbench importar `nwind (1).sql`.

La cadena de conexión está en `APIWeb10/APIWeb/appsettings.json`:

```json
"ConnectionStrings": {
  "ConexionMySQL": "Server=localhost;Database=nwind;User Id=root;Password=root;"
}
```

### 2) Aplicar migraciones de Identity

Las tablas de Identity (`AspNetUsers`, `AspNetRoles`, etc.) se crean con las migraciones. Desde la carpeta `APIWeb10/APIWeb`:

```bash
dotnet ef database update
```

Si no tienes `dotnet-ef`, instálalo:

```bash
dotnet tool install --global dotnet-ef
```

### 3) Google OAuth

1. Ir a [Google Cloud Console](https://console.cloud.google.com/) → APIs y servicios → Credenciales.
2. Crear **ID de cliente de OAuth** tipo **Aplicación web**.
3. Agregar **Orígenes de JavaScript autorizados**:
   - `http://localhost:5023`
   - `https://localhost:7214`
   - `http://localhost:5500`
   - `https://127.0.0.1:5500`
   - `https://TU-DOMINIO.ngrok-free.app` (si lo vas a exponer)
4. Copiar el **Client ID** (termina en `.apps.googleusercontent.com`).
5. Pegar en `APIWeb10/APIWeb/appsettings.json`:

```json
"Authentication": {
  "Google": {
    "ClientId": "TU_CLIENT_ID.apps.googleusercontent.com"
  }
}
```

> **Nota:** No se necesita el `Client Secret` para este flujo con Google Identity Services (`id_token`).

Si la app está en modo **Testing**, solo los usuarios agregados en *Test users* podrán iniciar sesión. Para permitir cualquier cuenta, publica la app en la pantalla de consentimiento OAuth.

### 4) JWT

En `appsettings.json` hay una clave de ejemplo. Para producción cámbiala y guárdala en variable de entorno:

```json
"Jwt": {
  "Key": "CAMBIA_ESTA_CLAVE_POR_UNA_MUY_LARGA_Y_ALEATORIA",
  "Issuer": "APIWeb10",
  "Audience": "APIWeb10Users",
  "DurationInMinutes": 60
}
```

Variable de entorno alternativa: `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`.

## Ejecutar localmente

### Opción A: Solo API

```bash
cd APIWeb10/APIWeb
dotnet restore
dotnet run --urls http://localhost:5023
```

URLs:
- Raíz (login): `http://localhost:5023/`
- Dashboard: `http://localhost:5023/dashboard.html`
- Swagger: `http://localhost:5023/swagger`

### Opción B: API + Live Server (desarrollo)

1. Ejecutar API con HTTPS: `dotnet run` (usa `https://localhost:7214` y `http://localhost:5023`).
2. Abrir `login.html` con Live Server (`http://127.0.0.1:5500/login.html`). El JS detecta el puerto `5500` y llama a `https://localhost:7214` automáticamente.

## Exponer con ngrok

```bash
ngrok http 5023
```

Agrega la URL que te dé ngrok a los *Authorized JavaScript origins* de Google. Si usas dominio estático:

```bash
ngrok http --url=tu-dominio.ngrok-free.app 5023
```

La API sirve los archivos estáticos desde `wwwroot/`, por lo que todo funciona desde el mismo origen.

## Endpoints principales

### Autenticación
| Método | Ruta | Auth | Descripción |
| --- | --- | --- | --- |
| POST | `/api/Auth/register` | No | Crear usuario con email/contraseña |
| POST | `/api/Auth/login` | No | Login con email/contraseña → devuelve JWT |
| POST | `/api/Auth/google` | No | Login con Google (`idToken`) → crea usuario si no existe |
| GET | `/api/Auth/google-config` | No | Devuelve el Client ID configurado |

### CRUD (requieren JWT `Authorization: Bearer <token>`)
| Recurso | Ruta | Operaciones |
| --- | --- | --- |
| Productos | `/api/Products` | GET, POST, PUT, DELETE |
| Categorías | `/api/Categories` | GET, POST, PUT, DELETE |
| Clientes | `/api/Customers` | GET, POST, PUT, DELETE |
| Empleados | `/api/Employees` | GET, POST, PUT, DELETE |
| Pedidos | `/api/Orders` | GET, POST, PUT, DELETE |
| Detalle pedidos | `/api/OrderDetails` | GET, POST, PUT, DELETE (clave compuesta) |
| Transportistas | `/api/Shippers` | GET, POST, PUT, DELETE |
| Proveedores | `/api/Suppliers` | GET, POST, PUT, DELETE |

En Swagger puedes autorizar con el botón **Authorize** usando el token obtenido en `/api/Auth/login` o `/api/Auth/google`.

## Uso desde cliente externo

1. Obtener token con `/api/Auth/register` + `/api/Auth/login` o Google.
2. Enviar `Authorization: Bearer <token>` en todas las peticiones a los endpoints CRUD.
3. CORS permite `http://127.0.0.1:5500` y `http://localhost:5500`. Si vas a consumir desde otro origen, agrega ese dominio en `Program.cs` → `WithOrigins(...)`.

## Seguridad

- Las claves y secretos **no** deben subirse al repo. Usa variables de entorno en producción.
- El `Google Client Secret` **no** se usa en este flujo; no es necesario incluirlo.
- Las rutas CRUD están protegidas con `[Authorize]`.
- Usa claves JWT largas y únicas en despliegues reales.

## Licencia

Este proyecto es con fines académicos/prácticos.
