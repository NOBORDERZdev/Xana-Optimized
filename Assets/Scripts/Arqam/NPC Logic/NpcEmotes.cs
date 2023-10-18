using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace NPC {
    public class NpcEmotes : MonoBehaviour
    {
        public RuntimeAnimatorController npcController;
        [SerializeField] NpcBehaviourSelector npcBehaviour;
        [SerializeField] NpcMovementController npcMC;
        [SerializeField] Animator animationController;

        private int minAnimTime = 5;
        private int maxAnimTime = 20;
        private GameObject spawnCharacterObjectRemote;
        private Coroutine emotCoroutine;

        private void Awake()
        {
            spawnCharacterObjectRemote = EmoteAnimationPlay.Instance.spawnCharacterObjectRemote;
        }

        public IEnumerator PlayEmote()
        {
            if (!npcMC.isMoving)
            {
                AssetBundle.UnloadAllAssetBundles(false);
                Resources.UnloadUnusedAssets();
                int rand;
                rand = UnityEngine.Random.Range(0, (EmoteAnimationPlay.Instance.emoteAnim.Count > 10 ? 10 : EmoteAnimationPlay.Instance.emoteAnim.Count)); // EmoteAnimationPlay.Instance.emoteAnim.Count
                //Debug.Log("<color=red> rand: " + rand + "</color>");
                if (EmoteAnimationPlay.Instance.emoteAnim[rand].group.Contains("Dance") || EmoteAnimationPlay.Instance.emoteAnim[rand].group.Contains("Moves"))
                {
                    string BundleUrl;
                    string name = EmoteAnimationPlay.Instance.emoteAnim[rand].name;
#if UNITY_ANDROID
                    BundleUrl = EmoteAnimationPlay.Instance.emoteAnim[rand].android_file;
#elif UNITY_IOS
                    BundleUrl = EmoteAnimationPlay.Instance.emoteAnim[rand].ios_file;
#elif UNITY_EDITOR
                    BundleUrl = EmoteAnimationPlay.Instance.emoteAnim[rand].android_file;
#endif
                    string bundlePath = Path.Combine(XanaConstants.xanaConstants.r_EmoteStoragePersistentPath, BundleUrl + ".unity3d");
                    if (EmoteAnimationPlay.Instance.CheckForIsAssetBundleAvailable(Path.Combine(XanaConstants.xanaConstants.r_EmoteStoragePersistentPath, name + ".unity3d")))
                    {
                        emotCoroutine = StartCoroutine(LoadAssetBundleFromStorage(Path.Combine(XanaConstants.xanaConstants.r_EmoteStoragePersistentPath, name + ".unity3d")));
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
                                            Invoke(nameof(StopAiEmote), UnityEngine.Random.Range(minAnimTime, maxAnimTime));
                                        }
                                        SaveAssetBundle(www.bytes, name);
                                        assetBundle.Unload(false);
                                    }
                                }
                                catch (Exception)
                                {
                                    npcBehaviour.isPerformingAction = false;
                                    if (npcBehaviour.ActionCoroutine != null)
                                        StopCoroutine(npcBehaviour.ActionCoroutine);
                                    npcBehaviour.ActionCoroutine = StartCoroutine(npcBehaviour.PerformAction());
                                    throw;
                                }
                            }
                        }
                    }
                }
                else
                {
                    npcBehaviour.isPerformingAction = false;
                    if (npcBehaviour.ActionCoroutine != null)
                        StopCoroutine(npcBehaviour.ActionCoroutine);
                    npcBehaviour.ActionCoroutine = StartCoroutine(npcBehaviour.PerformAction());
                }
            }
        }

        IEnumerator LoadAssetBundleFromStorage(string bundlePath)
        {
            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();
            AssetBundleCreateRequest bundle = AssetBundle.LoadFromFileAsync(bundlePath);
            yield return bundle;
            AssetBundle assetBundle = bundle.assetBundle;
            if (assetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                npcBehaviour.isPerformingAction = false;
                if (npcBehaviour.ActionCoroutine != null)
                    StopCoroutine(npcBehaviour.ActionCoroutine);
                npcBehaviour.ActionCoroutine = StartCoroutine(npcBehaviour.PerformAction());
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
                Invoke(nameof(StopAiEmote), UnityEngine.Random.Range(minAnimTime, maxAnimTime));
                if (assetBundle != null)
                    assetBundle.Unload(false);
            }

            emotCoroutine = null;
        }

        void SaveAssetBundle(byte[] data, string name)
        {
            //Create the Directory if it does not exist
            string path = Path.Combine(XanaConstants.xanaConstants.r_EmoteStoragePersistentPath, name + ".unity3d");
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
            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();
            npcBehaviour.isPerformingAction = false;

            if (npcBehaviour.ActionCoroutine != null)
                StopCoroutine(npcBehaviour.ActionCoroutine);

            npcBehaviour.ActionCoroutine = StartCoroutine(npcBehaviour.PerformAction());
        }

        public void ForceFullyStopEmote()
        {
            if (emotCoroutine != null)
                StopCoroutine(emotCoroutine);

            animationController.runtimeAnimatorController = npcController;
            animationController.SetBool("IsGrounded", true);
            animationController.SetBool("IsEmote", false);
            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();
        }


    }
}
