using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mini_Fat
{
   public static class Fat
    {
        public static int[] Fat_table;
        //Fat()
        //{
        //    Fat_table = new int[1024];
        //}
        public static void initialize()
        {
            Fat_table = new int[1024];
            Fat_table[0] = -1;
            Fat_table[1] = -1;
            Fat_table[2] = -1;
            Fat_table[3] = -1;
            Fat_table[4] = -1;
        }
        public static void Write_Fat_table()
        {
            using (FileStream fs = new FileStream(@"C:\Users\mom\source\repos\Mini_Fat\Mini_Fat\Fat_File.txt", FileMode.Open, FileAccess.Write))
            {
                fs.Seek(1024, SeekOrigin.Begin);
                byte[] result = new byte[4096];
                Buffer.BlockCopy(Fat_table, 0, result, 0, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    // Using write() method
                    fs.WriteByte(result[i]);
                }
                fs.Close();
            }
        }
        public static int[] Get_Fat_table()
        {
            Fat.initialize();
            byte[] result = new byte[4096];
           // int[] int_result = new int[1024];
            using (FileStream fs = new FileStream(@"C:\Users\mom\source\repos\Mini_Fat\Mini_Fat\Fat_File.txt", FileMode.Open, FileAccess.Read))
            {
                fs.Seek(1024, SeekOrigin.Begin);
                fs.Read(result, 0, result.Length);

                fs.Close();
                //int_result = Array.ConvertAll(result, Convert.ToInt32);
                Buffer.BlockCopy(result, 0, Fat_table, 0, result.Length);
                return Fat_table;
            }
            //return int_result;

        }
        public static void print_Fat_table()
        {
           // int[] res = Get_Fat_table();
            for (int i = 0; i < Fat_table.Length; i++)
            {
                Console.WriteLine(i + " -  " + Fat_table[i]);
            }
        }
        public static int Get_available_Block()
        {
            int i;
           // int[] res = Get_Fat_table();
            for (i = 0; i < Fat_table.Length; i++)
            {
                if (Fat_table[i] == 0)
                {
                    return i;
                }
            }
            
            return -1;
        }
        public static int Get_Next(int index) {
           
                return Fat_table[index];
        }
        public static void Set_Next(int index,int value)
        {
            Fat_table[index] = value;
        }
        public static int Get_available_Blocks()
        {
            int i,c=0;
            for (i = 0; i < Fat_table.Length; i++)
            {
                if (Fat_table[i] == 0)
                {
                   c+=1;
                }
            }
            return c;
        }
        public static int Get_free_space()
        {
            int fs = Get_available_Blocks() * 1024;
            return fs;
        }
    }
}


