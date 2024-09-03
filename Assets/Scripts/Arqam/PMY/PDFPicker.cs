using UnityEngine;
using Paroxe.PdfRenderer;

public class PDFPicker : MonoBehaviour
{
    public PDFViewer pdfViewer; // Assign your PDFRenderer component here
    public string filePath;
    public void PickPDF()
    {
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path != null)
            {
                pdfViewer.FilePath = path;
                pdfViewer.transform.root.gameObject.SetActive(true);
                // Load your file here
            }
        }, new string[] { "application/pdf" });
    }

}
