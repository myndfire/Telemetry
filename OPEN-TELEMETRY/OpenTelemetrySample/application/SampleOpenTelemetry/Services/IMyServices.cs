namespace SampleOpenTelemetry.Services
{
    public interface IMyServices
    {
        public Task<bool> Ping();
        public Task Pong();
    }
}
