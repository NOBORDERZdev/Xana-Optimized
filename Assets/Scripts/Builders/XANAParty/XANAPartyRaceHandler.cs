using UnityEngine;

public class XANAPartyRaceHandler : MonoBehaviour
{
    public bool ForceRaceStart = false;

    void OnEnable()
    {
        BuilderEventManager.AfterWorldInstantiated += StartRaceSinglePlayer;
    }

    void OnDisable()
    {
        BuilderEventManager.AfterWorldInstantiated -= StartRaceSinglePlayer;
    }


    private void Update()
    {
        if (ForceRaceStart)
        {
            new Delayed.Action(() => { BuilderEventManager.XANAPartyRaceStart?.Invoke(); }, 1f);
            ForceRaceStart = false;
        }
    }

    void StartRaceSinglePlayer()
    {
        if (GamificationComponentData.instance.SinglePlayer)
        {
            new Delayed.Action(() => { BuilderEventManager.XANAPartyRaceStart?.Invoke(); }, 1f);
        }
    }

}