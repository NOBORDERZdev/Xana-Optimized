using UnityEngine;
using Models;
using Photon.Pun;

public class HyperLinkPopComponent : ItemComponent
{
    HyperLinkComponentData hyperLinkComponentData;
    string titleText, buttonData;
    public void Init(HyperLinkComponentData hyperLinkComponentData)
    {
        this.hyperLinkComponentData = hyperLinkComponentData;
    }

    void SetHelpButtonNarration()
    {
        titleText = hyperLinkComponentData.titleHelpButtonText;
        buttonData = hyperLinkComponentData.helpButtonData;
        if (hyperLinkComponentData.titleHelpButtonText.IsNullOrEmpty())
        {
            titleText = "Enter a Title to display";
        }
        if (hyperLinkComponentData.helpButtonData.IsNullOrEmpty())
        {
            buttonData = "Enter Help message to display";
        }
        BuilderEventManager.OnHyperLinkPopupCollisionEnter?.Invoke(titleText, buttonData, hyperLinkComponentData.urlData, this.transform);
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            SetHelpButtonNarration();
        }
    }

    private void OnCollisionExit(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.OnHyperLinkPopupCollisionExit?.Invoke();
        }
    }
}