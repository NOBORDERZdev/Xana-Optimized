using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayParticleOnClick : MonoBehaviour, IPointerClickHandler
{
    public ParticleSystem particleSystem; // Drag your Particle System here in the inspector

    public void OnPointerClick(PointerEventData eventData)
    {
        if (particleSystem != null)
        {
            Debug.Log("Partical Blow");
            particleSystem.Play();
        }
    }
}
