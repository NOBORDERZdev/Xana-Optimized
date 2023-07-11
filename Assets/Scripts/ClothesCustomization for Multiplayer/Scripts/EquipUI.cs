using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipUI : MonoBehaviour/*, IPointerClickHandler*/
{
    [HideInInspector]
    public GameObject unit;
    //private ChangeGear changeGearScript;
    //private Equipment equipmentScript;
    private Text textChild;
   
    private void Start()
    {
        unit = GameManager.Instance.mainCharacter;
        //changeGearScript = unit.GetComponent<ChangeGear>();
        //equipmentScript = unit.GetComponent<Equipment>();
  
        Invoke("DefaultClothes", 0.1f);
    }
    public void DefaultClothes()
    {
            //ChangeCostume(null,"MDshirt");
            //ChangeCostume(null,"MDpant");
            //ChangeCostume(null,"MDshoes");
            //changeGearScript.EquipItem(null, "Hair", "MDhairs");       
    }    
    public void BackFromArtbone()
    {
        GameManager.Instance.ChangeCharacterAnimationState(false);
    }
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    //public void ChangeCostume(GameObject abc, string CostumeName)
    //{
    //    //print("ChaneCostumeCalled");
    //    if(CostumeName.Contains("pant"))
    //    {
    //                  AddOrRemoveClothes(abc, "naked_legs", "Legs", CostumeName, 0);
    //        //print("Costume name in Change costume" + CostumeName);
    //      }
    //    else
    //         if (CostumeName.Contains("boots") || CostumeName.Contains("shoes") || CostumeName.Contains("sho"))
    //    {
    //        AddOrRemoveClothes(abc, "naked_slug", "Feet", CostumeName, 7);
  
    //    }
    //    else
    //         if (CostumeName.Contains("gambeson") || CostumeName.Contains("shirt") || CostumeName.Contains("arabic"))
    //    {
    //             AddOrRemoveClothes(abc, "naked_chest", "Chest", CostumeName, 1);
    //     }
    //    else
    //         if (CostumeName.Contains("hair"))
    //    {
    //        AddOrRemoveClothes(abc, "bald_head", "Hair", CostumeName, 2);
    //    }
    //}
    //public void ChangeCostume(GameObject abc, string CostumeName,GameObject applyOn)
    //{
    //    if (CostumeName.Contains("pant"))
    //    {
    //        AddOrRemoveClothes(abc, "naked_legs", "Legs", CostumeName, 0,applyOn);
    //        //print("Costume name in Change costume" + CostumeName);
    //    }
    //    else
    //         if (CostumeName.Contains("boots") || CostumeName.Contains("shoes") || CostumeName.Contains("sho"))
    //    {
    //        AddOrRemoveClothes(abc, "naked_slug", "Feet", CostumeName, 7, applyOn);

    //    }
    //    else
    //         if (CostumeName.Contains("gambeson") || CostumeName.Contains("shirt"))
    //    {
    //        AddOrRemoveClothes(abc, "naked_chest", "Chest", CostumeName, 1, applyOn);
    //    }
    //    else
    //         if (CostumeName.Contains("hair"))
    //    {
    //        AddOrRemoveClothes(abc, "bald_head", "Hair", CostumeName, 2, applyOn);
    //    }
    //}
    //public void AddOrRemoveClothes(GameObject abc, string nakedSlug, string clothesType, string clothesSlug, int equippedItemsIndex)
    //{
    //        changeGearScript.EquipItem(abc, clothesType, clothesSlug);
    //}
    //public void AddOrRemoveClothes(GameObject abc, string nakedSlug, string clothesType, string clothesSlug, int equippedItemsIndex,GameObject applyOn)
    //{
    //    changeGearScript.EquipItem(abc, clothesType, clothesSlug,applyOn);
    //}
}
