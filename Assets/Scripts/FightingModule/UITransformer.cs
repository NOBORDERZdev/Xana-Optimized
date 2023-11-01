using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITransformer : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] string keyID = "";
    [SerializeField] bool isSelected = false;

    /// <summary>
    /// For Positioning of the UI
    /// </summary>
    private Vector2 pointerOffset;
    private bool isDragging;
    private Vector2 originalPosition;
    [SerializeField] RectTransform rangeRectTransform; //Range with in the UI can be moved


    private Vector3 initialScale;
    private RectTransform rectTransform;

    private Color highlightColor;
    [SerializeField] Image image;
    private Color originalColor;

    bool editMode;
    private void OnEnable()
    {
        Debug.Log("<color=orange> Subscribed all Events </color>");
        UITransformController.openModificationEvent += SetUp;
        UITransformController.uiClickEvent += DeselectThisUI;
        UITransformController.resetEvent += ResetUI;
        UITransformController.resetAllEvent += LoadDefaultData;
        UITransformController.saveAndCloseEvent += SaveUIData;
        UITransformController.sliderChangeEvent += OnSliderValueChanged;
        UITransformController.OnEditModeChanged += EnableEditMode;
       
    }
    private void OnDisable()
    {
        Debug.Log("<color=orange> Unsunscribed all Events </color>");
        UITransformController.openModificationEvent -= SetUp;
        UITransformController.uiClickEvent -= DeselectThisUI;
        UITransformController.resetEvent -= ResetUI;
        UITransformController.resetAllEvent -= LoadDefaultData;
        UITransformController.saveAndCloseEvent -= SaveUIData;
        UITransformController.sliderChangeEvent -= OnSliderValueChanged;
        UITransformController.OnEditModeChanged -= EnableEditMode;
    }

    void EnableEditMode(bool edit)
    {
        editMode = edit;
        Debug.Log("Edit Mode : " + edit);
    }
    private void SetUp(Color hlColor, RectTransform range)
    {
        if (rangeRectTransform == null)
            rangeRectTransform = range;
        rectTransform = GetComponent<RectTransform>();

        LoadUIData();
        initialScale = Vector3.one;
        originalPosition = rectTransform.localPosition;
        if (image == null)
            image = GetComponent<Image>();
        originalColor = image.color;
        highlightColor = hlColor;
    }
    private void ResetUI()
    {
        if (isSelected)
        {
            LoadDefaultData();
        }
    }
    private void LoadDefaultData()
    {
        if (PlayerPrefs.HasKey(keyID + "def"))
        {
            string dataValue = PlayerPrefs.GetString(keyID + "def");
            string[] dataComponents = dataValue.Split(',');

            if (dataComponents.Length == 5)
            {
                float scaleX = float.Parse(dataComponents[0]);
                float scaleY = float.Parse(dataComponents[1]);
                float scaleZ = float.Parse(dataComponents[2]);
                Vector3 scale = new Vector3(scaleX, scaleY, scaleZ);

                float positionX = float.Parse(dataComponents[3]);
                float positionY = float.Parse(dataComponents[4]);
                Vector3 position = new Vector3(positionX, positionY, rectTransform.localPosition.z);

                rectTransform.localScale = scale;
                rectTransform.localPosition = position;
            }
        }
    }
    private void SaveUIData()
    {
        Vector3 scale = rectTransform.localScale;
        Vector3 position = rectTransform.localPosition;

        string dataValue = scale.x + "," + scale.y + "," + scale.z + "," + position.x + "," + position.y;

        PlayerPrefs.SetString(keyID + "mod", dataValue);
        PlayerPrefs.Save();
        UITransformController.uiClickEvent(); // First Deselect All UI
    }
    private void LoadUIData()
    {
        if (PlayerPrefs.HasKey(keyID + "mod"))
        {
            string dataValue = PlayerPrefs.GetString(keyID + "mod");
            string[] dataComponents = dataValue.Split(',');

            if (dataComponents.Length == 5)
            {
                float scaleX = float.Parse(dataComponents[0]);
                float scaleY = float.Parse(dataComponents[1]);
                float scaleZ = float.Parse(dataComponents[2]);
                Vector3 scale = new Vector3(scaleX, scaleY, scaleZ);

                float positionX = float.Parse(dataComponents[3]);
                float positionY = float.Parse(dataComponents[4]);
                Vector3 position = new Vector3(positionX, positionY, rectTransform.localPosition.z);

                rectTransform.localScale = scale;
                rectTransform.localPosition = position;
            }
        }
    }



    /// <summary>
    /// This Functions Controls the Scaling of the Ui
    /// </summary>
    /// <param name="value"></param>

    private void OnSliderValueChanged(float value)
    {
        if (isSelected)
        {
            Vector3 newScale = initialScale * value;
            rectTransform.localScale = newScale;
            Vector2 clampedPosition = ClampPositionWithinRange(rectTransform.localPosition);
            rectTransform.localPosition = clampedPosition;
        }
    }
    void DeselectThisUI()
    {
        isSelected = false;
        image.color = originalColor;
    }
    void SelectThisUI()
    {
        UITransformController.uiClickEvent(); // First Deselect All UI
        UITransformController.SetSliderValueEvent(rectTransform.localScale.x);
        isSelected = true; // Then Select this UI 
        image.color = highlightColor;

    }


    /// <summary>
    /// Following Functions and Callbacks Haandle the Positioning System
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (!editMode)
            return;
        if (isDragging)
        {
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rangeRectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
            {
                Vector2 newPosition = localPointerPosition - pointerOffset;

                Vector2 clampedPosition = ClampPositionWithinRange(newPosition);
                rectTransform.localPosition = clampedPosition;

            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!editMode)
            return;
        //Debug.Log(eventData.pointerId);
        //if (!isDragging && eventData.pointerId == 0)
        if (!isDragging)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out pointerOffset);
            isDragging = true;
            SelectThisUI();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!editMode)
            return;
        isDragging = false;
        //if (eventData.pointerId == 0)
        //{
        //    isDragging = false;
        //}
    }

    private Vector2 ClampPositionWithinRange(Vector2 position)
    {
        Vector2 rangeMin = rangeRectTransform.rect.min + rectTransform.rect.size * 0.5f * rectTransform.localScale;
        Vector2 rangeMax = rangeRectTransform.rect.max - rectTransform.rect.size * 0.5f * rectTransform.localScale;

        Vector2 clampedPosition = Vector2.Max(rangeMin, Vector2.Min(position, rangeMax));

        return clampedPosition;
    }



}
