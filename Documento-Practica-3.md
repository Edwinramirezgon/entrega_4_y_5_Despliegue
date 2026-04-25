# Guía de Despliegue — Práctica 3
## TradingJournal — IIS (On-Premises) y Azure App Service

## Integrantes
1. Edwin Ramirez Gonzalez
2. Juan Jose Rua David
3. Felipe Olaya Beneitez
4. Julian Andres Ramirez Bedoya
5. Argenis Alejandro Ruiz Cotes

---

## URLs del proyecto

| Servicio | URL |
|---|---|
| API Local (IIS) | `http://localhost:8080` |
| Frontend Local (IIS) | `http://localhost:8081` |
| API Azure | `https://tradingjournal-api-bsa5cfdggthmdgfr.brazilsouth-01.azurewebsites.net` |
| Frontend Azure | `https://tradingjournal-web-cxg5hhcxhue6g3a8.brazilsouth-01.azurewebsites.net` |

---

## Parte 1 — Publicación en IIS (On-Premises)

### Paso 1 — Habilitar IIS en Windows
1. Abre **"Activar o desactivar características de Windows"**
2. Marca **Internet Information Services**
3. Expande y marca también **ASP.NET 4.x** y **Herramientas de administración web**
4. Clic en **Aceptar** y espera que instale

### Paso 2 — Configurar base de datos local (SQL Server)

> La API usa SQL Server local con Windows Authentication. La base de datos se crea automáticamente al iniciar la API por primera vez gracias a las migraciones.

Verifica que el archivo `TradingJournal.API\appsettings.json` tenga esta cadena de conexión activa:
```json
"WindowsSecurity": "Server=localhost;DataBase=TradingJournal;Encrypt=False;Integrated Security=SSPI; persist security info=True;"
```

Y que `Program.cs` use `WindowsSecurity` en desarrollo (ya está configurado así por defecto).

> ⚠️ Si SQL Server usa una instancia con nombre (ej: `SQLEXPRESS`), cambia `Server=localhost` por `Server=localhost\SQLEXPRESS`.

### Paso 3 — Publicar el Backend (API)
```powershell
dotnet publish "c:\Users\EDWIN\Downloads\Despliegue 3\entrega-2-despliegue-sw\TradingJournal\TradingJournal.API\TradingJournal.API.csproj" -c Release -o "C:\inetpub\wwwroot\TradingJournalAPI"
```

### Paso 4 — Publicar el Frontend
> ⚠️ Antes de publicar, asegúrate que `TradingJournal.Web\wwwroot\appsettings.json` tenga:
> ```json
> { "ApiBaseUrl": "http://localhost:8080/" }
> ```

```powershell
dotnet publish "c:\Users\EDWIN\Downloads\Despliegue 3\entrega-2-despliegue-sw\TradingJournal\TradingJournal.Web\TradingJournal.Web.csproj" -c Release -o "C:\inetpub\wwwroot\TradingJournalWeb"
```

### Paso 5 — Instalar ASP.NET Core Hosting Bundle
```powershell
winget install Microsoft.DotNet.HostingBundle.8
```

### Paso 6 — Crear App Pool para la API en IIS
1. Abre **Administrador de IIS** (`inetmgr`)
2. Clic derecho en **Grupos de aplicaciones** → **Agregar grupo de aplicaciones**
3. Rellena:
   - **Nombre:** `TradingJournalAPI`
   - **Versión .NET CLR:** `Sin código administrado`
   - **Modo de canalización:** `Integrado`
4. Clic en **Aceptar**

### Paso 7 — Crear Sitio para la API en IIS
1. Clic derecho en **Sitios** → **Agregar sitio web**
2. Rellena:
   - **Nombre:** `TradingJournalAPI`
   - **Grupo de aplicaciones:** `TradingJournalAPI`
   - **Ruta física:** `C:\inetpub\wwwroot\TradingJournalAPI`
   - **Puerto:** `8080`
3. Clic en **Aceptar**

### Paso 8 — Crear App Pool para el Frontend en IIS
1. Clic derecho en **Grupos de aplicaciones** → **Agregar grupo de aplicaciones**
2. Rellena:
   - **Nombre:** `TradingJournalWeb`
   - **Versión .NET CLR:** `Sin código administrado`
   - **Modo de canalización:** `Integrado`
3. Clic en **Aceptar**

### Paso 9 — Crear Sitio para el Frontend en IIS
1. Clic derecho en **Sitios** → **Agregar sitio web**
2. Rellena:
   - **Nombre:** `TradingJournalWeb`
   - **Grupo de aplicaciones:** `TradingJournalWeb`
   - **Ruta física:** `C:\inetpub\wwwroot\TradingJournalWeb\wwwroot`
   - **Puerto:** `8081`
3. Clic en **Aceptar**

### Paso 10 — Reiniciar IIS
```powershell
iisreset
```

### Paso 11 — Verificar IIS
| URL | Resultado esperado |
|---|---|
| `http://localhost:8080/swagger` | Swagger UI |
| `http://localhost:8080/health` | `OK` |
| `http://localhost:8081` | Frontend TradingJournal |

---

## Parte 2 — Publicación en Azure App Service

### Paso 1 — Instalar Azure CLI
```powershell
winget install Microsoft.AzureCLI
```

### Paso 2 — Instalar extensión Azure App Service en VS Code
1. Abre VS Code
2. Extensiones (`Ctrl+Shift+X`) → busca **Azure App Service** → Instalar

### Paso 3 — Iniciar sesión en Azure desde VS Code
1. Clic en el ícono de **Azure** en la barra lateral
2. Clic en **Sign in to Azure**
3. Completa el login en el navegador

### Paso 4 — Crear Azure SQL Database
1. Entra a **portal.azure.com**
2. Busca **SQL Database** → **Crear**
3. Rellena:
   - **Grupo de recursos:** `TradingJournalRG`
   - **Nombre BD:** `TradingJournal`
   - **Servidor:** `tradingjournal-srv`
   - **Región:** `Brazil South`
   - **Autenticación:** SQL Authentication
   - **Usuario:** `Sqlsa`
   - **Contraseña:** `Trading2026*`

### Paso 5 — Configurar Firewall del SQL Server
1. En el portal ve a **SQL Server** `tradingjournal-srv`
2. **Redes** → **Acceso público**
3. Activa **"Permitir que los servicios de Azure accedan al servidor"**
4. Clic en **Agregar IP de cliente**
5. Clic en **Guardar**

### Paso 6 — Crear App Service para la API
En VS Code:
1. Clic derecho en **App Services** → **Create New Web App**
2. Rellena:
   - **Nombre:** `tradingjournal-api`
   - **Runtime:** `.NET 8 (LTS)`
   - **OS:** `Linux`
   - **Región:** `Brazil South`
   - **Plan:** `Free (F1)`

### Paso 7 — Configurar cadena de conexión para Azure

Verifica que `TradingJournal.API\appsettings.json` tenga la cadena de conexión de Azure SQL correcta:
```json
"Connectionwhitparams": "Server=tcp:tradingjournal-srv.database.windows.net,1433;Initial Catalog=TradingJournal;Persist Security Info=False;User ID=Sqlsa;Password=Trading2026*;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

Y que `Program.cs` use `Connectionwhitparams` en producción (ya está configurado así).

### Paso 8 — Publicar la API en Azure

```powershell
dotnet publish "c:\Users\EDWIN\Downloads\Despliegue 3\entrega-2-despliegue-sw\TradingJournal\TradingJournal.API\TradingJournal.API.csproj" -c Release -o "c:\Users\EDWIN\Downloads\Despliegue 3\entrega-2-despliegue-sw\publish\api"
```

Comprimir y desplegar:
```powershell
Compress-Archive -Path "c:\Users\EDWIN\Downloads\Despliegue 3\entrega-2-despliegue-sw\publish\api\*" -DestinationPath "c:\Users\EDWIN\Downloads\Despliegue 3\entrega-2-despliegue-sw\publish\api.zip" -Force

az webapp deploy --name tradingjournal-api --resource-group appsvc_windows_brazilsouth --src-path "c:\Users\EDWIN\Downloads\Despliegue 3\entrega-2-despliegue-sw\publish\api.zip" --type zip
```

### Paso 9 — Configurar Application Settings de la API
En VS Code:
1. Expande `tradingjournal-api` → **Application Settings**
2. Agrega:
   - `ASPNETCORE_ENVIRONMENT` = `Production`

### Paso 10 — Activar HTTPS Only en la API
1. En VS Code clic derecho en `tradingjournal-api` → **Open in Portal**
2. **Configuración** → **Configuración general**
3. Activa **HTTPS Only** → **Guardar**

### Paso 11 — Crear App Service para el Frontend
En VS Code:
1. Clic derecho en **App Services** → **Create New Web App**
2. Rellena:
   - **Nombre:** `tradingjournal-web`
   - **Runtime:** `.NET 8 (LTS)`
   - **OS:** `Linux`
   - **Región:** `Brazil South`
   - **Plan:** `Free (F1)`

### Paso 12 — Configurar URL de la API para el Frontend en Azure

Antes de publicar, actualiza `TradingJournal.Web\wwwroot\appsettings.json`:
```json
{ "ApiBaseUrl": "https://tradingjournal-api-bsa5cfdggthmdgfr.brazilsouth-01.azurewebsites.net/" }
```

### Paso 13 — Publicar el Frontend en Azure

```powershell
dotnet publish "c:\Users\EDWIN\Downloads\Despliegue 3\entrega-2-despliegue-sw\TradingJournal\TradingJournal.Web\TradingJournal.Web.csproj" -c Release -o "c:\Users\EDWIN\Downloads\Despliegue 3\entrega-2-despliegue-sw\publish\web"
```

Comprimir y desplegar:
```powershell
Compress-Archive -Path "c:\Users\EDWIN\Downloads\Despliegue 3\entrega-2-despliegue-sw\publish\web\wwwroot\*" -DestinationPath "c:\Users\EDWIN\Downloads\Despliegue 3\entrega-2-despliegue-sw\publish\web.zip" -Force

az webapp deploy --name tradingjournal-web --resource-group appsvc_windows_brazilsouth --src-path "c:\Users\EDWIN\Downloads\Despliegue 3\entrega-2-despliegue-sw\publish\web.zip" --type zip
```

### Paso 14 — Configurar Application Settings del Frontend
En VS Code:
1. Expande `tradingjournal-web` → **Application Settings**
2. Agrega:
   - `ASPNETCORE_ENVIRONMENT` = `Production`

### Paso 15 — Activar HTTPS Only en el Frontend
1. En VS Code clic derecho en `tradingjournal-web` → **Open in Portal**
2. **Configuración** → **Configuración general**
3. Activa **HTTPS Only** → **Guardar**

### Paso 16 — Verificar Azure
| URL | Resultado esperado |
|---|---|
| `https://tradingjournal-api-bsa5cfdggthmdgfr.brazilsouth-01.azurewebsites.net/swagger` | Swagger UI |
| `https://tradingjournal-api-bsa5cfdggthmdgfr.brazilsouth-01.azurewebsites.net/health` | `OK` |
| `https://tradingjournal-web-cxg5hhcxhue6g3a8.brazilsouth-01.azurewebsites.net` | Frontend TradingJournal |

---

## Evidencias requeridas

### IIS
- [ ] Creación App Pool `TradingJournalAPI`
- [ ] Creación App Pool `TradingJournalWeb`
- [ ] Configuración sitio API (ruta, puerto 8080)
- [ ] Configuración sitio Frontend (ruta, puerto 8081)
- [ ] `http://localhost:8081` funcionando
- [ ] `http://localhost:8080/health` respondiendo OK

### Azure
- [ ] Azure SQL Database creada
- [ ] App Service `tradingjournal-api` creado
- [ ] App Service `tradingjournal-web` creado
- [ ] Application Settings configurados
- [ ] HTTPS Only activado
- [ ] `https://tradingjournal-api-.../health` respondiendo OK
- [ ] `https://tradingjournal-web-...` frontend funcionando
