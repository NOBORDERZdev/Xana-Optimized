using UnityEngine;
using Paroxe.PdfRenderer;
using UnityEngine.Android;
using UnityEngine.iOS;

public class PDFPicker : MonoBehaviour
{
    public PDFViewer pdfViewer; // Assign your PDFRenderer component here
    public string filePath;

    public void PickPDF()
    {
#if UNITY_ANDROID
        // Check if the permission is already granted
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            // Request permission to read external storage
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path != null)
            {
                OpenDocumentPicker(path);
                // Load your file here
            }
        }, new string[] { "application/pdf" });

#elif UNITY_IOS                
        if (PHPhotoLibrary.AuthorizationStatus() != PHAuthorizationStatus.Authorized)
        {
            // Request access to the photo library
            PHPhotoLibrary.RequestAuthorization(status =>
            {
                if (status == PHAuthorizationStatus.Authorized)
                {
                    // Access granted, open the file picker
                    OpenDocumentPicker();
                }
                else
                {
                    // Access denied, handle accordingly
                    Debug.LogError("User denied access to the photo library.");
                    // Show a message or handle denial as needed
                    return;
                }
            });
        }
        else{
        OpenDocumentPicker(path);
        }
#endif
    }

    private void OpenDocumentPicker(string path)
    {
        pdfViewer.FilePath = path;
        pdfViewer.transform.root.gameObject.SetActive(true);
    }


}
