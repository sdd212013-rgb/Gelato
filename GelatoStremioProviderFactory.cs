using System.Net.Http;
using Gelato.Configuration;
using Microsoft.Extensions.Logging;

namespace Gelato;

/// <summary>
/// Fábrica que crea instancias de GelatoStremioProvider a partir de la configuración.
/// Este fork usa GetBaseUrl() para soportar cualquier addon Stremio, no solo AIOStreams.
/// </summary>
public class GelatoStremioProviderFactory
{
    private readonly IHttpClientFactory _http;
    private readonly ILogger<GelatoStremioProvider> _log;

    public GelatoStremioProviderFactory(
        IHttpClientFactory http,
        ILogger<GelatoStremioProvider> log)
    {
        _http = http;
        _log = log;
    }

    public GelatoStremioProvider? Create(PluginConfiguration? cfg)
    {
        if (cfg == null) return null;

        var baseUrl = cfg.GetBaseUrl();

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            // No hay addon configurado aún — devolvemos null, no un error
            return null;
        }

        return new GelatoStremioProvider(baseUrl, _http, _log);
    }
}
