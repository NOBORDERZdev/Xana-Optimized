using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DomeDataForMap : MonoBehaviour
{
    public int domeId;
    [HideInInspector]
    public Image myImage;

    Button myBtn;
    [SerializeField]
    private DomeMinimapDataHolder manager;
    void Start()
    {
        myImage = GetComponent<Image>();
        myBtn = GetComponent<Button>();
        myBtn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        manager.OnClickTeleportPlayerDomePosition(domeId);
    }

}
