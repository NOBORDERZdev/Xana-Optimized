using UnityEditor;
using UnityEngine;

public static class RectTransformContextMenu
{
    [MenuItem("CONTEXT/RectTransform/Set Anchors to Corners")]
    private static void SetAnchorsToCorners(MenuCommand menuCommand)
    {
        RectTransform rectTransform = menuCommand.context as RectTransform;
        if (rectTransform == null) return;

        RectTransform parentRectTransform = rectTransform.parent as RectTransform;
        if (parentRectTransform == null)
        {
            Debug.LogWarning("Parent is not a RectTransform.");
            return;
        }

        Undo.RecordObject(rectTransform, "Set Anchors to Corners");

        Vector2 newAnchorMin = new Vector2(
            rectTransform.anchorMin.x + rectTransform.offsetMin.x / parentRectTransform.rect.width,
            rectTransform.anchorMin.y + rectTransform.offsetMin.y / parentRectTransform.rect.height);

        Vector2 newAnchorMax = new Vector2(
            rectTransform.anchorMax.x + rectTransform.offsetMax.x / parentRectTransform.rect.width,
            rectTransform.anchorMax.y + rectTransform.offsetMax.y / parentRectTransform.rect.height);

        rectTransform.anchorMin = newAnchorMin;
        rectTransform.anchorMax = newAnchorMax;

        rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;

        EditorUtility.SetDirty(rectTransform);
    }
}