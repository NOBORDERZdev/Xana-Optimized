using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;

public class RaffleTicketHandler : MonoBehaviour
{

    [Header("*UI References*")]
    [SerializeField] private TextMeshProUGUI _displayTicketCountTextSmallLandScape;
    [SerializeField] private TextMeshProUGUI _displayTicketCountTextLargeLandScape;
    [SerializeField] private TextMeshProUGUI _displayTicketCountTextSmallPotrait;
    [SerializeField] private TextMeshProUGUI _displayTicketCountTextLargePotrait;
    [SerializeField] private TextMeshProUGUI _ticketCountTextLandScape;
    [SerializeField] private TextMeshProUGUI _ticketCountTextPotrait;
    [SerializeField] private TextMeshProUGUI _giftTicketsPopUpDescriptionTextLandScape;
    [SerializeField] private TextMeshProUGUI _giftTicketsPopUpDescriptionTextPotrait;
    [Header("*GameObject References*")]
    [SerializeField] private GameObject _giftTicketsPopUpLandScape;
    [SerializeField] private GameObject _giftTicketsPopUpPotrait;
    [Header("*API Data*")]
    [SerializeField] private Pavilion _summitDomesVisitedByUser;
    [SerializeField] private RaffleTicketsCount _summitRaffleTicketsEranedByUser;
    [Header("*variables*")]
    [SerializeField] private List<int> _allVisitedDomeIds = new List<int>();
    [SerializeField] private int _totalNumberOfDomes;
    [SerializeField] private int _totalNumberOfTickets;
     private int _earnTicketsInOneCycle;

    // Start is called before the first frame update
    void Start()
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
            Debug.Log("Error update Domes Visit" + www.error);

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
            Debug.Log("Sucessfully update Tickets");
        else
            Debug.Log("Error update Tickets" + " " + www.error + " " + www.result);

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
    private void UpdateUI()
    {
        _displayTicketCountTextSmallLandScape.text = "(" + _totalNumberOfTickets + ")";
        _displayTicketCountTextLargeLandScape.text= "(" + _totalNumberOfTickets + ")";
        _displayTicketCountTextSmallPotrait.text= "(" + _totalNumberOfTickets + ")";
        _displayTicketCountTextLargePotrait.text= "(" + _totalNumberOfTickets + ")";
    }
    private void TransferDatatoMainDomeList()
    {
        foreach (var item in _summitDomesVisitedByUser.domeVisits)
        {
            _allVisitedDomeIds.Add(item.domeId);
        }
        _totalNumberOfTickets = _summitRaffleTicketsEranedByUser.tickets;
        UpdateUI();
    }
    private void UpdateTicketsData()
    {
        _earnTicketsInOneCycle = 1;
        _totalNumberOfTickets += _earnTicketsInOneCycle;
        CheckForAnyRaffleticketGifts(_totalNumberOfTickets);
    }
    private void CheckForAnyRaffleticketGifts(int RaffleTickets)
    {
        if (RaffleTickets % 5 == 0)
        {
            _earnTicketsInOneCycle += 5;
            _totalNumberOfTickets += _earnTicketsInOneCycle;
            StartCoroutine(SaveUpdatedTicketsCount(_earnTicketsInOneCycle-1));
            StartCoroutine(RewardPopUp("Gift 5 extra tickets your total coin count is a multiple of 5!","05"));
        }
        if (_totalNumberOfDomes == _allVisitedDomeIds.Count)
        {
            _earnTicketsInOneCycle += 50;
            _totalNumberOfTickets += _earnTicketsInOneCycle;
            StartCoroutine(SaveUpdatedTicketsCount(_earnTicketsInOneCycle-1));
            StartCoroutine(RewardPopUp("50 raffle tickets assign to you for the completion of each summit dome ","50"));
        }
        _earnTicketsInOneCycle = 0;
        UpdateUI();
    }
    IEnumerator RewardPopUp(string value,string number)
    {
        yield return new WaitForSeconds(5f);
        _giftTicketsPopUpDescriptionTextPotrait.text = value;
        _ticketCountTextLandScape.text = number;
        _ticketCountTextPotrait.text = number;

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