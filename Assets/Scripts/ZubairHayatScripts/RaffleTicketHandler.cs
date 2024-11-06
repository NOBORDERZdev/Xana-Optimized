using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class RaffleTicketHandler : MonoBehaviour
{

    [Header("*UI References*")]
    [SerializeField] private TextMeshProUGUI _displayTicketCountTextSmallLandScape;
    [SerializeField] private TextMeshProUGUI _displayTicketCountTextSmallPotrait;
    [SerializeField] private TextMeshProUGUI _ticketCountTextLandScape;
    [SerializeField] private TextMeshProUGUI _ticketCountTextPotrait;
    [SerializeField] private TextMeshProUGUI _giftTicketsPopUpDescriptionTextLandScape;
    [SerializeField] private TextMeshProUGUI _giftTicketsPopUpDescriptionTextPotrait;
    [Header("*GameObject References*")]
    [SerializeField] private GameObject _giftTicketsPopUpLandScape;
    [SerializeField] private GameObject _giftTicketsPopUpPotrait;
    [SerializeField] private GameObject _raffleTicketsInfoPopup;
    [Header("*API Data*")]
    [SerializeField] private Pavilion _summitDomesVisitedByUser;
    [SerializeField] private RaffleTicketsCount _summitRaffleTicketsEranedByUser;
    [Header("*variables*")]
    [SerializeField] private List<int> _allVisitedDomeIds = new List<int>();
    [SerializeField] private int _totalNumberOfDomes;
    [SerializeField] private int _totalNumberOfTickets;
    private int _earnTicketsInOneCycle;
    private int _tempTotalTickets;

    // Start is called before the first frame update
    void Awake()
    {
        GetAllvisitedDomeIds();
    }

    #region API Calling
    private async void GetAllvisitedDomeIds()
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.GetUserVisitedDomes;
        UnityWebRequest response = UnityWebRequest.Get(url);
        response.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        try
        {
            await response.SendWebRequest();
            if (response.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(response.error);
            }
            else
            {
                _summitDomesVisitedByUser = JsonConvert.DeserializeObject<Pavilion>(response.downloadHandler.text.ToString());
            }

        }
        catch (System.Exception ex)
        {

        }
        response.Dispose();
        GetAllRaffleTickets();
    }
    private async void GetAllRaffleTickets()
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.GetUserRaffleTickets;
        UnityWebRequest response = UnityWebRequest.Get(url);
        response.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        try
        {
            await response.SendWebRequest();
            if (response.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(response.error);
            }
            else
            {
                _summitRaffleTicketsEranedByUser = JsonConvert.DeserializeObject<RaffleTicketsCount>(response.downloadHandler.text.ToString());
            }

        }
        catch (System.Exception ex)
        {

        }
        response.Dispose();
        TransferDatatoMainDomeList();
    }
    private IEnumerator SaveUpdatedDomeVisitCount(int dome_Id)
    {
        string token = ConstantsGod.AUTH_TOKEN;
        UnityWebRequest www;
        www = UnityWebRequest.Post(ConstantsGod.API_BASEURL + ConstantsGod.UpdateVisitedDomes + dome_Id, "POST");
        www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();
        while (!www.isDone)
        {
            yield return null;
        }
        if (www.result != UnityWebRequest.Result.ConnectionError && www.result != UnityWebRequest.Result.ProtocolError)
            Debug.Log("Sucessfully update Domes Visit");
        else
            Debug.Log("Error update Domes Visit");

        www.Dispose();
    }

    private IEnumerator SaveUpdatedTicketsCount(int raffleTickets)
    {
        string token = ConstantsGod.AUTH_TOKEN;
        UnityWebRequest www;
        string jsonData = "{\"bonusTickets\":" + raffleTickets + "}";
        www = UnityWebRequest.Put(ConstantsGod.API_BASEURL + ConstantsGod.UpdateUserRaffleTickets, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();
        while (!www.isDone)
        {
            yield return null;
        }
        if (www.result != UnityWebRequest.Result.ConnectionError && www.result != UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Sucessfully update Tickets");
            ShowPopupOnSuccessTicketGain();
        }
        else
            Debug.Log("Tickets-- Something went wrong -- "+ www.downloadHandler.text);

        www.Dispose();
    }
    #endregion

    #region Custom Functions
    public void UpdateData(int id)
    {
        if (!_allVisitedDomeIds.Contains(id))
        {
            _allVisitedDomeIds.Add(id);
            StartCoroutine(SaveUpdatedDomeVisitCount(id));
            UpdateTicketsData();
        }
    }
    public void RaffleTciketsInfo()
    {
        if (_raffleTicketsInfoPopup.activeSelf)
            return;
        else
            _raffleTicketsInfoPopup.SetActive(true);

    }
    private void UpdateUI()
    {
        //_displayTicketCountTextSmallLandScape.text = "(" + _totalNumberOfTickets + ")";
        _displayTicketCountTextSmallLandScape.text ="" + _totalNumberOfTickets ;
        _displayTicketCountTextSmallPotrait.text = "(" + _totalNumberOfTickets + ")";
    }

    private void TransferDatatoMainDomeList()
    {
        _totalNumberOfTickets = _summitRaffleTicketsEranedByUser.tickets;
        UpdateUI();

        if (_summitDomesVisitedByUser.domeVisits == null || _summitDomesVisitedByUser.domeVisits.Count == 0)
        {
            return;
        }

        foreach (var item in _summitDomesVisitedByUser.domeVisits)
        {
            _allVisitedDomeIds.Add(item.domeId);
        }
    }
    private void UpdateTicketsData()
    {
        _earnTicketsInOneCycle = 1;
        _tempTotalTickets = _totalNumberOfTickets;
        _tempTotalTickets += _earnTicketsInOneCycle;
        CheckForAnyRaffleticketGifts(_tempTotalTickets);
    }
    private void CheckForAnyRaffleticketGifts(int RaffleTickets)
    {

        if (_totalNumberOfDomes == _allVisitedDomeIds.Count)
        {
            _earnTicketsInOneCycle += 50;
            _tempTotalTickets += _earnTicketsInOneCycle;
            StartCoroutine(RewardPopUp("You have received your gift!", "50", 8f));
            _earnTicketsInOneCycle = 0;
        }
        else if (RaffleTickets % 5 == 0)
        {
            _earnTicketsInOneCycle += 4;
            _tempTotalTickets += _earnTicketsInOneCycle;
            _earnTicketsInOneCycle = 0;
            StartCoroutine(RewardPopUp("You have received your gift!", "05", 5f));
        }
        else
        {
            StartCoroutine(RewardPopUp("You have received your gift!", "01", 5f));
        }
        //UpdateUI();
    }
    IEnumerator RewardPopUp(string statement, string number,float time)
    {
        yield return new WaitForSeconds(time);

        switch (number)
        {
            case "01":
                StartCoroutine(SaveUpdatedTicketsCount(1));
                break;
            case "05":
                StartCoroutine(SaveUpdatedTicketsCount(5));
                break;
            case "50":
                StartCoroutine(SaveUpdatedTicketsCount(50));
                break;
        }

        _giftTicketsPopUpDescriptionTextPotrait.text = statement;
        _giftTicketsPopUpDescriptionTextLandScape.text = statement;

        _ticketCountTextLandScape.text = number;
        _ticketCountTextPotrait.text = number;
    }

    void ShowPopupOnSuccessTicketGain()
    {
        // Increase the Count When API Call is Successfull
        _totalNumberOfTickets = _tempTotalTickets;
        UpdateUI();

        if (ScreenOrientationManager._instance.isPotrait)
            _giftTicketsPopUpPotrait.SetActive(true);
        else
            _giftTicketsPopUpLandScape.SetActive(true);

    }

    #endregion
}

#region Jason Format 
[System.Serializable]
public class Pavilion
{
    public List<PavilionID> domeVisits;
}
[System.Serializable]
public class PavilionID
{
    public int domeId;
}
[System.Serializable]
public class RaffleTicketsCount
{
    public int tickets = 0;
}

#endregion