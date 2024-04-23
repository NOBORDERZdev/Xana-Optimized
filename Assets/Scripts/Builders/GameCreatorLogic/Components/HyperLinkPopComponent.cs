using UnityEngine;
using Models;
using Photon.Pun;

public class HyperLinkPopComponent : ItemComponent
{
    HyperLinkComponentData hyperLinkComponentData;
    string titleText, buttonData;
    string RuntimeItemID = "";

    public void Init(HyperLinkComponentData hyperLinkComponentData)
    {
        this.hyperLinkComponentData = hyperLinkComponentData;
        RuntimeItemID = this.GetComponent<XanaItem>().itemData.RuntimeItemID;

        // Remove leading and trailing spaces
        string inputText = this.hyperLinkComponentData.titleHelpButtonText.Trim();
        // Replace all spaces between lines with an empty string
        string hyperLinkCleanedText = System.Text.RegularExpressions.Regex.Replace(inputText, @"\s+", " ");

        this.hyperLinkComponentData.titleHelpButtonText = hyperLinkCleanedText;
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
            BuilderEventManager.onComponentActivated?.Invoke(_componentType);
            PlayBehaviour();
        }
    }

    private void OnCollisionExit(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.OnHyperLinkPopupCollisionExit?.Invoke();
        }
    }

    #region BehaviourControl
    private void StartComponent()
    {
        SetHelpButtonNarration();
    }
    private void StopComponent()
    {


    }

    public override void StopBehaviour()
    {
        if (isPlaying)
        {
            isPlaying = false;
            StopComponent();
        }
    }

    public override void PlayBehaviour()
    {
        isPlaying = true;
        StartComponent();
    }

    public override void ToggleBehaviour()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
            PlayBehaviour();
        else
            StopBehaviour();
    }
    public override void ResumeBehaviour()
    {
        PlayBehaviour();
    }

    public override void AssignItemComponentType()
    {
        _componentType = Constants.ItemComponentType.HyperLinkPopComponent;
    }

    public override void CollisionExitBehaviour()
    {
        //CollisionExit();
    }

    public override void CollisionEnterBehaviour()
    {
        //CollisionEnter();
    }

    #endregion
}