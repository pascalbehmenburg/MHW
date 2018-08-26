using Microsoft.Win32;
using System;
using System.IO;

namespace MHW_Save_Editor
{
    class Utility
    {
        public static string getSteamPath()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string steamPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", "");
            if (steamPath != "")
            {
                steamPath = steamPath + "/userdata";
                Console.WriteLine("Found SteamPath " + steamPath);
                bool foundGamePath = false;
                foreach (string userdir in Directory.GetDirectories(steamPath))
                {
                    foreach (string gamedir in Directory.GetDirectories(userdir))
                        if (gamedir.Contains("582010"))
                        {
                            steamPath = (gamedir + "\\remote").Replace('/', '\\');
                            Console.WriteLine("Found GameDir " + steamPath);
                            foundGamePath = true;
                            break;
                        }

                    if (foundGamePath)
                        break;
                }
            }
            return steamPath;
        }
    }
}
