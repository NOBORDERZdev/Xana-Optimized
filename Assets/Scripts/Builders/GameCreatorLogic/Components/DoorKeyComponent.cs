using UnityEngine;
using Models;
using Photon.Pun;

public class DoorKeyComponent : ItemComponent
{
    private DoorKeyComponentData doorKeyComponentData;

    private bool activateComponent = false;
    string RuntimeItemID = "";

    public void Init(DoorKeyComponentData _doorKeyComponentData)
    {
        this.doorKeyComponentData = _doorKeyComponentData;
        activateComponent = true;
        RuntimeItemID = this.GetComponent<XanaItem>().itemData.RuntimeItemID;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (PlayerCanvas.Instance.transform.parent != _other.transform)
            {
                PlayerCanvas.Instance.transform.SetParent(_other.transform);
                PlayerCanvas.Instance.transform.localPosition = Vector3.up * PlayerCanvas.Instance.transform.localPosition.y;

            }
            PlayerCanvas.Instance.cameraMain = GamificationComponentData.instance.playerControllerNew.ActiveCamera.transform;
            if (this.doorKeyComponentData.isKey && !this.doorKeyComponentData.isDoor)
            {
                if (!KeyValidation()) return;

                _other.gameObject.GetComponent<KeyValues>()._dooKeyValues.Add(this.doorKeyComponentData.selectedKey);

                PlayerCanvas.Instance.ToggleKey(true);
                //this.gameObject.SetActive(false);
                PlayerCanvas.Instance.keyCounter.text = "x" + _other.gameObject.GetComponent<KeyValues>()._dooKeyValues.Count.ToString();
                GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.AllBuffered, RuntimeItemID, Constants.ItemComponentType.none);
            }


            if (this.doorKeyComponentData.isDoor && !this.doorKeyComponentData.isKey)
            {

                if (!DoorKeyValidation()) return;
                bool isDoorFind = false;
                KeyValues values = _other.gameObject.GetComponent<KeyValues>();
                foreach (var item in values._dooKeyValues)
                {
                    if (item.Equals(this.doorKeyComponentData.selectedDoorKey.ToString()))
                    {
                        values._dooKeyValues.Remove(item);
                        if (values._dooKeyValues.Count <= 0)
                            PlayerCanvas.Instance.ToggleKey(false);

                        isDoorFind = true;
                        PlayerCanvas.Instance.keyCounter.text = "x" + values._dooKeyValues.Count.ToString();
                        break;
                    }
                }


                if (isDoorFind)
                {
                    //this.gameObject.SetActive(false);
                    GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.AllBuffered, RuntimeItemID, Constants.ItemComponentType.none);
                    Toast.Show("The keys match!");
                    return;
                }
                if (values._dooKeyValues.Count > 0)
                    PlayerCanvas.Instance.ToggleWrongKey();

            }
        }
    }
    private bool KeyValidation()
    {
        if (this.doorKeyComponentData.selectedKey.Equals("Select Key")) return false;
        if (string.IsNullOrWhiteSpace(this.doorKeyComponentData.selectedKey)) return false;
        return true;
    }
    private bool DoorKeyValidation()
    {
        if (this.doorKeyComponentData.selectedDoorKey.Equals("Select Key")) return false;
        if (string.IsNullOrWhiteSpace(this.doorKeyComponentData.selectedDoorKey)) return false;
        return true;
    }

    #region BehaviourControl
    private void StartComponent()
    {

    }
    private void StopComponent()
    {


    }

    public override void StopBehaviour()
    {
        isPlaying = false;
        StopComponent();
    }

    public override void PlayBehaviour()
    {
        isPlaying = true;
        StartComponent();
    }

    public override void ToggleBehaviour()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
            PlayBehaviour();
        else
            StopBehaviour();
    }
    public override void ResumeBehaviour()
    {
        PlayBehaviour();
    }

    public override void AssignItemComponentType()
    {
        _componentType = Constants.ItemComponentType.DoorKeyComponent;
    }

    #endregion
}