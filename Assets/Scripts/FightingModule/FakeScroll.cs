using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FakeScroll : MonoBehaviour
{
    void Update()
    {
        transform.GetComponent<Scrollbar>().size = 0;
    }
}
