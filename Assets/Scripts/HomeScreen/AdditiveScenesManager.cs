using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveScenesManager : MonoBehaviour
{
    public float sceneDelay;

    public string sceneTest;
    public string sceneTest2;
    public string sceneTest3;
    public string sceneTest4;

    public GameObject SNSmodule;
    public GameObject SNSMessage;
    

    private void Awake()
    {
        sceneDelay = .5f;
        StartCoroutine(AddDelayStore(sceneDelay/3));
    }
    private void Start()
    {
        //StartCoroutine(LoadScenes());
        StartCoroutine(AddDelay(sceneDelay));
        StartCoroutine(AddDelaySNSFeedModule(sceneDelay));
        StartCoroutine(AddDelaySNSMessageModule(sceneDelay));
    }

    //IEnumerator LoadScenes() {
    //    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneTest, LoadSceneMode.Additive);
    //    while (!asyncLoad.isDone)
    //    {
    //        yield return null;
    //    }
    //    BodyCustomizer.Instance.BodyCustomCallFromStore();
    //    asyncLoad = SceneManager.LoadSceneAsync(sceneTest2, LoadSceneMode.Additive);
    //    while (!asyncLoad.isDone)
    //    {
    //        yield return null;
    //    }
    //    asyncLoad = SceneManager.LoadSceneAsync(sceneTest3, LoadSceneMode.Additive);
    //    while (!asyncLoad.isDone)
    //    {
    //        yield return null;
    //    }
    //    asyncLoad = SceneManager.LoadSceneAsync(sceneTest4, LoadSceneMode.Additive);
    //    while (!asyncLoad.isDone)
    //    {
    //        yield return null;
    //    }
    //    GameManager.Instance.mainCharacter.GetComponent<DefaultEnteriesforManican>().IntializeCharacter();
    //}

    IEnumerator AddDelayStore(float delay)
    {
        
        yield return new WaitForSeconds(delay);
         AsyncOperation async=SceneManager.LoadSceneAsync(sceneTest2, LoadSceneMode.Additive);
        while(!async.isDone)
        {
            yield return null;
        }

         //yield return new WaitForSeconds(1);
        BodyCustomizer.Instance.BodyCustomCallFromStore();
    }


    IEnumerator AddDelay(float delay)
    {
        
        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(sceneTest, LoadSceneMode.Additive);
    }

    IEnumerator AddDelaySNSFeedModule(float delay)
    {
        
        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(sceneTest3, LoadSceneMode.Additive);
    }
    IEnumerator AddDelaySNSMessageModule(float delay)
    {
       
        yield return new WaitForSeconds(delay);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneTest4, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        GameManager.Instance.mainCharacter.GetComponent<AvatarController>().IntializeAvatar();
        LoadingHandler.Instance.HideLoading();
        if (!XanaConstants.xanaConstants.JjWorldSceneChange && !XanaConstants.xanaConstants.orientationchanged)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        //if (LoadingHandler.Instance != null)
        //{
        //    if (Screen.orientation == ScreenOrientation.Landscape)
        //    {
        //        LoadingHandler.Instance.Loading_WhiteScreen.SetActive(true);
        //        yield return new WaitForSeconds(4f);
        //        LoadingHandler.Instance.Loading_WhiteScreen.SetActive(false);

        //    }
        //    LoadingHandler.Instance.HideLoading();
        //}
    }
   
}
