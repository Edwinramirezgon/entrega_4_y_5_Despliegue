# TradingJournal — CI/CD Pipeline

## Integrantes del equipo

> _(Completen con sus nombres completos aquí)_

---

## Videos de evidencia

| Actividad | Enlace |
|-----------|--------|
| Integración Continua (CI) | _(Agregar enlace al video de CI)_ |
| Entrega Continua (CD) | _(Agregar enlace al video de CD)_ |

---

## Descripción del proyecto

**TradingJournal** es una aplicación web de registro y seguimiento de operaciones de trading, compuesta por:

- `TradingJournal.API` — Backend ASP.NET Core 8 (Web API REST)
- `TradingJournal.Web` — Frontend Blazor WebAssembly
- `TradingJournal.Shared` — Librería compartida de entidades y DTOs

### URLs de la aplicación desplegada

| Componente | URL |
|------------|-----|
| API (Backend) | https://tradingjournalapi-evfgeaese2gpg2e2.canadacentral-01.azurewebsites.net |
| Web (Frontend) | https://tradingjournalweb-hyh0hna6dweka3bh.canadacentral-01.azurewebsites.net |

---

## Fechas de despliegue

| Evento | Fecha |
|--------|-------|
| Primer despliegue CI/CD | _(Agregar fecha)_ |
| Último despliegue exitoso | _(Agregar fecha)_ |

---

## Actividad 1: Integración Continua (CI)

### Herramientas utilizadas

- Visual Studio 2022 / .NET 8
- GitHub (control de versiones)
- Azure DevOps (pipeline CI)

### Pipeline — `azure-pipelines.yml`

El pipeline se activa automáticamente con cada push a la rama `main` y ejecuta los siguientes pasos:

| Paso | Descripción |
|------|-------------|
| `UseDotNet@2` | Instala el SDK de .NET 8 |
| `CmdLine@2` | Instala `dotnet-ef` (Entity Framework CLI) |
| `dotnet build` API | Compila el proyecto `TradingJournal.API` en modo Release |
| `dotnet build` Web | Compila el proyecto `TradingJournal.Web` en modo Release |
| `dotnet ef migrations script` | Genera el script SQL de migraciones de base de datos |
| `DotNetCoreCLI@2` publish API | Publica la API como artefacto (`win-x86`, self-contained) |
| `DotNetCoreCLI@2` publish Web | Publica el frontend como artefacto (`win-x86`, self-contained) |
| `PublishBuildArtifacts@1` | Publica todos los artefactos bajo el nombre `drop` |

### Artefactos generados

```
drop/
├── api/          ← Publicación de TradingJournal.API
├── web/          ← Publicación de TradingJournal.Web
└── migrate.sql   ← Script de migraciones de base de datos
```

---

## Actividad 2: Entrega Continua (CD)

### Infraestructura en Azure

| Recurso | Valor |
|---------|-------|
| Resource Group | `TradingJournal` |
| Región | Canada Central |
| App Service — API | `TradingJournalAPI` |
| App Service — Web | `TradingJournalWeb` |
| Base de datos | Azure SQL Server |

### Flujo de despliegue automático

1. Se realiza un `git push` a la rama `main` del repositorio GitHub.
2. Azure DevOps detecta el cambio y dispara el pipeline CI automáticamente.
3. El pipeline compila, genera artefactos y los publica en `drop`.
4. La etapa CD toma los artefactos y los despliega en los App Services mediante MSDeploy (WMSVC).
5. La aplicación queda disponible en las URLs de Azure sin intervención manual.

### Método de despliegue

- Protocolo: **MSDeploy / WMSVC**
- Configuración: `Release`
- Runtime: `win-x86`, self-contained
- Base de datos: Azure SQL Server, actualizada con el script `migrate.sql` generado en CI

---

## Evidencias paso a paso

### Configuración de infraestructura en Azure

**Paso 1 — Creación del grupo de recursos en Azure**

Se crea el grupo de recursos `TradingJournal` en la región Canada Central desde el portal de Azure. Este grupo contendrá todos los recursos de la aplicación (App Services, SQL Server, base de datos).

![01](./Evidencias/01.png)

---

**Paso 2 — Grupo de recursos creado exitosamente**

Confirmación en el portal de Azure de que el grupo de recursos `TradingJournal` fue creado correctamente y está disponible para alojar los recursos del proyecto.

![02](./Evidencias/02.png)

---

**Paso 3 — Creación del App Service de TradingJournalAPI desde Visual Studio**

Desde el asistente de publicación de Visual Studio se configura y crea el App Service `TradingJournalAPI` directamente en Azure, seleccionando el grupo de recursos, región y plan de servicio correspondientes.

![04](./Evidencias/04.png)

---

**Paso 4 — Verificación del App Service en el portal de Azure**

Se verifica en la consola de Azure que el App Service `TradingJournalAPI` fue creado correctamente y aparece listado dentro del grupo de recursos `TradingJournal`.

![05](./Evidencias/05.png)

---

**Paso 5 — Revisión del grupo de recursos con la API y el plan de servicio**

Vista del grupo de recursos `TradingJournal` en Azure donde se confirma la presencia del App Service de la API junto con su App Service Plan asociado.

![06](./Evidencias/06.png)

---

**Paso 6 — Creación del App Service completada desde Visual Studio**

Visual Studio confirma que el proceso de creación del App Service `TradingJournalAPI` finalizó satisfactoriamente y el recurso está listo para recibir publicaciones.

![07](./Evidencias/07.png)

---

**Paso 7 — Creación del servidor de base de datos en Azure desde Visual Studio**

Desde el asistente de dependencias de Visual Studio se configura y crea el servidor de Azure SQL Server que alojará la base de datos de la aplicación. En este punto Azure solo permite autenticación mediante Microsoft Entra ID, no usuario y contraseña.

![08](./Evidencias/08.png)

---

**Paso 8 — Creación de la base de datos en el servidor desde Visual Studio**

Se crea la base de datos `TradingJournal` dentro del servidor SQL recién creado. Se configura desde Visual Studio usando el asistente de servicios conectados. Azure actualmente solo permite configurar el acceso al servidor SQL mediante Microsoft Entra ID en este flujo, no permite usuario y contraseña directamente.

![09](./Evidencias/09.png)

---

### Configuración de la publicación

**Paso 9 — Conexión a la base de datos desde Visual Studio**

Se establece la conexión a la base de datos de Azure SQL desde Visual Studio para verificar la conectividad y configurar la cadena de conexión del proyecto.

![11](./Evidencias/11.png)

---

**Paso 10 — Selección de secretos en la configuración de publicación**

En el perfil de publicación se dejan marcados únicamente los secretos necesarios (cadena de conexión a la base de datos), asegurando que solo la información sensible requerida sea incluida en la configuración de despliegue.

![12](./Evidencias/12.png)

---

**Paso 11 — Configuración de la URL del backend en el frontend**

En el archivo `Program.cs` del proyecto `TradingJournal.Web` se configura la URL base de la API (`TradingJournalAPI`) para que el frontend Blazor apunte correctamente al backend desplegado en Azure.

![13](./Evidencias/13.png)

---

**Paso 12 — Configuración de ejecución automática de migraciones al publicar**

En las opciones de publicación se activa la opción para que Entity Framework ejecute las migraciones de base de datos automáticamente durante el proceso de despliegue, garantizando que el esquema de la BD esté siempre actualizado.

![14](./Evidencias/14.png)

---

**Paso 13 — Selección de opciones de publicación según versión y suscripción**

Se configuran las opciones de publicación adecuadas según la versión de .NET utilizada (net8.0) y el tipo de suscripción de Azure disponible: runtime `win-x86`, self-contained, configuración Release.

![15](./Evidencias/15.png)

---

### Resolución de errores de conexión a base de datos

**Paso 14 — Error de publicación por fallo de conexión a la base de datos**

Al intentar publicar por primera vez, el proceso falla debido a que el usuario no tiene permisos suficientes para conectarse al servidor de Azure SQL. El error indica que la autenticación con las credenciales configuradas no es válida.

![16](./Evidencias/16.png)

---

**Paso 15 — Asignación del rol de administrador en la base de datos al usuario de Entra ID**

Para resolver el error de conexión, se accede al portal de Azure y se asigna el rol de administrador de Azure SQL al usuario de Microsoft Entra ID del equipo, otorgando los permisos necesarios para gestionar la base de datos.

![17](./Evidencias/17.png)

---

**Paso 16 — Creación de usuario SQL con credenciales desde SSMS**

Con el usuario de Entra ID ya como administrador, se conecta a la base de datos mediante SQL Server Management Studio (SSMS) y se ejecuta un script para crear un usuario SQL con usuario y contraseña tradicionales, permitiendo la autenticación por credenciales desde la aplicación.

![18](./Evidencias/18.png)

---

**Paso 17 — Actualización de la cadena de conexión con las nuevas credenciales**

Se modifica la cadena de conexión en el perfil de publicación para usar el usuario y contraseña SQL recién creados, reemplazando la autenticación de Entra ID por credenciales estándar compatibles con la aplicación.

![19](./Evidencias/19.png)

---

**Paso 18 — Habilitación de autenticación mixta en el servidor SQL**

En la configuración del servidor de Azure SQL se desactiva la restricción de "solo Microsoft Entra ID" para permitir también la autenticación con usuario y contraseña SQL, habilitando así el modo de autenticación mixta.

![20](./Evidencias/20.png)

---

### Publicación exitosa del backend

**Paso 19 — Publicación exitosa de la API y verificación de conexión a la base de datos**

Con los permisos y credenciales correctamente configurados, la publicación de `TradingJournal.API` se completa exitosamente. Se verifica que la API tiene conectividad con la base de datos de Azure SQL accediendo al endpoint de Swagger.

![21](./Evidencias/21.png)

---

### Publicación del frontend

**Paso 20 — Publicación del frontend desde Visual Studio**

Se inicia el proceso de publicación del proyecto `TradingJournal.Web` (Blazor WebAssembly) desde Visual Studio usando el perfil de publicación configurado para el App Service `TradingJournalWeb`.

![22](./Evidencias/22.png)

---

**Paso 21 — Publicación del frontend completada exitosamente**

Visual Studio confirma que la publicación del frontend Blazor WebAssembly en el App Service `TradingJournalWeb` de Azure finalizó sin errores.

![23](./Evidencias/23.png)

---

**Paso 22 — Revisión de parámetros de configuración de la publicación del frontend**

Se revisan los parámetros del perfil de publicación del frontend: App Service destino, configuración Release, runtime y URL del sitio publicado.

![24](./Evidencias/24.png)

---

**Paso 23 — Verificación del frontend en producción con conexión a la API**

Se accede a la URL del frontend desplegado en Azure y se verifica que la aplicación Blazor carga correctamente, se comunica con la API y todas las funcionalidades operan sin errores.

![25](./Evidencias/25.png)

---

### Configuración del pipeline CI en Azure DevOps

**Paso 24 — Creación de un nuevo proyecto en Azure DevOps**

Se accede a Azure DevOps y se crea un nuevo proyecto llamado `TradingJournal` que alojará los pipelines de CI y CD conectados al repositorio de GitHub.

![26](./Evidencias/26.png)

---

**Paso 25 — Desactivación de la creación clásica de pipelines**

En la configuración del proyecto de Azure DevOps se desactiva la opción de creación de pipelines clásicos (Build y Release en modo visual), forzando el uso de pipelines YAML como buena práctica moderna.

![27](./Evidencias/27.png)

---

**Paso 26 — Verificación de Parallel Jobs activos**

Se verifica en la configuración de Azure DevOps que el proyecto tiene al menos un Parallel Job disponible, requisito indispensable para que los pipelines puedan ejecutarse. Sin este recurso los pipelines quedan en cola indefinidamente.

![28](./Evidencias/28.png)

---

**Paso 27 — Creación del pipeline CI en YAML y guardado en el repositorio**

Se crea el pipeline de integración continua usando el editor YAML de Azure DevOps, configurando el trigger en la rama `main`, los pasos de build, generación de script SQL y publicación de artefactos. El archivo `azure-pipelines.yml` se guarda directamente en el repositorio de GitHub.

![29](./Evidencias/29.png)

---

**Paso 28 — Archivo `azure-pipelines.yml` en el repositorio de GitHub**

Vista del archivo `azure-pipelines.yml` ya commiteado en el repositorio de GitHub, mostrando la definición completa del pipeline CI con todos sus pasos configurados.

![30](./Evidencias/30.png)

---

**Paso 29 — Ejecución de prueba del pipeline CI**

Se dispara manualmente la primera ejecución del pipeline CI para verificar que todos los pasos funcionan correctamente: instalación del SDK, build de API y Web, generación del script de migraciones y publicación de artefactos.

![31](./Evidencias/31.png)

---

### Configuración del pipeline CD en Azure DevOps

**Paso 30 — Creación del pipeline de Release (CD)**

Se crea un nuevo pipeline de Release en Azure DevOps que tomará los artefactos generados por el pipeline CI y los desplegará automáticamente en los App Services de Azure.

![32](./Evidencias/32.png)

---

**Paso 31 — Configuración de parámetros del pipeline de Release**

Se configuran los parámetros del pipeline de Release: nombre del pipeline, fuente de artefactos (pipeline CI `drop`), etapas de despliegue para API y Web, y conexión de servicio a la suscripción de Azure.

![33](./Evidencias/33.png)

---

**Paso 32 — Activación del trigger de despliegue continuo**

Se activa el trigger de Continuous Deployment en el pipeline de Release para que se dispare automáticamente cada vez que el pipeline CI genera una nueva build exitosa, completando así el flujo CI/CD sin intervención manual.

![34](./Evidencias/34.png)

---

**Paso 33 — Configuración de la tarea Azure App Service Deploy**

Se configuran los parámetros de la tarea de despliegue al App Service: suscripción de Azure, nombre del App Service destino (`TradingJournalAPI` / `TradingJournalWeb`), paquete de artefactos y método de despliegue MSDeploy.

![35](./Evidencias/35.png)

---

**Paso 34 — Configuración de la tarea Azure SQL Database Deployment**

Se configura la tarea de despliegue de base de datos que ejecutará el script `migrate.sql` generado en CI contra la base de datos de Azure SQL, manteniendo el esquema sincronizado con cada despliegue.

![36](./Evidencias/36.png)

---

**Paso 35 — Guardado del pipeline de Release**

Se guarda la configuración completa del pipeline de Release con todas las etapas, tareas y triggers configurados, dejando el flujo CD listo para ejecutarse automáticamente.

![37](./Evidencias/37.png)

---

### Prueba del flujo CI/CD completo

**Paso 36 — Creación de nueva entidad para probar el flujo**

Para validar el flujo CI/CD de extremo a extremo, se crea una nueva entidad `Pruebaci` en el proyecto `TradingJournal.Shared` y se registra en el `DataContext` de Entity Framework, simulando un cambio real de funcionalidad.

![38](./Evidencias/38.png)

---

**Paso 37 — Creación del controlador para la nueva entidad**

Se crea el controlador `PruebaciControllers` en `TradingJournal.API` exponiendo los endpoints REST para la nueva entidad, completando el cambio que será desplegado automáticamente por el pipeline.

![39](./Evidencias/39.png)

---

**Paso 38 — Creación de la migración de base de datos**

Se ejecuta `dotnet ef migrations add pruebacis` para generar la migración de Entity Framework que refleja el nuevo modelo en la base de datos. Esta migración será convertida a script SQL por el pipeline CI.

![40](./Evidencias/40.png)

---

**Paso 39 — Push a GitHub dispara el pipeline CI automáticamente**

Se realiza el commit y push de todos los cambios a la rama `main` del repositorio en GitHub. Azure DevOps detecta el push inmediatamente y dispara el pipeline CI de forma automática sin ninguna intervención manual.

![41](./Evidencias/41.png)

---

**Paso 40 — Pipeline CI finalizado exitosamente**

El pipeline de integración continua completa todos sus pasos sin errores: instalación del SDK, build de API y Web, generación del script SQL de migraciones y publicación de los artefactos en `drop`.

![42](./Evidencias/42.png)

---

**Paso 41 — Pipeline CD disparado y finalizado exitosamente**

Al terminar el pipeline CI, el trigger de Continuous Deployment activa automáticamente el pipeline de Release. Este despliega los artefactos en los App Services de Azure y ejecuta el script de migraciones en la base de datos, todo sin intervención manual.

![43](./Evidencias/43.png)

---

**Paso 42 — Verificación de los cambios desplegados en producción**

Se accede a la URL de la API en Azure y se verifica en Swagger que el nuevo endpoint de `Pruebaci` está disponible, confirmando que el flujo CI/CD completo funcionó correctamente: el cambio en el código se reflejó automáticamente en producción.

![44](./Evidencias/44.png)

---

## Estructura del repositorio

```
├── TradingJournal.API/       ← Backend ASP.NET Core 8
├── TradingJournal.Web/       ← Frontend Blazor WebAssembly
├── TradingJournal.Shared/    ← Entidades y DTOs compartidos
├── Evidencias/               ← Capturas de pantalla CI/CD (44 imágenes)
├── azure-pipelines.yml       ← Definición del pipeline CI/CD
└── README.md
```
