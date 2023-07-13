using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PlayerCanvas : MonoBehaviour
{
    public static PlayerCanvas Instance;

    [SerializeField] GameObject keyImage;
    [SerializeField] GameObject wrongKey;
    public TextMeshPro keyCounter;

    [SerializeField] GameObject blindLight;
    [SerializeField] GameObject[] blindAdditionalLights;
    [SerializeField] GameObject sceneDirectionalLightBind;
    [SerializeField] Color ambientColorBlind;
    [SerializeField] Color oldAmbientColorBlind;

    internal Transform cameraMain;
    bool isKeyEnabled;

    private void Start()
    {
        if (Instance != this || Instance == null) Instance = this;
        isKeyEnabled = false;
    }

    public void ToggleKey(bool _value)
    {
        keyImage.SetActive(_value);
        isKeyEnabled = _value;
    }

    public void ToggleWrongKey() => StartCoroutine(WrongKeyRoutine());
    IEnumerator WrongKeyRoutine()
    {
        wrongKey.SetActive(true);
        keyImage.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        wrongKey.SetActive(false);
    }


    public void ToggleBlindLight(bool _value, float _angle)
    {

        blindLight.GetComponent<Light>().innerSpotAngle = 0;
        blindLight.GetComponent<Light>().spotAngle = (_angle * 8) + (20);
        blindLight.SetActive(_value);

        foreach (var item in blindAdditionalLights)
        {
            item.SetActive(_value);
        }

        sceneDirectionalLightBind.SetActive(!_value);

        RenderSettings.ambientLight = (_value) ? ambientColorBlind : oldAmbientColorBlind;
    }

    private void LateUpdate()
    {
        if (isKeyEnabled && cameraMain != null)
        {
            Quaternion cameraRotation = cameraMain.rotation;
            Quaternion targetRotation = Quaternion.Euler(0, cameraMain.eulerAngles.y, 180);
            keyImage.transform.rotation = targetRotation;
        }
    }


}

