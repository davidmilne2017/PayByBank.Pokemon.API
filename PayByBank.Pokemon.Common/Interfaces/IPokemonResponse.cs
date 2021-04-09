using PayByBank.Pokemon.Common.Domain.Translation;

namespace PayByBank.Pokemon.Common.Interfaces
{
    public interface IPokemonResponse
    {
        public string Habitat { get; set; }
        public bool IsLegendary { get; set; }
        public TranslationType TranslationType { get; set; }
    }
}
