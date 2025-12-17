namespace SchemaRecognizer.Core.Helpers;

public static class FilesHelper
{
    public static FileInfo GetFileInfoByPath(string path)
    {
        var fileInfo = Path.IsPathRooted(path)
            ? new FileInfo(path)
            : new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path));

        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }

        return fileInfo;
    }
}