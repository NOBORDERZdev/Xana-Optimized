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
        //        CanvasButtonsHandler.inst.joystick.gameObject.GetComponent<TouchButton>().SetupNewInits(buttonID);
        //        break;

        //    case "LK":
        //        CanvasButtonsHandler.inst.LK.gameObject.GetComponent<TouchButton>().SetupNewInits(buttonID);
        //        break;

        //    case "LP":
        //        CanvasButtonsHandler.inst.LP.gameObject.GetComponent<TouchButton>().SetupNewInits(buttonID);
        //        break;

        //    case "HK":
        //        CanvasButtonsHandler.inst.HK.gameObject.GetComponent<TouchButton>().SetupNewInits(buttonID);
        //        break;

        //    case "HP":
        //        CanvasButtonsHandler.inst.HP.gameObject.GetComponent<TouchButton>().SetupNewInits(buttonID);
        //        break;

        //    case "SP":
        //        CanvasButtonsHandler.inst.SP.gameObject.GetComponent<TouchButton>().SetupNewInits(buttonID);
        //        break;
        //    case "BLOCK":
        //        CanvasButtonsHandler.inst.B.gameObject.GetComponent<TouchButton>().SetupNewInits(buttonID);
        //        break;
        //    default:
        //        print("Not Found");
        //        break;
        //}
    }
}
