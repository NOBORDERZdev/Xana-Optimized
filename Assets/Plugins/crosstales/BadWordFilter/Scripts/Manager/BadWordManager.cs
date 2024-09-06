using UnityEngine;
using System.Linq;
using System.Collections;
using Crosstales.BWF.Model.Enum;
using Crosstales.BWF.Provider;
using Crosstales.BWF.Filter;

namespace Crosstales.BWF.Manager
{
   /// <summary>Manager for for bad words.</summary>
   [DisallowMultipleComponent]
   [HelpURL("https://www.crosstales.com/media/data/assets/badwordfilter/api/class_crosstales_1_1_b_w_f_1_1_manager_1_1_bad_word_manager.html")]
   public class BadWordManager : Crosstales.BWF.Manager.BaseManager<BadWordManager, BadWordFilter>
   {
      #region Variables

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("ReplaceChars")] [Header("Specific Settings")] [Tooltip("Replace characters for bad words (default: *)."), SerializeField]
      private string replaceChars = "*"; //e.g. "?#@*&%!$^~+-/<>:;=()[]{}"

      [Tooltip("Replace mode operations on the input string (default: Default)."), SerializeField] private ReplaceMode mode;

      [Tooltip("Remove unnecessary spaces between letters in the input string (default: false)."), SerializeField]
      private bool removeSpaces;

      [Tooltip("Maximal text length for the space detection (default: 3)."), SerializeField] private int maxTextLength = 3;

      /// <summary>Remove unnecessary characters from the input string.</summary>
      public string removeChars;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("SimpleCheck")] [Tooltip("Use simple detection algorithm. This is the way to check for Chinese, Japanese, Korean and Thai bad words (default: false)."), SerializeField]
      private bool simpleCheck;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("BadWordProviderLTR")] [Header("Bad Word Providers")] [Tooltip("List of all left-to-right providers."), SerializeField]
      private System.Collections.Generic.List<BadWordProvider> badWordProviderLTR;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("BadWordProviderRTL")] [Tooltip("List of all right-to-left providers."), SerializeField]
      private System.Collections.Generic.List<BadWordProvider> badWordProviderRTL;


      [Header("Events")] public Crosstales.BWF.OnContainsCompleted OnContainsCompleted;
      public Crosstales.BWF.OnGetAllCompleted OnGetAllCompleted;
      public Crosstales.BWF.OnReplaceAllCompleted OnReplaceAllCompleted;

#if (!UNITY_WSA && !UNITY_WEBGL && !UNITY_XBOXONE) || UNITY_EDITOR
      private System.Threading.Thread _worker;
#endif

      #endregion


      #region Properties

      /// <summary>Replace characters for bad words.</summary>
      public string ReplaceChars
      {
         get => _filter?.ReplaceCharacters ?? replaceChars;
         set => _filter.ReplaceCharacters = replaceChars = value;
      }

      /// <summary>Replace mode operations on the input string.</summary>
      public ReplaceMode Mode
      {
         get => _filter?.Mode ?? mode;
         set => _filter.Mode = mode = value;
      }

      /// <summary>Remove unnecessary spaces between letters in the input string.</summary>
      public bool RemoveSpaces
      {
         get => _filter?.RemoveSpaces ?? removeSpaces;
         set => _filter.RemoveSpaces = removeSpaces = value;
      }

      /// <summary>Maximal text length for the space detection.</summary>
      public int MaxTextLength
      {
         get => _filter?.MaxTextLength ?? maxTextLength;
         set => _filter.MaxTextLength = maxTextLength = value;
      }

      /// <summary>Remove unnecessary characters from the input string.</summary>
      public string RemoveChars
      {
         get => _filter?.RemoveCharacters ?? removeChars;
         set => _filter.RemoveCharacters = removeChars = value;
      }
      
   /// <summary>Use simple detection algorithm. This is the way to check for Chinese, Japanese, Korean and Thai bad words.</summary>
      public bool SimpleCheck
      {
         get => _filter?.SimpleCheck ?? simpleCheck;
         set => _filter.SimpleCheck = simpleCheck = value;
      }

      /// <summary>List of all left-to-right providers.</summary>
      public System.Collections.Generic.List<BadWordProvider> BadWordProviderLTR
      {
         get => badWordProviderLTR;
         set => badWordProviderLTR = value;
      }

      /// <summary>List of all right-to-left providers.</summary>
      public System.Collections.Generic.List<BadWordProvider> BadWordProviderRTL
      {
         get => badWordProviderRTL;
         set => badWordProviderRTL = value;
      }

      /// <summary>Returns all sources for the manager.</summary>
      /// <returns>List with all sources for the manager</returns>
      public System.Collections.Generic.List<Crosstales.BWF.Data.Source> Sources => _filter?.Sources;

      /// <summary>Total number of Regex.</summary>
      /// <returns>Total number of Regex.</returns>
      public int TotalRegexCount => Sources.Sum(src => src.RegexCount);

      protected override Crosstales.BWF.OnContainsCompleted onContainsCompleted => OnContainsCompleted;
      protected override Crosstales.BWF.OnGetAllCompleted onGetAllCompleted => OnGetAllCompleted;
      protected override Crosstales.BWF.OnReplaceAllCompleted onReplaceAllCompleted => OnReplaceAllCompleted;

      #endregion


      #region MonoBehaviour methods

      protected override void Awake()
      {
         base.Awake();

         if (Instance == this)
            Load();
      }

      private void OnValidate()
      {
         if (replaceChars != ReplaceChars)
            ReplaceChars = replaceChars;

         if (mode != Mode)
            Mode = mode;

         if (removeSpaces != RemoveSpaces)
            RemoveSpaces = removeSpaces;

         if (removeChars != RemoveChars)
            RemoveChars = removeChars;

         if (simpleCheck != SimpleCheck)
            SimpleCheck = simpleCheck;
      }

      protected override void OnApplicationQuit()
      {
#if (!UNITY_WSA && !UNITY_WEBGL && !UNITY_XBOXONE) || UNITY_EDITOR
         _worker.CTAbort();
#endif

         base.OnApplicationQuit();
      }

      #endregion


      #region Public methods

      /// <summary>Resets this object.</summary>
      public static void ResetObject()
      {
         DeleteInstance();
      }

      /// <summary>Loads the current filter with all settings from this object.</summary>
      public void Load()
      {
         //Debug.Log("FILTER LOADED");
         _filter = new BadWordFilter(BadWordProviderLTR, BadWordProviderRTL, ReplaceChars, Mode, SimpleCheck, RemoveSpaces, DisableOrdering, RemoveChars);
      }

      /// <summary>Searches for bad words in a text.</summary>
      /// <param name="text">Text to check</param>
      /// <param name="sourceNames">Relevant sources (e.g. "english", optional)</param>
      /// <returns>True if a match was found</returns>
      public bool Contains(string text, params string[] sourceNames)
      {
         bool result = false;

         if (!string.IsNullOrEmpty(text) && _filter != null)
            result = _filter.Contains(text, sourceNames);

         return result;
      }

      /// <summary>Searches asynchronously for bad words in a text. Use the "OnContainsComplete"-callback to get the result.</summary>
      /// <param name="text">Text to check</param>
      /// <param name="sourceNames">Relevant sources (e.g. "english", optional)</param>
      public void ContainsAsync(string text, params string[] sourceNames)
      {
         StartCoroutine(containsAsync(text, sourceNames));
      }

      /// <summary>Searches for bad words in a text.</summary>
      /// <param name="text">Text to check</param>
      /// <param name="sourceNames">Relevant sources (e.g. "english", optional)</param>
      /// <returns>List with all the matches</returns>
      public System.Collections.Generic.List<string> GetAll(string text, params string[] sourceNames)
      {
         System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>();

         if (!string.IsNullOrEmpty(text))
            result = _filter?.GetAll(text, sourceNames);

         return result;
      }

      /// <summary>Searches asynchronously for bad words in a text. Use the "OnGetAllComplete"-callback to get the result.</summary>
      /// <param name="text">Text to check</param>
      /// <param name="sourceNames">Relevant sources (e.g. "english", optional)</param>
      public void GetAllAsync(string text, params string[] sourceNames)
      {
         StartCoroutine(getAllAsync(text, sourceNames));
      }

      /// <summary>Searches and replaces all bad words in a text.</summary>
      /// <param name="text">Text to check</param>
      /// <param name="markOnly">Only mark the words (default: false, optional)</param>
      /// <param name="prefix">Prefix for every found bad word (optional)</param>
      /// <param name="postfix">Postfix for every found bad word (optional)</param>
      /// <param name="sourceNames">Relevant sources (e.g. "english", optional)</param>
      /// <returns>Clean text</returns>
      public string ReplaceAll(string text, bool markOnly = false, string prefix = "", string postfix = "", params string[] sourceNames)
      {
         string result = text;

         if (!string.IsNullOrEmpty(text))
            result = _filter?.ReplaceAll(text, markOnly, prefix, postfix, sourceNames);

         return result;
      }

      /// <summary>Searches and replaces asynchronously all bad words in a text. Use the "OnReplaceAllComplete"-callback to get the result.</summary>
      /// <param name="text">Text to check</param>
      /// <param name="markOnly">Only mark the words (default: false, optional)</param>
      /// <param name="prefix">Prefix for every found bad word (optional)</param>
      /// <param name="postfix">Postfix for every found bad word (optional)</param>
      /// <param name="sourceNames">Relevant sources (e.g. "english", optional)</param>
      public void ReplaceAllAsync(string text, bool markOnly = false, string prefix = "", string postfix = "", params string[] sourceNames)
      {
         StartCoroutine(replaceAllAsync(text, markOnly, prefix, postfix, sourceNames));
      }

      /// <summary>Marks the text with a prefix and postfix.</summary>
      /// <param name="text">Text containing bad words</param>
      /// <param name="replace">Replace the bad words (default: false, optional)</param>
      /// <param name="prefix">Prefix for every found bad word (default: bold and red, optional)</param>
      /// <param name="postfix">Postfix for every found bad word (default: bold and red, optional)</param>
      /// <param name="sourceNames">Relevant sources (e.g. "english", optional)</param>
      /// <returns>Text with marked domains</returns>
      public string Mark(string text, bool replace = false, string prefix = "<b><color=red>", string postfix = "</color></b>", params string[] sourceNames)
      {
         string result = text;

         if (!string.IsNullOrEmpty(text))
            result = _filter?.Mark(text, replace, prefix, postfix, sourceNames);

         return result;
      }

      #endregion


      #region Private methods

      private IEnumerator containsAsync(string text, params string[] sourceNames)
      {
#if (!UNITY_WSA && !UNITY_WEBGL && !UNITY_XBOXONE) || UNITY_EDITOR
         if (_worker?.IsAlive != true)
         {
            bool result = true;

            _worker = new System.Threading.Thread(() => result = Contains(text, sourceNames));
            _worker.Start();

            do
            {
               yield return null;
            } while (_worker.IsAlive);

            onContainsComplete(text, result);
         }
#else
         Debug.LogWarning("'ContainsAsync' is not supported under the current platform!", this);
         yield return null;
#endif
      }

      private IEnumerator getAllAsync(string text, params string[] sourceNames)
      {
#if (!UNITY_WSA && !UNITY_WEBGL && !UNITY_XBOXONE) || UNITY_EDITOR
         if (_worker?.IsAlive != true)
         {
            System.Collections.Generic.List<string> result = null;

            _worker = new System.Threading.Thread(() => result = GetAll(text, sourceNames));
            _worker.Start();

            do
            {
               yield return null;
            } while (_worker.IsAlive);

            onGetAllComplete(text, result);
         }
#else
         Debug.LogWarning("'GetAllAsync' is not supported under the current platform!", this);
         yield return null;
#endif
      }

      private IEnumerator replaceAllAsync(string text, bool markOnly = false, string prefix = "", string postfix = "", params string[] sourceNames)
      {
#if (!UNITY_WSA && !UNITY_WEBGL && !UNITY_XBOXONE) || UNITY_EDITOR
         if (_worker?.IsAlive != true)
         {
            string result = null;

            _worker = new System.Threading.Thread(() => result = ReplaceAll(text, markOnly, prefix, postfix, sourceNames));
            _worker.Start();

            do
            {
               yield return null;
            } while (_worker.IsAlive);

            onReplaceAllComplete(text, result);
         }
#else
         Debug.LogWarning("'ReplaceAllAsync' is not supported under the current platform!", this);
         yield return null;
#endif
      }

      #endregion
   }
}
// © 2015-2024 crosstales LLC (https://www.crosstales.com)