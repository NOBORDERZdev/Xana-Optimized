
using UnityEngine;

public class OnOffdIirectionalVol : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject otherButton;
    public void ClickOnDirVol()
    {
        ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SpeakerRefrence>().MyspeakerSync2D();
        this.gameObject.SetActive(false);
        otherButton.SetActive(true);
     }
    public void ClickOffDirVol()
    {
        ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SpeakerRefrence>().MyspeakerSync3D();
        this.gameObject.SetActive(false);
        otherButton.SetActive(true);
     }
   
}
