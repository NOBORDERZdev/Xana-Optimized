using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToShoesHirokoKoshino : MonoBehaviour
{
    public static SwitchToShoesHirokoKoshino Instance;

    public GameObject KintDressLight;
    public GameObject KintPulloverLight;
    public GameObject LilyFlowerPrintCoordSetLight;
    public GameObject PrintDressLight;
    public GameObject YarnKnitLight;
    public GameObject SwirlCoordSetLight;
    public GameObject TaffetaGatheredDressLight;
    public GameObject FlowerPattrenCoordSetLight;
    public GameObject PaneledShirtPleatedSkirtLight;
    public GameObject SwirlDressLight;
   

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        SwitchLightFor_HirokoKoshino(PlayerPrefs.GetString("HirokoLight"));
    }

    public void SwitchLightFor_HirokoKoshino(string s)
    {
        //DisableAllLighting();
        if (KintDressLight == null)
            return;
        
        if (s.Equals("full_costume_hirokoch01"))
        {
            DisableAllLighting();
            TaffetaGatheredDressLight?.SetActive(true);
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("TaffetaGatheredDressLight Activted");
        }
        if (s.Equals("full_costume_hirokoch02"))
        {
            DisableAllLighting();
            PaneledShirtPleatedSkirtLight?.SetActive(true); 
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("Paneled Shirt Pleated SkirtLight Activted");
        }
        else if (s.Equals("full_costume_hirokoch03"))
        {
            DisableAllLighting();
            LilyFlowerPrintCoordSetLight?.SetActive(true);
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("lilly Flower Print light Dress Activted");
        }
        else if (s.Equals("full_costume_hirokoch04"))
        {
            //DisableAllLighting();
            PrintDressLight?.SetActive(true);
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("Print Co-ord Set Light Activted");
        }
        else if (s.Equals("full_costume_hirokoch05"))
        {
            DisableAllLighting();
            KintDressLight?.SetActive(true);
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("color block knit Light Activted");
        }
        else if (s.Equals("full_costume_hirokoch06"))
        {
            DisableAllLighting();
            KintPulloverLight?.SetActive(true);
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("Knit Pull over Light Activted");
        }
       else if (s.Equals("full_costume_hirokoch07"))
        {
            DisableAllLighting();
            YarnKnitLight.SetActive(true);
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("mixed Yarn Knit Light Activted");
        }
        else if (s.Equals("full_costume_hirokoch08"))
        {
            DisableAllLighting();
            FlowerPattrenCoordSetLight.SetActive(true); 
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("Flower Pattren Co ord Set Light  Activted");
        }
        else if (s.Equals("full_costume_hirokoch09"))
        {
            DisableAllLighting();
            SwirlCoordSetLight?.SetActive(true);
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("Swirl Co-ord Set Light Activted");
        }
        else if (s.Equals("full_costume_hirokoch10"))
        {
            DisableAllLighting();
            SwirlDressLight?.SetActive(true); 
            PlayerPrefs.SetString("HirokoLight", s);
            Debug.Log("Swirl Dress Light Activted");
        }


    }

    public void DisableAllLighting()
    {
        
        KintDressLight?.SetActive(false);
        KintPulloverLight?.SetActive(false);
        LilyFlowerPrintCoordSetLight?.SetActive(false);
        PrintDressLight?.SetActive(false);
        YarnKnitLight?.SetActive(false);
        SwirlCoordSetLight?.SetActive(false);
        TaffetaGatheredDressLight?.SetActive(false);
        FlowerPattrenCoordSetLight?.SetActive(false);
        PaneledShirtPleatedSkirtLight?.SetActive(false);
        SwirlDressLight?.SetActive(false);
    }

 
}
