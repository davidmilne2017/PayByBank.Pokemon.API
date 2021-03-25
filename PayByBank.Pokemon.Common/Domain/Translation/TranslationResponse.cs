using System;
using System.Collections.Generic;
using System.Text;

namespace PayByBank.Pokemon.Common.Domain.Translation
{
    public class TranslationResponse
    {
        public TranslationResponseSuccess Success { get; set; }
        public TranslationResponseContents Contents { get; set; }
    }
}
