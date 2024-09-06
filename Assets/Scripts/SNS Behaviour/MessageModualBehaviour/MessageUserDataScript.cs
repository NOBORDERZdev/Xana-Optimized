using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperStar.Helpers;
using System.IO;

public class MessageUserDataScript : MonoBehaviour
{
    //public AllFollowingRow allFollowingRow;

    //public TextMeshProUGUI textUserName;
    //public Image profileImage;

    //public Toggle selectionToggle;
    //// public Toggle selectionToggle;

    //private void OnDestroy()
    //{
    //    if (!string.IsNullOrEmpty(allFollowingRow.following.avatar))
    //    {
    //        AssetCache.Instance.RemoveFromMemory(profileImage.sprite);
    //        profileImage.sprite = null;
    //        SNS_APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
    //    }
    //}

    //public void LoadFeed(AllFollowingRow allFollowingRow1)
    //{
    //    allFollowingRow = allFollowingRow1;

    //    if (!string.IsNullOrEmpty(allFollowingRow.following.avatar))
    //    {
    //        bool isAvatarUrlFromDropbox = SNS_APIManager.Instance.CheckUrlDropboxOrNot(allFollowingRow.following.avatar);
    //        //Debug.LogError("isAvatarUrlFromDropbox: " + isAvatarUrlFromDropbox + " :name:" + FeedsByFollowingUserRowData.User.Name);
    //        if (isAvatarUrlFromDropbox)
    //        {
    //            AssetCache.Instance.EnqueueOneResAndWait(allFollowingRow.following.avatar, allFollowingRow.following.avatar, (success) =>
    //            {
    //                if (success)
    //                    AssetCache.Instance.LoadSpriteIntoImage(profileImage, allFollowingRow.following.avatar, changeAspectRatio: true);
    //            });
    //        }
    //        else
    //        {
    //            GetImageFromAWS(allFollowingRow.following.avatar, profileImage);//Get image from aws and save/load into asset cache.......
    //        }
    //    }
    //    if (!string.IsNullOrEmpty(allFollowingRow.following.name))
    //    {
    //        textUserName.text = allFollowingRow.following.name;
    //    }
    //    else
    //    {
    //        textUserName.text = allFollowingRow.following.email;
    //    }
    //}

    //public void OnClickSelectFriend()
    //{
    //    if (!selectionToggle.isOn)
    //    {
    //        GameObject friendItemObject = Instantiate(SNS_APIController.Instance.selectedFriendItemPrefab, SNS_MessageController.Instance.selectedFriendItemPrefabParent);
    //        //friendItemObject.GetComponent<MessageUserDataScript>().allFollowingRow = allFollowingRow;
    //        friendItemObject.GetComponent<MessageUserDataScript>().LoadFeed(allFollowingRow);
    //        selectionToggle.isOn = true;
    //        SNS_MessageController.Instance.CreateNewMessageUserList.Add(allFollowingRow.userId.ToString());
    //        SNS_MessageController.Instance.createNewMessageUserAvatarSPList.Add(profileImage.sprite);
    //        // SNS_APIController.Instance.allFollowingUserList.Remove(allFollowingRow.following.name);
    //        SNS_APIController.Instance.allChatMemberList.Add(allFollowingRow.following.name);

    //        if (SNS_MessageController.Instance.searchManagerFindFriends.searchScrollView.activeSelf)
    //        {
    //            MessageUserDataScript ddd = SNS_MessageController.Instance.searchManagerFindFriends.allMessageUserDataList.Find(x => x.allFollowingRow.id == allFollowingRow.id);
    //            ddd.selectionToggle.isOn = selectionToggle.isOn;
    //        }
    //    }
    //    else
    //    {
    //        for (int i = 0; i < SNS_MessageController.Instance.selectedFriendItemPrefabParent.childCount; i++)
    //        {
    //            if (SNS_MessageController.Instance.selectedFriendItemPrefabParent.GetChild(i).gameObject.GetComponent<MessageUserDataScript>().allFollowingRow.userId == allFollowingRow.userId)
    //            {
    //                // SNS_APIController.Instance.allFollowingUserList.Add(allFollowingRow.following.name);
    //                SNS_APIController.Instance.allChatMemberList.Remove(allFollowingRow.following.name);
    //                SNS_MessageController.Instance.CreateNewMessageUserList.Remove(allFollowingRow.userId.ToString());
    //                SNS_MessageController.Instance.createNewMessageUserAvatarSPList.Remove(profileImage.sprite);
    //                Destroy(SNS_MessageController.Instance.selectedFriendItemPrefabParent.GetChild(i).gameObject);
    //                break;
    //            }
    //        }
    //        selectionToggle.isOn = false;
    //    }
    //    SNS_MessageController.Instance.ActiveSelectionScroll();
    //}

    //public void OnClickDeleteFriend()
    //{
    //    for (int i = 0; i < SNS_MessageController.Instance.followingUserParent.childCount; i++)
    //    {
    //        if (SNS_MessageController.Instance.followingUserParent.GetChild(i).gameObject.GetComponent<MessageUserDataScript>().allFollowingRow.userId == allFollowingRow.userId)
    //        {
    //            //Debug.LogError("aaaaaaaaaaaaaaaaa");
    //            SNS_APIController.Instance.allChatMemberList.Remove(allFollowingRow.following.name);
    //            SNS_MessageController.Instance.CreateNewMessageUserList.Remove(allFollowingRow.userId.ToString());
    //            SNS_MessageController.Instance.createNewMessageUserAvatarSPList.Remove(profileImage.sprite);
    //            SNS_MessageController.Instance.ActiveSelectionScroll();
    //            SNS_MessageController.Instance.followingUserParent.GetChild(i).gameObject.GetComponent<MessageUserDataScript>().selectionToggle.isOn = false;
    //            Destroy(this.gameObject);
    //            break;
    //        }
    //    }
    //}

    //public void ResetScrollView()
    //{
    //    SNS_MessageController.Instance.selectionScrollView.transform.parent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    //    SNS_MessageController.Instance.selectionScrollView.transform.parent.parent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    //}

    //#region Get Image From AWS
    //public void GetImageFromAWS(string key, Image mainImage)
    //{
    //    //Debug.LogError("GetImageFromAWS key:" + key);
    //    //GetExtentionType(key);
    //    if (AssetCache.Instance.HasFile(key))
    //    {
    //        //Debug.LogError("Image Available on Disk");
    //        AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
    //        return;
    //    }
    //    else
    //    {
    //        AssetCache.Instance.EnqueueOneResAndWait(key, (ConstantsGod.r_AWSImageKitBaseUrl + key), (success) =>
    //        {
    //            if (success)
    //            {
    //                AssetCache.Instance.LoadSpriteIntoImage(mainImage, key, changeAspectRatio: true);
    //                //Debug.LogError("Save and Image download success");
    //            }
    //        });
    //    }
    //}

    ///*public static ExtentionType currentExtention;
    //public static ExtentionType GetExtentionType(string path)
    //{
    //    if (string.IsNullOrEmpty(path))
    //        return (ExtentionType)0;

    //    string extension = Path.GetExtension(path);
    //    if (string.IsNullOrEmpty(extension))
    //        return (ExtentionType)0;

    //    if (extension[0] == '.')
    //    {
    //        if (extension.Length == 1)
    //            return (ExtentionType)0;

    //        extension = extension.Substring(1);
    //    }

    //    extension = extension.ToLowerInvariant();
    //    //Debug.LogError("ExtentionType " + extension);
    //    if (extension == "png" || extension == "jpg" || extension == "jpeg" || extension == "gif" || extension == "bmp" || extension == "tiff" || extension == "heic")
    //    {
    //        currentExtention = ExtentionType.Image;
    //        return ExtentionType.Image;
    //    }
    //    else if (extension == "mp4" || extension == "mov" || extension == "wav" || extension == "avi")
    //    {
    //        currentExtention = ExtentionType.Video;
    //        return ExtentionType.Video;
    //    }
    //    else if (extension == "mp3" || extension == "aac" || extension == "flac")
    //    {
    //        currentExtention = ExtentionType.Audio;
    //        return ExtentionType.Audio;
    //    }
    //    return (ExtentionType)0;
    //}*/
    //#endregion
}