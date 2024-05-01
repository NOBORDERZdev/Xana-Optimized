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
            MainSceneEventHandler.OnBackFromGamePlay += SettingUpHomeSceneRef;
        }
    }


    public void OnDestroy()
    {
        MainSceneEventHandler.OnBackFromGamePlay -= SettingUpHomeSceneRef;
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
        Destroy(WorldManager.instance.uiHandlerRef.HomeWorldScreen.gameObject);
        WorldManager.instance.uiHandlerRef.HomeWorldScreen = gameObject;
        WorldManager.instance.uiHandlerRef.searchWorldHolder = searchWorldHolder;
        WorldManager.instance.uiHandlerRef.SearchWorldScreenHolder = SearchWorldScreenHolder;
        gameObject.transform.parent = WorldManager.instance.uiHandlerRef.Canvas.transform;
        gameObject.transform.SetAsFirstSibling();
        gameObject.GetComponent<RectTransform>().Stretch();
        gameObject.transform.up = Vector3.zero;
        gameObject.transform.right = Vector3.zero;
        gameObject.transform.localScale = Vector3.one;
    }
}
