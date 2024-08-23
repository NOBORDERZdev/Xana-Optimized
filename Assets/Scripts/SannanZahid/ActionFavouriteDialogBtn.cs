using SuperStar.Helpers;
using UnityEngine;
using UnityEngine.UI;

public class ActionFavouriteDialogBtn : MonoBehaviour
{
    public string AnimationName;
    public string ThumbnailURL;
    public EmoteReactionItemBtnHandler.ItemType TypeOfAction;
    public int IndexOfBtn = 0;

    [SerializeField] Transform _highLightObj;
    [SerializeField] Transform _crossBtnObj;
    [SerializeField] private Image _actionImg;

    private void OnEnable()
    {
        InitializeBtn();
    }

    private void OnDisable()
    {
        if (ThumbnailURL != "")
        {
           ///>--MemoryClean Stoped AssetCache.Instance.RemoveFromMemoryDelayCoroutine(ThumbnailURL, true);
            _actionImg.gameObject.SetActive(false);
            _crossBtnObj.gameObject.SetActive(false);
        }
    }

    public void EnableHighLightObj(bool flag)
    {
        _highLightObj.gameObject.SetActive(flag);
    }

    public bool ValidateSimilarData(string animationName, EmoteReactionItemBtnHandler.ItemType typeOfAction)
    {
        if(animationName == AnimationName && typeOfAction == TypeOfAction)
        {
            return true;
        }
        return false;
    }

    public bool ValidateSelectionOfFavouriteAction(bool flag)
    {
        if (AnimationName == "" && flag)
        {
            EnableHighLightObj(true);
            return true;
        }
        EnableHighLightObj(false);
        return false;
    }

    public void SetupActionSelected(ActionData dataObj)
    {
        GetComponent<Image>().color = Color.white;
        AnimationName = dataObj.AnimationName;
        TypeOfAction = dataObj.TypeOfAction;
        ThumbnailURL = dataObj.ThumbnailURL;
  
        LoadImageFromURL();
    }

    public void CancelSelectedAction()
    {
        Color myColor;
        if (ColorUtility.TryParseHtmlString("#FFFFFFB4", out myColor))
        {
            GetComponent<Image>().color = myColor;
        }

        _actionImg.sprite = default;
        AnimationName = default;
        ThumbnailURL = default;
        _actionImg.gameObject.SetActive(false);
        _crossBtnObj.gameObject.SetActive(false);
    }
    public void SaveActionSelected()
    {
        if (AnimationName != "")
        {
            ActionData actionData = new ActionData();
            actionData.AnimationName = AnimationName;
            actionData.TypeOfAction = TypeOfAction;
            actionData.ThumbnailURL = ThumbnailURL;
            PlayerPrefsUtility.SetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn, JsonUtility.ToJson(actionData).ToString());
        }
        else
        {
            PlayerPrefsUtility.SetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn, "");
        }
    }

    private void InitializeBtn()
    {
        if (PlayerPrefsUtility.GetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn) != "")
        {
            ActionData actionData = JsonUtility.FromJson<ActionData>(PlayerPrefsUtility.GetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn));
            AnimationName = actionData.AnimationName;
            TypeOfAction = actionData.TypeOfAction;
            ThumbnailURL = actionData.ThumbnailURL;
            LoadImageFromURL();
        }
    }
    private void LoadImageFromURL()
    {
        if (ThumbnailURL != "")
        {
            AssetCache.Instance.EnqueueOneResAndWait(ThumbnailURL, ThumbnailURL, (success) =>
            {
                if (success)
                {
                    AssetCache.Instance.LoadSpriteIntoImage(_actionImg, ThumbnailURL, changeAspectRatio: true);
                    _actionImg.gameObject.SetActive(true);
                    _crossBtnObj.gameObject.SetActive(true);
                    GetComponent<Image>().color = Color.white;
                    if (TypeOfAction == EmoteReactionItemBtnHandler.ItemType.Emote)
                    {
                        _actionImg.transform.localScale = Vector3.one;
                    }
                    else
                    {
                        _actionImg.transform.localScale = new Vector3(0.6f,0.6f,1f);
                    }
                }
                else
                {
                    Debug.Log("Download Failed");
                }
            });
        }
    }
}