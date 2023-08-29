using UnityEngine;


    public class StepsManager : MonoBehaviour
    {
    public AudioSource StepAudio;
    public float StepsVolume = 0.2f;
     
    public AudioClip[] sandclips, stoneClips, waterClips, metalClips, snowClips, wetClips, floorClips, grassClips = default;
    [Tooltip("Scale of the dust and track particles")]

    void Awake()
    {
        StepAudio.volume = StepsVolume;

    }
     public void EnterStep()
        {
           if (Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit footRay, 3))
            {
                if (StepAudio  && ReferrencesForDynamicMuseum.instance.playerControllerNew._IsGrounded)
                {
                    switch (footRay.collider.tag)
                    {

                        case "Footsteps/sand":
                            StepAudio.clip = sandclips[Random.Range(0, sandclips.Length - 1)];
                            StepAudio.Play();
                            break;
                        case "Footsteps/stone":
                            StepAudio.clip = stoneClips[Random.Range(0, stoneClips.Length - 1)];
                            StepAudio.Play();
                            break;
                        case "Footsteps/water":
                            StepAudio.clip = waterClips[Random.Range(0, waterClips.Length - 1)];
                            StepAudio.Play();
                            break;
                        case "Footsteps/metal":
                            StepAudio.clip = metalClips[Random.Range(0, metalClips.Length - 1)];
                            StepAudio.Play();
                            break;
                        case "Footsteps/snow":
                            StepAudio.clip = snowClips[Random.Range(0, snowClips.Length - 1)];
                            StepAudio.Play();
                            break;
                        case "Footsteps/wet":
                            StepAudio.clip = wetClips[Random.Range(0, wetClips.Length - 1)];
                            StepAudio.Play();
                            break;
                        case "Footsteps/floor":
                            StepAudio.clip = floorClips[Random.Range(0, wetClips.Length - 1)];
                           StepAudio.Play();
                            break;
                        case "Footsteps/grass":
                            StepAudio.clip = grassClips[Random.Range(0, wetClips.Length - 1)];
                            StepAudio.Play();
                            break;
                        default:
                            StepAudio.clip = floorClips[Random.Range(0, floorClips.Length - 1)];
                            StepAudio.Play();
                            break;
                    }
                }
              
            }
           
        }
      
      
    }

