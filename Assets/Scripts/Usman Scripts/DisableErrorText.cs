using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisableErrorText : MonoBehaviour
{

   
    private void OnDisable()
    {
        if (this.GetComponent<TextMeshProUGUI>())
        {
            this.GetComponent<TextMeshProUGUI>().color = new Color(this.GetComponent<TextMeshProUGUI>().color.r, this.GetComponent<TextMeshProUGUI>().color.g, this.GetComponent<TextMeshProUGUI>().color.b, 0);
        }
        else if(this.GetComponent<Text>())
        {
            
            this.GetComponent<Text>().color = new Color(this.GetComponent<Text>().color.r, this.GetComponent<Text>().color.g, this.GetComponent<Text>().color.b, 0);

        }
     }

}
