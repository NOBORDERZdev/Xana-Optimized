using AdvancedInputFieldPlugin;
using EnhancedUI.EnhancedScroller;
using SuperStar.Helpers;
using UnityEngine;

public class ScreenSpaceRefHolder : MonoBehaviour
{
    public static ScreenSpaceRefHolder instance;
    public AllWorldManage allWorldManageRef;
    public SpaceScrollInitializer spaceScrollInitializerRef;
    public EnhancedScroller fullPageScrollerSearch;
    public Transform SearchWorldScreenHolder;
    public GameObject searchWorldHolder;
    public AdvancedInputField searchWorldInput;
    public TMPro.TextMeshProUGUI worldFoundText;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            MainSceneEventHandler.OnBackRefAssign += SettingUpHomeSceneRef;
            MainSceneEventHandler.MakeScreenSpaceAdditive += ScreenSpacesMakeAdditive;
        }
    }


    public void OnDestroy()
    {
        MainSceneEventHandler.OnBackRefAssign -= SettingUpHomeSceneRef;
        MainSceneEventHandler.MakeScreenSpaceAdditive -= ScreenSpacesMakeAdditive;
    }

    public void ScreenSpacesMakeAdditive()
    {
        gameObject.transform.parent= APIBasepointManager.instance.gameObject.transform;
        SuperStar.Helpers.AssetCache.Instance.RemoveLoadedImagesFromMemory();
        gameObject.SetActive(false);
    }

    void SettingUpHomeSceneRef()
    {
        WorldManager.instance.worldSpaceHomeScreenRef.spaceCategoryScroller = spaceScrollInitializerRef;
        WorldManager.instance.worldFoundText = worldFoundText;
        WorldManager.instance.searchWorldControllerRef.scroller = fullPageScrollerSearch;
        GameObject temp = WorldManager.instance.uiHandlerRef.HomeWorldScreen.gameObject;
        Destroy(temp,2);
        WorldManager.instance.uiHandlerRef.HomeWorldScreen = gameObject;
        WorldManager.instance.uiHandlerRef.searchWorldHolder = searchWorldHolder;
        WorldManager.instance.uiHandlerRef.SearchWorldScreenHolder = SearchWorldScreenHolder;
        WorldManager.instance.worldSearchManager.searchWorldInput = searchWorldInput;
        gameObject.transform.parent = WorldManager.instance.uiHandlerRef.Canvas.transform;
        gameObject.transform.SetAsFirstSibling();
        gameObject.GetComponent<RectTransform>().Stretch();
        gameObject.transform.up = Vector3.zero;
        gameObject.transform.right = Vector3.zero;
        gameObject.transform.localScale = Vector3.one;

        MainSceneEventHandler.BackHomeSucessfully?.Invoke();
    }
}
