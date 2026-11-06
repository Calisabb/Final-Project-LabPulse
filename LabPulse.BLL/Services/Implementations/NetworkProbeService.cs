using LabPulse.BLL.DTOs.HealthChecks;
using LabPulse.BLL.Services.Contracts;
using LabPulse.DAL.DataContext.Entities;
using LabPulse.DAL.DataContext.Enums;
using System.Diagnostics;
using System.Net.Sockets;

namespace LabPulse.BLL.Services.Implementations;

public class NetworkProbeService : INetworkProbeService
{
    public Task<ProbeResultDto> ProbeAsync(Service service, CancellationToken cancellationToken = default)
    {
        return service.CheckType == CheckType.Http 
            ? throw new NotImplementedException() 
            : ProbeTcpAsync(service, cancellationToken);
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
        finally
        {
            sw.Stop();
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
