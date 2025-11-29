namespace SchemaRecognizer.Worker;

internal interface IExecutor
{
    void Run(FileInfo fileInfo);
}