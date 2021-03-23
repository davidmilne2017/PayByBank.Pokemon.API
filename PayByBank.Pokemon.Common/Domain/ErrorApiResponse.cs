using System.Collections.Generic;

namespace PayByBank.Pokemon.Common.Domain
{
    public class ErrorApiResponse
    {
        public string Type { get; set; }
        public int Status { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
    }
}
