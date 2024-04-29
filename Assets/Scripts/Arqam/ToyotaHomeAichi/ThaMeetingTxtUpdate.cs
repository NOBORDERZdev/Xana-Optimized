using UnityEngine;
using TMPro;

public class ThaMeetingTxtUpdate : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    // Start is called before the first frame update
    void Start()
    {
        tmp.text = "Join Meeting Now!";
        NFT_Holder_Manager.instance.meetingTxtUpdate = this;
    }

    public void UpdateMeetingTxt(string data)
    {
        tmp.text = "";
        tmp.text = data;
       // tmp.color = txtColor;
        tmp.alpha = 1f;
    }


}
