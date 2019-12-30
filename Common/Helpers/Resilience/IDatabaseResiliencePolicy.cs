using System;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.Resilience
{
    public interface IDatabaseResiliencePolicy
    {
        void Execute(Action operation);

        TResult Execute<TResult>(Func<TResult> operation);

        Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default);

        Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default);
    }
}
