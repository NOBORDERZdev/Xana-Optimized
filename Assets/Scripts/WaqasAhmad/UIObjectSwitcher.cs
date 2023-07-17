using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIObjectSwitcher : MonoBehaviour
{
    public Vector2 landscapeImgsize;
    public RectTransform landscapeImg;
    public Vector2 portraitImgsize;

    public GameObject[] landscapeUIObjs;
    public GameObject[] portraitUIObjs;


    private void OnEnable()
    {
        ChangeOrientation_waqas.switchOrientation += OrientationChanged;
    }
    private void OnDisable()
    {
        ChangeOrientation_waqas.switchOrientation -= OrientationChanged;
    }


    void OrientationChanged()
    {
        if (ChangeOrientation_waqas._instance.isPotrait)
        {
            foreach (GameObject landscapeAction in landscapeUIObjs)
                landscapeAction.SetActive(false);
            foreach (GameObject portraitAction in portraitUIObjs)
                portraitAction.SetActive(true);

            if (landscapeImg)
                landscapeImg.sizeDelta = portraitImgsize;
               // portraitImg.overrideSprite = landscapeImg.sprite;
        }
        else
        {
            foreach (GameObject landscapeAction in landscapeUIObjs)
                landscapeAction.SetActive(true);
            foreach (GameObject portraitAction in portraitUIObjs)
                portraitAction.SetActive(false);

            if (landscapeImg)
                landscapeImg.sizeDelta = landscapeImgsize;
        }
    }
}