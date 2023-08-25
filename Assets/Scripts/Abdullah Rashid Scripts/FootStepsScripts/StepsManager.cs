using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>
    /// This will manage the steps sounds and tracks for each animal, on each feet there's a Script StepTriger (Basic)
    /// </summary>
    public class StepsManager : MonoBehaviour
    {
       // public ParticleSystem Tracks;
      //  public ParticleSystem Dust;
        public float StepsVolume = 0.2f;
       // public int DustParticles = 30;

        [Tooltip("Scale of the dust and track particles")]
        public Vector3 Scale = Vector3.one;

        public AudioClip[] sandclips,stoneClips,waterClips,metalClips,snowClips,wetClips,floorClips,grassClips = default;
        [Tooltip("Distance to Instantiate the tracks on a terrain")]
       // public float trackOffset = 0.0085f;

        protected bool active = true;
        //Is Called by any of the "StepTrigger" Script on a feet when they collide with the ground.
        public PlayerControllerNew playerController;
       
        public void EnterStep(StepTrigger foot)
        {
            //if (Tracks && !Tracks.gameObject.activeInHierarchy)         //If is a prefab clone it!
            //{
            //    Tracks = Instantiate(Tracks,transform, false);
            //    Tracks.transform.localScale = Scale;
            //}

            //if (Dust && !Dust.gameObject.activeInHierarchy)
            //{
            //    Dust = Instantiate(Dust, transform, false);             //If is a prefab clone it!
            //    Dust.transform.localScale = Scale;
            //}

            if (!active) return;

            if (Physics.Raycast(foot.transform.position, Vector3.down, out RaycastHit footRay, 3))
            {
                if (foot.StepAudio && PlayerControllerNew.isJoystickDragging && ReferrencesForDynamicMuseum.instance.playerControllerNew._IsGrounded)
                {
                    switch (footRay.collider.tag)
                    {

                        case "Footsteps/sand":
                            foot.StepAudio.clip = sandclips[Random.Range(0, sandclips.Length - 1)];
                            foot.StepAudio.Play();
                            break;
                        case "Footsteps/stone":
                            foot.StepAudio.clip = stoneClips[Random.Range(0, stoneClips.Length - 1)];
                            foot.StepAudio.Play();
                            break;
                        case "Footsteps/water":
                            foot.StepAudio.clip = waterClips[Random.Range(0, waterClips.Length - 1)];
                            foot.StepAudio.Play();
                            break;
                        case "Footsteps/metal":
                            foot.StepAudio.clip = metalClips[Random.Range(0, metalClips.Length - 1)];
                            foot.StepAudio.Play();
                            break;
                        case "Footsteps/snow":
                            foot.StepAudio.clip = snowClips[Random.Range(0, snowClips.Length - 1)];
                            foot.StepAudio.Play();
                            break;
                        case "Footsteps/wet":
                            foot.StepAudio.clip = wetClips[Random.Range(0, wetClips.Length - 1)];
                            foot.StepAudio.Play();
                            break;
                        case "Footsteps/floor":
                            foot.StepAudio.clip = floorClips[Random.Range(0, wetClips.Length - 1)];
                            foot.StepAudio.Play();
                            break;
                        case "Footsteps/grass":
                            foot.StepAudio.clip = grassClips[Random.Range(0, wetClips.Length - 1)];
                            foot.StepAudio.Play();
                            break;
                        default:
                            foot.StepAudio.clip = floorClips[Random.Range(0, floorClips.Length - 1)];
                            foot.StepAudio.Play();
                            break;
                    }
                }
                //if (foot.StepAudio && sandclips.Length > 0) //If the track has an AudioSource Component and whe have some audio to play
                //{
                //    foot.StepAudio.clip = sandclips[Random.Range(0, sandclips.Length)];  //Set the any of the Audio Clips from the list to the Feet's AudioSource Component
                //    foot.StepAudio.Play();  //Play the Audio
                //}
            }
            //Track and particles
            //if (!foot.HasTrack)  // If we are ready to set a new track
            //{
            //    if (Physics.Raycast(foot.transform.position, -transform.up, out footRay, 1))
            //    {
            //        if (Tracks)
            //        {

            //            ParticleSystem.EmitParams ptrack = new ParticleSystem.EmitParams();
            //            ptrack.rotation3D = (Quaternion.FromToRotation(-foot.transform.forward, footRay.normal) * foot.transform.rotation).eulerAngles; //Get The Rotation
            //            ptrack.position = new Vector3(foot.transform.position.x, footRay.point.y + trackOffset, foot.transform.position.z); //Get The Position
            //            Tracks.Emit(ptrack, 1);
            //        }

            //        if (Dust)
            //        {
            //            Dust.transform.position = new Vector3(foot.transform.position.x, footRay.point.y + trackOffset, foot.transform.position.z); //Get The Position
            //            Dust.transform.rotation = (Quaternion.FromToRotation(-foot.transform.forward, footRay.normal) * foot.transform.rotation);
            //            Dust.transform.Rotate(-90, 0, 0);
            //            Dust.Emit(DustParticles);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Disable this script, ex.. deactivate when is sleeping or death
        /// </summary>
        /// <param name="value"></param>
        public virtual void EnableSteps(bool value)
        {
            active = value;
        }
    }
}
