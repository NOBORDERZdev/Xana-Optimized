using UnityEngine;
using System.Linq;
using Crosstales.BWF.Provider;
using Crosstales.BWF.Data;
using Crosstales.BWF.Util;

namespace Crosstales.BWF.Filter
{
   /// <summary>Filter for domains. The class can also replace all domains inside a string.</summary>
   public class DomainFilter : BaseFilter
   {
      #region Variables

      /// <summary>Replace characters for domains.</summary>
      public string ReplaceCharacters;

      private System.Collections.Generic.List<DomainProvider> _domainProvider = new System.Collections.Generic.List<DomainProvider>();

      private readonly System.Collections.Generic.List<DomainProvider> _tempDomainProvider;
      private readonly System.Collections.Generic.Dictionary<string, System.Text.RegularExpressions.Regex> _domainsRegex = new System.Collections.Generic.Dictionary<string, System.Text.RegularExpressions.Regex>();
      private readonly System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Text.RegularExpressions.Regex>> _debugDomainsRegex = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Text.RegularExpressions.Regex>>();
      private bool _ready;
      private bool _readyFirstTime;

      #endregion


      #region Properties

      /// <summary>List of all domain providers.</summary>
      /// <returns>All domain providers.</returns>
      public System.Collections.Generic.List<DomainProvider> DomainProvider
      {
         get => _domainProvider;
         set
         {
            _domainProvider = value;
            if (_domainProvider?.Count > 0)
            {
               foreach (DomainProvider dp in _domainProvider)
               {
                  if (dp != null)
                  {
                     if (Config.DEBUG_DOMAINS)
                     {
                        _debugDomainsRegex.CTAddRange(dp.DebugDomainsRegex);
                     }
                     else
                     {
                        _domainsRegex.CTAddRange(dp.DomainsRegex);
                     }
                  }
                  else
                  {
                     if (!Helper.isEditorMode)
                        Debug.LogError("DomainProvider is null!");
                  }
               }
            }
            else
            {
               _domainProvider = new System.Collections.Generic.List<DomainProvider>();
            }
         }
      }

      /// <summary>Checks the readiness status of the filter.</summary>
      /// <returns>True if the filter is ready.</returns>
      public override bool isReady
      {
         get
         {
            bool result = true;

            if (!_ready)
            {
               if (_tempDomainProvider?.Any(dp => dp != null && !dp.isReady) == true)
                  result = false;

               if (!_readyFirstTime && result)
               {
                  DomainProvider = _tempDomainProvider;

                  if (DomainProvider != null)
                  {
                     foreach (Source src in from dp in DomainProvider where dp != null from src in dp.Sources where src != null where !_sources.ContainsKey(src.SourceName) select src)
                     {
                        _sources.Add(src.SourceName, src);
                     }
                  }

                  _readyFirstTime = true;
               }
            }

            _ready = result;

            return result;
         }
      }

      #endregion


      #region Constructor

      /// <summary>Instantiate the class.</summary>
      /// <param name="domainProvider">List of all domain providers.</param>
      /// <param name="replaceCharacters">Replace characters for domains (default: *, optional).</param>
      /// <param name="disableOrdering">Disables the ordering of the 'GetAll'-method (default: false, optional).</param>
      public DomainFilter(System.Collections.Generic.List<DomainProvider> domainProvider, string replaceCharacters = "*", bool disableOrdering = false /*, string markPrefix, string markPostfix*/) : base(disableOrdering)
      {
         _tempDomainProvider = domainProvider;
         ReplaceCharacters = replaceCharacters;
      }

      #endregion


      #region Implemented methods

      public override bool Contains(string text, params string[] sourceNames)
      {
         bool result = false;

         if (isReady)
         {
            if (string.IsNullOrEmpty(text))
            {
               logContains();
            }
            else
            {
               #region DEBUG

               if (Config.DEBUG_DOMAINS)
               {
                  if (sourceNames == null || sourceNames.Length == 0)
                  {
                     foreach (System.Collections.Generic.List<System.Text.RegularExpressions.Regex> domainRegexes in _debugDomainsRegex.Values)
                     {
                        foreach (System.Text.RegularExpressions.Regex domainRegex in domainRegexes)
                        {
                           System.Text.RegularExpressions.Match match = domainRegex.Match(text);
                           if (match.Success)
                           {
                              Debug.Log($"Test string contains a domain: '{match.Value}' detected by regex '{domainRegex}'");
                              result = true;
                              break;
                           }
                        }
                     }
                  }
                  else
                  {
                     foreach (string domainResource in sourceNames)
                     {
                        if (_debugDomainsRegex.TryGetValue(domainResource, out System.Collections.Generic.List<System.Text.RegularExpressions.Regex> domainRegexes))
                        {
                           foreach (System.Text.RegularExpressions.Regex domainRegex in domainRegexes)
                           {
                              System.Text.RegularExpressions.Match match = domainRegex.Match(text);
                              if (match.Success)
                              {
                                 Debug.Log($"Test string contains a domain: '{match.Value}' detected by regex '{domainRegex}' from source '{domainResource}'");
                                 result = true;
                                 break;
                              }
                           }
                        }
                        else
                        {
                           logResourceNotFound(domainResource);
                        }
                     }
                  }
               }

               #endregion

               else
               {
                  if (sourceNames == null || sourceNames.Length == 0)
                  {
                     if (_domainsRegex.Values.Any(domainRegex => domainRegex.Match(text).Success))
                     {
                        result = true;
                     }
                  }
                  else
                  {
                     foreach (string domainResource in sourceNames)
                     {
                        if (_domainsRegex.TryGetValue(domainResource, out System.Text.RegularExpressions.Regex domainRegex))
                        {
                           System.Text.RegularExpressions.Match match = domainRegex.Match(text);
                           if (match.Success)
                           {
                              result = true;
                              break;
                           }
                        }
                        else
                        {
                           logResourceNotFound(domainResource);
                        }
                     }
                  }
               }
            }
         }
         else
         {
            logFilterNotReady();
         }

         return result;
      }

      public override System.Collections.Generic.List<string> GetAll(string text, params string[] sourceNames)
      {
         _getAllResult.Clear();

         if (isReady)
         {
            if (string.IsNullOrEmpty(text))
            {
               logGetAll();
            }
            else
            {
               #region DEBUG

               if (Config.DEBUG_DOMAINS)
               {
                  if (sourceNames == null || sourceNames.Length == 0)
                  {
                     foreach (System.Collections.Generic.List<System.Text.RegularExpressions.Regex> domainRegexes in _debugDomainsRegex.Values)
                     {
                        foreach (System.Text.RegularExpressions.Regex domainRegex in domainRegexes)
                        {
                           System.Text.RegularExpressions.MatchCollection matches = domainRegex.Matches(text);

                           foreach (System.Text.RegularExpressions.Capture capture in from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures select capture)
                           {
                              Debug.Log($"Test string contains a domain: '{capture.Value}' detected by regex '{domainRegex}'");

                              if (!_getAllResult.Contains(capture.Value))
                                 _getAllResult.Add(capture.Value);
                           }
                        }
                     }
                  }
                  else
                  {
                     foreach (string domainResource in sourceNames)
                     {
                        if (_debugDomainsRegex.TryGetValue(domainResource, out System.Collections.Generic.List<System.Text.RegularExpressions.Regex> domainRegexes))
                        {
                           foreach (System.Text.RegularExpressions.Regex domainRegex in domainRegexes)
                           {
                              System.Text.RegularExpressions.MatchCollection matches = domainRegex.Matches(text);

                              foreach (System.Text.RegularExpressions.Capture capture in from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures select capture)
                              {
                                 Debug.Log($"Test string contains a domain: '{capture.Value}' detected by regex '{domainRegex}'' from source '{domainResource}'");

                                 if (!_getAllResult.Contains(capture.Value))
                                    _getAllResult.Add(capture.Value);
                              }
                           }
                        }
                        else
                        {
                           logResourceNotFound(domainResource);
                        }
                     }
                  }
               }

               #endregion

               else
               {
                  if (sourceNames == null || sourceNames.Length == 0)
                  {
                     foreach (System.Text.RegularExpressions.Capture capture in from domainRegex in _domainsRegex.Values select domainRegex.Matches(text) into matches from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures where !_getAllResult.Contains(capture.Value) select capture)
                     {
                        _getAllResult.Add(capture.Value);
                     }
                  }
                  else
                  {
                     foreach (string domainResource in sourceNames)
                     {
                        if (_domainsRegex.TryGetValue(domainResource, out System.Text.RegularExpressions.Regex domainRegex))
                        {
                           System.Text.RegularExpressions.MatchCollection matches = domainRegex.Matches(text);

                           foreach (System.Text.RegularExpressions.Capture capture in from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures where !_getAllResult.Contains(capture.Value) select capture)
                           {
                              _getAllResult.Add(capture.Value);
                           }
                        }
                        else
                        {
                           logResourceNotFound(domainResource);
                        }
                     }
                  }
               }
            }
         }
         else
         {
            logFilterNotReady();
         }

         return DisableOrdering ? _getAllResult : _getAllResult.Distinct().OrderBy(x => x).ToList();
      }

      public override string ReplaceAll(string text, bool markOnly = false, string prefix = "", string postfix = "", params string[] sourceNames)
      {
         string result = text;

         if (isReady)
         {
            if (string.IsNullOrEmpty(text))
            {
               logReplaceAll();

               result = string.Empty;
            }
            else
            {
               #region DEBUG

               if (Config.DEBUG_DOMAINS)
               {
                  if (sourceNames == null || sourceNames.Length == 0)
                  {
                     foreach (System.Collections.Generic.List<System.Text.RegularExpressions.Regex> domainRegexes in _debugDomainsRegex.Values)
                     {
                        foreach (System.Text.RegularExpressions.Regex domainRegex in domainRegexes)
                        {
                           System.Text.RegularExpressions.MatchCollection matches = domainRegex.Matches(text);

                           foreach (System.Text.RegularExpressions.Capture capture in from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures select capture)
                           {
                              Debug.Log($"Test string contains a domain: '{capture.Value}' detected by regex '{domainRegex}'");

                              result = result.Replace(capture.Value, markOnly ? prefix + capture.Value + postfix : prefix + Helper.CreateString(ReplaceCharacters, capture.Value.Length) + postfix);
                           }
                        }
                     }
                  }
                  else
                  {
                     foreach (string domainResource in sourceNames)
                     {
                        if (_debugDomainsRegex.TryGetValue(domainResource, out System.Collections.Generic.List<System.Text.RegularExpressions.Regex> domainRegexes))
                        {
                           foreach (System.Text.RegularExpressions.Regex domainRegex in domainRegexes)
                           {
                              System.Text.RegularExpressions.MatchCollection matches = domainRegex.Matches(text);

                              foreach (System.Text.RegularExpressions.Capture capture in from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures select capture)
                              {
                                 Debug.Log($"Test string contains a domain: '{capture.Value}' detected by regex '{domainRegex}'' from source '{domainResource}'");

                                 result = result.Replace(capture.Value, markOnly ? prefix + capture.Value + postfix : prefix + Helper.CreateString(ReplaceCharacters, capture.Value.Length) + postfix);
                              }
                           }
                        }
                        else
                        {
                           logResourceNotFound(domainResource);
                        }
                     }
                  }
               }

               #endregion

               else
               {
                  if (sourceNames == null || sourceNames.Length == 0)
                  {
                     result = (from domainRegex in _domainsRegex.Values from System.Text.RegularExpressions.Match match in domainRegex.Matches(text) from System.Text.RegularExpressions.Capture capture in match.Captures select capture).Aggregate(result, (current, capture) => current.Replace(capture.Value, markOnly ? prefix + capture.Value + postfix : prefix + Helper.CreateString(ReplaceCharacters, capture.Value.Length) + postfix));
                  }
                  else
                  {
                     foreach (string domainResource in sourceNames)
                     {
                        if (_domainsRegex.TryGetValue(domainResource, out System.Text.RegularExpressions.Regex domainRegex))
                        {
                           System.Text.RegularExpressions.MatchCollection matches = domainRegex.Matches(text);

                           result = (from System.Text.RegularExpressions.Match match in matches from System.Text.RegularExpressions.Capture capture in match.Captures select capture).Aggregate(result, (current, capture) => current.Replace(capture.Value, markOnly ? prefix + capture.Value + postfix : prefix + Helper.CreateString(ReplaceCharacters, capture.Value.Length) + postfix));
                        }
                        else
                        {
                           logResourceNotFound(domainResource);
                        }
                     }
                  }
               }
            }
         }
         else
         {
            logFilterNotReady();
         }

         return result;
      }

      #endregion
   }
}
// © 2015-2024 crosstales LLC (https://www.crosstales.com)