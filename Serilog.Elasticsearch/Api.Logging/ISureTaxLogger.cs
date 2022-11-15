namespace Api.Logging
{
    public interface ISureTaxLogger
    {
        void LogInfo<T>(T logObject);
        void LogDebug<T>(T logObject);
        void LogWarning<T>(T logObject);
        void LogError<T>(T logObject);
        void LogFatal<T>(T logObject);
    }
}