using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XanaAi;
using static EmoteFilterManager;
using UnityEngine.AI;

namespace XanaAi
{
    public class AiEmote : MonoBehaviour
    {
        [SerializeField]AiController ai;
        [SerializeField] Animator animationController;
        public RuntimeAnimatorController controller;
        GameObject spawnCharacterObjectRemote;
        [SerializeField] int minAnimTime;
        [SerializeField] int maxAnimTime;
        [SerializeField] WanderingAI wandering;
        [SerializeField] NavMeshAgent agent;
        //[SerializeField] List<string> EmoteUrl;
        private void Awake()
        {
           // ai = GetComponent<AiController>();
            //animationController = GetComponent<Animator>();
            //wandering = GetComponent<WanderingAI>();
            controller = EmoteAnimationPlay.Instance.controller;
            spawnCharacterObjectRemote = EmoteAnimationPlay.Instance.spawnCharacterObjectRemote;
        }

        public IEnumerator PlayEmote()
        {
            if (!wandering.isMoving)
            {
                AssetBundle.UnloadAllAssetBundles(false);
                Resources.UnloadUnusedAssets();
                int rand;
                rand= UnityEngine.Random.Range(0, EmoteAnimationPlay.Instance.emoteAnim.Count);
                //print("PLAYING EMOTE WITH RNAD "+ rand);
                if (EmoteAnimationPlay.Instance.emoteAnim[rand].group.Contains("Dance") ||EmoteAnimationPlay.Instance.emoteAnim[rand].group.Contains("Moves"))
                {
                    string BundleUrl ;
                    string name = EmoteAnimationPlay.Instance.emoteAnim[rand].name;
                #if UNITY_ANDROID
                    BundleUrl = /*"https://cdn.xana.net/apitestxana/Defaults/1647854961406_animation.android"*/ EmoteAnimationPlay.Instance.emoteAnim[rand].android_file;
                #elif UNITY_IOS
                    BundleUrl = EmoteAnimationPlay.Instance.emoteAnim[rand].ios_file;
                #elif UNITY_EDITOR
                    BundleUrl = EmoteAnimationPlay.Instance.emoteAnim[rand].android_file;
                #endif
                    string bundlePath = Path.Combine(XanaConstants.xanaConstants.r_EmoteStoragePersistentPath, BundleUrl + ".unity3d");
                    if (EmoteAnimationPlay.Instance.CheckForIsAssetBundleAvailable(Path.Combine(XanaConstants.xanaConstants.r_EmoteStoragePersistentPath, name + ".unity3d"))){
                        StartCoroutine(LoadAssetBundleFromStorage(Path.Combine(XanaConstants.xanaConstants.r_EmoteStoragePersistentPath, name + ".unity3d")));
                    }
                    else
                    {
                        using (WWW www = new WWW(BundleUrl))
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
                                            overrideController.runtimeAnimatorController = controller;
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
                                            Invoke(nameof(StopAiEmote), UnityEngine.Random.Range(minAnimTime, maxAnimTime));
                                        }
                                        SaveAssetBundle(www.bytes, name);
                                        assetBundle.Unload(false);
                                    }
                                }
                                catch (Exception)
                                {
                                    print(" EXCEPTION OCCUR ");
                                    ai.isPerformingAction = false;
                                    if (ai.ActionCoroutine !=null)
                                    {
                                        ai.StopCoroutine(ai.ActionCoroutine);
                                    }
                                    ai.ActionCoroutine =  ai.StartCoroutine( ai.PerformAction());
                                    throw;
                                }
                            }
                        }
                    }
                }
                else
                {
                    ai.isPerformingAction = false;
                    if (ai.ActionCoroutine !=null)
                    {
                        ai.StopCoroutine(ai.ActionCoroutine);
                    }
                    ai.ActionCoroutine =  ai.StartCoroutine( ai.PerformAction());
                }
            }
        }

        IEnumerator LoadAssetBundleFromStorage(string bundlePath){
            
            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();
            AssetBundleCreateRequest bundle = AssetBundle.LoadFromFileAsync(bundlePath);
            yield return bundle;
            AssetBundle assetBundle = bundle.assetBundle;
            if (assetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                 ai.isPerformingAction = false;
                if (ai.ActionCoroutine !=null)
                {
                    ai.StopCoroutine(ai.ActionCoroutine);
                }
                ai.ActionCoroutine =  ai.StartCoroutine( ai.PerformAction());
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

                    //if (currentAnimationTab.Equals("Etc"))
                    //{
                    //    if (animatorremote.GetBool("EtcAnimStart"))
                    //    {
                    //        animatorremote.SetBool("EtcAnimStart", false);
                    //        //animatorremote.SetBool("moveToNextAnim", true);
                    //    }
                    //    else
                    //    {
                    //        //animatorremote.SetBool("moveToNextAnim", false);
                    //        animatorremote.SetBool("EtcAnimStart", true);
                    //        animatorremote.SetBool("Stand", false);
                    //    }
                }
                var animation = newRequest.allAssets;
                foreach (var anim in animation)
                {
                    GameObject go = (GameObject)anim;
                    if (go.name.Equals("Animation") || go.name.Equals("animation"))
                    {
                        var overrideController = new AnimatorOverrideController();
                        overrideController.runtimeAnimatorController = controller;
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
                        overrideController.runtimeAnimatorController = controller;
                    }
                }
                Invoke(nameof(StopAiEmote), UnityEngine.Random.Range(minAnimTime, maxAnimTime));
                if(assetBundle != null)
                    assetBundle.Unload(false);
            }
         }

        void SaveAssetBundle(byte[] data, string name ){
        //Create the Directory if it does not exist
        string path = Path.Combine(XanaConstants.xanaConstants.r_EmoteStoragePersistentPath, name + ".unity3d");
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        try
        {
            File.WriteAllBytes(path, data);
            //if (id.GetComponent<PhotonView>().IsMine)
            //{
            //    MyAnimLoader = false;
            //    isFetchingAnim = false;
            //    isAnimRunning = true;

            //    if (AnimObject != null)
            //    {
            //        AnimObject.transform.GetChild(3).gameObject.SetActive(false);
            //    }
            //    AnimationStarted?.Invoke(remoteUrlAnimationName);
            //}
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
        }

        void StopAiEmote(){
            animationController.runtimeAnimatorController = controller;
            animationController.SetBool("IsGrounded", true);
            animationController.SetBool("IsEmote",false); 
            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();
            ai.isPerformingAction = false;
            if (ai.ActionCoroutine !=null)
            {
                ai.StopCoroutine(ai.ActionCoroutine);
            }
            ai.ActionCoroutine =  ai.StartCoroutine( ai.PerformAction());
        }

        public void ForceFullyStopEmote(){ 
                        
            animationController.runtimeAnimatorController = controller;
            animationController.SetBool("IsGrounded", true);
            animationController.SetBool("IsEmote",false); 
            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();
        }

    }




}
