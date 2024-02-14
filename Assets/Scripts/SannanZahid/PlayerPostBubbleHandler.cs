using System.Text;
using UnityEngine;

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

        if (postTextF.text.Length >= 20)
        {
            _postText.text = InsertNewlines(postTextF.text);
        }
        else
        {
            _postText = postTextF;
        }
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
        if (txt.Length >= 20)
        {
            _postText.text = InsertNewlines(txt);
        }
        else
        {
            _postText.text = txt;
        }
    }

    public string InsertNewlines(string input)
    {
        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            stringBuilder.Append(input[i]);

            if ((i + 1) % 20 == 0)
            {
                stringBuilder.Append("\n");
            }
        }

        return stringBuilder.ToString();
    }
}
