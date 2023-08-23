using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToShoesHirokoKoshinoNFT : MonoBehaviour
{
    public static SwitchToShoesHirokoKoshinoNFT Instance;

    public GameObject KintDressLight;
    public GameObject KintPulloverLight;
    public GameObject LilyFlowerPrintCoordSetLight;
    public GameObject PrintDressLight;
    public GameObject YarnKnitLight;
    public GameObject SwirlCoordSetLight;
    public GameObject TaffetaGatheredDressLight;
   

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        SwitchLightFor_HirokoKoshino(PlayerPrefs.GetString("HirokoLight"));
    }

    public void SwitchLightFor_HirokoKoshino(string s)
    {
        //DisableAllLighting();

        if (s.Equals("full_costume_hirokoch01"))
        {
            DisableAllLighting();
            TaffetaGatheredDressLight.SetActive(true);
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("TaffetaGatheredDressLight Activted");
        }

       
    }

   public void DisableAllLighting()
    {
        KintDressLight.SetActive(false);
        KintPulloverLight.SetActive(false);
        LilyFlowerPrintCoordSetLight.SetActive(false);
        PrintDressLight.SetActive(false);
        YarnKnitLight.SetActive(false);
        SwirlCoordSetLight.SetActive(false);
        TaffetaGatheredDressLight.SetActive(false);
   
    }

 
}
