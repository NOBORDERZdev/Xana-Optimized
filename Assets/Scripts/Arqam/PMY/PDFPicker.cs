using UnityEngine;
using Paroxe.PdfRenderer;
//using UnityEngine.Android;
//#if UNITY_IOS
//using UnityEngine.iOS;
//#endif

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
string[] fileTypes = new string[] { "public.pdf" };

NativeFilePicker.PickFile((string path) =>
{
    if (path == null)
    {
        Debug.LogError("File picking was cancelled or failed on iOS.");
        return;
    }

    Debug.Log("Picked PDF file path: " + path);

    // Do something with the PDF file, e.g., open it
     OpenDocumentPicker(path);

}, fileTypes);

//NativeFilePicker.PickFile((string path) =>
//{
//    if (path == null)
//    {
//        Debug.LogError("File picking was cancelled or failed on iOS.");
//        return;
//    }

//    Debug.Log("Picked PDF file path: " + path);

//    // Attempt to open the file using the iOS file system
//    OpenDocumentPicker(path);

//}, new string[] { "public.pdf" });

#endif
    }

    private void OpenDocumentPicker(string path)
    {
        pdfViewer.FilePath = path;
        pdfViewer.transform.root.gameObject.SetActive(true);
    }


}
