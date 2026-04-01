<p align="center">
  <img src="logo.png" width="120" alt="Gelato Logo">
</p>

<h1 align="center">Gelato — Multi-Addon Fork</h1>
<p align="center"><em>Integración Stremio para Jellyfin — Compatible con cualquier addon de la comunidad</em></p>

<p align="center">
  <a href="https://github.com/j4ckgrey/Gelato">Upstream original</a> •
  <a href="https://github.com/lostb1t/Gelato">Repo raíz</a>
</p>

---

## ¿Qué es esto?

Fork de [j4ckgrey/Gelato](https://github.com/j4ckgrey/Gelato) que elimina la dependencia exclusiva de **AIOStreams** y permite usar **cualquier addon Stremio** de la comunidad directamente en Jellyfin 10.11.2 en Windows.

## ✨ Cambios respecto al original

| Original | Este fork |
|---|---|
| Solo funciona con AIOStreams | Compatible con cualquier manifest Stremio |
| Un único manifest configurable | Múltiples addons simultáneos |
| UI enfocada a AIOStreams | UI genérica con ejemplos de varios addons |

## 🍿 Addons compatibles

Cualquier addon que exponga un manifest Stremio estándar en `/manifest.json`:

- **[Torrentio](https://torrentio.strem.fun)** — Streams desde múltiples fuentes torrent
- **[CineCalidad](https://stremio.cine-calidad.com)** — Contenido en español Latino
- **[AIOStreams](https://aiostreams.elfhosted.com)** — Agregador con soporte debrid
- **[Cinemeta](https://v3-cinemeta.strem.io)** — Metadatos TMDB/IMDB
- Y cualquier otro addon de la [comunidad Stremio](https://www.stremio.com/addon-sdk)

## 🚀 Instalación en Jellyfin 10.11.2 (Windows)

### 1. Agregar repositorio de plugins

En Jellyfin → **Dashboard → Plugins → Repositories** → Agregar:

```
https://raw.githubusercontent.com/TU_USUARIO/Gelato/gh-pages/repository.json
```

### 2. Instalar el plugin

Dashboard → Plugins → Catálogo → **Gelato** → Instalar

### 3. Configurar

1. Reinicia Jellyfin después de instalar
2. Ve a **Plugins → Gelato → Configuración**
3. Pega la URL de tu manifest Stremio favorito:
   - Torrentio: `https://torrentio.strem.fun/manifest.json`
   - CineCalidad: `https://stremio.cine-calidad.com/manifest.json`
   - AIOStreams: `http://TU_IP:PUERTO/stremio/HASH/manifest.json`
4. (Opcional) Agrega addons adicionales en la sección de abajo
5. Guarda y reinicia Jellyfin

### 4. Agregar rutas a la biblioteca

Agrega las rutas base a una biblioteca de Jellyfin:
- Películas: `stremio/movies`
- Series: `stremio/series`

Luego ejecuta un **Escaneo de biblioteca**.

## 🏗️ Compilar desde fuente (Windows)

```powershell
# Requiere .NET 9 SDK
dotnet build Gelato.csproj -c Release
# El .dll queda en bin/Release/net9.0/Gelato.dll
# Cópialo a: %APPDATA%\jellyfin\plugins\Gelato\
```

## 📋 Requisitos

- Jellyfin **10.11.2** o superior
- .NET 9 Runtime (incluido en el instalador de Jellyfin)
- Windows 10/11 (también funciona en Linux/Docker)
- Un addon Stremio compatible (ver lista arriba)

## ❓ FAQ

**¿Necesito cuenta de debrid?**
No es obligatorio. Torrentio funciona sin debrid (aunque más lento). Para mejor experiencia, usa Real-Debrid o Torbox.

**¿Puedo usar CineCalidad para contenido en español?**
Sí, es uno de los mejores addons para contenido latino. Agrega `https://stremio.cine-calidad.com/manifest.json` en la configuración.

**¿Cómo agrego múltiples addons?**
En la configuración, usa la sección "Addons Adicionales" para agregar tantos manifests como quieras. Los streams de todos se combinan.

**¿Funciona con Torrentio configurado (con API key de debrid)?**
Sí. Usa tu URL personalizada de Torrentio con tus parámetros configurados.

## Créditos

- [lostb1t](https://github.com/lostb1t) — Autor original de Gelato
- [j4ckgrey](https://github.com/j4ckgrey) — Fork intermedio
- Este fork agrega soporte multi-addon para la comunidad

## Licencia

GPL-3.0 — Ver [LICENSE](LICENSE)
# Multi-addon fork
