using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using System.Xml.Linq;

public class DocxReader
{
    public static string ConvertDocxToHtml(string filePath)
    {
        XElement htmlContent = null;

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {
            // Aceptar revisiones antes de la conversión
            RevisionAccepter.AcceptRevisions(doc);

            WmlToHtmlConverterSettings settings = new WmlToHtmlConverterSettings();
            htmlContent = WmlToHtmlConverter.ConvertToHtml(doc, settings);
        }

        // Convertir XElement a string
        return htmlContent?.ToString();
    }
}
