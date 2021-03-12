using System;
using System.Collections.Generic;
using System.Linq;
using VariableKeywordMatcher.Interface;
using VariableKeywordMatcher.Model;

namespace VariableKeywordMatcher
{
    public class Matcher
    {
        private readonly List<MatchProviderBase> _providers;

        /// <summary>
        /// 
        /// </summary>
        public readonly bool IsCaseSensitive;

        /// <summary>
        /// name of available providers
        /// </summary>
        public List<string> ProviderTypes => _providers?.Select(x => x.GetProviderName()).ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providers"></param>
        /// <param name="isCaseSensitive"></param>
        protected internal Matcher(IEnumerable<MatchProviderBase> providers, bool isCaseSensitive)
        {
            IsCaseSensitive = isCaseSensitive;
            _providers = new List<MatchProviderBase>(providers);
        }

        /// <summary>
        /// create a matching spell cache for the string
        /// </summary>
        /// <returns></returns>
        public MatchCache CreateStringCache(string str)
        {
            CheckProviders();
            var sd = new MatchCache(str);
            foreach (var provider in _providers)
            {
                provider.AppendDescriptions(ref sd);
            }
            return sd;
        }

        /// <summary>
        /// using keywords to match the cache of the original sting
        /// </summary>
        /// <param name="matchCache">cache of the original sting</param>
        /// <param name="keywords">list of keywords</param>
        /// <returns></returns>
        public MatchResult Match(MatchCache matchCache, IEnumerable<string> keywords)
        {
            CheckProviders();
            var result = MatchResult.CreateUnmatchedResult(matchCache.OriginalString, keywords, IsCaseSensitive);
            foreach (var r in _providers.Select(provider => provider.DoMatches(matchCache, keywords)))
            {
                result.Merge(r);
            }
            return result;
        }

        /// <summary>
        /// using keywords to match the cache of the original sting
        /// </summary>
        /// <param name="matchCache">cache of the original sting</param>
        /// <param name="keyword">single keyword</param>
        /// <returns></returns>
        public MatchResult Match(MatchCache matchCache, string keyword)
        {
            return Match(matchCache, new[] { keyword });
        }

        /// <summary>
        /// using keywords to match the string
        /// </summary>
        /// <param name="str">sting</param>
        /// <param name="keywords">list of keywords</param>
        /// <returns></returns>
        public MatchResult Match(string str, IEnumerable<string> keywords)
        {
            var mc = CreateStringCache(str);
            return Match(mc, keywords);
        }

        /// <summary>
        /// using keyword to match the string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public MatchResult Match(string str, string keyword)
        {
            var mc = CreateStringCache(str);
            return Match(mc, new[] { keyword });
        }

        private void CheckProviders()
        {
            if (!(_providers?.Count > 0))
            {
                throw new Exception("Keyword match provider is not been initialed! Please create the Shooter from the ShootingString.Builder");
            }
        }
    }
}
