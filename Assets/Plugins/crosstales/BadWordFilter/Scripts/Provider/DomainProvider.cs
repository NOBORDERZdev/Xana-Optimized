using System.Linq;
using UnityEngine;

namespace Crosstales.BWF.Provider
{
   /// <summary>Base class for domain providers.</summary>
   public abstract class DomainProvider : BaseProvider
   {
      #region Variables

      protected readonly System.Collections.Generic.List<Crosstales.BWF.Model.Domains> _domains = new System.Collections.Generic.List<Crosstales.BWF.Model.Domains>();

      private System.Collections.Generic.Dictionary<string, System.Text.RegularExpressions.Regex> _domainsRegex = new System.Collections.Generic.Dictionary<string, System.Text.RegularExpressions.Regex>();
      private System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Text.RegularExpressions.Regex>> _debugDomainsRegex = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Text.RegularExpressions.Regex>>();

      private const string DOMAIN_REGEGX_START = @"\b{0,1}((ht|f)tp(s?)\:\/\/)?[\w\-\.\@]*[\.]";
      //private const string domainRegexEnd = @"(:\d{1,5})?(\/|\b)([\a-zA-Z0-9\-\.\?\!\,\=\'\/\&\%#_]*)?\b";
      private const string DOMAIN_REGEGX_END = @"(:\d{1,5})?(\/|\b)";

      #endregion


      #region Properties

      /// <summary>RegEx for domains.</summary>
      public System.Collections.Generic.Dictionary<string, System.Text.RegularExpressions.Regex> DomainsRegex
      {
         get => _domainsRegex;
         protected set => _domainsRegex = value;
      }

      /// <summary>Debug-version of "RegEx for domains".</summary>
      public System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Text.RegularExpressions.Regex>> DebugDomainsRegex
      {
         get => _debugDomainsRegex;
         protected set => _debugDomainsRegex = value;
      }

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         //do nothing, just allow to enable/disable the script
      }

      #endregion


      #region Implemented methods

      public override void Load()
      {
         if (ClearOnLoad)
            _domains.Clear();
      }

      protected override void init()
      {
         DomainsRegex.Clear();

         if (Crosstales.BWF.Util.Config.DEBUG_DOMAINS)
            Debug.Log("++ DomainProvider started in debug-mode ++", this);

         foreach (Crosstales.BWF.Model.Domains domain in _domains)
         {
            if (Crosstales.BWF.Util.Config.DEBUG_DOMAINS)
            {
               try
               {
                  System.Collections.Generic.List<System.Text.RegularExpressions.Regex> domainRegexes = new System.Collections.Generic.List<System.Text.RegularExpressions.Regex>(domain.DomainList.Count);
                  domainRegexes.AddRange(domain.DomainList.Select(line => new System.Text.RegularExpressions.Regex(DOMAIN_REGEGX_START + line + DOMAIN_REGEGX_END, RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5)));

                  if (!DebugDomainsRegex.ContainsKey(domain.Source.SourceName))
                     DebugDomainsRegex.Add(domain.Source.SourceName, domainRegexes);
               }
               catch (System.Exception ex)
               {
                  Debug.LogError($"Could not generate debug regex for source '{domain.Source.SourceName}': {ex}", this);

                  if (Crosstales.BWF.Util.Constants.DEV_DEBUG)
                     Debug.Log(domain.DomainList.CTDump(), this);
               }
            }
            else
            {
               try
               {
                  if (!DomainsRegex.ContainsKey(domain.Source.SourceName))
                     DomainsRegex.Add(domain.Source.SourceName, new System.Text.RegularExpressions.Regex($"{DOMAIN_REGEGX_START}({string.Join("|", domain.DomainList.ToArray())}){DOMAIN_REGEGX_END}", RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5));
               }
               catch (System.Exception ex)
               {
                  Debug.LogError($"Could not generate exact regex for source '{domain.Source.SourceName}': {ex}", this);

                  if (Crosstales.BWF.Util.Constants.DEV_DEBUG)
                     Debug.Log(domain.DomainList.CTDump(), this);
               }
            }

            if (Crosstales.BWF.Util.Config.DEBUG_DOMAINS)
               Debug.Log($"Domain resource '{domain.Source}' loaded and {domain.DomainList.Count} entries found.", this);
         }

         isReady = true;
         //raiseOnProviderReady();
      }

      #endregion
   }
}
// © 2015-2024 crosstales LLC (https://www.crosstales.com)