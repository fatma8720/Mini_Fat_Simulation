using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini_Fat
{
    public class Directory : Directory_Entry
    {
       public List<Directory_Entry> Directory_table;
       public Directory parent;

        public Directory(char[] File_name, byte File_attribut, int First_cluster, Directory parent, int File_size) :base(new string (File_name), File_attribut, First_cluster,File_size)
        {
            this.File_name = File_name;
            this.File_attribut = File_attribut;
            this.First_cluster = First_cluster; // cause exception error in fat index in get next
            this.parent = parent;
            Directory_table = new List<Directory_Entry>();
            if (parent != null)
            {
                this.parent = parent;
            }
        }
        public static byte[] Convert_string_Bytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        public static string  Convert_Byte_string(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
        public static List<byte[]> SplitBytes(byte[] bytes)
        {
            List<byte[]> big_divided_list = new List<byte[]>();
            int number_of_full_size_blocks = bytes.Length / 1024;
            int remainder = bytes.Length % 1024;
            for (int i = 0; i < number_of_full_size_blocks; i++)
            {
                byte[] arr = new byte[1024];
                for (int j = i * 1024, k = 0; k < 1024; j++, k++)
                {
                    arr[k] = bytes[j];
                }
                big_divided_list.Add(arr);
            }
            if (remainder > 0)
            {
                byte[] arr2 = new byte[1024];
                for (int i = number_of_full_size_blocks * 1024, k = 0; k < remainder; i++, k++)
                {
                    arr2[k] = bytes[i];
                }
                big_divided_list.Add(arr2);
            }
            return big_divided_list;
        }
        public void Write_Directory()
        {
            //Convert directory to byte
            byte[] Directory_table_bytes = new byte[32 * (Directory_table.Count)];
            byte[] Directory_entry_bytes = new byte[32];
            //put evry byte in whole directoru by entry entry in huge array 
            for (int i = 0; i < Directory_table.Count; i++)
            {
                Directory_entry_bytes = Directory_table[i].Get_Bytes();
                int c, j;
                for (j = i * 32, c = 0; c < 32; c++, j++)
                {
                    Directory_table_bytes[j] = Directory_entry_bytes[c];
                }
            }
            //count blocks and calculate remainder
            int Number_of_required_blocks = (int)Math.Ceiling(Directory_table_bytes.Length / 1024.0);
            int Number_of_fulllsize_blocks = Directory_table_bytes.Length / 1024;
            int Remainder = Directory_table_bytes.Length % 1024;
            //last index - frist empty place after fat index
            int Fat_index;
            int Last_index = -1;
            if (First_cluster != 0)
            {
                Fat_index = First_cluster;
            }
            else
            {
                Fat_index = Fat.Get_available_Block();
                First_cluster = Fat_index;
            }
            // divide array passed to be wrriten in virtual disk into blocks in list of arrays of bytes each one assign to (name,attribuit,.....) 
            List<byte[]> ls = new List<byte[]>();
            
            for (int I = 0; I < Number_of_fulllsize_blocks; I++)
            {
                byte[] b = new byte[1024];
                for (int j = 0; j < Directory_table_bytes.Length; j++)
                {
                    b[j% 1024] = Directory_table_bytes[j];
                    if ((j + 1) % 1024 == 0)
                        ls.Add(b);
                }
            }
            if (Remainder > 0)
            {
                byte[] b = new byte[1024];
                int start = Number_of_fulllsize_blocks * 1024;
                for (int i = start; i < (start + Remainder); i++)
                    b[i%1024] = Directory_table_bytes[i];
            }

            //check number of free blocks and if it enought or not
            if (Number_of_required_blocks <= Fat.Get_available_Blocks())
            {
                for (int i = 0; i < Number_of_fulllsize_blocks; i++)
                {
                    if (Fat_index != -1)
                    {
                        Virtual_Disk.Write_Block(ls[i], Fat_index);
                        Fat.Set_Next(Fat_index, -1);
                        if (Last_index != -1)
                        {
                            Fat.Set_Next(Last_index, Fat_index);

                        }
                        Last_index = Fat_index;
                        Fat_index = Fat.Get_available_Block();
                        // remainder *********************
                    }
                }
            }
            if (this.parent != null)
            {
                this.parent.Write_Directory();
            }
            //  Remainder = Fat_index;
            //Fat_index = Fat.Get_available_Block();
            //Virtual_Disk.Write_Block(ls[fu], Fat_index);
            //Fat.Set_Next(Fat_index, -1);
            Fat.Write_Fat_table();
        }
        public void Read_Directory() ///////// change in directory table list in the top of the code or not ---- code 1 or 2 ... next or fatindex equals -1 ??? ************
        {
            //List<Directory_Entry> Directory_Entry_list = new List<Directory_Entry>();
            int Fat_index=0, next = 0;
            if (First_cluster != 0)
            {
                Fat_index = First_cluster;
                next = Fat.Get_Next(Fat_index);
            }
            List<byte> B = new List<byte>();
            while (Fat_index.Equals(-1)){
                B.AddRange(Virtual_Disk.Get_Block(Fat_index));
                Fat_index = next;
                if (Fat_index != -1)
                {
                    next = Fat.Get_Next(Fat_index);
                }
            }

            //List<byte[]> ls = new List<byte[]>();
            //ls = SplitBytes(B.ToArray());
            //for (int I = 0; I < ls.Count / 2; I++)
            //{
            //    Directory_table.Add(Get_Directory_Entry(ls[I]));
            //}

            //if (this.First_cluster != 0)
            //{
            //    Directory_table = new List<Directory_Entry>();
            //    int fatIndex = this.First_cluster;
            //    int lastIndex = Fat.Get_Next(fatIndex);
            //    List<byte> ls = new List<byte>();
            //    do
            //    {
            //        ls.AddRange(Virtual_Disk.Get_Block(fatIndex));
            //        fatIndex = lastIndex;
            //        if (fatIndex != -1)
            //            lastIndex = Fat.Get_Next(fatIndex);
            //    }
            //    while (lastIndex!=-1);
            //    for (int i = 0; i < ls.Count; i++)
            //    {
            //        byte[] b = new byte[32];
            //        for (int k = i * 32, m = 0; m < b.Length && k < ls.Count; m++, k++)
            //        {
            //            b[m] = ls[k];
            //        }
            //        if (b[0] == 0)
            //            break;
            //        Directory_table.Add(Get_Directory_Entry(b));
            //    }
            //}
        }
        public int Search_Directory(string filename)
        {
            Read_Directory(); 
            for (int i = 0; i < Directory_table.Count; i++)
            {
                if (new string(Directory_table[i].File_name) == filename)
                {
                    return i;
                }
            }
                return -1;
        }
        public void update_content(Directory_Entry d)
        {
            Read_Directory();
            int index = Search_Directory(new string(d.File_name));
            if(index!=-1)
            {
                Directory_table.RemoveAt(index);
                Directory_table.Insert(index, d);
                Write_Directory();
            }
        }
        public void delete_directory()
        {
            if (First_cluster != 0)
            {
                int index = First_cluster;
                int next = Fat.Get_Next(index);
                do
                {
                    Fat.Set_Next(index, 0);
                    index = next;
                    if (index != -1)
                    {
                        next = Fat.Get_Next(index);
                    }
                }
                while (index != -1);
            }

            //check if it is parent
            if (parent != null)
            {
                parent.Read_Directory();
                int index_parnet = parent.Search_Directory(new string(this.File_name));
                if (index_parnet != -1)
                {
                    parent.Directory_table.RemoveAt(index_parnet);
                    parent.Write_Directory();
                    Fat.Write_Fat_table();
                }
            }
            //    if (this.First_cluster != 0)
            //    {
            //        int cluster = this.First_cluster;
            //        int next = Fat.Get_Next(cluster);
            //        do
            //        {
            //            Fat.Set_Next(cluster, 0);
            //            cluster = next;
            //            if (cluster != -1)
            //                next = Fat.Get_Next(cluster);
            //        }
            //        while (cluster != -1);
            //    }
            //    if (this.parent != null)
            //    {
            //        int index = this.parent.Search_Directory(new string(this.File_name));
            //        if (index != -1)
            //        {
            //            this.parent.Directory_table.RemoveAt(index);
            //            this.parent.Write_Directory();

            //        }
            //    }
            //    if (Program.current_directory == this)
            //    {
            //        if (this.parent != null)
            //        {
            //            Program.current_directory = this.parent;
            //            Program.currentPath = Program.currentPath.Substring(0, Program.currentPath.LastIndexOf('\\'));
            //            Program.current_directory.Read_Directory();
            //        }
            //    }
            //    Fat.Write_Fat_table();
            //}
        }

    }

   
}
