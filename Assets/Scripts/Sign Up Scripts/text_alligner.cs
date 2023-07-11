using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class text_alligner : MonoBehaviour
{
    Text txt;
    // Start is called before the first frame update

    private void Awake()
    {
        txt = GetComponent<Text>();

    }


    private void Start()
    {
        if (GameManager.currentLanguage == "ja")
        {
                txt.text = "アカウントをお持ちではありませんか？  <color=#008FFF>新規登録</color>";
            }
            else if (GameManager.currentLanguage == "en")
{
    txt.text = "Don't have an account?  <color=#008FFF>Sign Up</color>";
}
    }
}

//void OnApplicationFocus(bool focus)
//    {
//        if (focus)
//        {
//            if (GameManager.currentLanguage == "ja")
//            {
//                txt.text = "アカウントをお持ちではありませんか？  <color=#008FFF>新規登録</color>";
//            }
//            else if (GameManager.currentLanguage == "en")
//            {
//                txt.text = "Don't have an account?  <color=#008FFF>Sign Up</color>";
//            }
//        }
//    }
//}
    // Update is called once per frame


