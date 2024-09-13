using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DomeBannerClick : MonoBehaviour, IPointerClickHandler
{

    public int DomeId;



    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked....");
        SummitDomeImageHandler.ShowNftData?.Invoke(DomeId);
    }

  
   
}
