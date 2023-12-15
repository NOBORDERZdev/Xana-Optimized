using UnityEngine;

public class PlayerPostBubbleHandler : MonoBehaviour
{
    [SerializeField]
    Transform _cameraTransform;
    [SerializeField]
    TMPro.TMP_Text _postText;
    public Transform BubbleObj;

    public void ActivatePostFirendBubble(bool flag)
    {
        BubbleObj.gameObject.SetActive(flag);
    }
    void Update()
    {
        BubbleObj.LookAt(_cameraTransform);
    }
    public void UpdateText(string txt)
    {
        _postText.text = txt;
    }
}
