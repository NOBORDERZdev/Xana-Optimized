using UnityEngine;

public class VideoDetectTrigger : MonoBehaviour
{

    bool added;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PhotonLocalPlayer"))
        {
            if (!added)
            {
                other.gameObject.GetComponent<CharcterBodyParts>().Nose.AddComponent<DetectVideo>();
                added = true;
                gameObject.GetComponent<Collider>().enabled= false;
                Debug.Log("DetectVideo added");
            }
           
        }
    }
}
