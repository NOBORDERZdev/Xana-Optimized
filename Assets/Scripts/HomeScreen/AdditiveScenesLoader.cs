using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveScenesLoader : MonoBehaviour
{
    public bool SwitchXanaToXSummit=false;
    public float sceneDelay;
    string sceneTest= "LoginSignupScene";
    string sceneTest1= "XSummitLoginSignupScene";
    string sceneTest2= "InventoryScene";
    string sceneTest3= "SNSFeedModuleScene";
    string sceneTest4= "SNSMessageModuleScene";
    [HideInInspector]
    public GameObject SNSmodule;
    [HideInInspector]
    public GameObject SNSMessage;
    public HomeFooterHandler homeBottomTab;
    

    public static bool isAppOpen = false;

    private void Start()
    {
        if(ConstantsHolder.xanaConstants.openLandingSceneDirectly && !ConstantsHolder.xanaConstants.isBackFromWorld)
        {
            sceneDelay = .5f;
            StartCoroutine(AddDelayStore(sceneDelay / 3));
            StartCoroutine(AddDelay(sceneDelay));
            GameManager.Instance.isAllSceneLoaded = true;
            return;
        }

        if(!ConstantsHolder.xanaConstants.JjWorldSceneChange)
        {
            sceneDelay = .5f;
            StartCoroutine(AddDelayStore(sceneDelay / 3));
            StartCoroutine(AddDelay(sceneDelay));
            StartCoroutine(AddDelaySNSFeedModule(sceneDelay));
           // StartCoroutine(AddDelaySNSMessageModule(sceneDelay));
        }
    }
    IEnumerator AddDelayStore(float delay)
    {
        yield return new WaitForSeconds(delay);
         AsyncOperation async = SceneManager.LoadSceneAsync(sceneTest2, LoadSceneMode.Additive);
        while(!async.isDone)
        {
            yield return null;
        }
        BodyFaceCustomizer.Instance.BodyCustomCallFromStore();
    }
    IEnumerator AddDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!SwitchXanaToXSummit)
        {
            SceneManager.LoadSceneAsync(sceneTest, LoadSceneMode.Additive);

        }
        else
        {
            SceneManager.LoadSceneAsync(sceneTest1, LoadSceneMode.Additive);
        }
    }
        IEnumerator AddDelaySNSFeedModule(float delay)
    {
        yield return new WaitForSeconds(delay);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneTest3, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        if (ConstantsHolder.xanaConstants.isBackfromSns)
        {
            homeBottomTab.OnClickFeedButton();
            ConstantsHolder.xanaConstants.isBackfromSns=false;
        }
        LoadingHandler.Instance.HideLoading();

        // Xana Analytics
        if (!isAppOpen)
        {
            isAppOpen = true;
            GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.App_Started.ToString());
        }
        GameManager.Instance.isAllSceneLoaded = true;
        //MainSceneEventHandler.MemoryRelaseAfterLoading?.Invoke();
    }
    //IEnumerator AddDelaySNSMessageModule(float delay)
    //{
       
    //    yield return new WaitForSeconds(delay);
    //    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneTest4, LoadSceneMode.Additive);
    //    while (!asyncLoad.isDone)
    //    {
    //        yield return null;
    //    }
    //    //GameManager.Instance.mainCharacter.GetComponent<AvatarController>().IntializeAvatar();
    //    if (ConstantsHolder.xanaConstants.isBackfromSns)
    //    {
    //        homeBottomTab.OnClickFeedButton();
    //        ConstantsHolder.xanaConstants.isBackfromSns=false;
    //    }
    //    LoadingHandler.Instance.HideLoading();
    //   // LoadingHandler.Instance.HideLoading(ScreenOrientation.Portrait, ConstantsHolder.xanaConstants.isBackFromWorld);
    //}
}
