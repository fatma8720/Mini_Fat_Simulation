using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini_Fat
{
    // file entity how it differ from directory entry 
   public class File_Entry : Directory_Entry
    {
        public string content;
        public Directory parent;
        public File_Entry(string name, byte File_attribut, int First_cluster,int File_size,string content, Directory pa) : base(name, File_attribut, First_cluster, File_size)
        {
            this.content = content;
            if (pa != null)
            {
                parent = pa;
            }
        }
        //public Directory_Entry GetDirectory_Entry()
        //{
        //    Directory_Entry me = new Directory_Entry(new string(this.File_name), this.File_attribut, this.First_cluster);
        //    return me;
        //}
      /*  public void WriteFileContent2()
        {
            // write file in virsual disk 
            byte[] content_byte = Convert_string_Bytes(content);
            int Number_of_required_blocks = (int)Math.Ceiling(content_byte.Length / 1024.0);
            int Number_of_fulllsize_blocks = content_byte.Length / 1024;
            int Remainder = content_byte.Length % 1024;
            List<byte[]> bytesls = Directory.SplitBytes(content_byte);
            int Fat_index, last_index = -1;
            if (this.First_cluster != 0)
            {
                Fat_index = this.First_cluster;
            }
            else
            {
                Fat_index = Fat.Get_available_Block();
                this.First_cluster = Fat_index;
            }
            if (Number_of_required_blocks <= Fat.Get_available_Blocks())
            {
                for (int i = 0; i < bytesls.Count; i++)
                {
                    if (Fat_index != -1)
                    {
                        Virtual_Disk.Write_Block(bytesls[i], Fat_index);
                        Fat.Set_Next(Fat_index, -1);
                        if (last_index != -1)
                            Fat.Set_Next(last_index, Fat_index);
                        last_index = Fat_index;
                        Fat_index = Fat.Get_available_Block();
                    }

                }
            }
            Fat.Write_Fat_table();
        }*/
        public void WriteFileContent()
        {
            byte[] content_byte = Encoding.Default.GetBytes(content);
         //   byte[] content_byte = Convert_string_Bytes(content);
            int Number_of_required_blocks = (int)Math.Ceiling(content_byte.Length / 1024.0);
            int Number_of_fulllsize_blocks = content_byte.Length / 1024;
            int Remainder = content_byte.Length % 1024;
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
                for (int j = 0; j < content_byte.Length; j++)
                {
                    b[j % 1024] = content_byte[j];
                    if ((j + 1) % 1024 == 0)
                        ls.Add(b);
                }
            }
            if (Remainder > 0)
            {
                byte[] b = new byte[1024];
                int start = Number_of_fulllsize_blocks * 1024;
                for (int i = start; i < (start + Remainder); i++)
                    b[i % 1024] = content_byte[i];
                ls.Add(b);
            }

            //check number of free blocks and if it enought or not
            if (Number_of_required_blocks <= Fat.Get_available_Blocks())
            {
                for (int i = 0; i < ls.Count; i++)
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
                    }
                }
            }
            Fat.Write_Fat_table();
        }
       /* public void ReadFileContent2()
        {
            // read block from virsual and put it in file content
            int fatIndex = 0, lastIndex = 0;
            if (First_cluster != 0 && Fat.Get_Next(First_cluster) != 0)
            {
                 content = null;
                 fatIndex =First_cluster;
                 lastIndex = Fat.Get_Next(fatIndex);
                List<byte> ls = new List<byte>();
                do
                {
                    ls.AddRange(Virtual_Disk.Get_Block(fatIndex));
                    fatIndex = lastIndex;
                    if (fatIndex != -1)
                        lastIndex = Fat.Get_Next(fatIndex);
                }
                while (lastIndex != -1);
                //ls.ToArray() because function "BytesToString" take array of byte no list
                content = BytesToString(ls.ToArray());
            }
        }*/
        public void ReadFileContent()
        {
            //Directory_table = new List<Directory_Entry>();
            int Fat_index = 0, next = 0;
            if (First_cluster != 0 && Fat.Get_Next(First_cluster) != 0)
            {
                content = null;
                Fat_index = First_cluster;
                next = Fat.Get_Next(Fat_index);
                List<byte> B = new List<byte>();
                do
                {
                    B.AddRange(Virtual_Disk.Get_Block(Fat_index));
                    Fat_index = next;
                    if (Fat_index != -1)
                    {
                        next = Fat.Get_Next(Fat_index);
                    }
                } while (next != -1);
                content = Encoding.Default.GetString(B.ToArray());
                //content = BytesToString(B.ToArray());
                //byte[] b = new byte[32];
                //for (int i = 0; i < B.Count; i++)
                //{

                //    b[i % 32] = B[i];
                //    if ((i + 1) % 32 == 0)
                //    {
                //        Directory_Entry d = Get_Directory_Entry(b);
                //        if (d.File_name[0] != '\0')

                //            Directory_table.Add(d);

                //    }
                //}

            }
        }
        public void DeleteFile()
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
                string ss = new string(File_name);
                int index_parnet = parent.Search_Directory(ss);
                if (index_parnet != -1)
                {
                    parent.Directory_table.RemoveAt(index_parnet);
                    parent.Write_Directory();
                    Fat.Write_Fat_table();
                }
            }
        }

    }
}
