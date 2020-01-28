using System.Threading;
using System.Threading.Tasks;

namespace Helpers.RulesPattern
{
    internal class DefaultRule<TInput> : Rule<TInput>
    {
        public override async Task ApplyAsync(TInput input, CancellationToken ct = default)
        {
            await Task.CompletedTask;
        }
    }

    internal class DefaultRule<TInput, TOutput> : Rule<TInput, TOutput>
    {
        private readonly TOutput _defaultOutput;

        public DefaultRule(TOutput defaultOutput = default)
        {
            _defaultOutput = defaultOutput;
        }

        public override async Task<TOutput> ApplyAsync(TInput input, CancellationToken ct = default)
        {
            return await Task.FromResult(_defaultOutput);
        }
    }
}