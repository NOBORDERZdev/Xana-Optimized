using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] GameObject keyImage;
    [SerializeField] GameObject wrongKey;
    public TextMeshProUGUI keyCounter;

    [SerializeField] GameObject blindLight;
    [SerializeField] GameObject[] blindAdditionalLights;
    internal GameObject sceneDirectionalLightBind;
    [SerializeField] internal Color ambientColorBlind;
    [SerializeField] internal Color oldAmbientColorBlind;

    internal Transform cameraMain;
    bool isKeyEnabled;

    private void Start()
    {
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
        if (sceneDirectionalLightBind == null)
            sceneDirectionalLightBind = SituationChangerSkyboxScript.instance.directionLight.gameObject;
        sceneDirectionalLightBind.SetActive(!_value);

        RenderSettings.ambientLight = (_value) ? ambientColorBlind : oldAmbientColorBlind;
    }

    private void LateUpdate()
    {
        if (isKeyEnabled && cameraMain != null)
        {
            if (!cameraMain.gameObject.activeInHierarchy)
            {
                //cameraMain = GamificationComponentData.instance.playerControllerNew.ActiveCamera.transform;
                cameraMain = ReferencesForGamePlay.instance.playerControllerNew.ActiveCamera.transform;
            }
            Quaternion targetRotation = Quaternion.Euler(0, cameraMain.eulerAngles.y, 0);
            this.transform.rotation = targetRotation;
        }
    }


}

