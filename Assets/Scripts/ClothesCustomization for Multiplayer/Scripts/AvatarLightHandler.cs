using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AvatarLightHandler : MonoBehaviour
{
    public List<lightClass> lights = new List<lightClass>();
    int tryCount;
    void Start()
    {

    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(SetLightForPlanet), 1f, 2f);
    }
    public void SetLightForPlanet()
    {
        tryCount++;
        if (tryCount > 3)
        {
            CancelInvoke(nameof(SetLightForPlanet));
        }
        if (SceneManager.GetActiveScene().name.StartsWith("Planet"))
        {
            if (lights.Count > 0)
            {
                foreach (var light in lights)
                {
                    light.light.intensity = light.value;
                }
            }
        }
    }
    [System.Serializable]
    public class lightClass
    {
        public Light light;
        public float value;
    }
}
