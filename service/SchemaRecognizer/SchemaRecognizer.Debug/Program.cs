using IronSoftware.Drawing;
using ImageType = IronPdf.Imaging.ImageType;

var pdf = PdfDocument.FromFile(Path.Join(Environment.CurrentDirectory, "../../../../", "pdf/vector/v1.pdf"));

// Extract all pages to a folder as image files
pdf.RasterizeToImageFiles(Path.Join(Environment.CurrentDirectory, "../../../../", "pdf/dev/jpg.jpg"),ImageType.Png, 72);
