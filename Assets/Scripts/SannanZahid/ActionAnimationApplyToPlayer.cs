using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ActionAnimationApplyToPlayer : MonoBehaviour
{
    public RuntimeAnimatorController controller;
    public static bool AnimationPlaying = false;
    public static Action<RuntimeAnimatorController> PlayerAnimatorInitializer;

    private GameObject[] photonplayerObjects;

    private void OnEnable()
    {
        PlayerAnimatorInitializer += SetPlayerAnimator;
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }
    public void SetPlayerAnimator(RuntimeAnimatorController playerAnimator)
    {
        controller = playerAnimator;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
        PlayerAnimatorInitializer -= SetPlayerAnimator;
    }

    public void LoadAnimationAccrossInstance(string label)//, Action downloadCompleteCallBack
    {
        StartCoroutine(DownloadAddressableActionAnimation(label));//,downloadCompleteCallBack
    }

    public void StopAnimation()
    {
        GameObject player;
        if (AvatarSpawnerOnDisconnect.Instance.currentDummyPlayer != null)
        {
            AvatarSpawnerOnDisconnect.Instance.currentDummyPlayer.transform.localPosition = new Vector3(0f, -0.081f, 0);
        }
        player = ReferencesForGamePlay.instance.m_34player;
        if (player != null)
        {
            object[] viewMine = { player.GetComponent<PhotonView>().ViewID };
            RaiseEventOptions options = new RaiseEventOptions();
            options.CachingOption = EventCaching.DoNotCache;
            options.Receivers = ReceiverGroup.All;
            PhotonNetwork.RaiseEvent(1, viewMine as object, options,
                SendOptions.SendReliable);
        }
        AvatarSpawnerOnDisconnect.Instance.spawnPoint.GetComponent<PlayerController>().enabled = true;
        AnimationPlaying = false;
    }

    public void DisableAnimationReaction(int viewId)
    {
        photonplayerObjects = null;
        photonplayerObjects = Photon.Pun.Demo.PunBasics.MutiplayerController.instance.playerobjects.ToArray();
        Animator animatorremote = null;

        for (int i = 0; i < photonplayerObjects.Length; i++)
        {
            if (photonplayerObjects[i] != null)
            {
                if (photonplayerObjects[i].GetComponent<PhotonView>().ViewID == viewId)
                {
                    animatorremote = photonplayerObjects[i].gameObject.GetComponent<Animator>();
                    animatorremote.runtimeAnimatorController = controller;
                    animatorremote.SetBool("IsEmote", false);

                }
            }
        }
    }

    private IEnumerator DownloadAddressableActionAnimation(string label)//, Action downloadCompleteCallBack
    {
        if (label != "" && Application.internetReachability != NetworkReachability.NotReachable)
        {
            int playerId = ReferencesForGamePlay.instance.m_34player.GetComponent<PhotonView>().ViewID;
            AsyncOperationHandle<AnimationClip> loadOp;
            loadOp = Addressables.LoadAssetAsync<AnimationClip>("Action_" + label);
            while (!loadOp.IsDone)
                yield return loadOp;

            if (loadOp.Status == AsyncOperationStatus.Succeeded)
            {
                if (loadOp.Result == null || loadOp.Result.Equals(null))
                {
                    Debug.LogError("NULLLLLL");
                }
                else
                {
                    AnimationPlaying = true;
                    SyncDataWithPlayers(label);
                   StartCoroutine( ApplyAnimationToAnimatorSet(loadOp.Result, playerId));
                }
            }
        }
    }
    private IEnumerator DownloadAddressableActionAnimation(string label, int playerID)//, Action downloadCompleteCallBack
    {
        if (label != "" && Application.internetReachability != NetworkReachability.NotReachable)
        {
            AsyncOperationHandle loadOp;
            bool flag = false;
            loadOp = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(label, ref flag);
            if (!flag)
                loadOp = Addressables.LoadAssetAsync<AnimationClip>("Action_" + label);
            while (!loadOp.IsDone)
                yield return loadOp;

            if (loadOp.Status == AsyncOperationStatus.Succeeded)
            {

                if (loadOp.Result == null || loadOp.Result.Equals(null))
                {
                    Debug.LogError("DownloadAddressableActionAnimation -> "+ label);
                }
                else
                {
                    AddressableDownloader.Instance.MemoryManager.AddToReferenceList(loadOp, label);
                    StartCoroutine(ApplyAnimationToAnimatorSet(loadOp.Result as AnimationClip, playerID));
                }
            }
        }
    }
    private void SyncDataWithPlayers(string animationName)
    {
        Dictionary<object, object> sharedData = new Dictionary<object, object>();
        sharedData.Add(ReferencesForGamePlay.instance.m_34player.GetComponent<PhotonView>().ViewID.ToString(), animationName);

        RaiseEventOptions options = new RaiseEventOptions();
        options.CachingOption = EventCaching.DoNotCache;
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(12, sharedData, options,
            SendOptions.SendReliable);
    }
    private IEnumerator ApplyAnimationToAnimatorSet(AnimationClip animationClip,int playerId)
    {
        Animator animator;
        photonplayerObjects = null;
        photonplayerObjects = Photon.Pun.Demo.PunBasics.MutiplayerController.instance.playerobjects.ToArray();
        for (int i = 0; i < photonplayerObjects.Length; i++)
        {

            if (photonplayerObjects[i] != null)
            {
                animator = photonplayerObjects[i].gameObject.GetComponent<Animator>();
                if (photonplayerObjects[i].GetComponent<PhotonView>().ViewID == playerId)
                {
                    if (!PlayerSelfieController.Instance.selfiePanel.activeInHierarchy)
                    {
                        if (animator.GetBool("EtcAnimStart"))
                        {
                            animator.SetBool("Stand", true);
                            animator.SetBool("EtcAnimStart", false);
                        }
                        yield return new WaitForSeconds(0f);
                        var overrideController = new AnimatorOverrideController();
                        overrideController.runtimeAnimatorController = controller;

                        List<KeyValuePair<AnimationClip, AnimationClip>> keyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                        foreach (var clip in overrideController.animationClips)
                        {
                            if (clip.name == "emaotedefault")
                                keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, animationClip));
                            else
                                keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
                        }
                        overrideController.ApplyOverrides(keyValuePairs);

                        animator.runtimeAnimatorController = overrideController;

                        animator.SetBool("IsEmote", true);
                    }
                }
            }
        }
    }

    private void NetworkingClient_EventReceived(EventData obj)
    {

        if (obj.Code == 1)
        {
            object[] minePlayer = (object[])obj.CustomData;
            DisableAnimationReaction((int)minePlayer[0]);
        }
        else if (obj.Code == 12)
        {
            StartCoroutine(waittostart(obj));
        }
    }
    public IEnumerator waittostart(EventData obj)
    {
        List<EventData> data = new List<EventData>();
        data.Clear();
        if (data.Count > 0)
        {
            data.Add(obj);
            yield break;
        }
        data.Add(obj);


        while (data.Count > 0)
        {

            Dictionary<object, object> DataShared = new Dictionary<object, object>();

            DataShared = (Dictionary<object, object>)data[0].CustomData;

            foreach (KeyValuePair<object, object> keyValue in DataShared)
            {
                string keyDataShared = keyValue.Key.ToString();

                if (!string.IsNullOrEmpty(DataShared[keyDataShared].ToString()))
                    yield return StartCoroutine(DownloadAddressableActionAnimation(DataShared[keyDataShared].ToString(), int.Parse(keyDataShared)));

            }
            data.RemoveAt(0);

        }
        yield return null;
    }
}
