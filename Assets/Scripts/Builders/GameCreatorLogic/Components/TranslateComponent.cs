using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class TranslateComponent : ItemComponent
{
    #region Translate Module
    TranslateComponentData translateComponentData;
    float nextRadius = .5f;
    List<Vector3> translatePositions;
    int counter;
    bool moveForward, moveBackward;
    bool activateTranslateComponent = false;

    public void InitTranslate(TranslateComponentData translateComponentData)
    {
        this.translateComponentData = translateComponentData;
        translatePositions = new List<Vector3>();
        translatePositions = translateComponentData.translatePoints;
        moveForward = true;
        moveBackward = false;
        activateTranslateComponent = true;
        counter = 0;

        StartCoroutine(translateModule());
    }

    private bool CheckDistance()
    {
        if ((Vector3.Distance(this.transform.position, translatePositions[counter])) < nextRadius)
        {
            //counter = (counter == 0) ? 1 : 0;
            if (moveForward == true && counter < translatePositions.Count - 1)
            {
                counter++;
            }
            else
            {
                moveForward = false;
                moveBackward = true;
            }
            if (moveBackward == true && counter > 0)
            {
                counter--;
            }
            else
            {
                moveBackward = false;
                moveForward = true;
            }

            return false;
        }
        else return true;
    }

    IEnumerator translateModule()
    {
        while (activateTranslateComponent)
        {
            yield return new WaitForSeconds(0f);
            if (CheckDistance())
            {
                this.transform.position = Vector3.MoveTowards(
                   this.transform.position, translatePositions[counter],
                   translateComponentData.translateSpeed * Time.deltaTime
                   );
                if (this.translateComponentData.IsFacing) this.transform.LookAt(translatePositions[counter]);
            }
        }
        yield return null;
    }
    #endregion
}
