using NPC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using VLB_Samples;

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
   // GameObject spawnCharacterObjectRemote;
    Coroutine emotCoroutine;
    string emoteName;
    string emoteBundleUrl;
    string emoteBundlePath;
    //private int minAnimTime = 5;
    //private int maxAnimTime = 20;

  //  private void Awake()
  //  {
       // spawnCharacterObjectRemote = EmoteAnimationHandler.Instance.spawnCharacterObjectRemote;
   // }
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
    public IEnumerator PlayEmote()
    {
        if (!npcMC.isMoving)
        {
            while (KeepLoopingEmotes)
            {
                int _inValidAnimCount = 0;
                for (int i = 0; i < AnimPlayList.Count; i++)
                {
                    if (!KeepLoopingEmotes)
                    {
                        break;
                    }
                    if (SerachForEmoteWithName(AnimPlayList[i]))
                    {
                        if (CheckForIsAssetBundleAvailable(Path.Combine(ConstantsHolder.xanaConstants.r_EmoteStoragePersistentPath, emoteName + ".unity3d")))
                        {
                            emotCoroutine = StartCoroutine(LoadAssetBundleFromStorage(Path.Combine(ConstantsHolder.xanaConstants.r_EmoteStoragePersistentPath, emoteName + ".unity3d")));
                            yield return new WaitForSeconds(AnimPlayTimer[i]);
                        }
                        else
                        {
                            using (WWW www = new WWW(emoteBundleUrl))
                            {
                                while (!www.isDone)
                                {
                                    yield return null;
                                }
                                yield return www;
                                if (www.error != null)
                                {
                                    Debug.LogError("Got error in www request to get bundle");
                                    _inValidAnimCount++;
                                    www.Dispose();
                                    continue;
                                }
                                else
                                {
                                    AssetBundle assetBundle = www.assetBundle;
                                    if (assetBundle != null)
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
                                                    //PlayerAvatar.GetComponent<Animator>().runtimeAnimatorController = go.GetComponent<Animator>().runtimeAnimatorController;
                                                    //PlayerAvatar.GetComponent<Animator>().Play("Animation");

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

                                                        if (CheckSpecificAnimationPlaying("Sit") /*&& animationController.GetComponent<RpcManager>().DifferentAnimClicked*/)
                                                        {
                                                            //animationController.GetComponent<RpcManager>().DifferentAnimClicked = false;
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
                                                                //currentEtcAnimName = go.name;
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
                                            SaveAssetBundle(www.bytes, emoteName);
                                            assetBundle.Unload(false);
                                        }
                                    }
                                    else
                                    {
                                        _inValidAnimCount++;
                                        www.Dispose();
                                        continue;
                                    }
                                }

                                www.Dispose();
                            }
                            yield return new WaitForSeconds(AnimPlayTimer[i]);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(emoteBundleUrl))
                        {
                            using (WWW www = new WWW(emoteBundleUrl))
                            {
                                while (!www.isDone)
                                {
                                    yield return null;
                                }
                                yield return www;
                                if (www.error != null)
                                {
                                    Debug.LogError("Got error in www request to get bundle");
                                    _inValidAnimCount++;
                                    www.Dispose();
                                    continue;
                                }
                                else
                                {
                                    AssetBundle assetBundle = www.assetBundle;
                                    if (assetBundle != null)
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
                                                    //PlayerAvatar.GetComponent<Animator>().runtimeAnimatorController = go.GetComponent<Animator>().runtimeAnimatorController;
                                                    //PlayerAvatar.GetComponent<Animator>().Play("Animation");

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

                                                        if (CheckSpecificAnimationPlaying("Sit") /*&& animationController.GetComponent<RpcManager>().DifferentAnimClicked*/)
                                                        {
                                                            //animationController.GetComponent<RpcManager>().DifferentAnimClicked = false;
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
                                                                //currentEtcAnimName = go.name;
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
                                            if (!AnimPlayList[i].Contains(","))
                                            {
                                                SaveAssetBundle(www.bytes, emoteName);
                                            }
                                            assetBundle.Unload(false);
                                        }
                                    }
                                    else
                                    {
                                        _inValidAnimCount++;
                                        www.Dispose();
                                        continue;
                                    }
                                }

                                www.Dispose();
                            }
                            yield return new WaitForSeconds(AnimPlayTimer[i]);
                        }
                        else
                        {
                            _inValidAnimCount++;
                            continue;
                        }
                    }
                }
                if (_inValidAnimCount >= AnimPlayList.Count)
                {
                    Debug.LogError("Provided Animation names are incorrect or not found in emotes list");
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

    IEnumerator LoadAssetBundleFromStorage(string bundlePath)
    {
        AssetBundleCreateRequest bundle = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return bundle;
        AssetBundle assetBundle = bundle.assetBundle;
        if (assetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            //spaAIBhvrController.isPerformingAction = false;
            //if (spaAIBhvrController.ActionCoroutine != null)
            //    StopCoroutine(spaAIBhvrController.ActionCoroutine);
            //spaAIBhvrController.ActionCoroutine = StartCoroutine(spaAIBhvrController.PerformAction());
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
                        //PlayerAvatar.GetComponent<Animator>().runtimeAnimatorController = go.GetComponent<Animator>().runtimeAnimatorController;
                        //PlayerAvatar.GetComponent<Animator>().Play("Animation");

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

                            if (CheckSpecificAnimationPlaying("Sit") /*&& animationController.GetComponent<RpcManager>().DifferentAnimClicked*/)
                            {
                                //animationController.GetComponent<RpcManager>().DifferentAnimClicked = false;
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
                                    //currentEtcAnimName = go.name;
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
            //Invoke(nameof(StopAiEmote), /*UnityEngine.Random.Range(minAnimTime, maxAnimTime)*/SingleAnimPlayTime);
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

    void CheckIfAnimationListUpdated()
    {
        for (int i = 0; i < AnimPlayList.Count; i++)
        {
            if (SerachForEmoteWithName(AnimPlayList[i]))
            {
                KeepLoopingEmotes = true;
                break;
            }
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
            string[] splitStrings = new string[2];
            splitStrings = _emoteName.Split(',');
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
