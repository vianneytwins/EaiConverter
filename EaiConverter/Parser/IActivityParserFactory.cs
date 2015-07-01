namespace EaiConverter.Parser
{
    public interface IActivityParserFactory
    {
        IActivityParser GetParser(string activityType);
    }
}

