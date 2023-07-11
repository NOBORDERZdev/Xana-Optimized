using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DropdownScript : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(waitandActive());
        print("Enable");
     }
    IEnumerator waitandActive ()
    {
        yield return new WaitForEndOfFrame();
        gameObject.GetComponent<Dropdown>().Show();

    }
    // Start is called before the first frame update
    private void Start()
    {
 
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
