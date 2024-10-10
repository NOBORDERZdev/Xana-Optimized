using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class SummitDomeShaderApply : MonoBehaviour
{
    [HideInInspector]
    public int DomeId;
    [HideInInspector]
    public string ImageUrl;
    [HideInInspector]
    public string LogoUrl;
    public GameObject DomeBannerParent;
    public GameObject DomeText;
    public GameObject Frame;
    public MeshRenderer DomeMeshRenderer;
    public MeshRenderer LogoMeshRenderer;
    public SpriteRenderer LogoSpriteRenderer;

    public async void Init()
    {
        DomeBannerParent.SetActive(true);
        if (!string.IsNullOrEmpty(ImageUrl))
        {
            Texture2D texture=await DownloadDomeTexture(ImageUrl);
            DomeMeshRenderer.material.mainTexture = texture;
            DomeMeshRenderer.gameObject.SetActive(true);
            Frame.SetActive(true);
        }
        if (!string.IsNullOrEmpty(LogoUrl))
        {
            Texture2D texture = await DownloadDomeTexture(LogoUrl);
            LogoSpriteRenderer.sprite = ConvertToSprite(texture);
            ScaleSpriteToTargetSize();
            //texture.wrapMode = TextureWrapMode.Clamp;
            //AdjustUVs(LogoMeshRenderer, texture);
            //LogoMeshRenderer.material.mainTexture = texture;
            //LogoMeshRenderer.gameObject.SetActive(true);
        }
    }

    private Sprite ConvertToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    public float targetWidth = 1f;
    private void ScaleSpriteToTargetSize()
    {
        if (LogoSpriteRenderer == null || LogoSpriteRenderer.sprite == null) return;

        // Get the native size of the sprite
        float spriteWidth = LogoSpriteRenderer.sprite.bounds.size.x;
        float spriteHeight = LogoSpriteRenderer.sprite.bounds.size.y;

        // Calculate the aspect ratio
        float aspectRatio = spriteWidth / spriteHeight;

        // Calculate the target height based on the target width and aspect ratio
        float targetHeight = targetWidth / aspectRatio;

        // Calculate the scale factor needed to achieve the target size
        Vector3 scale = LogoSpriteRenderer.transform.localScale;
        scale.x = targetWidth / spriteWidth;
        scale.y = targetHeight / spriteHeight;

        // Apply the scale to the sprite
        LogoSpriteRenderer.transform.localScale = scale;
    }

    async Task<Texture2D> DownloadDomeTexture(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        await request.SendWebRequest();
        if ((request.result == UnityWebRequest.Result.ConnectionError) || (request.result == UnityWebRequest.Result.ProtocolError))
            Debug.Log(request.error);
        else
        {
            return DownloadHandlerTexture.GetContent(request);
        }
        request.Dispose();
        return null;
    }

    private void AdjustUVs(Renderer domeRenderer, Texture2D logoTexture)
    {
        Mesh mesh = domeRenderer.GetComponent<MeshFilter>().mesh;
        Vector2[] uvs = mesh.uv;

        float logoAspectRatio = (float)logoTexture.width / logoTexture.height;

        for (int i = 0; i < uvs.Length; i++)
        {
            // Adjust the UVs based on the logo's aspect ratio
            if (logoAspectRatio > 1f)
            {
                // Logo is wider than tall (rectangular)
                uvs[i] = new Vector2(uvs[i].x * logoAspectRatio, uvs[i].y);
            }
            else if (logoAspectRatio < 1f)
            {
                // Logo is taller than wide (rectangular)
                uvs[i] = new Vector2(uvs[i].x, uvs[i].y / logoAspectRatio);
            }
            else
            {
                // Logo is square, no need to adjust UVs
                uvs[i] = new Vector2(uvs[i].x, uvs[i].y);
            }
        }

        mesh.uv = uvs;
    }
}
