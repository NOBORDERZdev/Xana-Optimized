using NPC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SPAAIBehvrController : MonoBehaviour
{
    [SerializeField] NpcMovementController movementController;
    [SerializeField] SPAAIEmoteController npcEmotes;
    [SerializeField] NpcEmotes OldnpcEmotes;

    //[Space(5)]
    //[SerializeField] TMP_Text nameTxt;

    [HideInInspector]
    public bool isPerformingAction = false;
    [HideInInspector]
    public Coroutine ActionCoroutine = null;

    private Coroutine emoteCoroutine;
    private bool isNewlySpwaned = true;
    [SerializeField] int maxNpcBehaviourAction = 2;

    public SPAAIHandler spaAIHandlerRef;

    private void OnEnable()
    {
        if (spaAIHandlerRef)
        {
            if (spaAIHandlerRef.IsAIDataFetched & spaAIHandlerRef.IsPlayerTriggered)
            {
                isPerformingAction = false;
                StartCoroutine(PerformAction());
            }
        }
    }

    public IEnumerator PerformAction()
    {
        if (!isPerformingAction)
        {
            Debug.Log("============Perform Action Call");
            isPerformingAction = true;
            //int rand;
            //if (isNewlySpwaned)
            //{
            //    rand = 0;
            //    isNewlySpwaned = false;
            //}
            //else
            //    rand = Random.Range(0, maxNpcBehaviourAction);


            //switch (rand)
            //{
            //    case 0:                     // for wandering ai
            //        movementController.Wander();
            //        StopEmotes();
            //        break;
            //    case 1:                   // for AI Emote Playing
            if (emoteCoroutine != null)
            {
                StopCoroutine(emoteCoroutine);
            }
            if (OldnpcEmotes)
            {
                emoteCoroutine = StartCoroutine(OldnpcEmotes.PlayEmote());
            }
            else
            {
                emoteCoroutine = StartCoroutine(npcEmotes.PlayEmote());
            }
            //        break;
            //    default:                // for default case
            //        movementController.Wander();
            //        StopEmotes();
            //        break;
            //}
        }
        yield return null;
    }

    private void StopEmotes()
    {
        if (emoteCoroutine != null)
            StopCoroutine(emoteCoroutine);

        npcEmotes.ForceFullyStopEmote();
    }

    public void SetAiName(string name)
    {
        //nameTxt.text = name;
    }
}
