using Photon.Pun;
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
            if (GetComponent<PhotonView>() && GetComponent<PhotonView>().IsMine)
            {
                int avatarIndex = XanaConstants.xanaConstants.selectedAvatarNum;
                UpdatePresets(avatarIndex);
            }
        }
    }

    public void UpdatePresets(int num)
    {
            avatarController = GetComponent<AvatarController>();
            avatarController.StichItem(presetData[num].HairId, presetData[num].Hair, "Hair", avatarController.gameObject);
            avatarController.StichItem(presetData[num].PantId, presetData[num].Pant, "Legs", avatarController.gameObject);
            avatarController.StichItem(presetData[num].ShirtId, presetData[num].Shirt, "Chest", avatarController.gameObject);
            avatarController.StichItem(presetData[num].ShoeId, presetData[num].Shoe, "Feet", avatarController.gameObject);
    }


}
