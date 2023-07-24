using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;


public static class Logger
{
    private static readonly ILog log;

    static Logger()
    {
      

        string currentPath = Directory.GetCurrentDirectory();
        string parentPath = Directory.GetParent(currentPath)?.Parent?.Parent?.Parent?.FullName;

        string configPath = Path.Combine(parentPath + "\\Backend", "log4net.config");

        // Use the configPath to configure log4net


        XmlConfigurator.Configure(new FileInfo(configPath));



        log = LogManager.GetLogger(typeof(Logger));
    }

    public static ILog GetLogger(Type type)
    {
        return LogManager.GetLogger(type);
    }
}
