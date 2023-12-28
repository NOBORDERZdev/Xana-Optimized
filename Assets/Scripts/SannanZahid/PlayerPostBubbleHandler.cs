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
        _postText = postTextF;
    }

    public void ActivatePostFirendBubble(bool flag)
    {
       // Debug.LogError("ActivatePostFirendBubble -----> " + flag);
        BubbleObj.GetChild(0).gameObject.SetActive(flag);
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
        _postText.text = txt;
    }
}
