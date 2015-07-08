// use TinyGPIO.cs
// https://github.com/sample-by-jsakamoto/SignalR-on-RaspberryPi/blob/master/myapp/TinyGPIO.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using MvcApplication2;
using MvcApplication2.SignalR;
using System.Diagnostics;

namespace MvcApplication2.Controllers
{
    
    public class HomeController : Controller
    {
        [HttpPut]
        public ActionResult TurnOnLED()
        {
            return null;
        }

        [HttpPut]
        public ActionResult TurnOffLED()
        {
            return null;
        }

        public ActionResult Index()
        {
            var procStartInfo = new ProcessStartInfo
{
#if DEBUG
    FileName = "cmd",
    Arguments = "/c dir %windir%",
#else
                FileName = "/home/pi/libnfc/libnfc-1.7.0-rc7/examples/nfc-poll",
                Arguments = "",
#endif

    RedirectStandardOutput = true,
    UseShellExecute = false
};
            var process = new Process { StartInfo = procStartInfo };
            process.Start();
            var outputStr = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return View((object)outputStr);
        }

        public ActionResult OnButton(string parameterName)
        {
            string result;
            string consoleResult;
            var gpio25 = TinyGPIO.Export(25);
            gpio25.Direction = (GPIODirection)GPIODirection.Out;

            if (parameterName == "ON")
            {
                gpio25.Value = 1;
                result = "On";
                consoleResult = "Turn On LED...";
            }
            else
            {
                gpio25.Value = 0;
                result = "Off";
                consoleResult = "Turn Off LED...";
            }

            Console.WriteLine(consoleResult);
            return Json(new { success = true, show = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OnButtonTestGPIO(string currentStatus)
        {
            if (currentStatus == "true") GPIO.Instance.UpdateButtonStatus("false");
            else GPIO.Instance.UpdateButtonStatus("true");

            return null;
        }
    }
}







