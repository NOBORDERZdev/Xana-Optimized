using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFE3D;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class TimeLineManager : MonoBehaviour
{
    public static TimeLineManager instance;

    public TimelineAsset introCutScene, outroCutScene;
    private PlayableDirector playableDirector;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        StartIntroCutScene();
    }

    public void StartIntroCutScene()
    {
        // Assign the timeline asset to the Playable Director
        playableDirector.playableAsset = introCutScene;
        playableDirector.Play();
    }

    public void IntroCutSceneEnded()
    {
        UFE._StartGameGUI(1);
    }

    public void StartOutroCutScene()
    {
        // Assign the timeline asset to the Playable Director
        playableDirector.playableAsset = outroCutScene;
        playableDirector.Play();
        FightingGameManager.instance.FindPlayersAndManageWin();
    }

    public void OutroCutSceneEnded()
    {
        BattleGUI battleGUI = new BattleGUI();
        battleGUI.OnGameEnded();
    }
}
