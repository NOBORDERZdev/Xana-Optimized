using UnityEngine;

public class Ball : MonoBehaviour {
    [SerializeField] private Rigidbody _rb;
    private bool _isGhost;

    public void Init(Vector3 velocity, bool isGhost) {
        _isGhost = isGhost;
        _rb.AddForce(velocity, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Collid" + (other.gameObject.name));
        CollectibleComponent c = other.gameObject.GetComponentInParent<CollectibleComponent>();
        if (c != null)
            Destroy(c.gameObject, 0f);
    }  

}