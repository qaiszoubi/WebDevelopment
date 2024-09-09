using DinkToPdf;
using DinkToPdf.Contracts;

public class PdfService
{
    private readonly IConverter _converter;

    public PdfService(IConverter converter)
    {
        _converter = converter;
    }
    public byte[] Convert(HtmlToPdfDocument document)
    {
        return _converter.Convert(document);
    }
    public byte[] CreatePdf(string htmlContent)
    {
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 10 }
        };

        var objectSettings = new ObjectSettings
        {
            HtmlContent = htmlContent,
            WebSettings = { DefaultEncoding = "utf-8" }
        };

        var pdfDocument = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings }
        };

        return _converter.Convert(pdfDocument);
    }
}
