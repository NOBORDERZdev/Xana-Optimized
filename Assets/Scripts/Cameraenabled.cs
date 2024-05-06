using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameraenabled : MonoBehaviour
{
    
   
    public void pressedenablecamera() {

        PlayerCameraController.instance.AllowControl();
    }
}
