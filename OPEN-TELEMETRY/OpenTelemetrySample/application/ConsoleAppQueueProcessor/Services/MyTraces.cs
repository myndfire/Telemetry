﻿using System.Diagnostics;

namespace ConsoleAppQueueProcessor.Services
{
    public class MyTraces: IDisposable
    {
        public ActivitySource Source { get; }
        public MyTraces()
        {
            Source = new ActivitySource(Measures.InstrumentationName, Measures.InstrumentationName);
        }
        public void Dispose()
        {
            Source.Dispose();
        }
    }
}
