using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
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
    [Header("To Manage Penpenz Mini Game")]
    public bool isPenpenzMiniGame;

    [HideInInspector]
    public string WorldId;
    private bool alreadyTriggered;

    [Header("Dome Type and Category")]
    public DomeType _domeType;
    public DomeCategory _domeCategory;

    private void OnEnable()
    {
        if (APIBasepointManager.instance.IsXanaLive)
            WorldId = WorldIdMainnet;
        else
            WorldId = WorldIdTestnet;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.InRoom && !MutiplayerController.instance.isShifting)
        {
            if (other.GetComponent<PhotonView>() && other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine && !alreadyTriggered)
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
        //GameplayEntityLoader.instance.AssignRaffleTickets(DomeId);
        BuilderEventManager.LoadNewScene?.Invoke(DomeId, transform.GetChild(0).transform.position);
    }

    void TriggerSceneLoading(string WorldId)
    {
        ConstantsHolder.isSoftBankGame = isPenpenzMiniGame;
        if (isPenpenzMiniGame)
            ConstantsHolder.isPenguin = true;
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
        {
            ConstantsHolder.isFromXANASummit = true;
            ReferencesForGamePlay.instance.ChangeExitBtnImage(false);
        }
        if (HaveSubworlds)
        {
            ConstantsHolder.HaveSubWorlds = true;
            ConstantsHolder.domeId = DomeId;
        }
        ConstantsHolder.DomeType = _domeType.ToString();
        ConstantsHolder.DomeCategory = _domeCategory.ToString();
    }

    public enum DomeType
    {
        None,
        Game,
        Exhibition
    }

    public enum DomeCategory
    {
        None,
        Business,
        Sports,
        Music,
        Art,
        Education,
        Healing,
        Action,
        Race,
        Adventure,
        Story,
        NFT,
        DAO,
        Fun,
        Horror,
        Quiz,
        Idol,
        Vtuber,
        Space,
        AI,
        Local,
        Blockchain,
        Finance
    }
}
