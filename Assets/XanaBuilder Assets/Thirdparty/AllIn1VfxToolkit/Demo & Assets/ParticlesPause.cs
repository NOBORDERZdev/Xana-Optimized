using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesPause : MonoBehaviour
{
    public GameObject G1;
    //public GameObject G2;
    //public GameObject G3;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        G1.GetComponent<ParticleSystem>().Play(true);
        yield return new WaitForSeconds(.5f);
        G1.GetComponent<ParticleSystem>().Pause(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
