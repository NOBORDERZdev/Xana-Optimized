using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gridsetup : MonoBehaviour
{
   
    [SerializeField]
    GridLayoutGroup gridgroup;
    [SerializeField]
    public int rows;
    [SerializeField]
    public int columns;
    
    [SerializeField]
    GameObject parent;
    void Start()
    {
        StartCoroutine(ResetCells());
    }

    IEnumerator ResetCells()
    { 
        while(true)
        {
            UpdateGridCells();
            yield return new WaitForSeconds(0.25f);
        }
    }

    void UpdateGridCells()
    {
        float parentwidth = parent.GetComponent<RectTransform>().rect.width;
        parentwidth = parentwidth * 0.001f;
        float width = parent.GetComponent<RectTransform>().rect.width;

        width = (width / columns) - (parentwidth);



        gridgroup.cellSize = new Vector2(width, width);
        //  gridgroup.spacing = new Vector2(Mathf.Ceil(parentwidth/1.5f)  , Mathf.Ceil(parentwidth / 1.5f ));
        //gridgroup.padding.top = (int)Mathf.Ceil(parentwidth / 1.5f);
        gridgroup.padding.bottom = (int)Mathf.Ceil(parentwidth / 1.5f);

    }
}
