using System.Linq;
using UnityEngine;

namespace Crosstales.BWF.Provider
{
   /// <summary>Base class for bad word providers.</summary>
   public abstract class BadWordProvider : BaseProvider
   {
      #region Variables

      protected readonly System.Collections.Generic.List<Crosstales.BWF.Model.BadWords> _badwords = new System.Collections.Generic.List<Crosstales.BWF.Model.BadWords>();

      private System.Collections.Generic.Dictionary<string, System.Text.RegularExpressions.Regex> _exactBadwordsRegex = new System.Collections.Generic.Dictionary<string, System.Text.RegularExpressions.Regex>();
      private System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Text.RegularExpressions.Regex>> _debugExactBadwordsRegex = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Text.RegularExpressions.Regex>>();
      private System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> _simpleBadwords = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>();

      private const string EXACT_REGEX_START = @"(?<![\w\d])";
      private const string EXACT_REGEX_END = @"s?(?![\w\d])";

      #endregion


      #region Properties

      /// <summary>Exact RegEx for bad words.</summary>
      public System.Collections.Generic.Dictionary<string, System.Text.RegularExpressions.Regex> ExactBadwordsRegex
      {
         get => _exactBadwordsRegex;
         protected set => _exactBadwordsRegex = value;
      }

      /// <summary>Debug-version of "Exact RegEx for bad words".</summary>
      public System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Text.RegularExpressions.Regex>> DebugExactBadwordsRegex
      {
         get => _debugExactBadwordsRegex;
         protected set => _debugExactBadwordsRegex = value;
      }

      /// <summary>Simplified version of "RegEx for bad words".</summary>
      public System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> SimpleBadwords
      {
         get => _simpleBadwords;
         protected set => _simpleBadwords = value;
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
            _badwords.Clear();
      }

      protected override void init()
      {
         ExactBadwordsRegex.Clear();
         DebugExactBadwordsRegex.Clear();
         SimpleBadwords.Clear();

         if (Crosstales.BWF.Util.Config.DEBUG_BADWORDS)
            Debug.Log("++ BadWordProvider started in debug-mode ++", this);

         foreach (Crosstales.BWF.Model.BadWords badWord in _badwords)
         {
            if (Crosstales.BWF.Util.Config.DEBUG_BADWORDS)
            {
               try
               {
                  System.Collections.Generic.List<System.Text.RegularExpressions.Regex> exactRegexes = new System.Collections.Generic.List<System.Text.RegularExpressions.Regex>(badWord.BadWordList.Count);
                  exactRegexes.AddRange(badWord.BadWordList.Select(line => new System.Text.RegularExpressions.Regex(EXACT_REGEX_START + line + EXACT_REGEX_END, RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5)));

                  if (!DebugExactBadwordsRegex.ContainsKey(badWord.Source.SourceName))
                     DebugExactBadwordsRegex.Add(badWord.Source.SourceName, exactRegexes);
               }
               catch (System.Exception ex)
               {
                  Debug.LogError($"Could not generate debug regex for source '{badWord.Source.SourceName}': {ex}", this);

                  if (Crosstales.BWF.Util.Constants.DEV_DEBUG)
                     Debug.Log(badWord.BadWordList.CTDump(), this);
               }
            }
            else
            {
               try
               {
                  if (!ExactBadwordsRegex.ContainsKey(badWord.Source.SourceName))
                  {
                     ExactBadwordsRegex.Add(badWord.Source.SourceName, new System.Text.RegularExpressions.Regex($"{EXACT_REGEX_START}({string.Join("|", badWord.BadWordList.ToArray())}){EXACT_REGEX_END}", RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5));
                  }
               }
               catch (System.Exception ex)
               {
                  Debug.LogError($"Could not generate exact regex for source '{badWord.Source.SourceName}': {ex}", this);

                  if (Crosstales.BWF.Util.Constants.DEV_DEBUG)
                     Debug.Log(badWord.BadWordList.CTDump(), this);
               }
            }

            System.Collections.Generic.List<string> simpleWords = new System.Collections.Generic.List<string>(badWord.BadWordList.Count);

            simpleWords.AddRange(badWord.BadWordList);

            if (!SimpleBadwords.ContainsKey(badWord.Source.SourceName))
               SimpleBadwords.Add(badWord.Source.SourceName, simpleWords);

            if (Crosstales.BWF.Util.Config.DEBUG_BADWORDS)
               Debug.Log($"Bad word resource '{badWord.Source.SourceName}' loaded and {badWord.BadWordList.Count} entries found.", this);
         }

         isReady = true;
         //raiseOnProviderReady();
      }

      #endregion
   }
}
// © 2015-2024 crosstales LLC (https://www.crosstales.com)