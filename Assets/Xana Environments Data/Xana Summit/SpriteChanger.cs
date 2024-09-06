using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    public Sprite[] sprites; // Array of sprites to change
    public Material material; // Material whose albedo will change
    public float changeInterval = 2.0f; // Time interval in seconds

    private int currentSpriteIndex = 0;
    private float timer = 0.0f;
    private Texture2D[] spriteTextures;

    void Start()
    {
        // Preload the textures to avoid converting sprites every frame
        spriteTextures = new Texture2D[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            spriteTextures[i] = SpriteToTexture(sprites[i]);
        }

        // Initialize the material with the first sprite's texture
        if (sprites.Length > 0)
        {
            material.SetTexture("_BaseMap", spriteTextures[0]);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= changeInterval)
        {
            timer = 0.0f;
            ChangeSprite();
        }
    }

    void ChangeSprite()
    {
        currentSpriteIndex = (currentSpriteIndex + 1) % spriteTextures.Length;
        material.SetTexture("_BaseMap", spriteTextures[currentSpriteIndex]);
    }

    Texture2D SpriteToTexture(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
        {
            return sprite.texture;
        }
    }
}
