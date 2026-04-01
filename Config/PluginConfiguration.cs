using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Plugins;
using Microsoft.Extensions.Logging;

namespace Gelato.Configuration
{
    // -------------------------------------------------------------------------
    // Entrada para addons Stremio adicionales (multi-addon)
    // -------------------------------------------------------------------------
    public class StremioAddonEntry
    {
        public string Name { get; set; } = "";
        public string ManifestUrl { get; set; } = "";
        public bool Enabled { get; set; } = true;
    }

    public class PluginConfiguration : BasePluginConfiguration
    {
        // -------------------------------------------------------------------------
        // Propiedades originales — NO tocar, el resto del código las usa
        // -------------------------------------------------------------------------
        public string MoviePath { get; set; } =
            Path.Combine(Path.GetTempPath(), "gelato", "movies");

        public string SeriesPath { get; set; } =
            Path.Combine(Path.GetTempPath(), "gelato", "series");

        public int StreamTTL { get; set; } = 3600;
        public int CatalogMaxItems { get; set; } = 100;

        /// <summary>
        /// URL del manifest Stremio (addon principal).
        /// Acepta cualquier addon compatible: Torrentio, CineCalidad, AIOStreams, etc.
        /// Ejemplos:
        ///   https://torrentio.strem.fun/manifest.json
        ///   https://stremio.cine-calidad.com/manifest.json
        ///   http://localhost:2634/stremio/HASH/manifest.json
        /// </summary>
        public string Url { get; set; } = "";

        public bool EnableSubs { get; set; } = false;
        public bool EnableMixed { get; set; } = false;
        public bool FilterUnreleased { get; set; } = false;
        public int FilterUnreleasedBufferDays { get; set; } = 30;
        public bool DisableSourceCount { get; set; } = true;
        public bool P2PEnabled { get; set; } = false;
        public int P2PDLSpeed { get; set; } = 0;
        public int P2PULSpeed { get; set; } = 0;
        public string FFmpegAnalyzeDuration { get; set; } = "5M";
        public string FFmpegProbeSize { get; set; } = "40M";
        public bool CreateCollections { get; set; } = false;
        public int MaxCollectionItems { get; set; } = 100;
        public bool DisableSearch { get; set; } = false;

        public List<UserConfig> UserConfigs { get; set; } = new List<UserConfig>();

        // -------------------------------------------------------------------------
        // NUEVO — Addons adicionales (multi-addon)
        // -------------------------------------------------------------------------
        /// <summary>
        /// Lista de addons Stremio adicionales. Los streams de todos se combinan.
        /// </summary>
        public List<StremioAddonEntry> ExtraAddons { get; set; } = new List<StremioAddonEntry>
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
        // Métodos originales — NO tocar
        // -------------------------------------------------------------------------
        public string GetBaseUrl()
        {
            if (string.IsNullOrWhiteSpace(Url))
                throw new InvalidOperationException("Gelato Url not configured.");

            var u = Url.Trim().TrimEnd('/');

            if (u.EndsWith("/manifest.json", StringComparison.OrdinalIgnoreCase))
                u = u[..^"/manifest.json".Length];

            return u;
        }

        [JsonIgnore]
        [XmlIgnore]
        public GelatoStremioProvider? stremio;

        [JsonIgnore]
        [XmlIgnore]
        public Folder? MovieFolder;

        [JsonIgnore]
        [XmlIgnore]
        public Folder? SeriesFolder;

        public PluginConfiguration GetEffectiveConfig(Guid userId)
        {
            var userConfig = UserConfigs.FirstOrDefault(u => u.UserId == userId);
            if (userConfig is null)
                return this;
            return userConfig.ApplyOverrides(this);
        }
    }

    public class UserConfig
    {
        public Guid UserId { get; set; }
        public string Url { get; set; } = "";
        public string MoviePath { get; set; } = "";
        public string SeriesPath { get; set; } = "";
        public bool DisableSearch { get; set; } = false;

        public PluginConfiguration ApplyOverrides(PluginConfiguration baseConfig)
        {
            return new PluginConfiguration
            {
                // Campos sobreescribibles por usuario
                Url = Url,
                MoviePath = MoviePath,
                SeriesPath = SeriesPath,
                DisableSearch = DisableSearch,

                // Resto desde config base
                StreamTTL = baseConfig.StreamTTL,
                CatalogMaxItems = baseConfig.CatalogMaxItems,
                EnableSubs = baseConfig.EnableSubs,
                EnableMixed = baseConfig.EnableMixed,
                FilterUnreleased = baseConfig.FilterUnreleased,
                FilterUnreleasedBufferDays = baseConfig.FilterUnreleasedBufferDays,
                DisableSourceCount = baseConfig.DisableSourceCount,
                P2PEnabled = baseConfig.P2PEnabled,
                P2PDLSpeed = baseConfig.P2PDLSpeed,
                P2PULSpeed = baseConfig.P2PULSpeed,
                FFmpegAnalyzeDuration = baseConfig.FFmpegAnalyzeDuration,
                FFmpegProbeSize = baseConfig.FFmpegProbeSize,
                CreateCollections = baseConfig.CreateCollections,
                MaxCollectionItems = baseConfig.MaxCollectionItems,
                UserConfigs = baseConfig.UserConfigs,
                ExtraAddons = baseConfig.ExtraAddons,
            };
        }
    }

    public class GelatoStremioProviderFactory
    {
        private readonly IHttpClientFactory _http;
        private readonly ILoggerFactory _log;

        public GelatoStremioProviderFactory(
            IHttpClientFactory http,
            ILoggerFactory log
        )
        {
            _http = http;
            _log = log;
        }

        public GelatoStremioProvider Create(Guid userId)
        {
            var cfg = GelatoPlugin.Instance!.Configuration.GetEffectiveConfig(userId);
            return new GelatoStremioProvider(
                cfg.GetBaseUrl(),
                _http,
                _log.CreateLogger<GelatoStremioProvider>()
            );
        }

        public GelatoStremioProvider Create(PluginConfiguration cfg)
        {
            return new GelatoStremioProvider(
                cfg.GetBaseUrl(),
                _http,
                _log.CreateLogger<GelatoStremioProvider>()
            );
        }
    }
}
