using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingScreenUIBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject[] _settingPages;
    [SerializeField] private GameObject[] _settingTabsHighlighter;

    [Header("Graphics Screen")]
    [SerializeField] private Scrollbar _qualityButtonHighlighter;
    [SerializeField] private float[] _highlighterScrollValues;

    private void OnEnable()
    {
        GamePlayUIHandler.inst.gamePlayUIParent.SetActive(false);
        ShowSettingPage(0);
        SetQualityHighliter(PlayerPrefs.GetInt("QualitySettings"));
        //ReferencesForGamePlay.instance.QualityManager.SetQualityToggles(PlayerPrefs.GetInt("QualitySettings"));

    }
    private void OnDisable()
    {
        GamePlayUIHandler.inst.gamePlayUIParent.SetActive(true);
    }
    public void CloseButton()
    {
        GamePlayUIHandler.inst.gamePlayUIParent.SetActive(true);
    }

    public void ShowSettingPage(int index)
    {
        if (index >= _settingPages.Length)
            return;

        for (int i = 0; i < _settingPages.Length; i++)
        {
            _settingPages[i].SetActive(i == index);
            _settingTabsHighlighter[i].SetActive(i == index);
        }
    }

    public void SetQualityHighliter(int index)
    {
        //_qualityButtonHighlighter.value = _highlighterScrollValues[index];
        DOTween.To(() => _qualityButtonHighlighter.value, x => _qualityButtonHighlighter.value = x, _highlighterScrollValues[index], 0.5f);
    }
}
