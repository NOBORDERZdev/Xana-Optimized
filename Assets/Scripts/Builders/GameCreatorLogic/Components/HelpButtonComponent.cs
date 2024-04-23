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
        // Remove leading and trailing spaces
        string inputText = this.helpButtonComponentData.titleHelpButtonText.Trim();
        // Replace all spaces between lines with an empty string
        string hyperLinkCleanedText = System.Text.RegularExpressions.Regex.Replace(inputText, @"\s+", " ");

        this.helpButtonComponentData.titleHelpButtonText = hyperLinkCleanedText;

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
            infoPopup.msg = helpButtonComponentData.helpButtonData.Length == 0 ? "Define Rules here !" : helpButtonComponentData.helpButtonData + "\n";
            //infoPopup.contentText.text = helpButtonComponentData.helpButtonData;
            infoPopup.scrollView.enabled = false;
            infoPopup.scrollbar.SetActive(false);
            go.SetActive(true);
            infoPopup.Init();
            BuilderEventManager.EnableWorldCanvasCamera?.Invoke();
        }
    }

    private void OnCollisionEnter(Collision _other)
    {
        if ((_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine) && !this.helpButtonComponentData.IsAlwaysOn)
        {
            {
                BuilderEventManager.OnHelpButtonCollisionEnter?.Invoke(helpButtonComponentData.titleHelpButtonText, helpButtonComponentData.helpButtonData, this.gameObject);
                ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.InfoPopup);

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

    #region BehaviourControl
    private void StartComponent()
    {

    }
    private void StopComponent()
    {


    }

    public override void StopBehaviour()
    {
        isPlaying = false;
        StopComponent();
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
        _componentType = Constants.ItemComponentType.HelpButtonComponent;
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