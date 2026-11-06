using LabPulse.BLL.DTOs.HealthChecks;
using LabPulse.BLL.Services.Contracts;
using LabPulse.DAL.DataContext.Entities;
using LabPulse.DAL.DataContext.Enums;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Sockets;

namespace LabPulse.BLL.Services.Implementations;

public class NetworkProbeService : INetworkProbeService
{
    private readonly IHttpClientFactory _httpFactory;

    public NetworkProbeService(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    public Task<ProbeResultDto> ProbeAsync(Service service, CancellationToken cancellationToken = default)
    {
        return service.CheckType == CheckType.Http 
            ? ProbeHttpAsync(service, cancellationToken) 
            : ProbeTcpAsync(service, cancellationToken);
    }
    private async Task<ProbeResultDto> ProbeHttpAsync(Service service, CancellationToken cancellationToken)
    {
        var scheme = service.UseHttps ? "https" : "http";
        var path = string.IsNullOrWhiteSpace(service.HealthPath) ? "/" : service.HealthPath;
        if (!path.StartsWith('/')) path = "/" + path;
        var url = $"{scheme}://{service.Host}:{service.Port}{path}";

        using var client = _httpFactory.CreateClient("probe");
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(service.TimeoutMs);

        var sw = Stopwatch.StartNew();
        try
        {
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cts.Token);
            sw.Stop();
            var code = (int)response.StatusCode;
            var ok = code < 500;
            return new ProbeResultDto
            {
                IsOnline = ok,
                ResponceTimeMs = (int)sw.ElapsedMilliseconds,
                StatusCode = code,
                ErrorMessage = ok ? null : $"HTTP {code}"
            };
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return Fail("Connection timed out");
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }
    private async Task<ProbeResultDto> ProbeTcpAsync(Service service, CancellationToken cancellationToken)
    {
        using var tcpClient = new TcpClient();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(service.TimeoutMs);
        var sw = Stopwatch.StartNew();
        try
        {
            await tcpClient.ConnectAsync(service.Host, service.Port, cts.Token);
            sw.Stop();
            return new ProbeResultDto
            {
                IsOnline = true,
                ResponceTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return Fail("Connection timed out");
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }
    private static ProbeResultDto Fail(string message)
    { 
        return new ProbeResultDto
        {
            IsOnline = false,
            ErrorMessage = message
        };
    }
}
