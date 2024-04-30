using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;

namespace Crosstales.BWF.Provider
{
   /// <summary>Text-file based domain provider.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/badwordfilter/api/class_crosstales_1_1_b_w_f_1_1_provider_1_1_domain_provider_text.html")]
   public class DomainProviderText : DomainProvider
   {
      #region Variables

      private bool _webSuccess;

      #endregion


      #region Implemented methods

      public override void Load()
      {
         base.Load();

         if (Sources != null)
         {
            _loading = true;

            if (Crosstales.BWF.Util.Helper.isEditorMode)
            {
#if UNITY_EDITOR
               foreach (Crosstales.BWF.Data.Source source in Sources.Where(source => source != null))
               {
                  _webSuccess = false;

                  if (!string.IsNullOrEmpty(source.URL))
                     loadWebInEditor(source);

                  if (source.Resource != null && (!source.IsResourceFallback || (source.IsResourceFallback && !_webSuccess)))
                     loadResourceInEditor(source);
               }

               init();
#endif
            }
            else
            {
               foreach (Crosstales.BWF.Data.Source source in Sources.Where(source => source != null))
               {
                  _webSuccess = false;

                  if (!string.IsNullOrEmpty(source.URL))
                     StartCoroutine(loadWeb(source));

                  if (source.Resource != null && (!source.IsResourceFallback || (source.IsResourceFallback && !_webSuccess)))
                     StartCoroutine(loadResource(source));
               }
            }
         }
      }

      public override void Save()
      {
         Debug.LogWarning("Save not implemented!", this);
      }

      #endregion


      #region Private methods

      private IEnumerator loadWeb(Crosstales.BWF.Data.Source src)
      {
         string uid = System.Guid.NewGuid().ToString();
         coRoutines.Add(uid);

         if (Crosstales.Common.Util.NetworkHelper.isURL(src.URL))
         {
            using (UnityWebRequest www = UnityWebRequest.Get(src.URL.Trim()))
            {
               www.timeout = Crosstales.BWF.Util.Constants.WWW_TIMEOUT;
               www.downloadHandler = new DownloadHandlerBuffer();
               yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
               if (www.result != UnityWebRequest.Result.ProtocolError && www.result != UnityWebRequest.Result.ConnectionError)
#else
               if (!www.isHttpError && !www.isNetworkError)
#endif
               {
                  System.Collections.Generic.List<string> list = Crosstales.BWF.Util.Helper.SplitStringToLines(www.downloadHandler.text);

                  yield return null;

                  if (Crosstales.BWF.Util.Config.DEBUG)
                     Debug.Log($"{src}: {list.Count}", this);

                  src.Regexes = list.ToArray();

                  if (list.Count > 0)
                  {
                     _domains.Add(new Model.Domains(src, list));
                     _webSuccess = true;
                  }
                  else
                  {
                     Debug.LogWarning($"Source '{src.URL}' does not contain any active domains!", this);
                  }
               }
               else
               {
                  Debug.LogWarning($"Could not load source '{src.URL}'{System.Environment.NewLine}{www.error}{System.Environment.NewLine}Did you set the correct 'URL'?", this);
               }
            }
         }
         else
         {
            Debug.LogWarning($"'URL' is is invalid, null or empty!{System.Environment.NewLine}Please add a valid URL.", this);
         }

         coRoutines.Remove(uid);

         if (_loading && coRoutines.Count == 0)
         {
            _loading = false;
            init();
         }
      }

      private IEnumerator loadResource(Crosstales.BWF.Data.Source src)
      {
         string uid = System.Guid.NewGuid().ToString();
         coRoutines.Add(uid);

         if (src.Resource != null)
         {
            System.Collections.Generic.List<string> list = Crosstales.BWF.Util.Helper.SplitStringToLines(src.Resource.text);

            yield return null;

            if (Crosstales.BWF.Util.Config.DEBUG)
               Debug.Log($"{src}: {list.Count}", this);

            src.Regexes = list.ToArray();

            if (list.Count > 0)
            {
               _domains.Add(new Model.Domains(src, list));
            }
            else
            {
               Debug.LogWarning($"Resource '{src.Resource}' does not contain any active bad words!", this);
            }
         }
         else
         {
            Debug.LogWarning($"Resource field 'Source' is null or empty!{System.Environment.NewLine}Please add a valid resource.", this);
         }

         coRoutines.Remove(uid);

         if (_loading && coRoutines.Count == 0)
         {
            _loading = false;
            init();
         }
      }

      #endregion


      #region Editor-only methods

#if UNITY_EDITOR

      private void loadWebInEditor(Crosstales.BWF.Data.Source src)
      {
         if (Crosstales.Common.Util.NetworkHelper.isURL(src.URL))
         {
            try
            {
               System.Net.ServicePointManager.ServerCertificateValidationCallback = Crosstales.Common.Util.NetworkHelper.RemoteCertificateValidationCallback;

               using (System.Net.WebClient client = new Crosstales.Common.Util.CTWebClient())
               {
                  client.Encoding = System.Text.Encoding.UTF8;
                  string content = client.DownloadString(src.URL.Trim());

                  System.Collections.Generic.List<string> list = Util.Helper.SplitStringToLines(content);

                  if (Crosstales.BWF.Util.Config.DEBUG)
                     Debug.Log($"{src}: {list.Count}", this);

                  src.Regexes = list.ToArray();

                  if (list.Count > 0)
                  {
                     _domains.Add(new Model.Domains(src, list));
                     _webSuccess = true;
                  }
                  else
                  {
                     Debug.LogWarning($"Source '{src.URL}' does not contain any active domains!", this);
                  }
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogWarning($"Could not load source '{src.URL}'{System.Environment.NewLine}{ex}{System.Environment.NewLine}Did you set the correct 'URL'?", this);
            }
         }
         else
         {
            Debug.LogWarning($"'URL' is null or empty!{System.Environment.NewLine}Please add a valid URL.", this);
         }
      }

      private void loadResourceInEditor(Crosstales.BWF.Data.Source src)
      {
         if (src.Resource != null)
         {
            System.Collections.Generic.List<string> list = Crosstales.BWF.Util.Helper.SplitStringToLines(src.Resource.text);

            if (Crosstales.BWF.Util.Config.DEBUG)
               Debug.Log($"{src}: {list.Count}", this);

            src.Regexes = list.ToArray();

            if (list.Count > 0)
            {
               _domains.Add(new Crosstales.BWF.Model.Domains(src, list));
            }
            else
            {
               Debug.LogWarning($"Resource '{src.Resource}' does not contain any active bad words!", this);
            }
         }
         else
         {
            Debug.LogWarning($"Resource field 'Source' is null or empty!{System.Environment.NewLine}Please add a valid resource.", this);
         }
      }

#endif

      #endregion
   }
}
// © 2016-2024 crosstales LLC (https://www.crosstales.com)