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
#endif
    }

    private void OpenDocumentPicker(string path)
    {
        pdfViewer.FilePath = path;
        pdfViewer.transform.root.gameObject.SetActive(true);
    }


}
