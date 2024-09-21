using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceForPenguinAvatar : MonoBehaviour
{
    [Header("XANA Controller UI")]
    [Header("Landsacape")]
    [SerializeField] public GameObject XanaFeaturesLandsacape;
    [SerializeField] public GameObject XanaJumpLandsacape;
    [SerializeField] public GameObject EmoteFavLandsacape;
    [SerializeField] public GameObject EmotePanelsLandsacape;
    [SerializeField] public GameObject ReactionPanelsLandsacape;


    [Header("Potrait")]
    [SerializeField] public GameObject XanaFeaturesPotraite;
    [SerializeField] public GameObject XanaJumpPotraite;
    [SerializeField] public GameObject EmoteFavPotraite;
    [SerializeField] public GameObject EmotePanelsPotraite;


    public GameObject penguinJump;
    public GameObject penguinJumpPot;

    public void ActiveXanaUIData(bool isEnable)
    {
        XanaFeaturesLandsacape.SetActive(isEnable);
        EmoteFavLandsacape.SetActive(isEnable);
        EmotePanelsLandsacape.SetActive(isEnable);


        XanaFeaturesPotraite.SetActive(isEnable);
        EmoteFavPotraite.SetActive(isEnable);
        EmotePanelsPotraite.SetActive(isEnable);

        if (isEnable)
        {
            XanaJumpPotraite.SetActive(true);
            XanaJumpLandsacape.SetActive(true);

            Destroy(penguinJump);
            Destroy(penguinJumpPot);
        }
        else
        {

            penguinJump = Instantiate(XanaJumpLandsacape, XanaJumpLandsacape.transform.parent);
            penguinJumpPot = Instantiate(XanaJumpPotraite, XanaJumpPotraite.transform.parent);
            
            Destroy(penguinJump.GetComponent<UnityEngine.EventSystems.EventTrigger>());
            Destroy(penguinJumpPot.GetComponent<UnityEngine.EventSystems.EventTrigger>());

            XanaJumpPotraite.SetActive(false);
            XanaJumpLandsacape.SetActive(false);
        }

        GameplayEntityLoader.instance.PositionResetButton.SetActive(false);
    }

}
