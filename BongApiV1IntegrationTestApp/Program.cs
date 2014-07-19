using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BongApiV1;
using BongApiV1.Public;
using Microsoft.Win32;

namespace BongApiV1IntegrationTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var username = GetCurrentUserRegistryText("bong_user");
            var password = GetCurrentUserRegistryText("bong_cred");

            var session = new BongSession(username, password, waitMillisecondsBetweenCalls: 100);
        }

        static string GetCurrentUserRegistryText(string name, string defaultValue = "")
        {
            var retval = defaultValue;

            try
            {
                var key = Registry.CurrentUser.OpenSubKey("Software\\Thaliana");

                if (key != null)
                {
                    var o = key.GetValue(name);

                    if (o != null)
                    {
                        retval = o.ToString();
                    }
                }
            }
            catch (Exception)
            {
            }

            return retval;
        }
    }
}
