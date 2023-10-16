using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NPC;

public class NpcBehaviourSelector : MonoBehaviour
{
    [SerializeField] NpcMovementController movementController;
    [SerializeField] TMP_Text nameTxt;

    //[HideInInspector]
    public bool isPerformingAction = false;
    [HideInInspector]
    public Coroutine ActionCoroutine = null;    

    private Coroutine emoteCoroutine;
    private bool isNewlySpwaned = true;
    private int maxNpcBehaviourAction = 5;

    private void Start()
    {
        StartCoroutine(PerformAction());
    }

    public IEnumerator PerformAction()
    {
        if (!isPerformingAction)
        {
            isPerformingAction = true;
            int rand;
            if (isNewlySpwaned)
            {
                rand = 0;
                isNewlySpwaned = false;
            }
            else
                rand = 0; // Random.Range(0, maxNpcBehaviourAction);


            switch (rand)
            {
                case 0:                     // for wandering ai
                    movementController.Wander();
                    //selfie.ForceFullyDisableSelfie();
                    //if (emoteCoroutine != null)
                    //    StopCoroutine(emoteCoroutine);
                    //aiEmote.ForceFullyStopEmote();
                    break;
                //case 1:                    // for selfie control
                //    selfie.SelfieAction();
                //    if (emoteCoroutine != null)
                //        StopCoroutine(emoteCoroutine);
                //    aiEmote.ForceFullyStopEmote();
                //    break;
                //case 2:                   // for emotes display
                //    if (emoteCoroutine != null)
                //        StopCoroutine(emoteCoroutine);
                //    emoteCoroutine = StartCoroutine(aiEmote.PlayEmote());
                //    selfie.ForceFullyDisableSelfie();
                //    break;
                //case 3:                   // for jump
                //    aIJump.AiJump();
                //    if (emoteCoroutine != null)
                //        StopCoroutine(emoteCoroutine);
                //    aiEmote.ForceFullyStopEmote();
                //    break;
                //case 4:                  // for freeCam mode
                //    freeCam.PerformFreeCam();
                //    if (emoteCoroutine != null)
                //        StopCoroutine(emoteCoroutine);
                //    aiEmote.ForceFullyStopEmote();
                //    break;
                //default:                // for default case
                //    wandering.Wander();
                //    selfie.ForceFullyDisableSelfie();
                //    if (emoteCoroutine != null)
                //        StopCoroutine(emoteCoroutine);
                //    aiEmote.ForceFullyStopEmote();
                //    break;
            }
        }
        yield return null;
    }

    public void SetAiName(string name)
    {
        nameTxt.text = name;
    }

}
