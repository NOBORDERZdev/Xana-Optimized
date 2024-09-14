using Photon.Pun;
using System.Threading.Tasks;
using UnityEngine;


public class OnTriggerSceneSwitch : MonoBehaviour
{
    [Tooltip("Subworld data is loading admin panel then only required")]
    public int DomeId;
    public string WorldIdTestnet;
    public string WorldIdMainnet;
    public GameObject textMeshPro;
    [Header("To Manage subworld loading from admin")]
    public bool LoadDirectly;
    public bool LoadingFromSummitWorld;
    public bool HaveSubworlds;
    [HideInInspector]
    public string WorldId;
    private bool alreadyTriggered;

    private void OnEnable()
    {
        if (APIBasepointManager.instance.IsXanaLive)
            WorldId = WorldIdMainnet;
        else
            WorldId = WorldIdTestnet;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.InRoom)
        {
            if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine && !alreadyTriggered)
            {
                alreadyTriggered = true;
             
                if (DomeId == -1 || LoadDirectly)
                {
                    TriggerSceneLoading(WorldId);
                }
                else
                    TriggerSceneLoading();

                DisableCollider();
            }
        }
    }

    void TriggerSceneLoading()
    {
        BuilderEventManager.spaceXDeactivated?.Invoke();
        //GameplayEntityLoader.instance.AssignRaffleTickets(DomeId);
        BuilderEventManager.LoadNewScene?.Invoke(DomeId, transform.GetChild(0).transform.position);
    }

    void TriggerSceneLoading(string WorldId)
    {
        CheckSceneParemeter();
        BuilderEventManager.LoadSceneByName?.Invoke(WorldId, transform.GetChild(0).transform.position);
    }

    async void DisableCollider()
    {
        await Task.Delay(2000);
        alreadyTriggered = false;
    }


    void CheckSceneParemeter()
    {
        if (LoadingFromSummitWorld)
            ConstantsHolder.isFromXANASummit = true;
        if(HaveSubworlds)
        {
            ConstantsHolder.HaveSubWorlds = true;
            ConstantsHolder.domeId = DomeId;
        }
    }
}
