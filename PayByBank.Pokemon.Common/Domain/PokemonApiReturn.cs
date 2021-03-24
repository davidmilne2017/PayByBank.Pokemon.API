using System;
using System.Collections.Generic;
using System.Text;

namespace PayByBank.Pokemon.Common.Domain
{
    public class PokemonApiReturn
    {
        public IEnumerable<FlavorTextEntry> flavor_text_entries { get; set; }
        public Habitat habitat { get; set; }
        public bool is_legendary { get; set; }
        public IEnumerable<Name> names { get; set; }
    }

    public class FlavorTextEntry
    {
        public string flavor_text { get; set; }
        public Language language { get; set; }
    }

    public class Name
    {
        public Language language { get; set; }
        public string name { get; set; }
    }

    public class Language
    {
        public string name { get; set; }
    }

    public class Habitat
    {
        public string name { get; set; }
    }
}
