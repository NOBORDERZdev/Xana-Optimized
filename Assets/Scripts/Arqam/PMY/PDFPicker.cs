using UnityEngine;
using Paroxe.PdfRenderer;


public class PDFPicker : MonoBehaviour
{
    public PDFViewer pdfViewer; // Assign your PDFRenderer component here
    public string filePath;

    public void PickPDF()
    {
#if UNITY_ANDROID
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path != null)
            {
                OpenDocumentPicker(path);
                // Load your file here
            }
        }, new string[] { "application/pdf" });

#elif UNITY_IOS

        NativeFilePicker.PickFile((string path) =>
        {
            if (path == null)
            {
                Debug.LogError("File picking was cancelled or failed on iOS.");
                return;
            }

            Debug.Log("Picked PDF file path: " + path);

            // Attempt to open the file using the iOS file system
            OpenDocumentPicker(path);

        }, new string[] { "com.adobe.pdf" });

#endif
    }

    private void OpenDocumentPicker(string path)
    {
        pdfViewer.FilePath = path;
        pdfViewer.transform.root.gameObject.SetActive(true);
    }


}
