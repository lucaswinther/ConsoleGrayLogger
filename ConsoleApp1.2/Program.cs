using Gelf.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ConsoleGrayLogger
{
  public class Program
  {
    public static void Main()
    {
      var configuration = new ConfigurationBuilder()
          .AddJsonFile(@"appsettings.json")
          .Build();

      var serviceProvider = new ServiceCollection()
          .AddLogging()
          .BuildServiceProvider();

      try
      {
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        loggerFactory
            .AddConsole(configuration.GetSection("Logging:Console"))
            .AddGelf(configuration.GetSection("Logging:Graylog").Get<GelfLoggerOptions>());

        UseLogger(serviceProvider.GetRequiredService<ILogger<Program>>());
      }
      finally
      {
        // The LoggerFactory must be disposed before the program exits to ensure all queued messages are sent.
        ((IDisposable)serviceProvider).Dispose();
      }
    }

    private static void UseLogger(ILogger<Program> logger)
    {

      for (int i = 0; i < 10; i++)
      {
        const string framework = "HTTP_3";

        logger.LogInformation("Information log using {framework}", framework);
        logger.Log(LogLevel.Error, "DEU RUIM - ", framework);
        logger.LogCritical("DEU MUITO RUIM - ", framework);
        logger.LogError("DEU RUIM 2 - ", framework);
        logger.LogTrace("Passei por aqui usando ", framework);

        try
        {
          System.IO.StreamReader sw = new System.IO.StreamReader(@"C:\Temp\a.txt");
        }
        catch (Exception ex)
        {
          logger.LogError( ex ,"Error X");
        }



        using (logger.BeginScope(("scope_field_1", "foo")))
        {
          logger.LogDebug("Debug log from {framework}", framework);

          using (logger.BeginScope(new Dictionary<string, object>
          {
            ["scope_field_2"] = "bar",
            ["scope_field_3"] = "baz"
          }))
          {
            logger.LogTrace("Trace log from {framework}", framework);
          }

          logger.LogError(new EventId(), new Exception("Example exception!"),
              "Error log from {framework}", framework);
        }
      }

    }
  }
}
