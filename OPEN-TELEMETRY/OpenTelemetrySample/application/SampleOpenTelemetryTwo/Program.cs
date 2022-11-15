using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Reflection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using SampleOpenTelemetryTwo.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMyServices, MyServices>();
builder.Services.AddSingleton<MyTraces>();
builder.Services.AddSingleton<MyMetrics>();


//----------------- .Net Core Instrumentation ----------------------------

//only required for .NET Core 3.x. It does nothing in .NET 5 and isn't required
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);


//---------------  Advanced Asp.Net Core Instrumentation configuration  ---------------------------
//builder.Services.Configure<AspNetCoreInstrumentationOptions>(options =>
//{
//    // Filtering
//    //options.Filter = (httpContext) =>
//    //{
//    //    // only collect telemetry about HTTP GET requests
//    //    return httpContext.Request.Method.Equals("GET");
//    //};
//    // Enriching
//    options.Enrich = (activity, eventName, rawObject) =>
//    {
//        if (eventName.Equals("OnStartActivity"))
//        {
//            if (rawObject is HttpRequest httpRequest)
//            {
//                activity.SetTag("requestProtocol", httpRequest.Protocol);
//            }
//        }
//        else if (eventName.Equals("OnStopActivity"))
//        {
//            if (rawObject is HttpResponse httpResponse)
//            {
//                activity.SetTag("responseLength", httpResponse.ContentLength);
//            }
//        }
//    };

//    // Record exception
//    options.RecordException = true;
//});
//---------------------------------------------------------


//--------------  Tracing --------------------
var env = builder.Environment;
var applicationName = builder.Environment.ApplicationName;
var assemblyVersion = Assembly
    .GetExecutingAssembly()
    .GetCustomAttribute<AssemblyFileVersionAttribute>()!
    .Version;

builder.Services.AddOpenTelemetryTracing(builder =>
    {
        builder
            .SetResourceBuilder(
                ResourceBuilder
                .CreateDefault()
                //.CreateEmpty()
                .AddService(serviceName: applicationName, serviceVersion: applicationName)
                .AddAttributes(
                    new KeyValuePair<string, object>[]
                    {
                        new("deployment.environment", env.EnvironmentName),
                        new("host.name", Environment.MachineName),
                        new("application.name", applicationName),
                        new("application.assemblyversion",assemblyVersion)
                    })
                .AddEnvironmentVariableDetector() //Set using powershell: $env:OTEL_RESOURCE_ATTRIBUTES = 'key1=value1,key2=value2'
                )
            .AddSource("Azure.*")
            .SetSampler(new AlwaysOnSampler())
            .AddAspNetCoreInstrumentation(
                options =>
                {
                    options.Enrich = Enrich;
                    options.RecordException = true;
                })
            .AddHttpClientInstrumentation();
        //if (env.IsDevelopment())
        //{
            builder
                .AddConsoleExporter(
                options => options.Targets = ConsoleExporterOutputTargets.Console);
        //}

        builder
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://localhost:4317");
                //options.ExportProcessorType = ExportProcessorType.Simple;
            })
            .AddInstrumentation<MyTraces>()
            .AddSource(Measures.InstrumentationName);
    });

void Enrich(Activity activity, string eventName, object obj)
{
    if (obj is HttpRequest request)
    {
        var context = request.HttpContext;
        activity.AddTag("http.flavor", GetHttpFlavour(request.Protocol));
        activity.AddTag("http.scheme", request.Scheme);
        activity.AddTag("http.client_ip", context.Connection.RemoteIpAddress);
        activity.AddTag("http.request_content_length", request.ContentLength);
        activity.AddTag("http.request_content_type", request.ContentType);

        var user = context.User;
        if (user.Identity?.Name is not null)
        {
            activity.AddTag("enduser.id", user.Identity.Name);
            activity.AddTag(
                "enduser.scope",
                string.Join(',', user.Claims.Select(x => x.Value)));
        }
    }
    else if (obj is HttpResponse response)
    {
        activity.AddTag("http.response_content_length", response.ContentLength);
        activity.AddTag("http.response_content_type", response.ContentType);
    }
}

string? GetHttpFlavour(string protocol)
{
    if (HttpProtocol.IsHttp10(protocol))
    {
        return "1.0";
    }
    else if (HttpProtocol.IsHttp11(protocol))
    {
        return "1.1";
    }
    else if (HttpProtocol.IsHttp2(protocol))
    {
        return "2.0";
    }
    else if (HttpProtocol.IsHttp3(protocol))
    {
        return "3.0";
    }

    throw new InvalidOperationException($"Protocol {protocol} not recognised.");
}

builder.Services.AddOpenTelemetryMetrics(builder =>
{
    builder.AddHttpClientInstrumentation();
    builder.AddAspNetCoreInstrumentation();
    builder.AddInstrumentation<MyMetrics>();
    builder.AddMeter(Measures.InstrumentationName);
    builder.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"));
});

builder.Logging.AddOpenTelemetry(builder =>
{
    builder.IncludeFormattedMessage = true;
    builder.IncludeScopes = true;
    builder.ParseStateValues = true;
    builder.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"));
});

var app = builder.Build();

app.MapGet("/", async (ILogger<Program> logger, IMyServices myServices) =>
{
    //await myServices.Foo();
    //await myServices.Bar();
    await myServices.PublishMessage();
    return Results.Ok();
});

app.Run();