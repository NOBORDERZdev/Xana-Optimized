using ControlFreak2;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchMovementCanvas : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    private RectTransform rectTransform;
    private Vector2 pointerOffset;
    private Canvas canvas;

    public string ID;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BDButtonsLayoutManager.instance.currentSelectedObject = this.gameObject;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out pointerOffset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            rectTransform.localPosition = localPoint - pointerOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Handle any on end drag actions here
        HandleSetupInit(ID);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        BDButtonsLayoutManager.instance.currentSelectedObject = this.gameObject;   
    }

    public void HandleSetupInit(string buttonID) {
        print("Drag End : "+buttonID);
        //switch (buttonID)
        //{
        //    case "JOYSTICK":
        //        GamePlayUIHandler.inst.joystick.gameObject.GetComponent<TouchButton>().SetupNewInits(buttonID);
        //        break;

        //    case "LK":
        //        GamePlayUIHandler.inst.LK.gameObject.GetComponent<TouchButton>().SetupNewInits(buttonID);
        //        break;

        //    case "LP":
        //        GamePlayUIHandler.inst.LP.gameObject.GetComponent<TouchButton>().SetupNewInits(buttonID);
        //        break;

        //    case "HK":
        //        GamePlayUIHandler.inst.HK.gameObject.GetComponent<TouchButton>().SetupNewInits(buttonID);
        //        break;

        //    case "HP":
        //        GamePlayUIHandler.inst.HP.gameObject.GetComponent<TouchButton>().SetupNewInits(buttonID);
        //        break;

        //    case "SP":
        //        GamePlayUIHandler.inst.SP.gameObject.GetComponent<TouchButton>().SetupNewInits(buttonID);
        //        break;
        //    case "BLOCK":
        //        GamePlayUIHandler.inst.B.gameObject.GetComponent<TouchButton>().SetupNewInits(buttonID);
        //        break;
        //    default:
        //        print("Not Found");
        //        break;
        //}
    }
}
