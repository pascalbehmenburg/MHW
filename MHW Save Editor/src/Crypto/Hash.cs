using MHW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MHW_Save_Editor.src.Crypto
{
    class Hash
    {
        private byte[] data;
        private byte[] checksum;

        public Hash(byte[] data)
        {
            this.data = data;
        }

        public void GenerateChecksum()
        {
            var temp = new byte[data.Length-64];
            Array.Copy(data, 64, temp, 0, data.Length - 64);
            SHA1.Create().ComputeHash(temp).CopyTo(checksum, 0);
            Utility.bswap(checksum);
        }

        public bool ValidateChecksum(byte[] checksum)
        {
            return this.checksum == checksum;
        }

        public byte[] OverrideChecksum()
        {
            Array.Copy(checksum, 0, data, 12, 20);
            return data;
        }
    }
}
