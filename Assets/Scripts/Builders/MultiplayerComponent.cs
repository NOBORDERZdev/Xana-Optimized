using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MultiplayerComponent : MonoBehaviour
{
    public string RunTimeItemID;
    string _itemID;
    ItemData _itemData;
    IEnumerator start()
    {
        if (GamificationComponentData.instance != null && GamificationComponentData.instance.withMultiplayer)
        {
            Rigidbody _rbTemp = null;
            gameObject.TryGetComponent(out _rbTemp);
            if (_rbTemp == null)
                _rbTemp = gameObject.AddComponent<Rigidbody>();
            _rbTemp.isKinematic = true;

            BuilderMapDownload BMD = SituationChangerSkyboxScript.instance.builderMapDownload;
            foreach (var item in BMD.levelData.otherItems)
            {
                if (item.RuntimeItemID == RunTimeItemID)
                {
                    _itemID = item.ItemID;
                    _itemData = item;
                    break;
                }
            }
            string key = "pf" + _itemID + "_XANA";

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
                newObj.transform.localScale = _itemData.Scale;
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
            XanaItem xanaItem = gameObject.AddComponent<XanaItem>();
            xanaItem.itemData = _itemData;
            //if (!GamificationComponentData.instance.xanaItems.Exists(x => x == xanaItem))
            //    GamificationComponentData.instance.xanaItems.Add(xanaItem);
            //if (_itemData.addForceComponentData.isActive || _itemData.translateComponentData.avatarTriggerToggle)
            //    xanaItem.SetData(_itemData);
            if (!GamificationComponentData.instance.multiplayerComponentsxanaItems.Exists(x => x == xanaItem))
                GamificationComponentData.instance.multiplayerComponentsxanaItems.Add(xanaItem);
            if (!GamificationComponentData.instance.xanaItems.Exists(x => x == xanaItem))
                GamificationComponentData.instance.xanaItems.Add(xanaItem);
        }
    }
}