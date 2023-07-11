using UnityEngine;

public class Loading : MonoBehaviour
{
    public static Loading Instance;

    [SerializeField] private Transform loader;
    
    [Space]
    [SerializeField] private float rotateSpeed = 300f;

    private float rotateAngle = 0f;
    public bool WalletLoading;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            if(!WalletLoading)
            {
             gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if(gameObject.activeInHierarchy)
        {
            rotateAngle -= rotateSpeed * Time.deltaTime;
            loader.localEulerAngles = new Vector3(0f, 0f, rotateAngle % 360);
        }
    }

    public void ShowLoading()
    {
        gameObject.SetActive(true);
    }

    public void HideLoading()
    {
        gameObject.SetActive(false);     
        loader.localEulerAngles = new Vector3(0, 0, -90);
    }
}
