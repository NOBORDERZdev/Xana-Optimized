using UnityEngine;
using TMPro;
public class ThaMeetingTxtUpdate : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    // Start is called before the first frame update
    void Start()
    {
        tmp.text = "Meeting Room";
    }
    
    public void UpdateMeetingTxt(string data, Color txtColor = default)
    {
        tmp.text = data;
        tmp.color = txtColor;
    }
 
}
