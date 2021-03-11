using System.Collections.Generic;
using VariableKeywordMatcher.Interface;

namespace VariableKeywordMatcher.Model
{
    public class MatchCache
    {
        public MatchCache(string originalString)
        {
            OriginalString = originalString;
        }

        public string OriginalString { get; }

        public int StringLength => OriginalString?.Length ?? 0;

        public Dictionary<string, ISpellCache> SpellCaches { get; set; } = new Dictionary<string, ISpellCache>();
    }
}
