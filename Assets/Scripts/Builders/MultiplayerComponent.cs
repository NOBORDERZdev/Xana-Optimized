using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MultiplayerComponent : MonoBehaviourPun
{
    string RunTimeItemID;
    string ItemID;
    ItemData itemData;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);
        if (GamificationComponentData.instance.withMultiplayer)
        {
            MultiplayerComponentDatas multiplayerComponentdatas = new MultiplayerComponentDatas();
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gamificationMultiplayerComponentDatas", out object multiplayerComponentdatasObj))
            {

                multiplayerComponentdatas = JsonUtility.FromJson<MultiplayerComponentDatas>(multiplayerComponentdatasObj.ToString());

                if (multiplayerComponentdatas.multiplayerComponents != null)
                {
                    foreach (MultiplayerComponentData multiplayerComponentData in multiplayerComponentdatas.multiplayerComponents)
                    {
                        yield return new WaitForSeconds(0.1f);
                        if (multiplayerComponentData.viewID == photonView.ViewID)
                        {
                            RunTimeItemID = multiplayerComponentData.RuntimeItemID;
                            break;
                        }
                    }
                }
            }
            Rigidbody rb = null;
            gameObject.TryGetComponent(out rb);
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            yield return new WaitForSeconds(3f);

            BuilderMapDownload BMD = SituationChangerSkyboxScript.instance.builderMapDownload;
            foreach (var item in BMD.levelData.otherItems)
            {
                if (item.RuntimeItemID == RunTimeItemID)
                {
                    ItemID = item.ItemID;
                    itemData = item;
                    break;
                }
            }
            string key = "pf" + ItemID + "_XANA";
            //bool flag = false;

            //AsyncOperationHandle _async = AddressableDownloader.Instance.MemoryManager.GetReferenceIfExist(key, ref flag);
            //if (!flag)
            //    _async = Addressables.LoadAssetAsync<GameObject>(key);

            AsyncOperationHandle<GameObject> _async = Addressables.LoadAssetAsync<GameObject>(key);

            while (!_async.IsDone)
            {
                yield return null;
            }
            if (_async.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject newObj = Instantiate(_async.Result as GameObject, this.transform);
                newObj.transform.localPosition = Vector3.zero;
                newObj.transform.localEulerAngles = Vector3.zero;
                Component[] components = newObj.GetComponents<Component>();
                for (int i = components.Length - 1; i >= 0; i--)
                {
                    if (!(components[i] is Transform))
                    {
                        Destroy(components[i]);
                    }
                }
            }
            transform.SetParent(BMD.builderAssetsParent);
            BuilderItem xanaItem = gameObject.AddComponent<BuilderItem>();
            xanaItem.itemData = itemData;
            //if (!GamificationComponentData.instance.xanaItems.Exists(x => x == xanaItem))
            //    GamificationComponentData.instance.xanaItems.Add(xanaItem);
            if (PhotonNetwork.IsMasterClient || (itemData.addForceComponentData.isActive || itemData.translateComponentData.avatarTriggerToggle))
                xanaItem.SetData(itemData);
            if (!GamificationComponentData.instance.multiplayerComponentsxanaItems.Exists(x => x == xanaItem))
                GamificationComponentData.instance.multiplayerComponentsxanaItems.Add(xanaItem);
        }
    }
}