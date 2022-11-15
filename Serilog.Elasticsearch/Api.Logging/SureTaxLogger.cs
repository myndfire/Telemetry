using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace Api.Logging
{
    public class SureTaxLogger:ISureTaxLogger
    {
        public void LogInfo<T>(T logObject)
        {
            Log.Information("{@logObject}", logObject);
        }

        public void LogDebug<T>(T logObject)
        {
            Log.Debug("{@logObject}", logObject);
        }

        public void LogWarning<T>(T logObject)
        {
            Log.Warning("{@logObject}", logObject);
        }

        public void LogError<T>(T logObject)
        {
            Log.Error("{@logObject}", logObject);
        }

        public void LogFatal<T>(T logObject)
        {
            Log.Fatal("{@logObject}", logObject);
        }
    }
}
