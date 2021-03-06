using System;
using System.Collections.Generic;
using System.Linq;
using VariableKeywordMatcher.Interface;
using VariableKeywordMatcher.Model;

namespace VariableKeywordMatcher
{
    /// <summary>
    /// 
    /// </summary>
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
        public MatchCache CreateStringCache(string originalString)
        {
            CheckProviders();
            var sd = new MatchCache(originalString);
            foreach (var provider in _providers)
            {
                provider.AppendDescriptions(ref sd);
            }
            return sd;
        }

        /// <summary>
        /// using keywords to match the cache of the original string
        /// </summary>
        /// <param name="matchCache">cache of the original string</param>
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
        /// using keywords to match the cache of the original string
        /// </summary>
        /// <param name="matchCache">cache of the original string</param>
        /// <param name="keyword">single keyword</param>
        /// <returns></returns>
        public MatchResult Match(MatchCache matchCache, string keyword)
        {
            return Match(matchCache, new[] { keyword });
        }

        /// <summary>
        /// using keywords to match the string
        /// </summary>
        /// <param name="originalString">original string</param>
        /// <param name="keywords">list of keywords</param>
        /// <returns></returns>
        public MatchResult Match(string originalString, IEnumerable<string> keywords)
        {
            var mc = CreateStringCache(originalString);
            return Match(mc, keywords);
        }

        /// <summary>
        /// using keyword to match the string
        /// </summary>
        /// <param name="originalString">original string</param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public MatchResult Match(string originalString, string keyword)
        {
            var mc = CreateStringCache(originalString);
            return Match(mc, new[] { keyword });
        }

        /// <summary>
        /// using keyword to match the string
        /// </summary>
        /// <param name="matchCaches">caches of the original strings</param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public MatchResults Matchs(List<MatchCache> matchCaches, IEnumerable<string> keywords)
        {
            var mrs = new List<MatchResult>(matchCaches.Count);
            foreach (var matchCache in matchCaches)
            {
                var mr = Match(matchCache, keywords);
                mrs.Add(mr);
            }

            var ret = new MatchResults(matchCaches.Select(x => x.OriginalString).ToList(), keywords,
                this.IsCaseSensitive, mrs);

            return ret;
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
