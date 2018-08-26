using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MHW_Save_Editor
{
    class SaveFile
    {
        private readonly byte[] data;
        private byte[] checksum;
        private byte[] generatedChecksum;
        private bool decrypted;

        private uint magic;
        private int size;
        private Int64 steamID;

        public SaveFile(byte[] saveFileData)
        {
            this.data = saveFileData;

            for (int i = 0; i < 4; i++)
            {
                if (i == 0 && !(data[i] == 1))
                    break;

                if (i > 0 && !(data[i] == 0))
                    break;

                decrypted = true;
            }

            if (!decrypted)
                Decrypt();
            
            this.size = data.Length;
            readSteamID();
        }

        public void Decrypt()
        {            
            if (!decrypted)
            {
                MemoryStream data = new MemoryStream(this.data);
                var buffer = new byte[8];
                while (data.Position < data.Length)
                {
                    if (data.Read(buffer, 0, buffer.Length) != buffer.Length)
                    {
                        throw new Exception("Invalid read size");
                    }
                    buffer = Crypto.Decrypt(buffer);
                    data.Position = data.Position - 8;
                    data.Write(buffer, 0, buffer.Length);
                }
                this.decrypted = true;
                MessageBox.Show("File decrypted.", "Decryption", MessageBoxButton.OK);
            }

            MessageBox.Show("File loaded.", "Load", MessageBoxButton.OK);
        }

        public void Encrypt()
        {
            if (decrypted)
            {
                MemoryStream data = new MemoryStream(this.data);
                var buffer = new byte[8];

                while (data.Position < data.Length)
                {
                    if (data.Read(buffer, 0, buffer.Length) != buffer.Length)
                    {
                        throw new Exception("Invalid read size");
                    }
                    buffer = Crypto.Encrypt(buffer);
                    data.Position = data.Position - 8;
                    data.Write(buffer, 0, buffer.Length);
                }
                MessageBox.Show("File saved.", "Save", MessageBoxButton.OK);
            }
        }

        public int FileSize()
        {
            return size;
        }

        public bool isDecrypted()
        {
            return decrypted;
        }

        public void Save(string path)
        {
            Stream fStream = File.Create(path);
            fStream.Write(data, 0, data.Length);
            fStream.Close();
        }

        private void readSteamID()
        {
            var mem_stream = new MemoryStream(data);
            byte[] steam_id = new byte[8];
            mem_stream.Position = 0x28;
            mem_stream.Read(steam_id, 0, steam_id.Length);
            this.steamID = BitConverter.ToInt64(steam_id, 0);
        }

        public Int64 getSteamID()
        {
            return this.steamID;
        }
    }
}
