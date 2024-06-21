using ExitGames.Client.Photon;
using Metaverse;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ActionAnimationApplyToPlayer : MonoBehaviour, IInRoomCallbacks, IOnEventCallback
{
   
    public RuntimeAnimatorController controller;
    private GameObject[] photonplayerObjects;

    public void LoadAnimationAccrossInstance(string label)//, Action downloadCompleteCallBack
    {
        StartCoroutine(DownloadAddressableActionAnimation(label));//,downloadCompleteCallBack
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
                    Debug.LogError("Addressable Loadede ---->>>>>   " + playerId);

                   // downloadCompleteCallBack?.Invoke();
                   StartCoroutine( ApplyAnimationToAnimatorSet(loadOp.Result, playerId));
                }
            }
        }
    }

    private IEnumerator ApplyAnimationToAnimatorSet(AnimationClip animationClip,int playerId)
    {
        Animator animator;
        photonplayerObjects = null;
        photonplayerObjects = Photon.Pun.Demo.PunBasics.MutiplayerController.instance.playerobjects.ToArray();
        for (int i = 0; i < photonplayerObjects.Length; i++)
        {
            animator = photonplayerObjects[i].gameObject.GetComponent<Animator>();
            if (photonplayerObjects[i] != null)
            {
                if (photonplayerObjects[i].GetComponent<PhotonView>().ViewID == playerId)
                {
                    if (!PlayerSelfieController.Instance.selfiePanel.activeInHierarchy)
                    {
                        if (animator.GetBool("EtcAnimStart"))
                        {
                            animator.SetBool("Stand", true);
                            animator.SetBool("EtcAnimStart", false);
                            //foreach (var clip in animator.runtimeAnimatorController.animationClips)
                            //{
                            //    if (clip.name == "Stand")
                            //    {
                            //        yield return new WaitForSeconds(clip.length);
                            //    }
                            //}
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
                        // isPreviousBundleLoad = true;
                        // CheckSelfieOn();
                    }
                }
            }
        }
    }

    public void StopAnimation()
    {
        GameObject player;
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();

        {
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

            //iscashed = false;

            //PlayerPrefs.SetString(remoteUrlAnimation, "");

            AvatarSpawnerOnDisconnect.Instance.spawnPoint.GetComponent<PlayerController>().enabled = true;
            //PlayerPrefsUtility.SetEncryptedString(ConstantsGod.SELECTED_ANIMATION_NAME, "");
            //LoadEmoteAnimations.animClick = false;
        }
        //isAnimRunning = false;
        //MyAnimLoader = false;
        //alreadyRuning = true;
        //AnimationStopped?.Invoke(remoteUrlAnimationName);
        //if (player.GetComponent<PhotonView>().IsMine)
        //{
        //    if (AnimObject != null)
        //    {
        //        AnimObject.transform.GetChild(3).gameObject.SetActive(false);
        //    }
        //    StopAllCoroutines();
        //    isFetchingAnim = false;
        //    AnimObject = null;
        //}
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
    /////
    ///


    private void NetworkingClient_EventReceived(EventData obj)
    {
        if (obj.Code == 1)
        {
            object[] minePlayer = (object[])obj.CustomData;
            DisableAnimationReaction((int)minePlayer[0]);
        }
        //if (obj.Code == 0)
        //{
        //    if (firsttimecall == false)
        //    {
        //        firsttimecall = true;
        //        remotePlayerId = (int)obj.CustomData;
        //    }
        //}

        //else if (obj.Code == 1)
        //{

        //    object[] minePlayer = (object[])obj.CustomData;
        //    DisableAnim((int)minePlayer[0]);
        //}
        //else if (obj.Code == 12)
        //{
        //    StartCoroutine(waittostart(obj));

        //}
    }
    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        //if (iscashed)
        //{
        //    RaiseEventOptions options = new RaiseEventOptions();
        //    options.CachingOption = EventCaching.DoNotCache;
        //    options.TargetActors = new int[] { newPlayer.ActorNumber };
        //    PhotonNetwork.RaiseEvent(12, cashed_data, options,
        //        SendOptions.SendReliable);
        //}
    }
    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
    }

    public void OnEvent(EventData photonEvent)
    {

    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
    }
}
