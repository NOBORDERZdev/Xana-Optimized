using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Collections.Generic;

public class MoodManager : MonoBehaviour
{
    public AnimationClip Idle, Move;
    public Animator PlayerAnimator;
    [SerializeField]
    AnimatorOverrideController overrideController;
    private void Start()
    {
       // overrideController = new AnimatorOverrideController(PlayerAnimator.runtimeAnimatorController);
      //  PlayerAnimator.runtimeAnimatorController = overrideController;

    }
    public bool PostMood = false;
    public string LastMoodSelected = "";
    public void ViewMoodActionAnimation(string animkey,string originalKey)
    {
        PostMood = true;
        LastMoodSelected = originalKey;
        StartCoroutine(DownloadAddressableAnimation(animkey));
    }
    public IEnumerator DownloadAddressableAnimation(string key)
    {
       // Debug.LogError("DownloadAddressableTexture === " + "Assets/Animations/Mood Animations/" + key);

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
                        Debug.LogError("Tag ----> " + stateName.ToString());
                        if (stateName.Contains("Idle"))
                        {
                            clips[i] = new KeyValuePair<AnimationClip, AnimationClip>(clips[i].Key, loadOp.Result);
                            yield return new WaitForSeconds(Time.deltaTime);
                            PlayerAnimator.SetBool("Menu Action", true);
                            PlayerAnimator.SetBool("IdleMenu", true);
                            break;
                        }
                    }
                    overrideController.ApplyOverrides(clips);
                }

            }
        }
    }
    public void SetMoodPosted(string animkey, bool walkFlag)
    {
        int NoOfAnimations = GameManager.Instance.ActorManager.GetNumberofIdleAnimations(animkey);
        if(NoOfAnimations == 1)
            StartCoroutine(DownloadAddressableAnimationToAnimator(animkey + " Idle" , "Idle"));
        else
            StartCoroutine(DownloadAddressableAnimationToAnimator(animkey + " Idle " + UnityEngine.Random.Range(1, 3), "Idle"));
       if(!walkFlag)
            StartCoroutine(DownloadAddressableAnimationToAnimator(animkey + " Walk", "Walk"));
    }
    public IEnumerator DownloadAddressableAnimationToAnimator(string key, string NodeAnimToReplace)
    {
      //  Debug.LogError("DownloadAddressableTexture === " + "Assets/Animations/Mood Animations/" + key);

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
                  //  Debug.LogError("NULLLLLL");
                }
                else
                {
                    var clips = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                    overrideController.GetOverrides(clips);
                    for (int i = 0; i < clips.Count; i++)
                    {
                        var stateName = clips[i].Key.name;
                        Debug.LogError("Tag ----> " + stateName.ToString()+"  Replace ---->  "+ NodeAnimToReplace);
                        if (stateName.Contains(NodeAnimToReplace))
                        {
                            Debug.LogError("Replaced");
                            clips[i] = new KeyValuePair<AnimationClip, AnimationClip>(clips[i].Key, loadOp.Result);
                            if (NodeAnimToReplace == "Idle")
                            {
                                GameManager.Instance.mainCharacter.GetComponent<Actor>().ActionClipTime = loadOp.Result.length;
                            }
                            yield return new WaitForSeconds(Time.deltaTime);
                            PlayerAnimator.SetBool("Menu Action", false);
                            break;
                        }
                    }

                    overrideController.ApplyOverrides(clips);
                    /* 
                     AnimatorController controller = PlayerAnimator.runtimeAnimatorController as AnimatorController;
                      foreach (var layer in controller.layers)
                      {
                          var stateMachine = layer.stateMachine;
                          foreach (var state in stateMachine.states)
                          {
                            //  Debug.LogError("NameOfAnimation Node === " + state.state.tag);
                              if (state.state.tag.Equals(NodeAnimToReplace))
                              {
                                  state.state.motion = loadOp.Result;
                                  if(NodeAnimToReplace == "Action")
                                  {
                                     // Debug.LogError("Length Of Clip "+loadOp.Result.length);
                                      GameManager.Instance.mainCharacter.GetComponent<Actor>().ActionClipTime = loadOp.Result.length;
                                  }
                                  yield return new WaitForSeconds(Time.deltaTime);
                                  PlayerAnimator.SetBool("Menu Action", false);
                                  break;
                              }
                          }
                      }*/
                }

            }
        }
    }
}
