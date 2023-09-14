using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ONPresschangeLook : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler , IPointerDownHandler, IPointerUpHandler 
{

   // public bool OnBool;
    public Sprite Activebtn;
    public Sprite UnActivebtn;
    public Image myimage;
    public Text TexttoChange;

    // Start is called before the first frame update
     void Awake()
    {
        TexttoChange = GetComponentInChildren<Text>();
        myimage = GetComponent<Image>();
        
    }

    public  void OnPointerEnter(PointerEventData eventData)
    {
        TexttoChange.color = Color.white;
        myimage.sprite = Activebtn;
       
    }

    public  void OnPointerExit(PointerEventData eventData)
    {
        TexttoChange.color = new(0.5568628f, 0.5568628f, 0.5568628f, 1f);
        myimage.sprite = UnActivebtn;
      
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TexttoChange.color = Color.white;
        myimage.sprite = Activebtn;
       
    }

    public  void OnPointerUp(PointerEventData eventData)
    {
        TexttoChange.color = new Color(0.5568628f, 0.5568628f, 0.5568628f, 1f);
        myimage.sprite = UnActivebtn;
       
    }
   
    //public void TogglePasswordhere()
    //{


    //    if (OnBool)
    //    {

    //        myimage.sprite = Activebtn;
    //        TexttoChange.color = Color.white;

    //    }
    //    else
    //    {

    //        myimage.sprite = UnActivebtn;
    //        TexttoChange.color =new Color(142,142,142);
    //    }
    //    OnBool = false;

    //}
}
