using System.Collections;
using UnityEngine;
using Models;
public class RotatorComponent : MonoBehaviour
{
    private RotatorComponentData rotatorComponentData;
    public void Init(RotatorComponentData rotatorComponentData)
    {
        this.rotatorComponentData = rotatorComponentData;
        StartCoroutine(RotateModule());
    }

    Vector3 currentRotation;
    IEnumerator RotateModule()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            currentRotation = gameObject.transform.rotation.eulerAngles;
            currentRotation += new Vector3(0f, rotatorComponentData.speed * Time.deltaTime, 0f);
            gameObject.transform.rotation = Quaternion.Euler(currentRotation);
            yield return null;
        }
    }
}