﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
// using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using MvcApplication2.SignalR;
using System.Diagnostics;

namespace MvcApplication2
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            RouteTable.Routes.MapHubs();

            AreaRegistration.RegisterAllAreas();

            // Use LocalDB for Entity Framework by default
            // Database.DefaultConnectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            InitiGPIO();

            Thread watcher = new Thread(new ThreadStart(ThreadWatcher));
            watcher.Start();
            Thread watch = new Thread(new ThreadStart(ThreadWatch));
            watch.Start();
            return;
        }

        private void ThreadWatch()
        {
            CurrentState = new GPIOState();
            while (true)
            {

                var sw1 = GPIO24_Sw1.Value != 0;

                lock (CurrentState)
                {
                    if (CurrentState.sw1 != sw1)
                    {
                        CurrentState.sw1 = sw1;
                        CurrentState.led1 = GPIO25_LED1.Value != 0;

                        if (CurrentState.sw1 == true)
                        {
                            GPIO.Instance.UpdateButtonStatus("Button Pressed!!");
                            Console.WriteLine("Button Pressed");
                        }
                        else
                        {
                            GPIO.Instance.UpdateButtonStatus("Button Not Pressed!!");
                            Console.WriteLine("Button Not Pressed!");
                        }
                    }
                }


                Thread.Sleep(10);
            }
        }

        public static TinyGPIO GPIO24_Sw1 { get; set; }
        public static GPIOState CurrentState { get; set; }
        public static TinyGPIO GPIO25_LED1 { get; set; }
        public static NFCState CurrentNFC { get; set; }
        public static string Arguments { get; set; }
        public static string FileName { get; set; }
        public static bool RedirectStandardOutput { get; set; }
        public static bool UseShellExecute { get; set; }

        private static void InitiGPIO()
        {
            // init GPIO 24 for Switch1
            GPIO24_Sw1 = TinyGPIO.Export(24);
            GPIO24_Sw1.Direction = GPIODirection.In;

            // init GPIO 25 for LED1
            GPIO25_LED1 = TinyGPIO.Export(25);
            GPIO25_LED1.Direction = GPIODirection.Out;
        }
       
        static private void ThreadWatcher()
        {
            Thread.Sleep(10);

            ProcessStartInfo procStartInfo = new ProcessStartInfo();
            procStartInfo.FileName = "/home/pi/libnfc/libnfc-1.7.0-rc7/utils/nfc-list";
            procStartInfo.Arguments = "";
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;

            string outputStr;

            for (; ; )
            {
                Thread.Sleep(10);

                Process process = new Process { StartInfo = procStartInfo };
                process.Start();
                outputStr = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                NFC.Instance.UpdateNFCStatus(outputStr);
            }
        }
    }
}

