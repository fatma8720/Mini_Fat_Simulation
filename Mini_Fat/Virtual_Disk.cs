using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mini_Fat
{
    public static class Virtual_Disk
    {
        public static void initialize(string path)
        {
            //Not exist .... do it fat file or file entity .. how to check if it exist or not? 
           
            if (!File.Exists(@"C:\Users\mom\source\repos\Mini_Fat\Mini_Fat\Fat_File.txt"))
            {
                FileStream file = new FileStream(@"C:\Users\mom\source\repos\Mini_Fat\Mini_Fat\Fat_File.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
 
                for (int i = 0; i < 1024 * 1024; i++)
                {
                    if (i < 1024)
                    {
                        // Using write() method
                        file.WriteByte(0);
                    }
                    if (i >= 1024 && i < 5120)
                    {
                        file.WriteByte((byte)'*');
                    }
                    if (i >= 5120)
                    {
                        file.WriteByte((byte)'#');
                    }
                }
                // Closing the file
                file.Close();
                Fat.initialize();
                Directory root = new Directory("H: ".ToCharArray(), 0x10, 5, null,0);
                root.Write_Directory();
             /****//// Fat.Set_Next(5, -1);
                Program.current_directory = root;
                Fat.Write_Fat_table();
                //Fat.initialize();
                //Directory root = new Directory("H: ".ToCharArray(), 0x10, 5, null);
                //root.Write_Directory();
                //Fat.Write_Fat_table();

                // initilize  virual initial values here or after else ?????????????????????????????????

            }
            //exist
            else
            {
                Fat.Get_Fat_table();
                Directory root = new Directory("H: ".ToCharArray(), 0x10, 5, null,0);
                root.Read_Directory();
                Program.current_directory = root;
                //Fat.Fat_table = Fat.Get_Fat_table();
                //Directory root = new Directory("H: ".ToCharArray(), 0x10, 5, null);
                //root.Read_Directory();
            }
            // FileStream instance
          
            // initializing values
           
        }
        public static void Write_Block(byte[] arr, int index)
        {
            using (FileStream fs = new FileStream(@"C:\Users\mom\source\repos\Mini_Fat\Mini_Fat\Fat_File.txt", FileMode.Open, FileAccess.Write))
            {
                fs.Seek(1024*index, SeekOrigin.Begin);
                for (int i = 0; i < arr.Length; i++)
                {
                    // Using write() method
                    fs.WriteByte(arr[i]);
                }
                fs.Close(); 
            }
        }

        public static byte[] Get_Block(int index)
        {
            byte[] result = new byte[1024];
            int[] int_result = new int[256];
            using (FileStream fs = new FileStream(@"C:\Users\mom\source\repos\Mini_Fat\Mini_Fat\Fat_File.txt", FileMode.Open, FileAccess.Read))
            {
                fs.Seek(1024*index, SeekOrigin.Begin);
                byte[] bytes = new byte[1024];
                fs.Read(bytes, 0, 1024);
                fs.Close();
                return bytes;
            }
        }
       
    }
}