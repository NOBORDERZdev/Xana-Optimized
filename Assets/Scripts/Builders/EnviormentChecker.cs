using UnityEngine;

public class EnvironmentChecker : MonoBehaviour
{
    //private int bufferGameObjectId;

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag(Constants.ItemTag))
    //    {
    //        if (collision.gameObject.GetInstanceID() == bufferGameObjectId) return;

    //        bufferGameObjectId = collision.gameObject.GetInstanceID();

    //        collision.gameObject.GetComponentsInParent<ItemComponent>().ForEachItem(d => d.CollisionEnterBehaviour());
    //    }

    //    if (collision.gameObject.CompareTag(Constants.GroundTag) || collision.gameObject.CompareTag(Constants.WaterTag)) bufferGameObjectId = -1;
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag(Constants.ItemTag))
    //    {
    //        collision.gameObject.GetComponentsInParent<ItemComponent>().ForEachItem(d => d.CollisionExitBehaviour());
    //    }
    //}

    //void Update()
    //{
    //    GroundedCheck();
    //}

    //private void GroundedCheck()
    //{
    //    // set sphere position, with offset
    //    Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y,
    //        transform.position.z);
    //    bool isGrounded = Physics.CheckSphere(spherePosition, 0, GamificationComponentData.instance.GroundLayers,
    //        QueryTriggerInteraction.Ignore);
    //    if (isGrounded)
    //    {
    //        bufferGameObjectId = -1;
    //    }
    //}
}