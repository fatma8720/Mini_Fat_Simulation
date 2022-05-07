using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini_Fat
{
   public static class Program
    {
        public static Directory current_directory;
        public static string currentPath;
        public static string file_path = @"C:\Users\mom\source\repos\Mini_Fat\Mini_Fat\Fat_File.txt";
        //parsing string to command and arguments 
        static string[] ParseArguments(string commandLine)
        {
            char[] parmChars = commandLine.ToCharArray();
            //bool inQuote = false; 
            for (int index = 0; index < parmChars.Length; index++)
            {

                if (/*!inQuote &&*/ parmChars[index] == ' ')
                    parmChars[index] = '\n';
            }
            return (new string(parmChars)).Split('\n');
        }
        public static void Main(string[] args)
        {
           
            Virtual_Disk.initialize(file_path);
            string s = "file";
            File_Entry o = new File_Entry("za", 0x0, 0, s.Length,s, current_directory);
           // o.WriteFileContent();
           // o.ReadFileContent();
            current_directory.Directory_table.Add(o);
            current_directory.Write_Directory();
            while (true)
            {
                currentPath = new string(current_directory.File_name);
                string curDir = new string(current_directory.File_name);
                Console.Write(curDir + "\\>");
                string dir = Console.ReadLine();
                string[] ParseArg = ParseArguments(dir);
                if (ParseArg[0].ToUpper() == "CLS")
                {
                    Commands.CLS();
                }
                else if (ParseArg[0].ToUpper() == "DIR")
                {
                    Commands.DIR();
                }
                else if (ParseArg[0].ToUpper() == "QUIT")
                {
                    Commands.QUIT();
                }
                else if (ParseArg[0].ToUpper() == "HELP")
                {
                    if (ParseArg.Length == 1)
                    {
                        Commands.HELP();
                    }
                    else if (ParseArg.Length > 1)
                    {
                        Commands.HELP(ParseArg[1]);
                    }
                }
                else if (ParseArg[0].ToUpper() == "MD")
                {
                    Commands.MD(ParseArg[1]);
                }
                else if (ParseArg[0].ToUpper() == "CD")
                {
                    Commands.CD(ParseArg[1]);
                }
                else if (ParseArg[0].ToUpper() == "RD")
                {
                    Commands.RD(ParseArg[1]);
                }
                else if (ParseArg[0].ToUpper() == "IMPORT")
                {
                    Commands.IMPORT(ParseArg[1]);
                }
                else if (ParseArg[0].ToUpper() == "DEL")
                {
                    Commands.DEL(ParseArg[1]);
                }
                else if (ParseArg[0].ToUpper() == "TYPE")
                {
                    Commands.TYPE(ParseArg[1]);
                }
                else if (ParseArg[0].ToUpper() == "RENAME")
                {
                    Commands.RENAME(ParseArg[1], ParseArg[2]);
                }
                else if (ParseArg[0].ToUpper() == "COPY")
                {
                    Commands.COPY(ParseArg[1], ParseArg[2]);
                }
                else if (ParseArg[0].ToUpper() == "EXPORT")
                {
                    Commands.EXPORT(ParseArg[1], ParseArg[2]);
                }
                else
                {
                    Console.WriteLine(" '{0}' is not recognized as an internal or external command,operable program or batch file.", ParseArg[0]);
                }
                //byte[] block = new byte[1];
                //block[0] = (byte)'*';
                //Fat.initialize();
                //Fat.Write_Fat_table();
                //fat.print_Fat_table();
                //int[] res = fat.Get_Fat_table();
                //for (int i = 0; i < res.Length; i++)
                //{
                //    Console.WriteLine(res[i]);
                //}

                //Console.WriteLine(Fat.Get_available_Block());
                //Console.WriteLine(Fat.Get_Next(5));
                // Fat.Set_Next(5, 7);
                // Fat.Write_Fat_table();
                // Console.WriteLine(Fat.Get_available_Block());
                // Console.WriteLine(Fat.Get_available_Blocks());
                // Fat.print_Fat_table();
                // Virtual_Disk.Write_Block(block, 6);
                // byte[] resblock = Virtual_Disk.Get_Block(6);
                // for (int i = 0; i < resblock.Length; i++)
                // {
                //     Console.WriteLine(i + " -  " + (char)resblock[i]);
                // }

                // Console.WriteLine((int)Math.Ceiling(2050 / 1024.0));

                Console.ReadKey();
            }

        }
    }
}