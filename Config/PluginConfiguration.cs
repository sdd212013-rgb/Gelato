using System;
using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace Gelato.Configuration;

/// <summary>
/// Configuración de un único addon Stremio compatible.
/// </summary>
public class StremioAddonEntry
{
    public string Name { get; set; } = "";
    public string ManifestUrl { get; set; } = "";
    public bool Enabled { get; set; } = true;
}

public class PluginUserConfig
{
    public Guid UserId { get; set; }

    /// <summary>URL del manifest Stremio del usuario (sobreescribe el global si no está vacío).</summary>
    public string ManifestUrl { get; set; } = "";

    public PluginConfiguration? ApplyOverrides(PluginConfiguration? global)
    {
        if (global == null) return null;
        var cfg = global.ShallowCopy();
        if (!string.IsNullOrWhiteSpace(ManifestUrl))
            cfg.ManifestUrl = ManifestUrl;
        return cfg;
    }
}

public class PluginConfiguration : BasePluginConfiguration
{
    // -------------------------------------------------------------------------
    // Configuración principal del addon
    // -------------------------------------------------------------------------

    /// <summary>
    /// URL base del manifest Stremio (cualquier addon compatible: Torrentio,
    /// CineCalidad, AIOStreams, Cinemeta, etc.)
    /// Ejemplo Torrentio:
    ///   https://torrentio.strem.fun/CONFIGURACION/manifest.json
    /// Ejemplo CineCalidad:
    ///   https://stremio.cine-calidad.com/manifest.json
    /// Ejemplo AIOStreams selfhosted:
    ///   http://localhost:2634/stremio/HASH/manifest.json
    /// </summary>
    public string ManifestUrl { get; set; } = "";

    // -------------------------------------------------------------------------
    // Addons adicionales (múltiples manifests, se agregan streams de todos)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Lista de addons Stremio adicionales. Los streams de todos se
    /// combinan y presentan al usuario ordenados por nombre.
    /// </summary>
    public List<StremioAddonEntry> ExtraAddons { get; set; } = new()
    {
        new StremioAddonEntry
        {
            Name = "Torrentio",
            ManifestUrl = "https://torrentio.strem.fun/manifest.json",
            Enabled = false
        },
        new StremioAddonEntry
        {
            Name = "CineCalidad",
            ManifestUrl = "https://stremio.cine-calidad.com/manifest.json",
            Enabled = false
        }
    };

    // -------------------------------------------------------------------------
    // Opciones de streams
    // -------------------------------------------------------------------------

    /// <summary>Habilita soporte P2P/torrents (requiere cliente compatible).</summary>
    public bool P2PEnabled { get; set; } = true;

    /// <summary>Prefijo de carpeta para películas en la biblioteca.</summary>
    public string MovieLibraryPath { get; set; } = "stremio/movies";

    /// <summary>Prefijo de carpeta para series en la biblioteca.</summary>
    public string SeriesLibraryPath { get; set; } = "stremio/series";

    // -------------------------------------------------------------------------
    // Configuración por usuario
    // -------------------------------------------------------------------------
    public List<PluginUserConfig> UserConfigs { get; set; } = new();

    // -------------------------------------------------------------------------
    // Propiedades de runtime (no serializadas, usadas internamente)
    // -------------------------------------------------------------------------
    [System.Text.Json.Serialization.JsonIgnore]
    public GelatoStremioProvider? stremio { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public object? MovieFolder { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public object? SeriesFolder { get; set; }

    public PluginConfiguration ShallowCopy() => (PluginConfiguration)MemberwiseClone();

    /// <summary>
    /// Devuelve la URL base del manifest sin el segmento /manifest.json final.
    /// Gelato espera la base URL, no la URL completa del manifest.
    /// </summary>
    public string GetBaseUrl()
    {
        var url = ManifestUrl?.Trim() ?? "";
        if (url.EndsWith("/manifest.json", StringComparison.OrdinalIgnoreCase))
            url = url[..^"/manifest.json".Length];
        return url.TrimEnd('/');
    }
}
