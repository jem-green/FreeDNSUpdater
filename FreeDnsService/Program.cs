using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace FreeDnsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (System.Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                    default:
                        Console.WriteLine("Usage:\n" + 
                                          "--install     Install the service.\n" +
                                          "--uninstall   Uninstall the service.\n");
                        break;
                }
            }
            else
            {
                ServiceBase.Run(new FreeDnsService());
            }
        }
    }
}
