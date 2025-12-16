using IronSoftware.Drawing;

var pdf = PdfDocument.FromFile(Path.Join(Environment.CurrentDirectory, "../../../../", "pdf/vector/v1.pdf"));

// Extract all pages to a folder as image files
pdf.RasterizeToImageFiles(Path.Join(Environment.CurrentDirectory, "../../../../", "pdf/dev/jpg.jpg"));

// Dimensions and page ranges may be specified
// pdf.RasterizeToImageFiles(@"C:\image\folder\example_pdf_image_*.jpg", 100, 80);

// Extract all pages as AnyBitmap objects
AnyBitmap[] pdfBitmaps = pdf.ToBitmap();