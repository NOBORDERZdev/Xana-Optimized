using AdvancedInputFieldPlugin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HomeScreenScrollHandler : ScrollRect
{
    [SerializeField]
    public DynamicScrollRect.DynamicScrollRect DynamicGrid;
    protected override void Start()
    {
        /*DynamicGrid.ParentSliderFlag = false;*/
      //  DynamicGrid.enabled = false;
        //DynamicGrid.scrollSensitivity = 0;
        // scrollSensitivity = 3;

    }
  /*  public bool Flag = true;
    public IEnumerator StartDrag()
    {
        yield return new WaitForSeconds(0.3f);
        Flag = true;
    }
    //public override void OnBeginDrag(PointerEventData eventData)
    //{
    //    if (Flag)
    //        base.OnBeginDrag(eventData);
    //}
    //public override void OnEndDrag(PointerEventData eventData)
    //{
    //    if (Flag)
    //        base.OnEndDrag(eventData);
    //}
  /*  public override void OnDrag(PointerEventData eventData)
    {
        Debug.LogError("PointReached = " + verticalNormalizedPosition);
  
        if (verticalNormalizedPosition < 0.05f && Flag)
        {
           // verticalNormalizedPosition = 0.05f;
            Flag = false;
            DynamicGrid.Flag = true;
            DynamicGrid.velocity = velocity.normalized * 10f;
           // DynamicGrid.scrollSensitivity = 1;

            // base.OnDrag(eventData);
             DynamicGrid.enabled = true;
            //DynamicGrid.verticalNormalizedPosition = 0.90f;
            //DynamicGrid.velocity= this.velocity*30f;
             this.enabled = false; 
        }
        else if (verticalNormalizedPosition > 0.05f && Flag)
        {
            base.OnDrag(eventData);
        }
    }*/
}
