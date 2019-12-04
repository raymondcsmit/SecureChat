using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Wrap;

namespace Helpers.Resilience
{
    public class DatabaseResiliencePolicy : IResiliencePolicy
    {
        private const int RetryCount = 3;
        private const int RetryDelay = 3000;
        private const int CircuitBreakerCount = 10;
        private const int CircuitBreakerDelay = 10000;


        private readonly AsyncPolicyWrap _asyncRetryPolicy;
        private readonly PolicyWrap _retryPolicy;
        private static readonly CircuitBreakerPolicy _circuitBreakerPolicy = Policy
            .Handle<DbException>()
            .CircuitBreaker(
                exceptionsAllowedBeforeBreaking: CircuitBreakerCount,
                durationOfBreak: TimeSpan.FromMilliseconds(CircuitBreakerDelay),
                onBreak: (ex, breakDelay) => { },
                onReset: () => { },
                onHalfOpen: () => { }
            );
        private static readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicyAsync = Policy
            .Handle<DbException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: CircuitBreakerCount,
                durationOfBreak: TimeSpan.FromMilliseconds(CircuitBreakerDelay),
                onBreak: (ex, breakDelay) => { },
                onReset: () => { },
                onHalfOpen: () => { }
            );

        public DatabaseResiliencePolicy(ILogger<DatabaseResiliencePolicy> logger)
        {
            _asyncRetryPolicy = Policy.Handle<DbException>().
                WaitAndRetryAsync(
                    retryCount: RetryCount,
                    sleepDurationProvider: retry => TimeSpan.FromMilliseconds(TimeSpan.FromMilliseconds(RetryDelay).TotalMilliseconds * retry),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogTrace($"{exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {RetryCount}");
                    }).WrapAsync(_circuitBreakerPolicyAsync);

            _retryPolicy = Policy.Handle<DbException>().
                WaitAndRetry(
                    retryCount: RetryCount,
                    sleepDurationProvider: retry => TimeSpan.FromMilliseconds(TimeSpan.FromMilliseconds(RetryDelay).TotalMilliseconds * retry),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogTrace($"{exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {RetryCount}");
                    }).Wrap(_circuitBreakerPolicy);

        }

        public void Execute(Action operation)
        {
            _retryPolicy.Execute(operation);
        }

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            return _retryPolicy.Execute(operation);
        }

        public async Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            await _asyncRetryPolicy.ExecuteAsync(operation);
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default)
        {
            return await _asyncRetryPolicy.ExecuteAsync(operation);
        }
    }
}
