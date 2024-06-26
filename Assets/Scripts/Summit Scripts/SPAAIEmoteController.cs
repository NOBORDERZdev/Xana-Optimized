using NPC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SPAAIEmoteController : MonoBehaviour
{
    public RuntimeAnimatorController npcController;
    public float SingleAnimPlayTime = 5f;
    public string CurrDanceAnimName;
    public List<string> AnimPlayList = new List<string>();
    public List<float> AnimPlayTimer = new List<float>();
    public bool KeepLoopingEmotes = true;

    [SerializeField] SPAAIBehvrController spaAIBhvrController;
    [SerializeField] NpcMovementController npcMC;
    [SerializeField] Animator animationController;
    private GameObject spawnCharacterObjectRemote;
    private Coroutine emotCoroutine;
    string emoteName;
    string emoteBundleUrl;
    string emoteBundlePath;
    //private int minAnimTime = 5;
    //private int maxAnimTime = 20;

    private void Awake()
    {
        spawnCharacterObjectRemote = EmoteAnimationHandler.Instance.spawnCharacterObjectRemote;
    }

    public IEnumerator PlayEmote()
    {
        if (!npcMC.isMoving)
        {
            while (KeepLoopingEmotes)
            {
                for (int i = 0; i < AnimPlayList.Count; i++)
                {
                    if (!KeepLoopingEmotes)
                    {
                        break;
                    }
                    if (SerachForEmoteWithName(AnimPlayList[i]))
                    {
                        if (EmoteAnimationHandler.Instance.CheckForIsAssetBundleAvailable(Path.Combine(ConstantsHolder.xanaConstants.r_EmoteStoragePersistentPath, emoteName + ".unity3d")))
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
                                    throw new Exception("WWW download had an error:" + www.error);
                                }
                                else
                                {
                                    try
                                    {
                                        AssetBundle assetBundle = www.assetBundle;
                                        if (assetBundle != null)
                                        {
                                            GameObject[] animation = assetBundle.LoadAllAssets<GameObject>();
                                            var remotego = animation[0];

                                            if (remotego.name.Equals("Animation"))
                                            {
                                                spawnCharacterObjectRemote = remotego.transform.gameObject;
                                                var overrideController = new AnimatorOverrideController();
                                                overrideController.runtimeAnimatorController = npcController;
                                                List<KeyValuePair<AnimationClip, AnimationClip>> keyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                                                foreach (var clip in overrideController.animationClips)
                                                {
                                                    if (clip.name == "emaotedefault")
                                                        keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, spawnCharacterObjectRemote.transform.GetComponent<Animation>().clip));
                                                    else
                                                        keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
                                                }
                                                overrideController.ApplyOverrides(keyValuePairs);
                                                animationController.runtimeAnimatorController = overrideController;
                                                animationController.SetBool("IsEmote", true);
                                                //Invoke(nameof(StopAiEmote), /*UnityEngine.Random.Range(minAnimTime, maxAnimTime)*/SingleAnimPlayTime);
                                            }
                                            SaveAssetBundle(www.bytes, emoteName);
                                            assetBundle.Unload(false);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        //continue;
                                        throw;
                                    }
                                }

                                www.Dispose();
                            }
                            yield return new WaitForSeconds(AnimPlayTimer[i]);
                        }
                    }
                    else
                    {
                        Debug.LogError("Skipped animation as not found in emotes list: " + AnimPlayList[i]);
                        continue;
                    }
                    if (i >= AnimPlayList.Count)
                    {
                        i = 0;
                    }
                }
            }
            if (!KeepLoopingEmotes)
            {
                spaAIBhvrController.isPerformingAction = false;
                if (spaAIBhvrController.ActionCoroutine != null)
                    StopCoroutine(spaAIBhvrController.ActionCoroutine);
                if (emotCoroutine != null)
                    StopCoroutine(emotCoroutine);

                animationController.runtimeAnimatorController = npcController;
                yield return new WaitForSeconds(1f);
                spaAIBhvrController.ActionCoroutine = StartCoroutine(spaAIBhvrController.PerformAction());
            }
            //AssetBundle.UnloadAllAssetBundles(false);
            //Resources.UnloadUnusedAssets();
            //int rand;
            //rand = UnityEngine.Random.Range(0, (EmoteAnimationHandler.Instance.emoteAnim.Count > 10 ? 10 : EmoteAnimationHandler.Instance.emoteAnim.Count)); // EmoteAnimationHandler.Instance.emoteAnim.Count
                                                                                                                                                             //Debug.Log("<color=red> rand: " + rand + "</color>");
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
                spaAIBhvrController.isPerformingAction = false;
                if (spaAIBhvrController.ActionCoroutine != null)
                    StopCoroutine(spaAIBhvrController.ActionCoroutine);
                spaAIBhvrController.ActionCoroutine = StartCoroutine(spaAIBhvrController.PerformAction());
                yield break;
        }
        else
        {
            AssetBundleRequest newRequest = assetBundle.LoadAllAssetsAsync<GameObject>();
            while (!newRequest.isDone)
            {
                yield return null;
            }

            var animation = newRequest.allAssets;
            foreach (var anim in animation)
            {
                GameObject go = (GameObject)anim;
                if (go.name.Equals("Animation") || go.name.Equals("animation"))
                {
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
                    var overrideController = new AnimatorOverrideController();
                    overrideController.runtimeAnimatorController = npcController;
                }
            }
            //Invoke(nameof(StopAiEmote), /*UnityEngine.Random.Range(minAnimTime, maxAnimTime)*/SingleAnimPlayTime);
            if (assetBundle != null)
                assetBundle.Unload(false);
        }
        emotCoroutine = null;
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

    bool SerachForEmoteWithName(string _emoteName)
    {
        int _emoteIndex = EmoteAnimationHandler.Instance.emoteAnim.FindIndex(obj => obj.name == _emoteName);
        if (_emoteIndex != -1)
        {
            if (EmoteAnimationHandler.Instance.emoteAnim[_emoteIndex].group.Contains("Dance") || EmoteAnimationHandler.Instance.emoteAnim[_emoteIndex].group.Contains("Moves"))
            {
                emoteName = EmoteAnimationHandler.Instance.emoteAnim[_emoteIndex].name;
                CurrDanceAnimName = emoteName;
#if UNITY_ANDROID
                emoteBundleUrl = EmoteAnimationHandler.Instance.emoteAnim[_emoteIndex].android_file;
#elif UNITY_IOS
                    emoteBundleUrl = EmoteAnimationHandler.Instance.emoteAnim[_emoteIndex].ios_file;
#elif UNITY_EDITOR
                    emoteBundleUrl = EmoteAnimationHandler.Instance.emoteAnim[_emoteIndex].android_file;
#endif
                emoteBundlePath = Path.Combine(ConstantsHolder.xanaConstants.r_EmoteStoragePersistentPath, emoteBundleUrl + ".unity3d");
            }
            return true;
        }
        else
        {
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
