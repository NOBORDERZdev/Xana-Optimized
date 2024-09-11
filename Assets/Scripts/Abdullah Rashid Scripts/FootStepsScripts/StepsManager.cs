using UnityEngine;

public class StepsManager : MonoBehaviour
{
    public AudioSource StepAudio;
    public float StepsVolume = 0.07f;
    public bool isplayer = false;
    public AudioClip[] sandclips, stoneClips, waterClips, metalClips, snowClips, wetClips, floorClips, grassClips = default;
    [Tooltip("Scale of the dust and track particles")]

    float distance = 3;

    private void OnEnable()
    {
        if (SoundSettings.soundManagerSettings)
        {
            SoundSettings.soundManagerSettings.OnBGMAudioMuted += DisableStepsSound;
        }
    }

    private void OnDisable()
    {
        if (SoundSettings.soundManagerSettings)
            SoundSettings.soundManagerSettings.OnBGMAudioMuted -= DisableStepsSound;
    }

    void Awake()
    {
        if (StepAudio != null)
        {
            StepAudio.volume = StepsVolume;
        }
        //if (ConstantsHolder.xanaConstants.isBuilderScene)
        //        distance = 0.2f;
    }

    public void EnterStep(float targetWalkSpeed) //it is calling from the animation event on Walk, Run, and Sprint animations.
    {
        if (SoundSettings.soundManagerSettings)
        {
            SoundSettings.soundManagerSettings.OnBGMAudioMuted += DisableStepsSound;
        }
        if (isplayer)
        {
            Ray ray = new Ray(gameObject.transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit footRay, distance))
            {
                float actualSpeed = ReferencesForGamePlay.instance.playerControllerNew.animationBlendValue;

                if (GetMovementState(targetWalkSpeed) == GetMovementState(actualSpeed))
                {
                    if (StepAudio && ReferencesForGamePlay.instance.playerControllerNew._IsGrounded)
                    {
                        //Debug.LogError(footRay.collider.name+" ==> "+ footRay.collider.tag);
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
                            case "Footsteps/grass":
                                StepAudio.PlayOneShot(grassClips[UnityEngine.Random.Range(0, grassClips.Length - 1)]);
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

    int GetMovementState(float speed)
    {
        if (speed > 1)
            return 4;

        if (speed < 0.75)
            return 3;

        if (speed < 0.05)
            return 2;

        return 0;
    }

    public void DisableStepsSound(bool _mute)
    {
        if(StepAudio)
            StepAudio.mute = _mute;
    }
}