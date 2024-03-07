using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisableErrorText : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnDisable()
    {
        this.GetComponent<TextMeshProUGUI>().color = new Color( this.GetComponent<TextMeshProUGUI>().color.r, this.GetComponent<TextMeshProUGUI>().color.g, this.GetComponent<TextMeshProUGUI>().color.b,0);
    }

}
