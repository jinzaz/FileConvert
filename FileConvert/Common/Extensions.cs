using Exceptionless;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileConvert.Common
{
    public static class Extensions
    {
        public static string Base64Filter(this string base64)
        {
            base64 = base64.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
            if (base64.Length % 4 > 0)
            {
                base64 = base64.PadRight(base64.Length + 4 - base64.Length % 4, '=');
            }
            return base64;
        }

        public static async Task<TResult> WaitAsync<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var delayTask = Task.Delay(timeout, timeoutCancellationTokenSource.Token);
                if (await Task.WhenAny(task, delayTask) == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    return await task;
                }
                throw new TimeoutException("The operation has timed out.");
            }
        }
    }
}
