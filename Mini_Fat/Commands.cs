using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Mini_Fat
{
   public static class Commands
    {
        public static void CLS()
        {
            Console.Clear();
        }
        public static void QUIT()
        {
            Environment.Exit(0);
        }
        public static void HELP()
        {
            Dictionary<string, string> My_dict1 =
               new Dictionary<string, string>()
               {
                                  {"CLS   ","Clear Screen"},
                                  {"QUIT  ","Close windows"},
                                  {"HELP  ","Provides Help information for commands." },
                                  {"CD    ","Change the current default directory to ."},
                                  {"DIR   ","List the contents of directory ."},
                                  {"COPY  ","Copies one or more files to another location"},
                                  {"DEL   ","Deletes one or more files."},
                                  {"MD    ","Creates a directory."},
                                  {"RD    ","Removes a directory."},
                                  {"RENAME","Renames a file."},
                                  {"TYPE  ","Displays the contents of a text file."},
                                  {"IMPORT","import text file(s) from your computer"},
                                  {"EXPORT","export text file(s) to your computer"}
               };
            Console.WriteLine("For more information on a specific command, type HELP command - name");
            foreach (KeyValuePair<string, string> ele1 in My_dict1)
            {
                Console.WriteLine("{0}                        {1}", ele1.Key, ele1.Value);
            }
            Console.WriteLine("For more information on tools see the command-line reference in the online help.");
        }
        public static void HELP(string arg)
        {
            bool b = false;
            Dictionary<string, string> My_dict1 =
             new Dictionary<string, string>(){
                                  {"CLS","Clear Screen"},
                                  {"HELP","Know Commands"},
                                  {"QUIT","Close windows"},
                                  {"CD","    Change the current default directory to ."},
                                  {"DIR","   List the contents of directory ."},
                                  {"COPY","  Copies one or more files to another location"},
                                  {"DEL","   Deletes one or more files."},
                                  {"MD","    Creates a directory."},
                                  {"RD","    Removes a directory."},
                                  {"RENAME","Renames a file."},
                                  {"TYPE","  Displays the contents of a text file."},
                                  {"IMPORT","import text file(s) from your computer"},
                                  {"EXPORT","export text file(s) to your computer"}
             };
            foreach (KeyValuePair<string, string> elem in My_dict1)
            {
                if (elem.Key == arg.ToUpper())
                {
                    Console.WriteLine("{0}                        {1}", elem.Key, elem.Value);
                    b = true;
                }

            }
            if (b!=true)
            {
                Console.WriteLine("This command is not supported by the help utility. Try \"{0} /?\"", arg);
            }

        }
        public static void Touch(string name)
        {
            if (name.Length < 11)
            {
                for (int i = name.Length; i < 11; i++)
                {
                    name += " ";
                }

            }
            if (Program.current_directory.Search_Directory(name) == -1)
            {
                if ((name.ToUpper()).Contains("TXT"))
                {
                    string s = "fatma we are here";
                    File_Entry o = new File_Entry(name, 0x0, 0, s.Length, s, Program.current_directory);
                    o.WriteFileContent();
                    Program.current_directory.Directory_table.Add(o);
                    Program.current_directory.Write_Directory();
                    if (Program.current_directory.parent != null)
                    {
                        Program.current_directory.parent.update_content(Program.current_directory.Get_Directory_Entry());
                        Program.current_directory.parent.Write_Directory();
                    }
                }
                else
                {
                    Console.WriteLine(" files should contains .txt extention");
                }
            }
            else
            {
                Console.WriteLine(" This File already exists");
            }
        }
        public static void MD(string name)
        {

            if (name.Length < 11)
            {
                for (int i = name.Length; i < 11; i++)
                {
                    name += " ";
                }

            }
            if (Program.current_directory.Search_Directory(name) == -1)
            {
                Directory_Entry d = new Directory_Entry(name, 0x10, 0, 0);
                    Program.current_directory.Directory_table.Add(d);
                    Program.current_directory.Write_Directory();

                    if (Program.current_directory.parent != null)
                    {
                        Program.current_directory.parent.update_content(Program.current_directory.Get_Directory_Entry());
                        Program.current_directory.parent.Write_Directory();
                    }
            }
            else
            {
                Console.WriteLine(" This Folder already exists");
            }
        }
        public static void RD(string name)
        {
            if (name.Length < 11)
            {
                for (int i = name.Length; i < 11; i++)
                {
                    name += " ";
                }

            }
            int index=Program.current_directory.Search_Directory(name);
            if (index != -1)
            {
                int fc = Program.current_directory.Directory_table[index].First_cluster;
                Directory d = new Directory(name.ToCharArray(), 0x10, fc, Program.current_directory,0);
                d.delete_directory();
                Program.current_directory.Write_Directory();
                if (Program.current_directory.parent != null)
                {
                    Program.current_directory.parent.Write_Directory();
                }
            }
            else
            {
                Console.WriteLine(" This Folder Not exists");
            }
        }
        public static void DEL(string file_name)
        {
            if (file_name.Length < 11)
            {
                for (int i = file_name.Length; i < 11; i++)
                {
                    file_name += " ";
                }

            }
            int index = Program.current_directory.Search_Directory(file_name);
            if (index != -1)
            {
                if (Program.current_directory.Directory_table[index].File_attribut == 0x0)
                {
                    int fc = Program.current_directory.Directory_table[index].First_cluster;
                    int size = Program.current_directory.Directory_table[index].File_size;
                    File_Entry o = new File_Entry(file_name, 0X0, fc, size, " ", Program.current_directory);
                    o.WriteFileContent();
                    o.DeleteFile();
                    Program.current_directory.Write_Directory();
                    if (Program.current_directory.parent != null)
                    {
                        Program.current_directory.parent.Write_Directory();
                    }
                }
                else
                {
                    Console.WriteLine("system can`t find the file specified.");
                }
            }
            else
            {
                Console.WriteLine("system can`t find the file specified.");
            }
        }
        public static void CD(string name)
        {
            string path;
            int index = Program.current_directory.Search_Directory(name);
            if (index != -1)
            {
                int fc = Program.current_directory.Directory_table[index].First_cluster;
                Directory d = new Directory(name.ToCharArray(), 0x10, fc, Program.current_directory,0);
                Program.current_directory = d;
                Program.currentPath += "\\" +new string( Program.current_directory.File_name);
                d.Read_Directory();
                //path = Program.currentPath;
                //path += "\\" + name.Trim(new char[] { '\0', ' ' });
                //    Program.currentPath = path;
            }
            else
            {
                Console.WriteLine(" This Folder Not exists");
            }
        }
        public static void DIR()
        {
            int file_numbers = 0, directories_numbers = 0, files_size = 0, free_space = Fat.Get_free_space();
            Console.WriteLine(" Directory of "+ Program.currentPath);
            for (int i = 0; i < Program.current_directory.Directory_table.Count; i++)
            {
                if (Program.current_directory.Directory_table[i].File_attribut == 0x0)
                {
                    Console.WriteLine("              " + Program.current_directory.Directory_table[i].File_size + "  " + new string(Program.current_directory.Directory_table[i].File_name));
                    file_numbers++;
                    files_size += Program.current_directory.Directory_table[i].File_size;
                }
                else
                {

                    Console.WriteLine("<DIR>            " + new string(Program.current_directory.Directory_table[i].File_name));
                    directories_numbers++;
                }
            }
            Console.WriteLine( file_numbers + "  File(s)             " + files_size + " bytes");
            Console.WriteLine( directories_numbers + "  Dir(s)             " + free_space + " bytes free");
        }
        public static void IMPORT(string file_path)
        {
            if (File.Exists(file_path))
            {
                string content = File.ReadAllText(file_path);
                int size = content.Length;
                int name_start = file_path.LastIndexOf("\\");
                string name;
                name = file_path.Substring(name_start + 1);
                int index = Program.current_directory.Search_Directory(name);
                if (index == -1)
                {
                    int fc;
                    if (size > 0)
                    {
                        fc = Fat.Get_available_Block();
                        File_Entry o = new File_Entry(name, 0x0, fc, size, content, Program.current_directory);
                        o.WriteFileContent();
                    }
                    else
                    {
                        fc = 0;
                        File_Entry o = new File_Entry(name, 0x0, fc, size, content, Program.current_directory);
                        o.WriteFileContent();
                    }
                  // size = (content.LastIndexOf("  "));
                    Directory_Entry d = new Directory_Entry(name, 0x0, fc, size);
                    Program.current_directory.Directory_table.Add(d);
                    Program.current_directory.Write_Directory();
                }
                else
                {
                    Console.WriteLine("this file already exsit.");
                }
            }
            else
            {
                Console.WriteLine("this file doesn`t exsit.");
            }
        }
        public static void EXPORT(string source, string destination)
        {
            int index = Program.current_directory.Search_Directory(source);
            if (index != -1)
            {
                if (System.IO.Directory.Exists(destination))
                {
                    int fc = Program.current_directory.Directory_table[index].First_cluster;
                    string content = null;
                    int size = Program.current_directory.Directory_table[index].File_size;
                    File_Entry o = new File_Entry(source, 0x0, fc, size, content, Program.current_directory);
                    o.ReadFileContent();
                    string des = destination + "\\" + source;
                    StreamWriter f = new StreamWriter(des);
                    f.Write(o.content);
                    f.Flush();
                    f.Close();
                }
                else
                {
                    Console.WriteLine("system can`t find the path specified in computer desk .");
                }
            }
            else
            {
                Console.WriteLine("this file doesn`t exsit in virtual disk.");
            }
        }
        public static void TYPE(string file_name)
        {
            int index = Program.current_directory.Search_Directory(file_name);

            if (index != -1)
            {
                int fc=Program.current_directory.Directory_table[index].First_cluster;
                int size= Program.current_directory.Directory_table[index].File_size;
                string content = null;
                File_Entry o = new File_Entry(file_name, 0x0, fc, size, content, Program.current_directory);
                o.ReadFileContent();
                Console.WriteLine(o.content);
            }
            else
            {
                Console.WriteLine("system can`t find this file.");
            }
        }
        public static void RENAME(string old_name, string new_name)
        {
            int index = Program.current_directory.Search_Directory(old_name);
            if (index != -1)
            {
                int b = Program.current_directory.Search_Directory(new_name);
                if (b==-1)
                {
                    Directory_Entry o = Program.current_directory.Directory_table[index];
                    if (new_name.Length < 11)
                    {
                        for (int i = new_name.Length; i < 11; i++)
                        {
                            new_name += " ";
                        }

                    }
                    o.File_name = new_name.ToCharArray();

                    Program.current_directory.Directory_table.RemoveAt(index);
                    Program.current_directory.Directory_table.Insert(index,o);
                    Program.current_directory.Write_Directory();
                    Program.current_directory.Read_Directory();
                }
                else
                {
                    Console.WriteLine("Duplicate file name exist or file can not found.");
                }
            }
            else
            {
                Console.WriteLine("system can`t find the path specified.");
            }
        }
        public static void COPY(string source, string destination)
        {
            int index = Program.current_directory.Search_Directory(source);
            if (index != -1)
            {
                EXPORT(source, "C:\\Users\\mom\\source\\repos\\Mini_Fat\\Mini_Fat");
                int name_start = destination.LastIndexOf("\\");
                string name;
                name = destination.Substring(name_start + 1);
                if (name.Length < 11)
                {
                    for (int i = name.Length; i < 11; i++)
                    {
                        name += " ";
                    }

                }
                int indexdest = Program.current_directory.Search_Directory(name);
                if (indexdest != -1)
                {
                    if (destination != new string(Program.current_directory.File_name.ToArray()))
                    {
                        Directory n = Program.current_directory;
                        CD(name);
                        IMPORT("C:\\Users\\mom\\source\\repos\\Mini_Fat\\Mini_Fat\\" + source);
                        Program.current_directory.Write_Directory();
                        if (Program.current_directory.parent != null)
                        {
                            Program.current_directory.parent.update_content(Program.current_directory.Get_Directory_Entry());
                            Program.current_directory.parent.Write_Directory();
                        }
                        Program.current_directory=n;
                        Program.currentPath= new string(Program.current_directory.File_name);
                    }
                    else
                    {
                        Console.WriteLine("can`t copy the file in same destination.");
                    }
                }
                else
                {
                    Console.WriteLine("system can`t find the path specified.");
                }
            }
            else
            {
                Console.WriteLine("system can`t find the path specified source.");
            }
        }
    }
}
