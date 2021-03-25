using System.Collections.Generic;

namespace PayByBank.Pokemon.Common.Domain.Pokemon
{
    public class PokemonApiReturn
    {
        public IEnumerable<FlavorTextEntry> flavor_text_entries { get; set; }
        public Habitat habitat { get; set; }
        public bool is_legendary { get; set; }
        public IEnumerable<Name> names { get; set; }
    }
}
