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
        public static byte[] Convert_string_Bytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        public static string BytesToString(byte[] string_bytes)
        {
            string streng = string.Empty;
            for (int i = 0; i < string_bytes.Length; i++)
            {
                if ((char)string_bytes[i] != '\0')
                    streng += (char)string_bytes[i];
                else
                    break;
            }
            return streng;
        }
        public void WriteFileContent()
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
        }
        public void ReadFileContent()
        {
            // read block from virsual and put it in file content
            if (this.First_cluster != 0)
            {
                content = null;
                int fatIndex = this.First_cluster;
                int lastIndex = Fat.Get_Next(fatIndex);
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
                content = "fatma`s \n team \n file";
            }
        }
        public void DeleteFile()
        {
            if (this.First_cluster != 0)
            {
                int fatIndex = this.First_cluster;
                int lastIndex = Fat.Get_Next(fatIndex);
                do
                {
                    Fat.Set_Next(fatIndex, 0);
                    fatIndex = lastIndex;
                    if (fatIndex != -1)
                        lastIndex = Fat.Get_Next(fatIndex);
                }
                while (fatIndex != -1);
            }
            if (this.parent != null)
            {
                int index = this.parent.Search_Directory(new string(this.File_name));
                if (index != -1)
                {
                    this.parent.Directory_table.RemoveAt(index);
                    this.parent.Write_Directory();
                    Fat.Write_Fat_table();
                }
            }
        }

    }
}
