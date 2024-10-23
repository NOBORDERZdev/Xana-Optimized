using UnityEngine;
using static StoreManager;

public class PresetController : MonoBehaviour
{
    [System.Serializable]
    public class PresetData
    {
        public string name = "";
        [Space(4)]
        public GameObject Hair;
        public int HairId;
        [Space(4)]
        public GameObject Pant;
        public int PantId;
        [Space(4)]
        public GameObject Shirt;
        public int ShirtId;
        [Space(4)]
        public GameObject Shoe;
        public int ShoeId;
    }
    public PresetData[] presetData;

    private AvatarController avatarController;

    private void Awake()
    {
        if (XanaConstants.xanaConstants.assetLoadType.Equals(XanaConstants.AssetLoadType.ByBuild))
        {
            UpdatePresets();
        }
    }

    public void UpdatePresets()
    {
        avatarController = GetComponent<AvatarController>();
        int avatarIndex = XanaConstants.xanaConstants.selectedAvatarNum;
        avatarController.StichItem(presetData[avatarIndex].HairId, presetData[avatarIndex].Hair, "Hair", avatarController.gameObject);
        avatarController.StichItem(presetData[avatarIndex].PantId, presetData[avatarIndex].Pant, "Legs", avatarController.gameObject);
        avatarController.StichItem(presetData[avatarIndex].ShirtId, presetData[avatarIndex].Shirt, "Chest", avatarController.gameObject);
        avatarController.StichItem(presetData[avatarIndex].ShoeId, presetData[avatarIndex].Shoe, "Feet", avatarController.gameObject);
    }


}
