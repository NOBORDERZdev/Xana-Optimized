using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Project Constants
public static class Constants
{
    public static int cursorLayer = 8;
    public static int defaultLayer = 0;
    public static int itemLayer = 9;

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
        collectible,
        rotatable,
        health
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
