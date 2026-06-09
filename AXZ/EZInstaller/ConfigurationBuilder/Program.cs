using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;


namespace ConfigurationBuilder
{
    internal class Program
    {
        [STAThread]
        static void Main()
        {
            string directory = @"C:\ProgramData\Autodesk\Revit\Addins\{0}\BeyondRevit\Resources\";

            Console.WriteLine("Which version? Sperate By Comma");
            string input = Console.ReadLine();

            string config = "";
            foreach (string year in input.Split(','))
            {
                string folder = string.Format(directory, year.Trim());
                foreach (string file in GetBmpFilesInCurrentDirectory())
                {
                    string line = string.Format("{0} => {1}{0}\n", file, folder);
                    config += line;
                }
                config += "\n";
            }

            Console.WriteLine(config);
            Clipboard.SetText(config);
            Console.WriteLine("\n\nConfiguration Copied to Clipboard!");
            Console.ReadLine();
        }

        public static List<string> GetBmpFilesInCurrentDirectory()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string[] bmpFilepaths = Directory.GetFiles(currentDirectory, "*.bmp");
            List<string> files = new List<string>();
            foreach (string bmpFilepath in bmpFilepaths)
            {
                files.Add(Path.GetFileName(bmpFilepath));
            }
            return files;
        }
    }
}
