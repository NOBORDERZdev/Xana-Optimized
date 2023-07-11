using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;
using UnityEngine.SceneManagement;

public class AvatarBorderSelectionManager : MonoBehaviour
{
    public static AvatarBorderSelectionManager Instance;

    public List<Outline1> AllAvatarBorderList = new List<Outline1>();

    public Color outlineColor;

    [SerializeField, Range(0f, 10f)]
    public float outlineWidth = 2f;

    public Outline1.Mode outlineMode;

    public bool isMainAvatarOnActionScreen = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        //AddingOtlineOnAllSkinMeshRenders();
        //SetDefaultSettingOfOutline();
    }

    public void AddingOtlineOnAllSkinMeshRenders()
    {
        SkinnedMeshRenderer[] skinnedMeshRenderers = this.transform.GetChild(0).GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            skinnedMeshRenderers[i].gameObject.AddComponent<Outline1>();
            skinnedMeshRenderers[i].gameObject.GetComponent<Outline1>().enabled = false;
            AllAvatarBorderList.Add(skinnedMeshRenderers[i].gameObject.GetComponent<Outline1>());
        }
        SetDefaultSettingOfOutline();
    }

    public void SetDefaultSettingOfOutline()
    {
        for (int i = 0; i < AllAvatarBorderList.Count; i++)
        {
            AllAvatarBorderList[i].outlineColor = outlineColor;
            AllAvatarBorderList[i].outlineWidth = outlineWidth;
            AllAvatarBorderList[i].enabled = false;

            if (SceneManager.GetActiveScene().name == "ARModulePlanDetectionScene")
            {
                AllAvatarBorderList[i].OutlineMode = Outline1.Mode.OutlineAll;
            }
            else
            {
                AllAvatarBorderList[i].OutlineMode = outlineMode;
            }
        }
    }

    public void AvatarSelectionBorder(bool isActive)
    {
        for (int i = 0; i < AllAvatarBorderList.Count; i++)
        {
            AllAvatarBorderList[i].enabled = isActive;
        }

        if (!isMainAvatarOnActionScreen)
        {
            if (!isActive)
            {
                ARFaceModuleManager.Instance.OnDeleteItemObject();
            }
            else
            {
                ARFaceModuleManager.Instance.currentSelectedItemObj = this.gameObject;
            }
            ARFaceModuleManager.Instance.OnDeleteScreenActive(isActive);
        }
    }
}