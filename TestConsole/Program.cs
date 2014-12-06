using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using Anticaptcha;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string key = "5f12031f91668a9022b788066739fd0e";
            anticaptcha ac = new anticaptcha(key);

            int uploadMode = 3;

            //Step 1. Uploading file CAPTCHA "01.png" to anticaptcha service pixodrom.com
            string res = "";
            while (!res.StartsWith("OK|"))
            {
                switch (uploadMode)
                {
                    case 1:
                        res = ac.UploadBase64("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");
                        break;
                    case 2:
                        res = ac.UploadImage(Image.FromFile("02.gif"));
                        break;
                    case 3:
                        ac.is_russian = 1;
                        res = ac.UploadURL("https://vk.com/captcha.php?s=1&sid=3");
                        break;
                    default:
                        res = ac.UploadFile("01.png");
                        break;
                }

                Console.WriteLine(res);
                Thread.Sleep(2000);
            }

            //get CAPTCHA ID
            string id = res.Split(new char[] {'|'})[1];

            string answer = "CAPCHA_NOT_READY";

            //Step 2.waiting for the CAPTCHA recognition
            while (answer == "CAPCHA_NOT_READY")
            {
                Thread.Sleep(3000);
                answer = ac.GetResult(id);
                Console.WriteLine(answer);
            }

            //If OK, CAPTCHA regognized well
            if (answer.StartsWith("OK|"))
            {
                answer = answer.Split(new char[] { '|' })[1];
                Console.WriteLine("Answer is '{0}'", answer);
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
