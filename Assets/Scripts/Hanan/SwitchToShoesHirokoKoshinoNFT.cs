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
        else if (s.Equals("full_costume_hirokoch03"))
        {
            DisableAllLighting();
            LilyFlowerPrintCoordSetLight.SetActive(true);
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("lilly Flower Print light Dress Activted");
        }
        else if (s.Equals("full_costume_hirokoch09"))
        {
            DisableAllLighting();
            SwirlCoordSetLight.SetActive(true);
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("Swirl Co-ord Set Light Activted");
        }
        else if (s.Equals("full_costume_hirokoch06"))
        {
            DisableAllLighting();
            KintPulloverLight.SetActive(true);
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("Knit Pull over Light Activted");
        }
        else if (s.Equals("full_costume_hirokoch05"))
        {
            DisableAllLighting();
            KintDressLight.SetActive(true);
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("color block knit Light Activted");
        }
        else if (s.Equals("full_costume_hirokoch04"))
        {
            DisableAllLighting();
            PrintDressLight.SetActive(true);
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("Print Co-ord Set Light Activted");
        }
       else if (s.Equals("full_costume_hirokoch07"))
        {
            DisableAllLighting();
            YarnKnitLight.SetActive(true);
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("mixed Yarn Knit Light Activted");
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
