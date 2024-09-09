using UnityEngine;

[CreateAssetMenu(fileName = "NewPermissionTexts", menuName = "Permissions/PermissionTexts")]
public class PermissionTexts : ScriptableObject
{
    [TextArea(3, 15)]
    public string micPermission;         // The title of the permission popup
    [TextArea(3, 15)]
    public string cameraPermission;   // The description or message shown in the popup
    [TextArea(3, 15)]
    public string galleryPermission;   // The description or message shown in the popup

}
