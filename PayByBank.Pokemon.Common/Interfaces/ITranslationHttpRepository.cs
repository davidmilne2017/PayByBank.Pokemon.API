using PayByBank.Pokemon.Common.Domain.Translation;
using System.Threading;
using System.Threading.Tasks;

namespace PayByBank.Pokemon.Common.Interfaces
{
    public interface ITranslationHttpRepository
    {
        public Task<string> TranslateText(string text, TranslationType translationType, CancellationToken cancellationToken);
    }
}
