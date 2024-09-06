using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DJMusicReactor : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject[] reactiveObjects;
    public GameObject[] emissionObjects;
    public GameObject[] lightGameObjects; // Treated as regular GameObjects
    public GameObject[] musicScale; // Array of GameObjects to be scaled
    public float reactiveSensitivity = 0.2f;
    public float emissionSensitivity = 0.2f;
    public float lightGameObjectSensitivity = 0.2f;
    public float maxEmissionIntensity = 5.0f;
    public float minScale = 3.0f; // Minimum scale value
    public float maxScale = 5.0f; // Maximum scale value
    public Gradient emissionColorGradient; // Define how color changes with amplitude

    private float[] audioData;

    void Start()
    {
        audioData = new float[256];
    }

    void Update()
    {
        if (audioSource.isPlaying)
        {
            audioSource.GetOutputData(audioData, 0);
            float sum = 0;

            foreach (var sample in audioData)
            {
                sum += Mathf.Abs(sample);
            }

            float average = sum / audioData.Length;

            // Toggle reactive objects
            ToggleGameObjects(reactiveObjects, average > reactiveSensitivity);

            // Animate emission properties
            AnimateEmissionProperties(average);

            // Toggle light game objects
            ToggleGameObjects(lightGameObjects, average > lightGameObjectSensitivity);

            // Scale musicScale objects
            ScaleMusicScaleObjects(average);
        }
    }

    private void ToggleGameObjects(GameObject[] objects, bool isActive)
    {
        foreach (var obj in objects)
        {
            obj.SetActive(isActive);
        }
    }

    private void AnimateEmissionProperties(float average)
    {
        float emissionIntensity = Mathf.Clamp(average * maxEmissionIntensity, 0, maxEmissionIntensity);
        Color emissionColor = emissionColorGradient.Evaluate(average);

        foreach (var obj in emissionObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = renderer.material;
                mat.SetColor("_EmissionColor", emissionColor * emissionIntensity);
            }
        }
    }

    private void ScaleMusicScaleObjects(float average)
    {
        float scale = Mathf.Lerp(minScale, maxScale, average);
        foreach (var obj in musicScale)
        {
            obj.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
