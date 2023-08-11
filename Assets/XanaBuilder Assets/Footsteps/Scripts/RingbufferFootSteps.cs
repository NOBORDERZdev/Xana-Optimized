using UnityEngine;

public class RingbufferFootSteps : MonoBehaviour
{
    PlayerControllerNew playerControllerNew;
    public ParticleSystem system;
    Vector3 lastEmit;

    public float delta = 1;
    public float gap = 0.1f;
    int dir = 1;

    void Start()
    {
        playerControllerNew = GamificationComponentData.instance.playerControllerNew;
        lastEmit = transform.position;
    }

    public void Update()
    {
        if (BlindfoldedDisplayComponent.footstepsBool == true)
        {
            if (Vector3.Distance(lastEmit, transform.position) > delta && playerControllerNew._IsGrounded)
            {
                Gizmos.color = Color.green;
                var pos = transform.position + (transform.right * gap * dir);
                dir *= -1;
                ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams();
                ep.position = pos;
                ep.rotation = transform.rotation.eulerAngles.y;
                system.Emit(ep, 1);
                lastEmit = transform.position;
            }
        }
    }
}