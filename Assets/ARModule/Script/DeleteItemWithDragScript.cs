using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteItemWithDragScript : MonoBehaviour
{
    public GameObject deleteBtn;

    public float minScale = 1;
    public float maxScale = 1.2f;

    public bool isPointerEnter = false;

    public void OnDeleteBtnSizeUp()
    {
        isPointerEnter = true;
        deleteBtn.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
    }

    public void OnDeleteBtnSizeDown()
    {
        PointerFalse();
        deleteBtn.transform.localScale = new Vector3(minScale, minScale, minScale);
    }

    void PointerFalse()
    {
        if(waitToFalse != null)
        {
            StopCoroutine(waitToFalse);
        }
        waitToFalse = StartCoroutine(WaitToPointerFalse());
    }

    Coroutine waitToFalse;
    IEnumerator WaitToPointerFalse()
    {
        yield return new WaitForSeconds(0.1f);
        isPointerEnter = false;
    }
}