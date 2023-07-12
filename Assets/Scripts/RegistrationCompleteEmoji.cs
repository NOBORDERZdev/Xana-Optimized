using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegistrationCompleteEmoji : MonoBehaviour
{
    public GameObject tada1;
    public GameObject tada2;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.currentLanguage == "ja")
        {
            tada1.SetActive(false);
            tada2.SetActive(true);
        }
        else if (GameManager.currentLanguage == "en")
        {
            tada1.SetActive(true);
            tada2.SetActive(false);
        }
    }

}
