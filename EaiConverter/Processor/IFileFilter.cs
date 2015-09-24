namespace EaiConverter.Processor
{
    public interface IFileFilter
    {
        bool IsFileAuthorized(string filePath);
    }
}
