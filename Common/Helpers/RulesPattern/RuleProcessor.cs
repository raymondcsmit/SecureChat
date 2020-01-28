using System.Threading;
using System.Threading.Tasks;

namespace Helpers.RulesPattern
{
    public static class RuleProcessor
    {
        public static RuleProcessor<TInput> Initialize<TInput>()
        {
            var processor = new RuleProcessor<TInput>();
            processor.AddRule(new DefaultRule<TInput>());
            return processor;
        }

        public static RuleProcessor<TInput, TOutput> Initialize<TInput, TOutput>(TOutput defaultOutput = default)
        {
            var processor = new RuleProcessor<TInput, TOutput>();
            processor.AddRule(new DefaultRule<TInput, TOutput>(defaultOutput));
            return processor;
        }
    }

    public class RuleProcessor<TInput> : Rule<TInput>
    {
        public override async Task ApplyAsync(TInput input, CancellationToken ct = default)
        {
            await Next.ApplyAsync(input, ct);
        }
    }

    public class RuleProcessor<TInput, TOutput> : Rule<TInput, TOutput>
    {
        public override async Task<TOutput> ApplyAsync(TInput input, CancellationToken ct = default)
        {
            return await Next.ApplyAsync(input, ct);
        }
    }
}