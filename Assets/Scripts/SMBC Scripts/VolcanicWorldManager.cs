using UnityEngine;

public class VolcanicWorldManager : MonoBehaviour
{
    [SerializeField] private GameObject _ground;
    private void OnEnable()
    {
        BuilderEventManager.OnSMBCQuizWrongAnswer += RedirectToEarth;
    }
    private void OnDisable()
    {
        BuilderEventManager.OnSMBCQuizWrongAnswer -= RedirectToEarth;
    }

    private void RedirectToEarth()
    {
        _ground.SetActive(false);
        Invoke(nameof(BackToEarth), 3f);
    }

    private void BackToEarth()
    {
        GamePlayButtonEvents.OnExitButtonXANASummit?.Invoke();
    }
}
