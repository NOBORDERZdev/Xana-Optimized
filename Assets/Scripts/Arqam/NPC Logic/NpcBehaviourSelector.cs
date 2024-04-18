using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NPC;

public class NpcBehaviourSelector : MonoBehaviour
{
    [SerializeField] NpcMovementController movementController;
    [SerializeField] NpcJump npcJump;
    [SerializeField] NpcSelfie npcSelfie;
    [SerializeField] NpcFreeCam npcFreeCam;
    [SerializeField] NpcEmotes npcEmotes;
    [Space(5)]
    [SerializeField] TMP_Text nameTxt;

    [HideInInspector]
    public bool isPerformingAction = false;
    [HideInInspector]
    public Coroutine ActionCoroutine = null;

    private Coroutine emoteCoroutine;
    private bool isNewlySpwaned = true;
    [SerializeField] int maxNpcBehaviourAction = 6;

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
                rand = Random.Range(1, maxNpcBehaviourAction);


            switch (rand)
            {
                case 0:                     // for wandering ai
                    movementController.Wander();
                    npcSelfie.ForceFullyDisableSelfie();

                    StopEmotes();
                    break;
                case 1:                   // for jump
                    npcJump.AiJump();

                    StopEmotes();
                    break;
                case 2:                    // for selfie control
                    npcSelfie.SelfieAction();

                    StopEmotes();
                    break;
                case 3:                  // for freeCam mode
                    npcFreeCam.PerformFreeCam();

                    StopEmotes();
                    break;
                case 4:                   // for emotes display
                    if (emoteCoroutine != null)
                        StopCoroutine(emoteCoroutine);
                    emoteCoroutine = StartCoroutine(npcEmotes.PlayEmote());
                    npcSelfie.ForceFullyDisableSelfie();
                    break;
                default:                // for default case
                    movementController.Wander();
                    npcSelfie.ForceFullyDisableSelfie();

                    StopEmotes();
                    break;
            }
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
        nameTxt.text = name;
    }

}
