using NPC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class SPAAIEmoteController : MonoBehaviour
{
    public RuntimeAnimatorController npcController;
    public string CurrDanceAnimName;
    public List<string> AnimPlayList = new List<string>();
    public List<float> AnimPlayTimer = new List<float>();
    public bool KeepLoopingEmotes = true;

    [SerializeField] SPAAIBehvrController spaAIBhvrController;
    [SerializeField] NpcMovementController npcMC;
    [SerializeField] Animator animationController;

    Coroutine emotCoroutine;
    public int _inValidAnimCount;
    string emoteName;
    string emoteBundleUrl;
    string emoteBundlePath;

    public IEnumerator PlayEmote()
    {
        if (!npcMC.isMoving)
        {
            while (KeepLoopingEmotes)
            {
                _inValidAnimCount = 0;
                for (int i = 0; i < AnimPlayList.Count; i++)
                {
                    if (!KeepLoopingEmotes)
                    {
                        break;
                    }
                    emotCoroutine = StartCoroutine(DownloadAddressableActionAnimation(AnimPlayList[i]));
                    yield return new WaitForSeconds(AnimPlayTimer[i]);
                    #region Old Asset Bundle Implementation
                    //if (SerachForEmoteWithName(AnimPlayList[i]))
                    //{
                    //    if (CheckForIsAssetBundleAvailable(Path.Combine(ConstantsHolder.xanaConstants.r_EmoteStoragePersistentPath, emoteName + ".unity3d")))
                    //    {
                    //        emotCoroutine = StartCoroutine(LoadAssetBundleFromStorage(Path.Combine(ConstantsHolder.xanaConstants.r_EmoteStoragePersistentPath, emoteName + ".unity3d"), _currentObject));
                    //        yield return new WaitForSeconds(AnimPlayTimer[i]);
                    //    }
                    //    else
                    //    {
                    //        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(emoteBundleUrl))
                    //        {
                    //            yield return www.SendWebRequest();
                    //            if (www.result != UnityWebRequest.Result.Success)
                    //            {
                    //                Debug.LogError("Got error in www request to get bundle: " + www.error);
                    //                _inValidAnimCount++;
                    //                continue;
                    //            }
                    //            else
                    //            {
                    //                AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(www);
                    //                if (assetBundle != null)
                    //                {
                    //                    AssetBundleRequest newRequest = assetBundle.LoadAllAssetsAsync<GameObject>();
                    //                    yield return newRequest;
                    //                    if (newRequest.isDone)
                    //                    {
                    //                        var animation = newRequest.allAssets;
                    //                        foreach (var anim in animation)
                    //                        {
                    //                            GameObject go = (GameObject)anim;
                    //                            if (go.name.Equals("Animation") || go.name.Equals("animation"))
                    //                            {
                    //                                if (animationController.GetBool("EtcAnimStart"))   // Added by Ali Hamza
                    //                                {
                    //                                    animationController.SetBool("Stand", true);
                    //                                    animationController.SetBool("EtcAnimStart", false);
                    //                                    foreach (var clip in animationController.runtimeAnimatorController.animationClips)
                    //                                    {
                    //                                        if (clip.name == "Stand")
                    //                                        {
                    //                                            yield return new WaitForSeconds(clip.length);
                    //                                        }
                    //                                    }
                    //                                }
                    //                                var overrideController = new AnimatorOverrideController();
                    //                                overrideController.runtimeAnimatorController = npcController;
                    //                                List<KeyValuePair<AnimationClip, AnimationClip>> keyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                    //                                foreach (var clip in overrideController.animationClips)
                    //                                {
                    //                                    if (clip.name == "emaotedefault")
                    //                                        keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, go.transform.GetComponent<Animation>().clip));
                    //                                    else
                    //                                        keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
                    //                                    overrideController.ApplyOverrides(keyValuePairs);
                    //                                    animationController.runtimeAnimatorController = overrideController;
                    //                                    animationController.SetBool("IsEmote", true);
                    //                                }
                    //                            }
                    //                            else
                    //                            {
                    //                                if (animationController != null)    //Added by Ali Hamza
                    //                                {
                    //                                    animationController.SetBool("Stand", true);
                    //                                    animationController.SetBool("EtcAnimStart", false);
                    //                                    if (CheckSpecificAnimationPlaying("Sit"))
                    //                                    {
                    //                                        foreach (var clip in animationController.runtimeAnimatorController.animationClips)
                    //                                        {
                    //                                            if (clip.name == "Stand")
                    //                                            {
                    //                                                yield return new WaitForSeconds(clip.length);
                    //                                            }
                    //                                        }
                    //                                    }
                    //                                    else if (CheckSpecificAnimationPlaying("Sit"))
                    //                                    {
                    //                                        break;
                    //                                    }
                    //                                }
                    //                                if (animationController != null)
                    //                                    animationController.SetBool("Stand", false);
                    //                                animationController.SetBool("EtcAnimStart", true);
                    //                                var overrideController = new AnimatorOverrideController();
                    //                                overrideController.runtimeAnimatorController = npcController;
                    //                                List<KeyValuePair<AnimationClip, AnimationClip>> keyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                    //                                foreach (var clip in overrideController.animationClips)
                    //                                {
                    //                                    if (animationController.GetBool("EtcAnimStart"))
                    //                                    {
                    //                                        if (clip.name == "crouchDefault" || clip.name == "standDefault")
                    //                                        {
                    //                                            keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, go.transform.GetChild(0).GetComponent<Animation>().clip));
                    //                                        }
                    //                                        else
                    //                                            keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
                    //                                        overrideController.ApplyOverrides(keyValuePairs);
                    //                                        animationController.runtimeAnimatorController = overrideController;
                    //                                    }
                    //                                    else
                    //                                    {
                    //                                        if (clip.name == "Sit")
                    //                                            keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, go.transform.GetComponent<Animation>().GetClip("Stand")));
                    //                                        else
                    //                                            keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
                    //                                        overrideController.ApplyOverrides(keyValuePairs);
                    //                                        animationController.runtimeAnimatorController = overrideController;
                    //                                    }
                    //                                    animationController.SetBool("IsEmote", false);
                    //                                }
                    //                            }
                    //                        }
                    //                        SaveAssetBundle(www.downloadHandler.data, emoteName);
                    //                        assetBundle.Unload(false);
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    _inValidAnimCount++;
                    //                    continue;
                    //                }
                    //            }
                    //        }
                    //        yield return new WaitForSeconds(AnimPlayTimer[i]);
                    //    }
                    //}
                    //else
                    //{
                    //    if (!string.IsNullOrEmpty(emoteBundleUrl))
                    //    {
                    //        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(emoteBundleUrl))
                    //        {
                    //            yield return www.SendWebRequest();
                    //            if (www.result != UnityWebRequest.Result.Success)
                    //            {
                    //                Debug.LogError("Got error in www request to get bundle: " + www.error);
                    //                _inValidAnimCount++;
                    //                continue;
                    //            }
                    //            else
                    //            {
                    //                AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(www);
                    //                if (assetBundle != null)
                    //                {
                    //                    AssetBundleRequest newRequest = assetBundle.LoadAllAssetsAsync<GameObject>();
                    //                    yield return newRequest;
                    //                    if (newRequest.isDone)
                    //                    {
                    //                        var animation = newRequest.allAssets;
                    //                        foreach (var anim in animation)
                    //                        {
                    //                            GameObject go = (GameObject)anim;
                    //                            if (go.name.Equals("Animation") || go.name.Equals("animation"))
                    //                            {
                    //                                if (animationController.GetBool("EtcAnimStart"))   // Added by Ali Hamza
                    //                                {
                    //                                    animationController.SetBool("Stand", true);
                    //                                    animationController.SetBool("EtcAnimStart", false);
                    //                                    foreach (var clip in animationController.runtimeAnimatorController.animationClips)
                    //                                    {
                    //                                        if (clip.name == "Stand")
                    //                                        {
                    //                                            yield return new WaitForSeconds(clip.length);
                    //                                        }
                    //                                    }
                    //                                }
                    //                                var overrideController = new AnimatorOverrideController();
                    //                                overrideController.runtimeAnimatorController = npcController;
                    //                                List<KeyValuePair<AnimationClip, AnimationClip>> keyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                    //                                foreach (var clip in overrideController.animationClips)
                    //                                {
                    //                                    if (clip.name == "emaotedefault")
                    //                                        keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, go.transform.GetComponent<Animation>().clip));
                    //                                    else
                    //                                        keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
                    //                                    overrideController.ApplyOverrides(keyValuePairs);
                    //                                    animationController.runtimeAnimatorController = overrideController;
                    //                                    animationController.SetBool("IsEmote", true);
                    //                                }
                    //                            }
                    //                            else
                    //                            {
                    //                                if (animationController != null)    //Added by Ali Hamza
                    //                                {
                    //                                    animationController.SetBool("Stand", true);
                    //                                    animationController.SetBool("EtcAnimStart", false);
                    //                                    if (CheckSpecificAnimationPlaying("Sit"))
                    //                                    {
                    //                                        foreach (var clip in animationController.runtimeAnimatorController.animationClips)
                    //                                        {
                    //                                            if (clip.name == "Stand")
                    //                                            {
                    //                                                yield return new WaitForSeconds(clip.length);
                    //                                            }
                    //                                        }
                    //                                    }
                    //                                    else if (CheckSpecificAnimationPlaying("Sit"))
                    //                                    {
                    //                                        break;
                    //                                    }
                    //                                }
                    //                                if (animationController != null)
                    //                                    animationController.SetBool("Stand", false);
                    //                                animationController.SetBool("EtcAnimStart", true);
                    //                                var overrideController = new AnimatorOverrideController();
                    //                                overrideController.runtimeAnimatorController = npcController;
                    //                                List<KeyValuePair<AnimationClip, AnimationClip>> keyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                    //                                foreach (var clip in overrideController.animationClips)
                    //                                {
                    //                                    if (animationController.GetBool("EtcAnimStart"))
                    //                                    {
                    //                                        if (clip.name == "crouchDefault" || clip.name == "standDefault")
                    //                                        {
                    //                                            keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, go.transform.GetChild(0).GetComponent<Animation>().clip));
                    //                                        }
                    //                                        else
                    //                                            keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
                    //                                        overrideController.ApplyOverrides(keyValuePairs);
                    //                                        animationController.runtimeAnimatorController = overrideController;
                    //                                    }
                    //                                    else
                    //                                    {
                    //                                        if (clip.name == "Sit")
                    //                                            keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, go.transform.GetComponent<Animation>().GetClip("Stand")));
                    //                                        else
                    //                                            keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
                    //                                        overrideController.ApplyOverrides(keyValuePairs);
                    //                                        animationController.runtimeAnimatorController = overrideController;
                    //                                    }
                    //                                    animationController.SetBool("IsEmote", false);
                    //                                }
                    //                            }
                    //                        }
                    //                        if (!AnimPlayList[i].Contains(","))
                    //                        {
                    //                            SaveAssetBundle(www.downloadHandler.data, emoteName);
                    //                        }
                    //                        assetBundle.Unload(false);
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    _inValidAnimCount++;
                    //                    continue;
                    //                }
                    //            }
                    //        }
                    //        yield return new WaitForSeconds(AnimPlayTimer[i]);
                    //    }
                    //    else
                    //    {
                    //        _inValidAnimCount++;
                    //        continue;
                    //    }
                    //}
                    #endregion
                }
                if (_inValidAnimCount >= AnimPlayList.Count)
                {
                    Debug.LogError("Provided Animation names are incorrect or Addressable keys not found");
                    KeepLoopingEmotes = false;
                }
            }
            if (!KeepLoopingEmotes)
            {
                spaAIBhvrController.isPerformingAction = false;
                if (spaAIBhvrController.ActionCoroutine != null)
                    StopCoroutine(spaAIBhvrController.ActionCoroutine);
                if (emotCoroutine != null)
                    StopCoroutine(emotCoroutine);
                CheckIfAnimationListUpdated();
                animationController.runtimeAnimatorController = npcController;
                yield return new WaitForSeconds(1f);
                spaAIBhvrController.ActionCoroutine = StartCoroutine(spaAIBhvrController.PerformAction());
            }
        }
    }

    private IEnumerator DownloadAddressableActionAnimation(string label)//, Action downloadCompleteCallBack
    {
        if (label != "" && Application.internetReachability != NetworkReachability.NotReachable)
        {
            AsyncOperationHandle<AnimationClip> loadOp;

            if (label.Contains("Breakdance") && !APIBasepointManager.instance.IsXanaLive)
            {
                int index = label.IndexOf('d');
                if (index != -1 && char.IsLower(label[index]))
                {
                    char[] charArray = label.ToCharArray();
                    charArray[index] = char.ToUpper(charArray[index]);
                    label = new string(charArray);
                }
            }

            loadOp = Addressables.LoadAssetAsync<AnimationClip>("Action_" + label);

            while (!loadOp.IsDone)
            {
                _inValidAnimCount++;
                //Debug.LogError("Animation addressable call failed to load animation" + label);
                yield break;
            }

            if (loadOp.IsValid() && loadOp.Result != null)
            {
                if (loadOp.Status == AsyncOperationStatus.Succeeded)
                {
                    if (loadOp.Result == null)
                    {
                        Debug.LogError("Network null.....");
                        _inValidAnimCount++;
                        yield break;
                    }
                    else
                    {
                        AddressableDownloader.bundleAsyncOperationHandle.Add(loadOp);
                        StartCoroutine(ApplyAnimationToAnimatorSet(loadOp.Result as AnimationClip));
                    }
                }
                else
                {
                    _inValidAnimCount++;
                    yield break;
                }
            }
            else
            {
                //Debug.LogError($"Failed to load asset: Action_{label}. Error: {loadOp.Status.ToString()}");
                Addressables.ClearDependencyCacheAsync("Action_" + label);
                Addressables.ReleaseInstance(loadOp);
                Addressables.Release(loadOp);
                _inValidAnimCount++;
                yield break;
            }
        }
        else
        {
            Debug.LogError("Network not reachable or key name is empty");
        }
    }

    private IEnumerator ApplyAnimationToAnimatorSet(AnimationClip animationClip)
    {
        Animator animator;
        animator = animationController;
        if (animator.GetBool("EtcAnimStart"))
        {
            animator.SetBool("Stand", true);
            animator.SetBool("EtcAnimStart", false);
        }
        yield return new WaitForSeconds(0f);
        var overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = npcController;

        List<KeyValuePair<AnimationClip, AnimationClip>> keyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var clip in overrideController.animationClips)
        {
            if (clip.name == "emaotedefault")
            {
                //Debug.Log("Setting Clip");
                keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, animationClip));
            }
            else
                keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
        }
        overrideController.ApplyOverrides(keyValuePairs);

        animator.runtimeAnimatorController = overrideController;
        CurrDanceAnimName = animationClip.name;
        animator.SetBool("IsEmote", true);
    }

    void CheckIfAnimationListUpdated()
    {
        for (int i = 0; i < AnimPlayList.Count; i++)
        {
            foreach (var locator in Addressables.ResourceLocators)
            {
                if (AnimPlayList[i].Contains("Breakdance") && !APIBasepointManager.instance.IsXanaLive)
                {
                    int index = AnimPlayList[i].IndexOf('d');
                    if (index != -1 && char.IsLower(AnimPlayList[i][index]))
                    {
                        char[] charArray = AnimPlayList[i].ToCharArray();
                        charArray[index] = char.ToUpper(charArray[index]);
                        AnimPlayList[i] = new string(charArray);
                    }
                }
                IList<IResourceLocation> locations;
                if (locator.Locate("Action_" + AnimPlayList[i], typeof(object), out locations))
                {
                    KeepLoopingEmotes = true;
                    break;
                }
            }
        }
    }
    
    bool SerachForEmoteWithName(string _emoteName)
    {
        int _emoteIndex = GameplayEntityLoader.instance.ActionEmoteSystem.EmoteManager.EmoteServerData.data.animationList.FindIndex(obj => obj.name == _emoteName);
        if (_emoteIndex != -1)
        {
            if (GameplayEntityLoader.instance.ActionEmoteSystem.EmoteManager.EmoteServerData.data.animationList[_emoteIndex].group.Contains("Dance") ||
                GameplayEntityLoader.instance.ActionEmoteSystem.EmoteManager.EmoteServerData.data.animationList[_emoteIndex].group.Contains("Moves") ||
                GameplayEntityLoader.instance.ActionEmoteSystem.EmoteManager.EmoteServerData.data.animationList[_emoteIndex].group.Contains("Idle") ||
                GameplayEntityLoader.instance.ActionEmoteSystem.EmoteManager.EmoteServerData.data.animationList[_emoteIndex].group.Contains("Etc") ||
                GameplayEntityLoader.instance.ActionEmoteSystem.EmoteManager.EmoteServerData.data.animationList[_emoteIndex].group.Contains("Walk"))
            {
                emoteName = GameplayEntityLoader.instance.ActionEmoteSystem.EmoteManager.EmoteServerData.data.animationList[_emoteIndex].name;
                CurrDanceAnimName = emoteName;
#if UNITY_ANDROID
                emoteBundleUrl = GameplayEntityLoader.instance.ActionEmoteSystem.EmoteManager.EmoteServerData.data.animationList[_emoteIndex].android_file;
#elif UNITY_IOS
            emoteBundleUrl = GameplayEntityLoader.instance.ActionEmoteSystem.EmoteManager.EmoteServerData.data.animationList[_emoteIndex].ios_file;
#elif UNITY_EDITOR
            emoteBundleUrl = GameplayEntityLoader.instance.ActionEmoteSystem.EmoteManager.EmoteServerData.data.animationList[_emoteIndex].android_file;
#endif
                emoteBundlePath = Path.Combine(ConstantsHolder.xanaConstants.r_EmoteStoragePersistentPath, emoteBundleUrl + ".unity3d");
                // Unload the previously loaded AssetBundle if any
                AssetBundle loadedBundle = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(bundle => bundle.name == emoteBundlePath);
                if (loadedBundle != null)
                {
                    loadedBundle.Unload(false);
                }
                return true;
            }
            else
            {
                emoteBundleUrl = "";
                return false;
            }
        }
        else if (!string.IsNullOrEmpty(_emoteName) && _emoteName.Contains(","))
        {
            string[] splitStrings = _emoteName.Split(',');
#if UNITY_ANDROID
            emoteBundleUrl = splitStrings[0];
#elif UNITY_IOS
        emoteBundleUrl = splitStrings[1];
#elif UNITY_EDITOR
        emoteBundleUrl = splitStrings[0];
#endif
            return false;
        }
        else
        {
            emoteBundleUrl = "";
            return false;
        }
    }

    public bool CheckForIsAssetBundleAvailable(string path)
    {
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator LoadAssetBundleFromStorage(string bundlePath, GameObject _currentGameObject)
    {
        // Unload the previously loaded AssetBundle if any
        AssetBundle loadedBundle = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(bundle => bundle.name == bundlePath);
        if (loadedBundle != null)
        {
            loadedBundle.Unload(false);
        }
        AssetBundleCreateRequest bundle = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return bundle;
        AssetBundle assetBundle = bundle.assetBundle;
        if (assetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }
        else
        {
            AssetBundleRequest newRequest = assetBundle.LoadAllAssetsAsync<GameObject>();
            while (!newRequest.isDone)
            {
                yield return null;
            }
            if (newRequest.isDone)
            {
                var animation = newRequest.allAssets;
                foreach (var anim in animation)
                {
                    GameObject go = (GameObject)anim;
                    if (go.name.Equals("Animation") || go.name.Equals("animation"))
                    {
                        if (animationController.GetBool("EtcAnimStart"))   // Added by Ali Hamza
                        {
                            animationController.SetBool("Stand", true);
                            animationController.SetBool("EtcAnimStart", false);
                            foreach (var clip in animationController.runtimeAnimatorController.animationClips)
                            {
                                if (clip.name == "Stand")
                                {
                                    yield return new WaitForSeconds(clip.length);
                                }
                            }
                        }
                        var overrideController = new AnimatorOverrideController();
                        overrideController.runtimeAnimatorController = npcController;
                        List<KeyValuePair<AnimationClip, AnimationClip>> keyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                        foreach (var clip in overrideController.animationClips)
                        {
                            if (clip.name == "emaotedefault")
                                keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, go.transform.GetComponent<Animation>().clip));
                            else
                                keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
                            overrideController.ApplyOverrides(keyValuePairs);
                            animationController.runtimeAnimatorController = overrideController;
                            animationController.SetBool("IsEmote", true);
                        }
                    }
                    else
                    {
                        if (animationController != null)    //Added by Ali Hamza
                        {
                            animationController.SetBool("Stand", true);
                            animationController.SetBool("EtcAnimStart", false);
                            if (CheckSpecificAnimationPlaying("Sit"))
                            {
                                foreach (var clip in animationController.runtimeAnimatorController.animationClips)
                                {
                                    if (clip.name == "Stand")
                                    {
                                        yield return new WaitForSeconds(clip.length);
                                    }
                                }
                            }
                            else if (CheckSpecificAnimationPlaying("Sit"))
                            {
                                break;
                            }
                        }
                        if (animationController != null)
                            animationController.SetBool("Stand", false);
                        animationController.SetBool("EtcAnimStart", true);
                        var overrideController = new AnimatorOverrideController();
                        overrideController.runtimeAnimatorController = npcController;
                        List<KeyValuePair<AnimationClip, AnimationClip>> keyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                        foreach (var clip in overrideController.animationClips)
                        {
                            if (animationController.GetBool("EtcAnimStart"))
                            {
                                if (clip.name == "crouchDefault" || clip.name == "standDefault")
                                {
                                    keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, go.transform.GetChild(0).GetComponent<Animation>().clip));
                                }
                                else
                                    keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
                                overrideController.ApplyOverrides(keyValuePairs);
                                animationController.runtimeAnimatorController = overrideController;
                            }
                            else
                            {
                                if (clip.name == "Sit")
                                    keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, go.transform.GetComponent<Animation>().GetClip("Stand")));
                                else
                                    keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
                                overrideController.ApplyOverrides(keyValuePairs);
                                animationController.runtimeAnimatorController = overrideController;
                            }
                            animationController.SetBool("IsEmote", false);
                        }
                    }
                }
            }
            if (assetBundle != null)
                assetBundle.Unload(false);
        }
        emotCoroutine = null;
    }

    bool CheckSpecificAnimationPlaying(string stateName)       //Added by Ali Hamza
    {
        return animationController.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    void SaveAssetBundle(byte[] data, string name)
    {
        //Create the Directory if it does not exist
        string path = Path.Combine(ConstantsHolder.xanaConstants.r_EmoteStoragePersistentPath, name + ".unity3d");
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
        try
        {
            File.WriteAllBytes(path, data);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }


    void StopAiEmote()
    {
        animationController.runtimeAnimatorController = npcController;
        animationController.SetBool("IsGrounded", true);
        animationController.SetBool("IsEmote", false);
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();
        spaAIBhvrController.isPerformingAction = false;

        if (spaAIBhvrController.ActionCoroutine != null)
            StopCoroutine(spaAIBhvrController.ActionCoroutine);

        spaAIBhvrController.ActionCoroutine = StartCoroutine(spaAIBhvrController.PerformAction());
    }

    public void ForceFullyStopEmote()
    {
        if (emotCoroutine != null)
            StopCoroutine(emotCoroutine);

        animationController.runtimeAnimatorController = npcController;
        animationController.SetBool("IsGrounded", true);
        animationController.SetBool("IsEmote", false);
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();
    }


}
