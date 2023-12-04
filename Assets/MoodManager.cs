using System.Buffers;
using System;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using UnityEngine.InputSystem;

public class MoodManager : MonoBehaviour
{
    public AnimationClip Idle, Move;
    public Animator PlayerAnimator;
    private void OnEnable()
    {
       /* AnimatorOverrideController overrideController = new AnimatorOverrideController(PlayerAnimator.runtimeAnimatorController);
        PlayerAnimator.runtimeAnimatorController = overrideController;
        if(overrideController["Idle"])
        {
            Debug.LogError("Yessss");
        }
        else
        {
            Debug.LogError("Noooo");
        }
        if (overrideController["Move"])
        {
            Debug.LogError("Yessss");
        }
        else
        {
            Debug.LogError("Noooo");
        }
        overrideController["Idle"] = Idle;
        overrideController["Move"] = Move;


        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(overrides);
        ;
        Debug.LogError("NameOfAnimation Node === " + overrideController.runtimeAnimatorController.animationClips[0].name);



        // Replace the existing clip with a new one
        for (int i = 0; i < overrides.Count; i++)
        {
            Debug.LogError("NameOfAnimationToReplace 1 === " + overrides[i].Key.ToString());
            Debug.LogError("NameOfAnimationToReplace 2 === " + overrides[i].Key);

        }*/

        var controller = PlayerAnimator.runtimeAnimatorController as AnimatorController;
        foreach (var layer in controller.layers)
        {
            var stateMachine = layer.stateMachine;
            foreach (var state in stateMachine.states)
            {
                Debug.LogError("NameOfAnimation Node === " + state.state.tag);
                if(state.state.tag.Equals("Move"))
                    state.state.motion = Move;
                else if (state.state.tag.Equals("Action"))
                    state.state.motion = Idle;
            }
        }
    }
    public bool PostMood = false;
    public string LastMoodSelected = "";
    public void ViewMoodActionAnimation(string animkey,string originalKey)
    {
        /* var controller = PlayerAnimator.runtimeAnimatorController as AnimatorController;
         foreach (var layer in controller.layers)
         {
             var stateMachine = layer.stateMachine;
             foreach (var state in stateMachine.states)
             {
                 Debug.LogError("NameOfAnimation Node === " + state.state.tag);
                 if (state.state.tag.Equals("Menu Action"))
                     state.state.motion = Move;

             }
         }*/
        Debug.LogError("DownloadAddressableTexture === " + animkey);
        PostMood = true;
        LastMoodSelected = originalKey;
        StartCoroutine(DownloadAddressableAnimation(animkey));
    }
    public void SetMoodPosted(string animkey)
    {
        int NoOfAnimations = GameManager.Instance.ActorManager.GetNumberofIdleAnimations(animkey);
        if(NoOfAnimations == 1)
            StartCoroutine(DownloadAddressableAnimationToAnimator(animkey + " Idle" , "Action"));
        else
            StartCoroutine(DownloadAddressableAnimationToAnimator(animkey + " Idle " + UnityEngine.Random.Range(1, 3), "Action"));
        StartCoroutine(DownloadAddressableAnimationToAnimator(animkey + " Walk", "Move"));
    }
    public IEnumerator DownloadAddressableAnimation(string key)
    {
        Debug.LogError("DownloadAddressableTexture === " + "Assets/Animations/Mood Animations/" + key);

        if (key != "" && Application.internetReachability != NetworkReachability.NotReachable)
        {

            AsyncOperationHandle<AnimationClip> loadOp;
            // bool flag = false;

            //  loadOp = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(key, ref flag);
            //  if (!flag)
            //      loadOp =
            loadOp = Addressables.LoadAssetAsync<AnimationClip>("Assets/Animations/Mood Animations/"+ key +".anim");

            while (!loadOp.IsDone)
                yield return loadOp;

            if (loadOp.Status == AsyncOperationStatus.Succeeded)
            {
                if (loadOp.Result == null || loadOp.Result.Equals(null))   // Added by Ali Hamza to resolve avatar naked issue
                {
                    Debug.LogError("NULLLLLL");
                }
                else
                {
                    Debug.LogError("FOUND ----->");
                    var controller = PlayerAnimator.runtimeAnimatorController as AnimatorController;
                    foreach (var layer in controller.layers)
                    {
                        var stateMachine = layer.stateMachine;
                        foreach (var state in stateMachine.states)
                        {
                            Debug.LogError("NameOfAnimation Node === " + state.state.tag);
                            if (state.state.tag.Equals("Menu Action"))
                            {
                                state.state.motion = loadOp.Result;
                                yield return new WaitForSeconds(Time.deltaTime);
                                PlayerAnimator.SetBool("Menu Action", true);
                                PlayerAnimator.SetBool("IdleMenu", true);
                                break;
                            }
                        }
                    }
                }

            }
        }
    }
    public IEnumerator DownloadAddressableAnimationToAnimator(string key, string NodeAnimToReplace)
    {
        Debug.LogError("DownloadAddressableTexture === " + "Assets/Animations/Mood Animations/" + key);

        if (key != "" && Application.internetReachability != NetworkReachability.NotReachable)
        {

            AsyncOperationHandle<AnimationClip> loadOp;
            // bool flag = false;

            //  loadOp = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(key, ref flag);
            //  if (!flag)
            //      loadOp =
            loadOp = Addressables.LoadAssetAsync<AnimationClip>("Assets/Animations/Mood Animations/" + key + ".anim");
            while (!loadOp.IsDone)
                yield return loadOp;

            if (loadOp.Status == AsyncOperationStatus.Succeeded)
            {
                if (loadOp.Result == null || loadOp.Result.Equals(null))   // Added by Ali Hamza to resolve avatar naked issue
                {
                    Debug.LogError("NULLLLLL");
                }
                else
                {
                    Debug.LogError("FOUND ----->");
                    var controller = PlayerAnimator.runtimeAnimatorController as AnimatorController;
                    foreach (var layer in controller.layers)
                    {
                        var stateMachine = layer.stateMachine;
                        foreach (var state in stateMachine.states)
                        {
                            Debug.LogError("NameOfAnimation Node === " + state.state.tag);
                            if (state.state.tag.Equals(NodeAnimToReplace))
                            {
                                state.state.motion = loadOp.Result;
                                if(NodeAnimToReplace == "Action")
                                {
                                    Debug.LogError("Length Of Clip "+loadOp.Result.length);
                                  GameManager.Instance.mainCharacter.GetComponent<Actor>().ActionClipTime = loadOp.Result.length;
                                }
                                yield return new WaitForSeconds(Time.deltaTime);
                                PlayerAnimator.SetBool("Menu Action", false);
                                break;
                            }
                        }
                    }
                }

            }
        }
    }
}
