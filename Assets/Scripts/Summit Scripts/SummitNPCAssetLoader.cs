using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SummitNPCAssetLoader : MonoBehaviour
{
    /*void Custom_InitializeAvatar(SavingCharacterDataClass _data = null)
    {
        string folderPath = GameManager.Instance.GetStringFolderPath();
        if (File.Exists(folderPath) && File.ReadAllText(folderPath) != "") //Check if data exist
        {
            SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
            if (_data != null)
                _CharacterData = _data;
            else
                _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(folderPath));

            clothJson = File.ReadAllText(folderPath);
            var gender = _CharacterData.gender ?? "Male";
            var avatarController = this.gameObject.GetComponent<AvatarController>();
            sceneName = SceneManager.GetActiveScene().name; // updating scene name if scene changed.
            if (sceneName.Equals("Home") || sceneName.Equals("UGC")) // for store/ main menu
            {
                if (string.IsNullOrEmpty(_CharacterData.avatarType) || _CharacterData.avatarType == "OldAvatar")
                {
                    int _rand = Random.Range(0, characterBodyParts.randomPresetData.Length);
                    DownloadRandomPresets(_CharacterData, _rand);
                }
                else
                {
                    CharacterHandler.instance.ActivateAvatarByGender(_CharacterData.gender);
                    SetAvatarClothDefault(gameObject, _CharacterData.gender);

                    if (_CharacterData.myItemObj.Count > 0)
                    {
                        for (int i = 0; i < _CharacterData.myItemObj.Count; i++)
                        {
                            var item = _CharacterData.myItemObj[i];
                            string type = _CharacterData.myItemObj[i].ItemType;
                            if (!string.IsNullOrEmpty(_CharacterData.myItemObj[i].ItemName) && !_CharacterData.myItemObj[i].ItemName.Contains("default", System.StringComparison.CurrentCultureIgnoreCase))
                            {

                                HashSet<string> itemTypes = new HashSet<string> { "Legs", "Chest", "Feet", "Hair", "EyeWearable", "Glove", "Chain" };
                                if (itemTypes.Any(item => type.Contains(item)))
                                {
                                    //getHairColorFormFile = true;
                                    if (!item.ItemName.Contains("md", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        if (type.Contains("Hair"))
                                        {
                                            if (!string.IsNullOrEmpty(_CharacterData.hairItemData) && _CharacterData.hairItemData.Contains("No hair") && wornHair)
                                                UnStichItem("Hair");
                                            else
                                                StartCoroutine(addressableDownloader.DownloadAddressableObj(item.ItemID, item.ItemName, type, gender, avatarController, _CharacterData.HairColor));
                                        }
                                        else
                                            StartCoroutine(addressableDownloader.DownloadAddressableObj(item.ItemID, item.ItemName, type, gender, avatarController, Color.clear));
                                    }
                                    else
                                    {
                                        if (PlayerPrefs.HasKey("Equiped") || xanaConstants.isNFTEquiped)
                                        {
                                            if (item.ItemType.Contains("Chest") && wornShirt)
                                            {
                                                UnStichItem("Chest");
                                                characterBodyParts.TextureForShirt(null);
                                            }
                                            else if (item.ItemType.Contains("Hair") && wornHair)
                                            {
                                                UnStichItem("Hair");
                                            }
                                            else if (item.ItemType.Contains("Legs") && wornPant)
                                            {
                                                UnStichItem("Legs");
                                                characterBodyParts.TextureForPant(null);
                                            }
                                            else if (item.ItemType.Contains("Feet") && wornShoes)
                                            {
                                                UnStichItem("Feet");
                                                characterBodyParts.TextureForShoes(null);
                                            }
                                            else if (item.ItemType.Contains("EyeWearable") && wornEyeWearable)
                                            {
                                                UnStichItem("EyeWearable");
                                            }
                                            else if (item.ItemType.Contains("Glove") && wornGloves)
                                            {
                                                UnStichItem("Glove");
                                                characterBodyParts.TextureForGlove(null);
                                            }
                                            else if (item.ItemType.Contains("Chain") && wornChain)
                                            {
                                                UnStichItem("Chain");
                                            }
                                        }
                                        else
                                        {
                                            WearDefaultItem(type, this.gameObject, gender);
                                        }
                                    }

                                }
                                else
                                {
                                    WearDefaultItem(type, this.gameObject, gender);
                                }
                            }
                            else // wear the default item of that specific part.
                            {
                                if (xanaConstants.isNFTEquiped && type.Contains("Chest"))
                                {
                                    if (wornShirt)
                                        UnStichItem("Chest");
                                    characterBodyParts.TextureForShirt(null);
                                }
                                else
                                {
                                    //As one preset not have footwear, so unstitch the previous pair
                                    if (_CharacterData.myItemObj[0].ItemName == "Boy_Pant_V009" && _CharacterData.myItemObj[1].ItemName == "Boy_Shirt_V009")
                                    {
                                        if (item.ItemType.Contains("Feet"))
                                            UnStichItem("Feet");
                                    }
                                    else
                                    {
                                        WearDefaultItem(type, this.gameObject, gender);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        WearDefaultItem("Legs", this.gameObject, gender);
                        WearDefaultItem("Chest", this.gameObject, gender);
                        WearDefaultItem("Feet", this.gameObject, gender);
                        WearDefaultItem("Hair", this.gameObject, gender);

                        if (wornEyeWearable)
                            UnStichItem("EyeWearable");
                        if (wornChain)
                            UnStichItem("Chain");
                        if (wornGloves)
                        {
                            UnStichItem("Glove");
                            characterBodyParts.TextureForGlove(null);
                        }
                    }
                    if (_CharacterData.charactertypeAi == true && !UGCManager.isSelfieTaken)
                    {
                        ApplyAIData(_CharacterData, this.gameObject);
                    }
                    characterBodyParts.LoadBlendShapes(_CharacterData, this.gameObject);
                }
            }
        }
    }*/

}
