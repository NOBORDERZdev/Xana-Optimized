using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.Tweens;
using static ServerSIdeCharacterHandling;

public class Swipe_menu : MonoBehaviour
{
    public GameObject scrollbar;
    public List<GameObject> items;
    [SerializeField] private Transform contentParent;
    [SerializeField] private float scroll_pos = 0;
    [SerializeField] private float[] pos;
    [SerializeField] private GameObject SelectedOBJ;



    void Start()
    {
        
            foreach (GameObject item in items)
            {
                GameObject itemGameObject = Instantiate(item, contentParent);
            }
        
    }

    private void Update()
    {
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);
        for(int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }

        if(Input.GetMouseButton(0))
        {
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value;
        }
        else
        {
            for(int i = 0; i < pos.Length; i++)
            {
                if(scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                {
                    scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                 
                }
            }
        }
        for(int i = 0; i < pos.Length; i++)
        {
            if(scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
            {
               
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1.2f, 1.2f), 0.1f);
                transform.GetChild(i).GetComponent<Image>().enabled = true;
                SelectedOBJ = transform.GetChild(i).gameObject;

                for (int a = 0; a < pos.Length; a++)
                {
                    if(a != i)
                    {
                        transform.GetChild(a).localScale = Vector2.Lerp(transform.GetChild(a).localScale, new Vector2(1f, 1f), 0.1f);
                       
                    }
                   
                }
               
            }else
            {
                transform.GetChild(i).GetComponent<Image>().enabled = false;
            }
        }
    }
    public void OnClickNext()
    {
        if (SelectedOBJ != null)
        {
            UserRegisterationManager.instance.LogoImage.GetComponent<Image>().sprite = SelectedOBJ.transform.GetChild(0).GetComponent<Image>().sprite;
            UserRegisterationManager.instance.LogoImageforname .GetComponent<Image>().sprite = SelectedOBJ.transform.GetChild(0).GetComponent<Image>().sprite;
            SelectedOBJ.GetComponent<PresetData_Jsons>().ChangecharacterOnCLickFromserver();


        }
    }

}






