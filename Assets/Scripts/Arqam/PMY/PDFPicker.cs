using UnityEngine;
using System.IO;
using Paroxe.PdfRenderer;
using SimpleFileBrowser;
public class PDFPicker : MonoBehaviour
{
    public PDFViewer pdfViewer; // Assign your PDFRenderer component here
    public string filePath;
    public void PickPDF()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("PDF Files", ".pdf"));
        FileBrowser.ShowLoadDialog((paths) => { LoadAndRenderPDF(paths[0]); }, null, FileBrowser.PickMode.Files, false, "Select PDF", "Select");
    }

    private void LoadAndRenderPDF(string path)
    {
        if (pdfViewer != null)
        {
            // byte[] pdfData = File.ReadAllBytes(path);
            // pdfRenderer.LoadPDF(pdfData); // Implement PDF loading in your renderer
            pdfViewer.FilePath = path;
            pdfViewer.transform.root.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("PDF Renderer is not assigned.");
        }
    }
}
