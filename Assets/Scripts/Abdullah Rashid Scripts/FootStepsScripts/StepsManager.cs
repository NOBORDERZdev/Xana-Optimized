using UnityEngine;


    public class StepsManager : MonoBehaviour
    {
    public AudioSource StepAudio;
    public float StepsVolume = 0.05f;
    public bool isplayer = false;
    public AudioClip[] sandclips, stoneClips, waterClips, metalClips, snowClips, wetClips, floorClips = default;
    [Tooltip("Scale of the dust and track particles")]

    void Awake()
    {
        if (StepAudio!= null)
        {
            StepAudio.volume = StepsVolume;
        }
    }
    public void EnterStep()
    {
        if (isplayer)
        {
            if (Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit footRay, 3))
            {
                if (StepAudio && ReferrencesForDynamicMuseum.instance.playerControllerNew._IsGrounded)
                {
                    switch (footRay.collider.tag)
                    {

                        case "Footsteps/sand":
                            StepAudio.PlayOneShot(sandclips[UnityEngine.Random.Range(0, sandclips.Length - 1)]);
                            break;
                        case "Footsteps/stone":
                            StepAudio.PlayOneShot(stoneClips[UnityEngine.Random.Range(0, stoneClips.Length - 1)]);
                            break;
                        case "Footsteps/water":
                            StepAudio.PlayOneShot(waterClips[UnityEngine.Random.Range(0, waterClips.Length - 1)]);
                            break;
                        case "Footsteps/metal":
                            StepAudio.PlayOneShot(metalClips[UnityEngine.Random.Range(0, metalClips.Length - 1)]);
                            break;
                        case "Footsteps/snow":
                            StepAudio.PlayOneShot(snowClips[UnityEngine.Random.Range(0, snowClips.Length - 1)]);
                            break;
                        case "Footsteps/wet":
                            StepAudio.PlayOneShot(wetClips[UnityEngine.Random.Range(0, wetClips.Length - 1)]);
                            break;
                        case "Footsteps/floor":
                            StepAudio.PlayOneShot(floorClips[UnityEngine.Random.Range(0, floorClips.Length - 1)]);
                            break;
                        default:
                            StepAudio.PlayOneShot(floorClips[UnityEngine.Random.Range(0, floorClips.Length - 1)]);
                            break;
                    }
                }

            }

        }
    }
      
    }

