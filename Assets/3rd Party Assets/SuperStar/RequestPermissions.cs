using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class RequestPermissions : MonoBehaviour,AndroidPermissionsManager.Delegate
{
	private int _IDENTIFIER_MULTIPLE_PERMISSION = 1;
	private int _IDENTIFIER_PERMISSION_STORAGE = 2;
	private int _IDENTIFIER_PERMISSION_CAMERA = 3;
	private int _IDENTIFIER_PERMISSION_LOCATION = 4;

    //	public Text resultText;

    private void Start()
    {
		RequestMultiplePermissions();
    }
    public void RequestMultiplePermissions()
	{
		Stack<string> permissions = new Stack<string>();
		permissions.Push(Permission.ExternalStorageWrite);
		permissions.Push(Permission.Camera);
		permissions.Push(Permission.CoarseLocation);

		AndroidPermissionsManager.instance.RequestMultiplePermission(permissions,this,_IDENTIFIER_MULTIPLE_PERMISSION);
	}

	public void RequestStoragePermission()
	{
		AndroidPermissionsManager.instance.RequestSinglePermission(Permission.ExternalStorageWrite, this, _IDENTIFIER_PERMISSION_STORAGE);
	}
	public void RequestCameraPermission()
	{
		AndroidPermissionsManager.instance.RequestSinglePermission(Permission.Camera, this, _IDENTIFIER_PERMISSION_CAMERA);
	}
	public void RequestLocationPermission()
	{
		AndroidPermissionsManager.instance.RequestSinglePermission(Permission.CoarseLocation, this, _IDENTIFIER_PERMISSION_LOCATION);
	}



	public void OnCompleteAskingPermissionRequest(int identifier)
	{
		//Debug.Log("permission Asking Completed for " + identifier);
		//resultText.text = "permission Asking Completed for " + identifier;
	}
}
