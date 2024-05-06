using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using WebSocketSharp;

namespace Sign_Up_Scripts
{
    public class ErrorHandler : MonoBehaviour
    {
        private TextAsset _csvAsset;
        private List<ErrorMessage> _errorMessageList;

        public TextMeshProUGUI ActiveErrorText;
        public bool CapitalizeMessage;
        [SerializeField]
        private ErrorMessage _defaultMessage;

        [SerializeField] private string SheetLink;
        private bool isError;

        private void OnEnable()
        {
            // _csvAsset = Resources.Load<TextAsset>(ErrorSheetPath);
            // if (_csvAsset)
            // {
            LoadSheet();
            // }
            // else
            // {
            //     Debug.LogWarning("CSV Sheet could not be found");
            // }
        }

        private IEnumerator GetErrorSheet()
        {
            UnityWebRequest www = UnityWebRequest.Get(SheetLink);

            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            //Debug.LogError(www.downloadHandler.text);
            if (www.isHttpError || www.isNetworkError)
            {
                Debug.Log("Network Error");
                isError = true;
            }

            if (!isError)
            {
                string csv = www.downloadHandler.text;
                if (!csv.IsNullOrEmpty())
                {
                    _errorMessageList = CSVSerializer.Deserialize<ErrorMessage>(csv).ToList();
                }
            }
            else
            {
                StopAllCoroutines();
                LoadSheet(); //Load sheet again in case of error
                isError = false;
            }

        }

        private void LoadSheet()
        {
            // if (_csvAsset)
            // {
            //     string csv = _csvAsset.text;
            //     _errorMessageList = CSVSerializer.Deserialize<ErrorMessage>(csv).ToList();
            // }
            StartCoroutine(GetErrorSheet());
        }

        public void ShowErrorMessage(string errorCode, TextMeshProUGUI textToBeSet)
        {
            ActiveErrorText = textToBeSet;
            ErrorMessage errorMessage = default;
            if (_errorMessageList != null && _errorMessageList.Count > 0)
            {
                //print((int)errorCode);
                errorMessage = _errorMessageList.Find(x => x.ErrorCode == errorCode);
                if (string.IsNullOrEmpty(errorMessage.EnglishMessage) && string.IsNullOrEmpty(errorMessage.JapaneseMessage))
                {
                    errorMessage.EnglishMessage = errorCode;
                    errorMessage.JapaneseMessage = errorCode;
                }
            }
            else
            {
                errorMessage.EnglishMessage = errorCode;
                errorMessage.JapaneseMessage = errorCode;
            }
            SetActiveText(errorMessage);
        }

        //public void ShowErrorMessage(string errorCode, Text textToBeSet)
        //{
        //    ActiveErrorText = textToBeSet;
        //    //ErrorMessage errorMessage = default;
        //    //if (_errorMessageList != null && _errorMessageList.Count > 0)
        //    //{
        //    //    print((int)errorCode);
        //    //    errorMessage = _errorMessageList.Find(x => x.ErrorCode == (int)errorCode);
        //    //}
        //    //else
        //    //{

        //    //}
        //    //SetActiveText(errorMessage);

        //    if (LocalizationManager.localisationDict == null || LocalizationManager.localisationDict.Count <= 0) return;

        //    if (LocalizationManager.localisationDict.TryGetValue(errorCode, out RecordsLanguage find))
        //    {
        //        if (!LocalizationManager.forceJapanese)
        //        {
        //            switch (GameManager.currentLanguage)
        //            {
        //                case "en":
        //                    ActiveErrorText.text = find.English;
        //                    break;
        //                case "ja":
        //                    ActiveErrorText.text = find.Japanese;
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            ActiveErrorText.text = find.Japanese;
        //        }
        //    }
        //    else
        //    {
        //        ActiveErrorText.text = errorCode;
        //    }

        //}



        private void SetActiveText(ErrorMessage ErrorMessage)
        {
            if (!ActiveErrorText) return;

            switch (GameManager.currentLanguage)
            {
                case "en":
                    ActiveErrorText.text = CapitalizeMessage ? ErrorMessage.EnglishMessage.ToUpper() : ErrorMessage.EnglishMessage;
                    break;
                case "ja":
                    ActiveErrorText.text = CapitalizeMessage ? ErrorMessage.JapaneseMessage.ToUpper() : ErrorMessage.JapaneseMessage;
                    break;
                default:
                    ActiveErrorText.text = CapitalizeMessage ? ErrorMessage.EnglishMessage.ToUpper() : ErrorMessage.EnglishMessage;
                    break;
            }
        }
    }

    [Serializable]
    public struct ErrorMessage
    {
        public string ErrorCode;
        public string EnglishMessage;
        public string JapaneseMessage;

    }

    public enum ErrorType
    {
        Default_Message,
        Poor_Connection,
        Invalid_Username,
        Authentication_Code_is_Incorrect,
        Fields__empty,
        Name_Field__empty,
        Passwords_do_not_match,
        Password_field__empty,
        OTP_fields__empty,
        Same_Email_is_already_registered,
        Please_enter_valid_email,
        Email_field__empty,
        Same_Mobile_Number_is_registered,
        Phone_number__empty,
        Name_is_invalid,
        User_Details,
        Update_Profile,
        Wrong_Password,
        User_Already_Exist,
        User_Not_Valid,
        Enter_Valid_Number,
        User_Does_Not_Exist_with_Email,
        Incorrect_Phone_Format,
        Poor_connection_please_try_again,
        Passwords_cannot_less_than_eight_charcters,
        UserName_Has_Space,
        Special_chracater_not_included,
        Password_must_Contain_Number,
        Could_not_verify_signature
    }
}