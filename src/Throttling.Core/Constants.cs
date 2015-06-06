using System;
using System.Threading.Tasks;

namespace Throttling
{
    public static class Constants
    {
        public const int Status429TooManyRequests = 429;

        internal static readonly Task CompletedTask = CreateCompletedTask();

        private static Task CreateCompletedTask()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }
    }
}