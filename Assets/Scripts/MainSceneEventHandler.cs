using System;
using Unity.VisualScripting;

public static class MainSceneEventHandler
{
    //when you need to open the preset panel use this event
    public static Action OpenPresetPanel;
    //Events 
    public static Action OnSucessFullLogin;

    public static Action OnBackRefAssign;

    //when back from Gameplay scene this action release unused memory after loading all scenes
    public static Action MemoryRelaseAfterLoading;

    public static Action BackHomeSucessfully;

    public static Action MakeScreenSpaceAdditive;

    public static Action OpenLandingScene;
}
