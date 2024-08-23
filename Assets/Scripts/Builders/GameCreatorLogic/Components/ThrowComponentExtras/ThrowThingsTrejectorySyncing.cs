using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

[RequireComponent(typeof(LineRenderer))]
public class ThrowThingsTrejectorySyncing : MonoBehaviourPun
{
    int resolution = 300;
    int distance = 4;
    private LineRenderer lineRenderer;
    private Vector3[] points;
    GameObject aimCollsion;
    GameObject colliderAim;
    GameObject handBall;
    GameObject playerObj;
    Transform swordhandHook;
    Player player;

    void OnEnable()
    {
        aimCollsion = Instantiate(GamificationComponentData.instance.throwAimPrefab, Vector3.down * 2000, Quaternion.identity);

        //colliderAim = Instantiate(GamificationComponentData.instance.throwAimPrefab, Vector3.down * 2000, Quaternion.identity);

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.material = GamificationComponentData.instance.lineMaterial;
        lineRenderer.numCornerVertices = 30;
        lineRenderer.numCapVertices = 31;
        lineRenderer.positionCount = resolution;
        points = new Vector3[resolution];

        if (photonView.IsMine)
            return;
        if (!GamificationComponentData.instance.withMultiplayer)
        {
            gameObject.SetActive(false);
            return;
        }

        playerObj = FindPlayerusingPhotonView(photonView);
        if (playerObj != null)
        {
            IKMuseum iKMuseum = playerObj.GetComponent<IKMuseum>();
            swordhandHook = iKMuseum.m_SelfieStick.transform.parent;
            if (handBall == null)
            {
                handBall = Instantiate(GamificationComponentData.instance.handBall, swordhandHook);
                handBall.transform.localPosition = new Vector3(0, 0, 0.044f);
                handBall.transform.localRotation = Quaternion.Euler(0, -25.06f, 0);
                handBall.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        if (aimCollsion)
            Destroy(aimCollsion);
        if (colliderAim)
            Destroy(colliderAim);
    }

    [PunRPC]
    void Init(bool ballVisible, Vector3 startPosition, Vector3 initialVelocity)
    {
        if (photonView.Owner == player)
        {
            if (!ballVisible)
            {
                if (handBall != null)
                    handBall.SetActive(true);
                if (colliderAim)
                    colliderAim.SetActive(false);
            }
            else
            {
                UpdateTrajectory(startPosition, initialVelocity);
                if (colliderAim)
                    colliderAim.SetActive(true);
            }
            lineRenderer.enabled = ballVisible;
        }
    }

    void UpdateTrajectory(Vector3 startPosition, Vector3 initialVelocity)
    {
        if (handBall != null)
            handBall.SetActive(true);

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            t = t * distance;
            Vector3 point = CalculatePointInTrajectory(startPosition, initialVelocity, t);
            points[i] = point;
        }

        bool hitDetected = false; // if any collision detected

        for (int i = 0; i < resolution - 1; i++)
        {
            if (i < resolution - 1)
            {
                RaycastHit hitInfo;
                if (Physics.Linecast(points[i], points[i + 1], out hitInfo))
                {
                    if (colliderAim == null)
                        colliderAim = Instantiate(aimCollsion);

                    if (hitInfo.collider.CompareTag("Item") || hitInfo.collider.CompareTag("Footsteps/grass") || hitInfo.collider.CompareTag("Footsteps/floor"))//"Ground"
                    {
                        colliderAim.transform.position = hitInfo.point;
                        lineRenderer.positionCount = i;
                        hitDetected = true;
                        break;
                    }
                }
            }
        }

        // If no collision detected, Position setting here
        if (!hitDetected)
        {
            colliderAim.transform.position = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
        }

        lineRenderer.SetPositions(points);
    }

    Vector3 CalculatePointInTrajectory(Vector3 startPosition, Vector3 initialVelocity, float t)
    {
        float x = startPosition.x + initialVelocity.x * t;
        float y = startPosition.y + initialVelocity.y * t - 0.5f * Physics.gravity.magnitude * t * t;
        float z = startPosition.z + initialVelocity.z * t;

        return new Vector3(x, y, z);
    }

    public void CheckCollision()
    {
        if (colliderAim == null)
            colliderAim = Instantiate(aimCollsion);
        colliderAim.transform.position = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
    }

    GameObject FindPlayerusingPhotonView(PhotonView pv)
    {
        Player player = pv.Owner;
        foreach (GameObject playerObject in MutiplayerController.instance.playerobjects)
        {
            PhotonView _photonView = playerObject.GetComponent<PhotonView>();
            if (_photonView.Owner == player && _photonView.GetComponent<AvatarController>())
            {
                this.player = _photonView.Owner;
                return playerObject;
            }
        }
        return null;
    }
}