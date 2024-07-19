using UnityEngine;
using TMPro;

public class AddTmpComponent : MonoBehaviour
{
    private TextMeshPro _textMesh;
    public string Text;
    void Start()
    {
        _textMesh = gameObject.AddComponent<TextMeshPro>();
        //textMesh.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        _textMesh.fontSize = 18;
        _textMesh.text = Text;
        _textMesh.alignment = TextAlignmentOptions.Center;
        _textMesh.margin = new Vector4(2.11033f, -2.824954f, 8.039522f, -1.832686f);
    }

}
