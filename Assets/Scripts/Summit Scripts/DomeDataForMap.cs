using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DomeDataForMap : MonoBehaviour
{
    public int domeId;
    [HideInInspector]
    public Image MyImage;

    private Button _myBtn;
    [SerializeField]
    private DomeMinimapDataHolder manager;
    void Start()
    {
        MyImage = GetComponent<Image>();
        _myBtn = GetComponent<Button>();
        _myBtn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        manager.OnClickTeleportPlayerDomePosition(domeId);
    }

}
