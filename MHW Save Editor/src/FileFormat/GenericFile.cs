using MHW;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MHW_Save_Editor.Crypto;

namespace MHW_Save_Editor.FileFormat
{
    class GenericFile
    {
        private byte[] data;
        private Cipher cipher;

        private readonly string key;

        public GenericFile(byte[] data, string key)
        {
            this.data = data;
            this.key = key;
            this.cipher = new Cipher(key);
        }

        public void Decrypt()
        {
            if (MessageBox.Show("File other than Savefile detected, decrypt?", "Other file: Decrypt", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                data = cipher.Decipher(data);
                MessageBox.Show("File decrypted.", "Other file: Decrypt", MessageBoxButton.OK);
            }
        }

        public void Encrypt()
        {
            if (MessageBox.Show("File other than Savefile detected, encrypt?", "Other file: Encrypt", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                data = cipher.Encipher(data);
                MessageBox.Show("File encrypted.", "Other file: Encrypt", MessageBoxButton.OK);
            }
        }

        public void Save(string path)
        {
            Stream fStream = File.Create(path);
            fStream.Write(data, 0, data.Length);
            fStream.Close();
        }
    }
}
