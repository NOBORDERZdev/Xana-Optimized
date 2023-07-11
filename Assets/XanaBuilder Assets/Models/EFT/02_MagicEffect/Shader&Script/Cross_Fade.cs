using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross_Fade : MonoBehaviour

{
    [SerializeField] private float Period;
    // Start is called before the first frame update
    void Start()
    {
        LODGroup.crossFadeAnimationDuration = Period;
    }
}
