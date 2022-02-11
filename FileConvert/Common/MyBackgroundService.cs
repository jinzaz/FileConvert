using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileConvert.Common
{
    public abstract class MyBackgroundService : IHostedService, IDisposable
    {
        private Task _executingTask;

        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        //
        // 摘要:
        //     This method is called when the Microsoft.Extensions.Hosting.IHostedService starts.
        //     The implementation should return a task that represents the lifetime of the long
        //     running operation(s) being performed.
        //
        // 参数:
        //   stoppingToken:
        //     Triggered when Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)
        //     is called.
        //
        // 返回结果:
        //     A System.Threading.Tasks.Task that represents the long running operations.
        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        //
        // 摘要:
        //     Triggered when the application host is ready to start the service.
        //
        // 参数:
        //   cancellationToken:
        //     Indicates that the start process has been aborted.
        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(cancellationToken);
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            return Task.CompletedTask;
        }

        //
        // 摘要:
        //     Triggered when the application host is performing a graceful shutdown.
        //
        // 参数:
        //   cancellationToken:
        //     Indicates that the shutdown process should no longer be graceful.
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask != null)
            {
                try
                {
                    _stoppingCts.Cancel();
                }
                finally
                {
                    await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));
                }
            }
        }

        public virtual void Dispose()
        {
            _stoppingCts.Cancel();
        }
    }
}
