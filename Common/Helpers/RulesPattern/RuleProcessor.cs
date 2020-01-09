using System.Threading;
using System.Threading.Tasks;

namespace Helpers.RulesPattern
{
    public static class RuleProcessor
    {
        public static Rule<TInput> Initialize<TInput>()
        {
            var processor = new RuleProcessor<TInput>();
            processor.AddRule(new DefaultRule<TInput>());
            return processor;
        }

        public static Rule<TInput, TOutput> Initialize<TInput, TOutput>()
        {
            var processor = new RuleProcessor<TInput, TOutput>();
            processor.AddRule(new DefaultRule<TInput, TOutput>());
            return processor;
        }
    }

    internal class RuleProcessor<TInput> : Rule<TInput>
    {
        public override async Task ApplyAsync(TInput input, CancellationToken ct = default)
        {
            await Next.ApplyAsync(input, ct);
        }
    }

    internal class RuleProcessor<TInput, TOutput> : Rule<TInput, TOutput>
    {
        public override async Task<TOutput> ApplyAsync(TInput input, CancellationToken ct = default)
        {
            return await Next.ApplyAsync(input, ct);
        }
    }
}