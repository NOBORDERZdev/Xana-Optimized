using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Collections.Generic;
using static CanvasGroupFade;

public class MoodManager : MonoBehaviour
{
   // public Animator PlayerAnimator;
  //  [SerializeField]
  //  AnimatorOverrideController overrideController;
    public bool PostMood = false;
    public string LastMoodSelected = "";
    public void ViewMoodActionAnimation(string animkey,string originalKey, AnimatorOverrideController overrideController, Animator _anim)
    {
        PostMood = true;
        LastMoodSelected = originalKey;
        StartCoroutine(DownloadAddressableAnimation(animkey,  overrideController, _anim));
    }
    public IEnumerator DownloadAddressableAnimation(string key, AnimatorOverrideController overrideController, Animator _anim)
    {
        if (key != "" && Application.internetReachability != NetworkReachability.NotReachable)
        {
            AsyncOperationHandle<AnimationClip> loadOp;
            // bool flag = false;
            //  loadOp = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(key, ref flag);
            //  if (!flag)
            //      loadOp =
            loadOp = Addressables.LoadAssetAsync<AnimationClip>(key/*"Assets/Animations/Mood Animations/"+ key +".anim"*/);
            LoadingHandler.Instance.worldLoadingScreen.SetActive(true);
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
                    var clips = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                    overrideController.GetOverrides(clips);
                   
                    for (int i = 0; i < clips.Count; i++)
                    {
                        var stateName = clips[i].Key.name;
                      //  Debug.LogError("Tag ----> " + stateName.ToString());
                        if (stateName.Contains("Idle"))
                        {
                            clips[i] = new KeyValuePair<AnimationClip, AnimationClip>(clips[i].Key, loadOp.Result);
                            yield return new WaitForSeconds(Time.deltaTime);
                           // _anim.SetBool("IdleMenu", true);
                            break;
                        }
                    }
                    overrideController.ApplyOverrides(clips);
                    _anim.runtimeAnimatorController = overrideController;
                }
            }
            LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
        }
    }
    public void SetMoodPosted(string animkey, bool walkFlag,AnimatorOverrideController overrideController, Animator _anim)
    {
       // Debug.LogError("DownloadAddressableTexture === " + "Assets/Animations/Mood Animations/" + animkey);

        int NoOfAnimations = GameManager.Instance.ActorManager.GetNumberofIdleAnimations(animkey);
        if(NoOfAnimations == 1)
            StartCoroutine(DownloadAddressableAnimationToAnimator(animkey + " Idle" , "Idle", overrideController, _anim));
        else
            StartCoroutine(DownloadAddressableAnimationToAnimator(animkey + " Idle " + UnityEngine.Random.Range(1, 3), "Idle", overrideController, _anim));
       if(!walkFlag)
            StartCoroutine(DownloadAddressableAnimationToAnimator(animkey + " Walk", "Walk", overrideController, _anim));
    }
    public IEnumerator DownloadAddressableAnimationToAnimator(string key, string NodeAnimToReplace, AnimatorOverrideController overrideController, Animator _anim)
    {
       // Debug.LogError("DownloadAddressableTexture === " + "Assets/Animations/Mood Animations/" + key);
        if (key != "" && Application.internetReachability != NetworkReachability.NotReachable)
        {
            AsyncOperationHandle<AnimationClip> loadOp;
            // bool flag = false;
            //  loadOp = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(key, ref flag);
            //  if (!flag)
            //      loadOp =
            loadOp = Addressables.LoadAssetAsync<AnimationClip>( key/*"Assets/Animations/Mood Animations/" + key + ".anim"*/);
            while (!loadOp.IsDone)
                yield return loadOp;

            if (loadOp.Status == AsyncOperationStatus.Succeeded)
            {
                if (loadOp.Result == null || loadOp.Result.Equals(null))   // Added by Ali Hamza to resolve avatar naked issue
                {
                  //  Debug.LogError("NULLLLLL");
                }
                else
                {
                    OverridesAnimatorController(NodeAnimToReplace, overrideController, _anim, loadOp.Result);
                }
            }
            else
            {
                Debug.LogError("Get Animation Failed ----> ");
            }
        }
    }

    private void OverridesAnimatorController(string nodeAnimToReplace, AnimatorOverrideController overrideController, Animator anim, AnimationClip newClip)
    {
        if (overrideController == null)
        {
           // Debug.LogError("Override controller not set.");
            return;
        }

        if (newClip == null)
        {
           // Debug.LogError("New animation clip not set.");
            return;
        }

        // Get current overrides
        var clips = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(clips);


        // Iterate through the clips to find and replace the specified animation clip
        for (int i = 0; i < clips.Count; i++)
        {
            var stateName = clips[i].Key.name;
            if (stateName.Contains(nodeAnimToReplace))
            {
                clips[i] = new KeyValuePair<AnimationClip, AnimationClip>(clips[i].Key, newClip);

                if (nodeAnimToReplace == "Idle")
                {
                    anim.transform.GetComponent<Actor>().ActionClipTime = newClip.length;
                }

                StartCoroutine(ResetMenuActionBool(anim));
                break;
            }
        }

        // Apply the updated overrides back to the override controller
        overrideController.ApplyOverrides(clips);

        // Assign the override controller to the animator
        anim.runtimeAnimatorController = overrideController;

       // Debug.Log($"Replaced {nodeAnimToReplace} with {newClip.name}");
    }

    private IEnumerator ResetMenuActionBool(Animator _anim)
    {
        yield return new WaitForSeconds(Time.deltaTime);
        _anim.SetBool("Menu Action", false);
    }
}
