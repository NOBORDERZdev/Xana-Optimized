using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RFMButtonsLayoutManager : MonoBehaviour
{
    public Button saveLayoutBtn;
    public Button closePanelBtn;

    public GameObject currentSelectedObject;

    public GameObject joyStick, runButton, jumpButton, slideButton;

    public Slider sizeSlider;

    public Camera RFMCamera;

    private void OnEnable()
    {
        sizeSlider.onValueChanged.AddListener(ResizeUI);
    }
    private void OnDisable()
    {
        sizeSlider.onValueChanged.RemoveAllListeners();
    }
    public void SaveLayout() 
    {
        PlayerPrefs.SetFloat("joyX", joyStick.transform.localPosition.x);
        PlayerPrefs.SetFloat("joyY", joyStick.transform.localPosition.y);
        PlayerPrefs.SetFloat("joyScaleX", joyStick.transform.localScale.x);
        PlayerPrefs.SetFloat("joyScaleY", joyStick.transform.localScale.y);

        PlayerPrefs.SetFloat("runX", runButton.transform.localPosition.x);
        PlayerPrefs.SetFloat("runY", runButton.transform.localPosition.y);
        PlayerPrefs.SetFloat("runScaleX", runButton.transform.localScale.x);
        PlayerPrefs.SetFloat("runScaleY", runButton.transform.localScale.y);

        PlayerPrefs.SetFloat("jumpX", jumpButton.transform.localPosition.x);
        PlayerPrefs.SetFloat("jumpY", jumpButton.transform.localPosition.y);
        PlayerPrefs.SetFloat("jumpScaleX", jumpButton.transform.localScale.x);
        PlayerPrefs.SetFloat("jumpScaleY", jumpButton.transform.localScale.y);

        PlayerPrefs.SetFloat("slideX", slideButton.transform.localPosition.x);
        PlayerPrefs.SetFloat("slideY", slideButton.transform.localPosition.y);
        PlayerPrefs.SetFloat("slideScaleX", slideButton.transform.localScale.x);
        PlayerPrefs.SetFloat("slideScaleY", slideButton.transform.localScale.y);
    }

    public void LoadLayout() 
    {
        joyStick.transform.localPosition = new Vector3 (PlayerPrefs.GetFloat("joyX", 75), PlayerPrefs.GetFloat("joyY", 36), 0);
        joyStick.transform.localScale = new Vector3(PlayerPrefs.GetFloat("joyScaleX", 1), PlayerPrefs.GetFloat("joyScaleY", 1), 0);

        runButton.transform.localPosition = new Vector3(PlayerPrefs.GetFloat("runX", -52.5f), PlayerPrefs.GetFloat("runY", 143.29f), 0);
        runButton.transform.localScale = new Vector3(PlayerPrefs.GetFloat("runScaleX", 1), PlayerPrefs.GetFloat("runScaleY", 1), 0);

        jumpButton.transform.localPosition = new Vector3(PlayerPrefs.GetFloat("jumpX", -91), PlayerPrefs.GetFloat("jumpY", 73), 0);
        jumpButton.transform.localScale = new Vector3(PlayerPrefs.GetFloat("jumpScaleX", 1), PlayerPrefs.GetFloat("jumpScaleY", 1), 0);

        jumpButton.transform.localPosition = new Vector3(PlayerPrefs.GetFloat("slideX", -133.73f), PlayerPrefs.GetFloat("slideY", 143.29f), 0);
        jumpButton.transform.localScale = new Vector3(PlayerPrefs.GetFloat("slideScaleX", 1), PlayerPrefs.GetFloat("slideScaleY", 1), 0);
    }

    public void ResizeUI(float size) 
    {
        if (currentSelectedObject != null) 
        {
            currentSelectedObject.transform.localScale = new Vector3(1f + size, 1f + size, 0);
        }
    }




    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            {
                RaycastHit hit;
                Ray ray = RFMCamera.ScreenPointToRay(touch.position);
                print(touch.position);
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject touchedObject = hit.transform.gameObject;
                    print("hit a gameObject prolly");
                    print(hit.transform.name);
                    if (hit.transform.gameObject.CompareTag("resizable")) 
                    {
                        print("TOUCH2");
                        currentSelectedObject = touchedObject;
                    }
                }
            }           

        }
    }
}
