using UnityEngine;

public class LoadingWallet : MonoBehaviour
{
  //  public static LoadingWallet Instance;  

    [SerializeField] private Transform loader;
     [Space]
    [SerializeField] private float rotateSpeed = 300f;

    private float rotateAngle = 0f;
     private void Awake()
    {
        //if(Instance == null)
        //{
        //    Instance = this;
        // }
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
        //gameObject.SetActive(false);     
        //loader.localEulerAngles = new Vector3(0, 0, -90);
    }
}
