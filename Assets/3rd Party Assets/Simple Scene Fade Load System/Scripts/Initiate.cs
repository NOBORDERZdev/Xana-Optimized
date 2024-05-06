using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class Initiate
{
   public static bool areWeFading = false;

    //Create Fader object and assing the fade scripts and assign all the variables
    public static void Fade(string scene, Color col, float multiplier, bool isSetColorCompulsory = false)
    {
        if (areWeFading)
        {
            Debug.Log("Already Fading");
            return;
        }

        GameObject init = new GameObject();
        init.name = "Fader";
        Canvas myCanvas = init.AddComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        init.AddComponent<Fader>();
        init.AddComponent<CanvasGroup>();
        init.AddComponent<Image>();
        
        myCanvas.sortingOrder = 1000;

        Fader scr = init.GetComponent<Fader>();
        scr.fadeDamp = multiplier;
        scr.fadeScene = scene;
        if (!SceneManager.GetActiveScene().name.Equals("Home") || isSetColorCompulsory)
        {
            scr.fadeColor = col;
        }
       
        scr.start = true;
        areWeFading = true;
        scr.InitiateFader();  
        
    }

    public static void DoneFading() {
        areWeFading = false;
        
    }
}