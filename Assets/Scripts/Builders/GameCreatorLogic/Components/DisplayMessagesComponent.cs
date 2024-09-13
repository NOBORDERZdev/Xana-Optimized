using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Photon.Pun;

public class DisplayMessagesComponent : ItemComponent
{
    [SerializeField]
    private DisplayMessageComponentData displayMessageComponentData;
    public static IEnumerator currentCoroutine;

    public void Init(DisplayMessageComponentData displayMessageComponentData)
    {
        this.displayMessageComponentData = displayMessageComponentData;
        // Remove leading and trailing spaces
        string inputText = this.displayMessageComponentData.startDisplayMessage.Trim();
        // Replace all spaces between lines with an empty string
        string displayMessageCleanedText = System.Text.RegularExpressions.Regex.Replace(inputText, @"\s+", " ");
        this.displayMessageComponentData.startDisplayMessage = displayMessageCleanedText;
    }

    //oncollisionEnter to OnTriggerEnter
    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.onComponentActivated(_componentType);
            PlayBehaviour();
        }
    }

    #region BehaviourControl

    private void StartComponent()
    {
        ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.DisplayMessage);

        if (displayMessageComponentData.isStart)
        {
            BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke(displayMessageComponentData.startDisplayMessage, displayMessageComponentData.startTimerCount, true);
        }
        if (displayMessageComponentData.isEnd)
        {
            BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke(displayMessageComponentData.endDisplayMessage, 5, false);
        }

    }
    private void StopComponent()
    {
        BuilderEventManager.OnDisplayMessageCollisionEnter?.Invoke(displayMessageComponentData.endDisplayMessage, 0, false);
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
        _componentType = Constants.ItemComponentType.DisplayMessagesComponent;
    }

    public override void CollisionExitBehaviour()
    {
        //throw new System.NotImplementedException();
    }

    public override void CollisionEnterBehaviour()
    {
        //CollisionEnter();
    }

    #endregion
}