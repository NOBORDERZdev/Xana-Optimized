using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignTxtForJP : MonoBehaviour
{

    private void OnEnable()
    {
        if (GameManager.currentLanguage == "ja" || CustomLocalization.forceJapanese)
            transform.localPosition = new Vector3(80, 0, 0);
    }


}
