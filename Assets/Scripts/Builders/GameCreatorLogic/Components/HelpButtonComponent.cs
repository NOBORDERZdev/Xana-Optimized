using UnityEngine;
using Models;
using Photon.Pun;

public class HelpButtonComponent : ItemComponent
{
    [SerializeField]
    private HelpButtonComponentData helpButtonComponentData;

    public void Init(HelpButtonComponentData helpButtonComponentData)
    {
        this.helpButtonComponentData = helpButtonComponentData;
        if (this.helpButtonComponentData.IsAlwaysOn)
        {
            GamificationComponentData.instance.worldCameraEnable = true;
            GameObject go;
            HelpButtonComponentResizer infoPopup;
            go = Instantiate(GamificationComponentData.instance.helpParentReference, this.transform.position, new Quaternion(0, 0, 0, 0), GamificationComponentData.instance.worldSpaceCanvas.transform);
            go.transform.position += Vector3.up;
            go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            infoPopup = go.GetComponent<HelpButtonComponentResizer>();
            infoPopup.isAlwaysOn = helpButtonComponentData.IsAlwaysOn;
            infoPopup.titleText.text = helpButtonComponentData.titleHelpButtonText;
            infoPopup.contentText.text = helpButtonComponentData.helpButtonData;
            go.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision _other)
    {
        if ((_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine) && !this.helpButtonComponentData.IsAlwaysOn)
        {
            {
                BuilderEventManager.OnHelpButtonCollisionEnter?.Invoke(helpButtonComponentData.titleHelpButtonText, helpButtonComponentData.helpButtonData, this.gameObject);
            }
        }
    }

    private void OnCollisionExit(Collision _other)
    {
        if ((_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine) && !this.helpButtonComponentData.IsAlwaysOn)
        {
            BuilderEventManager.OnHelpButtonCollisionExit?.Invoke();
        }
    }
}