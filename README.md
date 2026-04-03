# CheckupMedico API

API REST en **.NET 10** para la gestión del servicio de checkup médico institucional. Permite autenticar colaboradores contra el Apigateway corporativo, consultar hospitales y kits disponibles según campus y perfil del colaborador, y generar el documento PDF de solicitud de checkup.

---

## Tabla de contenidos

1. [Arquitectura](#arquitectura)
2. [Estructura de la solución](#estructura-de-la-solución)
3. [Stack tecnológico](#stack-tecnológico)
4. [Requisitos previos](#requisitos-previos)
5. [Configuración](#configuración)
   - [appsettings](#appsettings)
   - [Ambientes](#ambientes)
   - [Variables de entorno en Azure](#variables-de-entorno-en-azure)
6. [Ejecución local](#ejecución-local)
7. [Endpoints](#endpoints)
   - [Auth](#auth)
   - [Catalog](#catalog)
   - [Checkup](#checkup)
8. [Autenticación](#autenticación)
9. [Middleware personalizado](#middleware-personalizado)
10. [Archivos de datos (Excel)](#archivos-de-datos-excel)
11. [CI/CD — GitHub Actions](#cicd--github-actions)
12. [Dependencias NuGet](#dependencias-nuget)

---

## Arquitectura

```
Cliente Angular (3 orígenes CORS permitidos)
				 │
				 ▼
┌─────────────────────────────────────────────────────┐
│                  CheckupMedico.Api                  │
│                                                     │
│  ┌─ ClientSecretMiddleware (cabecera X-Client-Secret)│
│  ├─ ExceptionMiddleware (manejo global de errores)  │
│  ├─ JwtBearer Authentication / Authorization        │
│  │                                                  │
│  ├─ AuthController                                  │
│  │     └─ AuthService                               │
│  │           └─ ColaboradorService ──► TEC Apigateway│
│  │                                    (OAuth2 + JWT) │
│  ├─ CatalogController                               │
│  │     └─ CatalogService                            │
│  │           ├─ RepoLocalFileHospital  ◄── Hospitals.xlsx │
│  │           └─ RepoLocalFileKit       ◄── Kits.xlsx      │
│  │                                                  │
│  └─ CheckupController                               │
│        └─ CheckupService                            │
│              ├─ CheckupITESMDoc (PDF)                │
│              └─ RepoLocalFileBillingConfig ◄─ Billing.xlsx│
└─────────────────────────────────────────────────────┘
```

### Flujo principal

1. El cliente obtiene un **JWT interno** llamando a `POST /api/auth/token` con nómina, correo y sociedad.
2. El token se genera tras validar al colaborador contra el **Apigateway corporativo** (TEC de Monterrey).
3. Con ese JWT, el cliente consulta campus disponibles (`GET /api/catalog/hospitals`) y hospitales filtrados por campus (`POST /api/catalog/hospitals/campus`).
4. Finalmente, `POST /api/checkup/create` genera y descarga el **PDF de solicitud de checkup**.

---

## Estructura de la solución

```
CheckupMedico.Solution.sln
│
├── Api/
│   └── CheckupMedico.Api             → Proyecto web principal (controllers, middlewares, startup)
│
├── Application/
│   ├── CheckupMedico.Application.Dto             → DTOs de request/response
│   ├── CheckupMedico.Application.Service         → Implementaciones de servicios de negocio
│   ├── CheckupMedico.Application.Service.Interface → Contratos de servicios de negocio
│   ├── CheckupMedico.Application.Doc             → Generación de documentos PDF
│   └── CheckupMedico.Application.Doc.Interface   → Contrato de generación de documentos
│
├── Domain/
│   ├── CheckupMedico.Domain.Entity               → Entidades del dominio
│   ├── CheckupMedico.Domain.Enum                 → Enumeraciones (SexEnum, ImageTypes)
│   └── CheckupMedico.Domain.Repository.Interface → Contratos de repositorios
│
├── Infrastructure/
│   ├── CheckupMedico.Infrastructure              → Repositorios de archivos Excel locales
│   ├── CheckupMedico.Infrastructure.EFCore       → (EF Core — reserved for future use)
│   ├── CheckupMedico.Infrastructure.External     → Cliente del Apigateway corporativo
│   ├── CheckupMedico.Infrastructure.External.Interface → Contrato del Apigateway
│   └── CheckupMedico.Infrastructure.External.Model   → Modelos de respuesta del Apigateway
│
└── Transversal/
		├── CheckupMedico.Transversal.Exception       → Excepciones de dominio personalizadas
		└── CheckupMedico.Util                        → Utilidades y helpers compartidos
```

---

## Stack tecnológico

| Capa                | Tecnología                                                          |
| ------------------- | ------------------------------------------------------------------- |
| Framework           | .NET 10 / ASP.NET Core 10                                           |
| Autenticación       | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer` 10.0.3) |
| Logging             | Serilog (consola + archivo JSON rotativo diario)                    |
| Documentación API   | Scalar (`MapScalarApiReference`) — solo en Development              |
| Generación de PDF   | QuestPDF / librería de documentos custom (`CheckupITESMDoc`)        |
| Datos locales       | ClosedXML — lectura de archivos `.xlsx` con caché en memoria        |
| Caché               | `IMemoryCache` (in-process)                                         |
| Integración externa | HttpClient contra TEC Apigateway (OAuth2 + JWT propio)              |
| CI/CD               | GitHub Actions → Azure App Service                                  |

---

## Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Acceso a la red del Apigateway corporativo (`https://apigateway-qa.tec.mx`) o VPN
- Los archivos de datos deben existir en `CheckupMedico.Api/Files/`:
  - `Hospitals.xlsx`
  - `Kits.xlsx`
  - `Billing.xlsx`

---

## Configuración

### appsettings

El archivo `appsettings.json` contiene la configuración base. Los archivos de ambiente la sobreescriben parcialmente.

| Sección               | Descripción                                                       |
| --------------------- | ----------------------------------------------------------------- |
| `Serilog`             | Logging estructurado JSON a consola y archivo rotativo en `Logs/` |
| `AllowedHosts`        | Hosts HTTP permitidos (producción: dominio del App Service)       |
| `CorsOrigins`         | Lista de orígenes Angular permitidos                              |
| `Jwt`                 | Clave de firma, issuer, audience y duración del token (minutos)   |
| `ApigatewayStructure` | Servidor, rutas y parámetros de los web services corporativos     |

**Detalle de `ApigatewayStructure.WebServices`:**

| Name                     | Propósito                                                         |
| ------------------------ | ----------------------------------------------------------------- |
| `Token`                  | Obtiene un `access_token` OAuth2 de Azure AD (client_credentials) |
| `TokenJWT`               | Obtiene un JWT propio del TEC con los claims del colaborador      |
| `GetEmployeeInformation` | Consulta contratos/sociedad del colaborador por nómina            |

### Ambientes

| Rama Git | `ASPNETCORE_ENVIRONMENT` | Archivo cargado                |
| -------- | ------------------------ | ------------------------------ |
| `main`   | `Development`            | `appsettings.Development.json` |
| `pprd`   | `Pprd`                   | `appsettings.Pprd.json`        |
| `prod`   | `Prod`                   | `appsettings.Prod.json`        |

Los archivos de ambiente solo sobreescriben el nivel de logging. Cualquier diferencia de URLs, claves u orígenes CORS entre ambientes debe configurarse en **Application Settings** del App Service en Azure Portal (las variables de entorno tienen mayor precedencia que los archivos JSON).

### Variables de entorno en Azure

Para cada App Service ir a **Configuration → Application Settings** y agregar:

| Variable                 | Development   | Pprd   | Prod   |
| ------------------------ | ------------- | ------ | ------ |
| `ASPNETCORE_ENVIRONMENT` | `Development` | `Pprd` | `Prod` |

> Para sobreescribir secciones anidadas de `appsettings.json` desde Application Settings, usar la notación doble guion bajo: `ApigatewayStructure__Server`, `Jwt__Key`, etc.

---

## Ejecución local

```bash
# Restaurar dependencias
dotnet restore

# Ejecutar en modo Development (http://localhost:5043 | https://localhost:7085)
dotnet run --project CheckupMedico.Api
```

La documentación interactiva estará disponible en:

```
https://localhost:7085/scalar/v1
```

> **Nota:** Scalar solo se expone cuando `ASPNETCORE_ENVIRONMENT=Development`.

---

## Endpoints

Todos los endpoints (excepto los de autenticación) requieren dos cosas:

1. **Cabecera `X-Client-Secret`** — valor igual a `Jwt:Key` del `appsettings.json`
2. **Cabecera `Authorization: Bearer <token>`** — JWT obtenido en `/api/auth/token`

### Auth

| Método | Ruta              | Auth               | Descripción                                             |
| ------ | ----------------- | ------------------ | ------------------------------------------------------- |
| `POST` | `/api/auth/token` | `[AllowAnonymous]` | Autentica al colaborador y devuelve el JWT interno      |
| `GET`  | `/api/auth/user`  | `[Authorize]`      | Devuelve el nombre completo del colaborador autenticado |

**Body `POST /api/auth/token`:**

```json
{
  "idCalaborador": "A00123456",
  "email": "nombre.apellido@tec.mx",
  "sociedad": "ITESM"
}
```

**Response `200 OK`:**

```json
{
  "success": true,
  "message": null,
  "data": {
    "token": "eyJhbGci...",
    "expiration": "2026-04-03T14:00:00Z"
  },
  "errors": null
}
```

---

### Catalog

| Método | Ruta                            | Descripción                                                                                                        |
| ------ | ------------------------------- | ------------------------------------------------------------------------------------------------------------------ |
| `GET`  | `/api/catalog/hospitals`        | Lista los campus disponibles                                                                                       |
| `POST` | `/api/catalog/hospitals/campus` | Lista hospitales por campus, filtrando el kit apropiado según el género y edad del colaborador (extraídos del JWT) |

**Body `POST /api/catalog/hospitals/campus`:**

```json
{
  "gender": 1,
  "campusName": "Monterrey"
}
```

> `gender`: `0` = Mujer, `1` = Hombre

---

### Checkup

| Método | Ruta                  | Descripción                                      |
| ------ | --------------------- | ------------------------------------------------ |
| `POST` | `/api/checkup/create` | Genera y descarga el PDF de solicitud de checkup |

**Body `POST /api/checkup/create`:** objeto `HospitalListDto` seleccionado del catálogo.

**Response:** archivo `application/pdf` con nombre `Checkup{userId}.pdf`.

---

## Autenticación

La API usa **JWT Bearer** auto-contenido. El flujo completo es:

```
Cliente → POST /api/auth/token (nómina + correo + sociedad)
							 │
							 ▼
				 AuthService
							 │
							 ├─► 1. Llama a Apigateway: obtiene OAuth2 access_token (Azure AD)
							 ├─► 2. Llama a Apigateway: obtiene TokenJWT (claims del colaborador)
							 └─► 3. Llama a Apigateway: obtiene datos de contrato/sociedad
							 │
							 ▼
				 Genera JWT interno firmado con Jwt:Key
				 Claims incluidos: NameIdentifier (nómina), Email,
													 DateOfBirth, Name, Sid (sociedad), Gender
							 │
							 ▼
Cliente recibe { token, expiration }
```

**Duración del token:** configurable en `Jwt:ExpirationMinutes` (default: 120 min). `ClockSkew = 0`.

---

## Middleware personalizado

### `ClientSecretMiddleware`

Valida la cabecera `X-Client-Secret` en **todas** las peticiones (excepciones: `OPTIONS`, `/swagger`, `/openapi`, `/scalar`, `/favicon.ico`).  
El valor esperado se obtiene de `Jwt:Key`.  
Si la cabecera está ausente o es incorrecta, responde `401 Unauthorized`.

### `ExceptionMiddleware`

Captura todas las excepciones no controladas y las convierte en respuestas JSON estandarizadas:

| Excepción               | HTTP  | Mensaje                        |
| ----------------------- | ----- | ------------------------------ |
| `ValidationException`   | `400` | Lista de errores de validación |
| `NotFoundException`     | `404` | "Recurso no encontrado."       |
| `UnauthorizedException` | `401` | "Sin autorización."            |
| Cualquier otra          | `500` | "Error interno del servidor."  |

Todas las respuestas de error siguen el envelope `ApiResponseDto<string>`:

```json
{
  "success": false,
  "message": "Sin autorización.",
  "data": null,
  "errors": null
}
```

---

## Archivos de datos (Excel)

Los catálogos se leen de archivos `.xlsx` ubicados en `CheckupMedico.Api/Files/` y se almacenan en `IMemoryCache`.  
Al iniciar la aplicación, `CacheCleaner.ClearAll()` limpia toda la caché garantizando datos frescos.

| Archivo          | Repositorio                  | Contenido                                              |
| ---------------- | ---------------------------- | ------------------------------------------------------ |
| `Hospitals.xlsx` | `RepoLocalFileHospital`      | Campus, nombre, ciudad, responsable, datos de contacto |
| `Kits.xlsx`      | `RepoLocalFileKit`           | Kit por hospital, género, rango de edad                |
| `Billing.xlsx`   | `RepoLocalFileBillingConfig` | Configuración de facturación por instituto             |

> Los tres repositorios de archivos se registran como **Singleton** en DI para que la caché persista durante toda la vida del proceso.

---

## CI/CD — GitHub Actions

Archivo: [`.github/workflows/main_checkupmedicodummyapi.yml`](.github/workflows/main_checkupmedicodummyapi.yml)

### Disparadores

```
push a: main | pprd | prod
workflow_dispatch (ejecución manual)
```

### Jobs

```
build  ──────────────────────────────────────────────────────────┐
	dotnet build --configuration Release                            │
	dotnet publish → artifact ".net-app"                           │
			 │                                                          │
			 ├── deploy-development  (solo si ref == refs/heads/main)   │
			 │     env: Development                                      │
			 │     App Service: checkupmedicodummyapi                   │
			 │     Secret: AZUREAPPSERVICE_PUBLISHPROFILE_6B67D0...     │
			 │                                                          │
			 ├── deploy-pprd  (solo si ref == refs/heads/pprd)          │
			 │     env: Pprd                                            │
			 │     App Service: checkupmedicodummyapi-pprd  ← REPLACE   │
			 │     Secret: AZUREAPPSERVICE_PUBLISHPROFILE_PPRD  ← REPLACE│
			 │                                                          │
			 └── deploy-prod  (solo si ref == refs/heads/prod)          │
						 env: Prod                                            │
						 App Service: checkupmedicodummyapi-prod  ← REPLACE   │
						 Secret: AZUREAPPSERVICE_PUBLISHPROFILE_PROD  ← REPLACE
```

### Secrets de GitHub requeridos

| Secret                                                            | Ambiente    | Estado         |
| ----------------------------------------------------------------- | ----------- | -------------- |
| `AZUREAPPSERVICE_PUBLISHPROFILE_6B67D08B8B084F0383E607CA982E5A3E` | Development | ✅ Configurado |
| `AZUREAPPSERVICE_PUBLISHPROFILE_PPRD`                             | Pprd        | ⚠️ Pendiente   |
| `AZUREAPPSERVICE_PUBLISHPROFILE_PROD`                             | Prod        | ⚠️ Pendiente   |

> Los publish profiles se obtienen desde **Azure Portal → App Service → Overview → Get publish profile**.

---

## Dependencias NuGet

| Paquete                                         | Versión |
| ----------------------------------------------- | ------- |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.3  |
| `Microsoft.AspNetCore.OpenApi`                  | 9.0.12  |
| `Microsoft.IdentityModel.Tokens`                | 8.16.0  |
| `System.IdentityModel.Tokens.Jwt`               | 8.16.0  |
| `Scalar.AspNetCore`                             | 2.13.1  |
| `Serilog.AspNetCore`                            | 10.0.0  |
| `Serilog.Formatting.Compact`                    | 3.0.0   |
| `Serilog.Settings.Configuration`                | 10.0.0  |
| `Serilog.Sinks.Console`                         | 6.1.1   |
| `Serilog.Sinks.File`                            | 7.0.0   |
