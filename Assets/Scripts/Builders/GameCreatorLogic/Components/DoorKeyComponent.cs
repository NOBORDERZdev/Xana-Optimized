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
            if (PlayerCanvas.Instance.transform.parent != ArrowManager.Instance.nameCanvas.transform)
            {
                PlayerCanvas.Instance.transform.SetParent(ArrowManager.Instance.nameCanvas.transform);
                PlayerCanvas.Instance.transform.localPosition = Vector3.up * 18.5f;

            }

            PlayerCanvas.Instance.cameraMain = GamificationComponentData.instance.playerControllerNew.ActiveCamera.transform;
            if (this.doorKeyComponentData.isKey && !this.doorKeyComponentData.isDoor)
            {
                if (!KeyValidation()) return;

                _other.gameObject.GetComponent<KeyValues>()._dooKeyValues.Add(this.doorKeyComponentData.selectedKey);
                Debug.LogError("DoorKey Component RuntimeItemID "+ RuntimeItemID);
                PlayerCanvas.Instance.ToggleKey(true);
                //this.gameObject.SetActive(false);
                GamificationComponentData.instance.doorKeyCount++;
                PlayerCanvas.Instance.keyCounter.text = "x" + GamificationComponentData.instance.doorKeyCount;
                if (GamificationComponentData.instance.DoorKeyObject == null)
                    GamificationComponentData.instance.DoorKeyObject = PhotonNetwork.Instantiate("DoorKey", Vector3.zero, Quaternion.identity);
                var hash = new ExitGames.Client.Photon.Hashtable();
                hash.Add("doorKeyCount", GamificationComponentData.instance.doorKeyCount);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                if (GamificationComponentData.instance.withMultiplayer)
                    GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, Constants.ItemComponentType.none);
                else GamificationComponentData.instance.GetObjectwithoutRPC(RuntimeItemID, Constants.ItemComponentType.none);

                BuilderEventManager.onComponentActivated?.Invoke(_componentType);
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
                        GamificationComponentData.instance.doorKeyCount--;
                        var hash = new ExitGames.Client.Photon.Hashtable();
                        hash.Add("doorKeyCount", GamificationComponentData.instance.doorKeyCount);
                        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                        if (values._dooKeyValues.Count <= 0)
                        {
                            PlayerCanvas.Instance.ToggleKey(false);
                            if (GamificationComponentData.instance.DoorKeyObject != null)
                                PhotonNetwork.Destroy(GamificationComponentData.instance.DoorKeyObject.GetPhotonView());
                        }
                        isDoorFind = true;
                        PlayerCanvas.Instance.keyCounter.text = "x" + values._dooKeyValues.Count.ToString();
                        break;
                    }
                }


                if (isDoorFind)
                {
                    //this.gameObject.SetActive(false);
                    if (GamificationComponentData.instance.withMultiplayer)
                        GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, Constants.ItemComponentType.none);
                    else GamificationComponentData.instance.GetObjectwithoutRPC(RuntimeItemID, Constants.ItemComponentType.none);
                    //Toast.Show("The keys match!");
                    BuilderEventManager.OnDoorKeyCollisionEnter?.Invoke("The keys match!");

                    ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.DoorOpen);

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