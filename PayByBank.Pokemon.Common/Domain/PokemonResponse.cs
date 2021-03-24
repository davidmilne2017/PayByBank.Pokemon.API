namespace PayByBank.Pokemon.Common.Domain
{
    public class PokemonResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Habitat { get; set; }
        public bool IsLegendary { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is PokemonResponse @object)
                return Name.Equals(@object.Name)
                    && Description.Equals(@object.Description)
                    && Habitat.Equals(@object.Habitat)
                    && IsLegendary.Equals(@object.IsLegendary);

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + Name.GetHashCode();
                hash = hash * 23 + Description.GetHashCode();
                hash = hash * 23 + Habitat.GetHashCode();
                hash = hash * 23 + IsLegendary.GetHashCode();
                return hash;
            }
        }
    }
}
