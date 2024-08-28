using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeightMngrAccToDevice : MonoBehaviour
{
    public RectTransform ObjToFollowHeightof;
    public VerticalLayoutGroup ObjToFollowVerticalGrp;
    public RectTransform CurrentObjRectTransform;

    public void AdjustParentHeight()
    {
        if (Mathf.Abs(ObjToFollowHeightof.rect.y) > 1000f)
        {
            StartCoroutine(SetParentHeight(204f));
        }
        else
        {
            StartCoroutine(SetParentHeight(0f));
        }
    }

    IEnumerator SetParentHeight(float _extraHeight)
    {
        yield return new WaitForSeconds(0.5f);
        CurrentObjRectTransform.sizeDelta = new Vector2(CurrentObjRectTransform.rect.x,
                                                               Mathf.Abs(ObjToFollowHeightof.rect.y) +
                                                                        ObjToFollowVerticalGrp.padding.top + _extraHeight);
    }

}
