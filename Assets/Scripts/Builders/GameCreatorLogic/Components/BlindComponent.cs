using System.Collections;
using Models;
using Photon.Pun;
using UnityEngine;

public class BlindComponent : ItemComponent
{
    private BlindComponentData blindComponentData;

    private bool activateComponent = false;
    private bool blindToggle;
    private bool isRunning;

    private GameObject player;

    Coroutine dimLightsCoroutine;

    public Light[] _light;
    public float[] _lightsIntensity;

    public int previousSkyID;
    public int skyBoxID = 21;


    private float againTouchDealy = .5f;
    private bool IsAgainTouchable = true;
    void Start()
    {
        _light = FindObjectsOfType<Light>();
        _lightsIntensity = new float[_light.Length];
        for (int i = 0; i < _light.Length; i++)
        {
            _lightsIntensity[i] = _light[i].intensity;
        }
    }

    public void Init(BlindComponentData _blindComponentData)
    {
        this.blindComponentData = _blindComponentData;

        activateComponent = true;

        blindToggle = _blindComponentData.isOff;
    }

    Coroutine blindComponentCo;
    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (PlayerCanvas.Instance.transform.parent != _other.transform)
            {
                PlayerCanvas.Instance.transform.SetParent(_other.transform);
                PlayerCanvas.Instance.transform.localPosition = Vector3.up * PlayerCanvas.Instance.transform.localPosition.y;
                PlayerCanvas.Instance.cameraMain = GamificationComponentData.instance.playerControllerNew.ActiveCamera.transform;
            }
            if (!IsAgainTouchable) return;

            IsAgainTouchable = false;
            //if (blindComponentData.time == 0 && !blindComponentData.isOff)
            //    return;
            //if (blindComponentCo == null && blindComponentData.time > 0)
            //    blindComponentCo = StartCoroutine(nameof(BlindComponentStart));

            GamificationComponentData.instance.buildingDetect.StopSpecialItemComponent();
            TimeStats._blindComponentStart?.Invoke(blindToggle, _light, _lightsIntensity, blindComponentData.time, blindComponentData.radius, this.gameObject, skyBoxID);
        }
    }

    IEnumerator BlindComponentStart()
    {
        while (blindComponentData.time > 0)
        {
            blindComponentData.time--;
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        IsAgainTouchable = false;
    }
    private void OnCollisionExit(Collision collision)
    {
        IsAgainTouchable = true;
    }
}