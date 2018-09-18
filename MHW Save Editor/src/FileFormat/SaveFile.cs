
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Windows;

namespace MHW
{
    class SaveFile
    {
        private Cipher cipher;

        public byte[] data;
        
        private readonly string key = "xieZjoe#P2134-3zmaghgpqoe0z8$3azeq";
        private int size;

        public SaveFile(byte[] data)
        {
            this.data = data;
            cipher = new Cipher(key);

            if (!isDecrypted())
                Decrypt();

            size = data.Length;
        }

        public bool isDecrypted()
        {
            return data[0] == 1 && data[1] == 0 && data[2] == 0 && data[3] == 0;
        }

        public void Decrypt()
        {            
            if (!isDecrypted())
            {
                data = cipher.Decipher(data);
                //MessageBox.Show("File decrypted.", "Decryption", MessageBoxButton.OK);
            }
        }

        public void Encrypt()
        {
            if (isDecrypted())
            {
                data = cipher.Encipher(data);
                //MessageBox.Show("File encrypted.", "Encryption", MessageBoxButton.OK);
            }
        }

        public int FileSize()
        {
            return size;
        }

        public void Save(string path, bool encrypt = true)
        {
            Array.Copy(Utility.bswap(GenerateChecksum()),0,data, 12, 20);
            if (encrypt)Encrypt();
            Stream fStream = File.Create(path);
            fStream.Write(data, 0, data.Length);
            fStream.Close();
        }

        public Int64 ReadSteamID()
        {
            var mem_stream = new MemoryStream(data);
            byte[] steam_id = new byte[8];
            mem_stream.Position = 0x28;
            mem_stream.Read(steam_id, 0, steam_id.Length);
            return BitConverter.ToInt64(steam_id, 0);
        }

        public void setSteamID(Int64 steamid)
        {
            byte[] newid =  BitConverter.GetBytes(steamid);
            Array.Copy(newid, 0, data, 0x28, newid.Length);        
        }

        public string GetChecksum()
        {
            return BitConverter.ToString(ReadChecksum()).Replace("-","");
        }

        private byte[] ReadChecksum()
        {
            byte[] checksum = new byte[20];
            Array.Copy(data, 12, checksum,0,20);
            return Utility.bswap(checksum);
        }
        
        public byte[] GenerateChecksum()
        {
            byte[] checksum = new byte[20];
            var temp = new byte[data.Length-64];
            Array.Copy(data, 64, temp, 0, data.Length - 64);
            SHA1.Create().ComputeHash(temp).CopyTo(checksum, 0);//
            return checksum;
        }
        
    }
}
