using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChinaTextLocalization : MonoBehaviour
{
    public Text LocalizeText;
    public TextMeshProUGUI LocalizeTextTMP;

    private Coroutine _myCoroutine;

    string currentText = "";

    private void OnEnable()
    {
        if (LocalizeText)
        {
            currentText = LocalizeText.text;
        }

        else if (LocalizeTextTMP)
        {
            currentText = LocalizeTextTMP.text;
        }

        // GameManager.chinaCurrentLanguage = "zh";

        LocalizeTextText();
    }

    //private void Start()
    //{
    //    Debug.Log(LocalizeText.text + " start");

    //    if (LocalizeText)
    //    {
    //        currentText = LocalizeText.text;
    //    }

    //    else if (LocalizeTextTMP)
    //    {
    //        currentText = LocalizeTextTMP.text;
    //    }

    //    LocalizeTextText();
    //}

    public void LocalizeTextText()
    {
        if (_myCoroutine != null)
        {
            StopCoroutine(_myCoroutine);
        }

        if (gameObject.activeInHierarchy)
        {
            _myCoroutine = StartCoroutine(StartTranslation());
        }
    }

    private IEnumerator StartTranslation()
    {
        //while (!ChinaCustomLocalization.IsReady)
        //{
        //    yield return null;
        //}

        //if (LocalizeText != null)
        //{
        //    StaticLocalizeTextText();

        //}
        //else if (LocalizeTextTMP != null)
        //{
        //    StaticLocalizeTextPro();
        //}
        yield return null;
    }

    private void StaticLocalizeTextPro()
    {

        //if (ChinaCustomLocalization.localisationDict == null || ChinaCustomLocalization.localisationDict.Count <= 0) return;

        //#region Old Method

        //// foreach (RecordsLanguage rl in ChinaCustomLocalization.LocalisationSheet)
        //// {
        ////     if (rl.Keys == LocalizeTextTMP.text && !LocalizeTextTMP.text.IsNullOrEmpty())
        ////     {
        ////         if (Application.systemLanguage == SystemLanguage.Japanese &&
        ////             !string.IsNullOrEmpty(rl.Japanese))
        ////             LocalizeTextTMP.text = rl.Japanese;
        ////         else if (Application.systemLanguage == SystemLanguage.English &&
        ////                  !string.IsNullOrEmpty(rl.English))
        ////             LocalizeTextTMP.text = rl.English;
        ////         _hasTranslated = true;
        ////         break;
        ////     }
        //// }

        //#endregion

        //if (ChinaCustomLocalization.localisationDict.TryGetValue(currentText, out ChinaRecordsLanguage find))
        //{
        //    if (!ChinaCustomLocalization.forceChinese)
        //    {
        //        switch (GameManager.chinaCurrentLanguage)
        //        {
        //            //case "en":
        //            //    LocalizeTextTMP.text = find.English;
        //            //    break;
        //            case "zh":
        //                LocalizeTextTMP.text = find.Chinese;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        LocalizeTextTMP.text = find.Chinese;
        //    }
        //}
    }

    private void StaticLocalizeTextText()
    {
        //if (ChinaCustomLocalization.localisationDict == null || ChinaCustomLocalization.localisationDict.Count <= 0)
        //    return;

       
        //if (ChinaCustomLocalization.localisationDict.TryGetValue(currentText, out ChinaRecordsLanguage find))
        //{
        //    if (!ChinaCustomLocalization.forceChinese)
        //    {
        //        switch (GameManager.chinaCurrentLanguage)
        //        {
        //            //case "en":
        //            //    LocalizeText.text = find.English;
        //            //    break;
        //            case "zh":
        //                LocalizeText.text = find.Chinese;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        LocalizeText.text = find.Chinese;
        //    }
        //}

    }

    /// <summary>
    /// Sets Unity's text component as text found in localisation sheet
    /// </summary>
    /// <param name="key">Key in localisation sheet</param>
    public void LocalizeTextText(string key) //Utility for external use
    {

        if (LocalizeText != null)
        {
            currentText = key;
            LocalizeText.text = key;
        }
        else if (LocalizeTextTMP != null)
        {
            currentText = key;
            LocalizeTextTMP.text = key;
        }
        LocalizeTextText();
    }

    /// <summary>
    /// Returns translation as string found by key (This function does not check if sheet is filled
    /// use LocalizeText() instead
    /// </summary>
    /// <param name="key">Key to find translation against</param>
    /// <returns>Found Key as string or key itself if not found in sheet</returns>
    public static string GetLocaliseTextByKey(string key)
    {
        //    if (ChinaCustomLocalization.localisationDict == null || ChinaCustomLocalization.localisationDict.Count <= 0) return key;
        //    if (ChinaCustomLocalization.localisationDict.TryGetValue(key, out ChinaRecordsLanguage find))

        //    {
        //        if (!ChinaCustomLocalization.forceChinese)
        //        {
        //            switch (GameManager.chinaCurrentLanguage)
        //            {
        //                //case "en":
        //                //    return find.English;
        //                case "zh":
        //                    return find.Chinese;
        //            }
        //        }
        //        else
        //        {
        //            return find.Chinese;
        //        }

        //    }
        //    Debug.LogWarning("Key not found. Please add it to sheet:" + key);
        //    return key; //Normally return key as it is if not found
        return "";
    }

}
