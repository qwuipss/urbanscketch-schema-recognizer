using SchemaRecognizer.Core.Pdf;
using SchemaRecognizer.Debug;
using Path = System.IO.Path;

var filePathToWrite = Path.Join(Environment.CurrentDirectory, "../../../../", "pdf/vector/v3.pdf");
var filePathToRead = Path.Join(Environment.CurrentDirectory, "../../../../", "pdf/vector/v1.pdf");
PdfCreator.Create(filePathToWrite, new GeometryExtractor().Extract(new FileInfo(filePathToRead)));


// var filePathToRead = Path.Join(Environment.CurrentDirectory, "../../../../", "pdf/vector/v3.pdf");
// PdfReader.Read(filePathToRead);
