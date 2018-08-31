using MHW_Save_Editor.src.Crypto;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace MHW
{
    class SaveFile
    {
        private Cipher cipher;
        private Hash hash;

        private byte[] data;
        private byte[] checksum;

        public bool validChecksum;
        
        private readonly string key = "xieZjoe#P2134-3zmaghgpqoe0z8$3azeq";
        private int size;

        public SaveFile(byte[] data)
        {
            this.data = data;
            cipher = new Cipher(key);
            hash = new Hash(data);

            if (!isDecrypted())
                Decrypt();


            checksum = ReadChecksum();
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
                MessageBox.Show("File decrypted.", "Decryption", MessageBoxButton.OK);
            }
        }

        public void Encrypt()
        {
            if (isDecrypted())
            {
                data = cipher.Decipher(data);
                MessageBox.Show("File encrypted.", "Encryption", MessageBoxButton.OK);
            }
        }

        public int FileSize()
        {
            return size;
        }

        public void Save(string path, bool replaceHash)
        {
            if (replaceHash)
            {
                hash.GenerateChecksum();
                data = hash.OverrideChecksum();
            }
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

        private byte[] ReadChecksum()
        {
            byte[] checksum = new byte[20];
            if (isDecrypted())
                Array.Copy(data, 12, checksum, 0, 20);
            this.checksum = checksum;
            return this.checksum;
        }

        public string GetChecksum()
        {
            return BitConverter.ToString(checksum).Replace("-", "");
        }
    }
}
