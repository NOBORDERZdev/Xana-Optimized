using UnityEngine;
public class GameplayCanvasDisabler : MonoBehaviour
{
    private void OnEnable()
    {
       ReferencesForGamePlay.instance.workingCanvas.SetActive(false);
    }

    private void OnDisable()
    {
        ReferencesForGamePlay.instance.workingCanvas.SetActive(true);
    }
}
