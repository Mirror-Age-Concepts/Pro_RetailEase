using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RetailEase.Application.Synchronization;

namespace RetailEase.Api.Services;

/// <summary>
/// Background service that periodically runs the transaction synchronization
/// pass over locally queued (outbox) transactions. Failed synchronizations are
/// retried through the synchronization engine's retry path.
/// </summary>
public class TransactionSyncWorker : BackgroundService
{
    private readonly ISynchronizationEngine _synchronizationEngine;
    private readonly ILogger<TransactionSyncWorker> _logger;
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(30);

    public TransactionSyncWorker(ISynchronizationEngine synchronizationEngine, ILogger<TransactionSyncWorker> logger)
    {
        _synchronizationEngine = synchronizationEngine;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var request = new SynchronizationRequest(StoreId: Guid.Empty, TerminalId: Guid.Empty);
                var result = await _synchronizationEngine.SynchronizeAsync(request, stoppingToken);

                if (result.ProcessedCount > 0)
                {
                    _logger.LogInformation(
                        "Transaction synchronization pass completed. Processed={Processed}, Succeeded={Succeeded}, Failed={Failed}.",
                        result.ProcessedCount,
                        result.SucceededCount,
                        result.FailedCount);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Periodic transaction synchronization failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
