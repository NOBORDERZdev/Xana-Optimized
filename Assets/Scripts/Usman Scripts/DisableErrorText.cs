using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableErrorText : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnDisable()
    {
        this.GetComponent<Text>().color = new Color( this.GetComponent<Text>().color.r, this.GetComponent<Text>().color.g, this.GetComponent<Text>().color.b,0);
    }

}
