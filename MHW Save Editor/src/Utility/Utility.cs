using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MHW
{
    static class Utility
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

        public static byte[] bswap(this byte[] data)
        {
            var swapped = new byte[data.Length];
            for (var i = 0; i < data.Length; i += 4)
            {
                swapped[i] = data[i + 3];
                swapped[i + 1] = data[i + 2];
                swapped[i + 2] = data[i + 1];
                swapped[i + 3] = data[i];
            }
            return swapped;
        }
        
        public static T[] Slice<T>(this T[] sliceable, int start, int end)
        {
            T[] result = new T[end-start];
            for(int i = start; i<end; i++)result[i]=sliceable[i];
            return result;
        }
        
        public static int StartingIndex<T>(this IList<T> x, IList<T> y)where T:IEquatable<T> {
            IEnumerable<int> index = Enumerable.Range(0, x.Count - y.Count + 1);
            for (int i = 0; i < y.Count; i++) {
                index = index.Where(n => EqualityComparer<T>.Default.Equals(x[n + i],y[i]));
            }
            return index.First();
        }
    }
}
