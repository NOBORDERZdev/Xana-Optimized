
using UnityEngine;

public class PlayVideoBoxCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PhotonLocalPlayer"))
        {
           this.transform.parent.gameObject.GetComponent<DynamicGalleryData>().videoPlayer.Play();
           this.transform.parent.gameObject.GetComponent<DynamicGalleryData>().videoPlayerWithStats.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PhotonLocalPlayer"))
        {
            this.transform.parent.gameObject.GetComponent<DynamicGalleryData>().videoPlayer.Pause();
            this.transform.parent.gameObject.GetComponent<DynamicGalleryData>().videoPlayerWithStats.Pause();
        }
    }
}
