using UnityEngine;

//Project Constants
public static class Constants
{
    public static int cursorLayer = 8;
    public static int defaultLayer = 0;
    public static int itemLayer = 9;
    public const string ItemTag = "Item";
    public const string GroundTag = "Footsteps/grass";
    public const string WaterTag = "Footsteps/water";

    public const float minPlayerUISpeed = 1f;
    public const float maxPlayerUISpeed = 20f;
    public const float minPlayerSprintSpeed = 4.5f;
    public const float maxPlayerSprintSpeed = 15f;
    public const float minPlayerUIJump = 1f;
    public const float maxPlayerUIJump = 20f;
    public const float minPlayerJumpHeight = 2f;
    public const float maxPlayerJumpHeight = 15f;

    public const string HomeScene = "Home";
    public const string BuilderScene = "XanaBuilder";
    public const string PlayerScene = "XanaPlayer";
    public const string MoralisScene = "Moralis";
    //public const string GetSingleWorldAPI = "https://api-test.xana.net/item/update-world/get-single-world/";

    public const string GetSingleWorldAPI = "https://api-test.xana.net/item/get-single-world/";

    public enum GameMode
    {
        play,
        edit
    }

    public enum GizmoMode
    {
        select,
        move,
        rotate,
        scale,
        None
    }

    public enum PlacedItemMode
    {
        Placed,
        Edit,
        Fail
    }

    public enum GizmoId
    {
        Move = 1,
        Rotate,
        Scale,
        Universal,
        None
    }

    public enum ItemType
    {
        ground,
        other
    }

    public enum Position
    {
        x,
        z,
        y
    }

    public enum Scale
    {
        x,
        z,
        y
    }

    public enum TerrainProperty
    {
        x,
        z
    }

    public enum TerrainScaleButtonPos
    {
        up,
        down,
        left,
        right
    }

    public enum ItemCategory
    {
        All,
        New,
        Props,
        Furniture,
        Foods,
        Traffic,
        Buildings,
        Env,
        Food,
        Etc,
        Signs,
        Effect,
        Light,
        Npc,
        Spawn,
        Cube,
        Polygon,
        Pillar,
        Stair,
        Custom,
        Round,
        Arch,
        Attach,
        Text
    }

    public enum ItemTheme
    {
        All,
        Driving,
        CherryBlossom,
        City
    }

    public enum SceneGizmoID
    {
        top,
        bottom,
        left,
        right,
        front,
        back
    }

    public enum KeyboardShortcutAction
    {
        copy,
        paste,
        duplicate,
        save,
        test,
        rename,
        object_tab_select,
        explorer_tab_select,
        select_gizmo,
        move_gizmo,
        rotate_gizmo,
        scale_gizmo,
        toggle_tabs,
        undo,
        redo
    }

    #region ServerApis

    #endregion

    #region Game Logic Creator
    public enum ItemComponentType
    {
        CollectibleComponent = 0,
        RotatorComponent = 1,
        TransformComponent = 2,
        TranslateComponent = 3,
        AddForceComponent = 4,
        TimerComponent = 5,
        TimeLimitComponent = 6,
        RandomNumberComponent = 7,
        NarrationComponent = 8,
        TimerCountdownComponent = 9,
        PowerProviderComponent = 10,
        AvatarChangerComponent = 11,
        enemyNPC = 12,
        ladder = 13,
        DoorKeyComponent = 14,
        chestKey = 15,
        ElapsedTimeComponent = 16,
        HelpButtonComponent = 17,
        HealthComponent = 18,
        SituationChangerComponent = 19,
        SpecialItemComponent = 20,
        ThrowThingsComponent = 21,
        NinjaComponent = 22,
        spawner = 23,
        DisplayMessagesComponent = 24,
        QuizComponent = 25,
        WarpFunctionComponent = 26,
        AudioComponent = 27,
        BlindfoldedDisplayComponent = 28,
        HyperLinkPopComponent = 29,
        BlindComponent = 30,
        PhysicsComponent = 31,
        TeleportComponent = 32,
        none = 33
    }
    #endregion

    #region Asset constants

    public const string prefabPrefix = "pf";
    public const string assetVersion = "AssetVersion";
    public const string materialPrefix = "mt";
    public const string editMode = "_EditMode";
    public const string placedMode = "_PlaceMode";
    public const string failMode = "_FailMode";

    public const string texturePrefix = "tx";
    public const string textureExt = ".png";

    public const string thumbnailSuffix = "_Thumbnail";

    public const string markerPreab = "Marker";


    public enum TextureType
    {
        _Diffuse,
        _Metallic,
        _NormalMap,
        _HeightMap,
        _Occlusion,
        _DetailMask,
        _Emission
    }
    #endregion


    #region COLOR
    public static Color selectColor = new Color(0, 143, 255, 255);
    public static Color failColor = Color.red;

    #endregion

    #region Shader Constants
    public const string outlineWidth = "_OutlineWidth";
    public const string outlineColor = "_OutlineColor";
    public const string color = "_Color";
    public const string BaseColor = "_BaseColor";
    public const string mainTexture = "_Main_Texture";
    #endregion
}
