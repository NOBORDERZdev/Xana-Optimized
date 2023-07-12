using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleChangeImage : MonoBehaviour
{
    public Toggle toggle;
    public GameObject imageToChange;
    public Sprite toggleOnImage,toggleOffImage;
    public void ChangeImage() {
        if (toggle.isOn)
        {
            imageToChange.GetComponent<Image>().sprite = toggleOnImage;
        }
        else
        {
            imageToChange.GetComponent<Image>().sprite = toggleOffImage;
        }
    }

    private void OnEnable()
    {
        ChangeImage();
    }
}
