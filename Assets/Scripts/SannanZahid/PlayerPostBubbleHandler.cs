using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPostBubbleHandler : MonoBehaviour
{
    [SerializeField]
    Transform _cameraTransform;
    [SerializeField]
    TMPro.TMP_Text _postText;
    public Transform BubbleObj;
    public Vector3 Offset;
    public void InitObj(Transform _bubbleObjF, TMPro.TMP_Text postTextF)
    {
        BubbleObj = _bubbleObjF;
        _postText = postTextF;

        _postText.text = postTextF.text;
        InsertNewlines(_postText);
    }

    public void ActivatePostFirendBubble(bool flag)
    {
        // Debug.LogError("ActivatePostFirendBubble -----> " + flag);
        if (BubbleObj != null && BubbleObj.transform.childCount > 0)
        {
            BubbleObj.GetChild(0).gameObject.SetActive(flag);
        }
    }
    void Update()
    {
        BubbleObj.position = Vector3.MoveTowards(BubbleObj.position, transform.position + Offset, 0.5f);
        // BubbleObj.rotation = Quaternion.Slerp(BubbleObj.rotation,
        //     Quaternion.LookRotation(_cameraTransform.position - BubbleObj.position), 
        //    Time.deltaTime);


        //BubbleObj.LookAt(_cameraTransform);
    }
    public void UpdateText(string txt)
    {
        if (txt != "" && txt != null)
        {
            _postText.text = txt;
            InsertNewlines(_postText);
        }
    }

    public void InsertNewlines(TMP_Text input)
    {
        StartCoroutine(ArrangeBubbleTxt(input));
    }

    private IEnumerator ArrangeBubbleTxt(TMP_Text tmpText)
    {
        ContentSizeFitter contentSizeFitter = tmpText.transform.parent.parent.GetComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        yield return new WaitForEndOfFrame();

        string str = tmpText.text;
        tmpText.text = "";
        for (int i = 0; i < str.Length; i++)
        {
            tmpText.text += str[i];
            tmpText.ForceMeshUpdate();

            var preferredWidth = tmpText.GetPreferredValues().x;
            if (preferredWidth > 260)
            {
                yield return new WaitForEndOfFrame();
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                yield return new WaitForEndOfFrame();
                contentSizeFitter.gameObject.GetComponent<RectTransform>().sizeDelta =
                    new Vector2(300f, contentSizeFitter.gameObject.GetComponent<RectTransform>().sizeDelta.y);

                tmpText.text = str;
                yield break;
            }
        }
    }
}
