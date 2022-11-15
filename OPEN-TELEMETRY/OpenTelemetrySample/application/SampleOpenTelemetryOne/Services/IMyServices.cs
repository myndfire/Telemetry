namespace SampleOpenTelemetryTwo.Services
{
    public interface IMyServices
    {
        public Task<bool> Foo();
        public Task Bar();
        public Task PublishMessage();
    }
}
