using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHW_Save_Editor.Crypto
{
    class Cipher
    {
        private Blowfish blowfish;
        private readonly string key;

        public Cipher(string key)
        {
            this.key = key;
            this.blowfish = new Blowfish(Encoding.Default.GetBytes(this.key));
        }

        public byte[] Encipher(byte[] data)
        {
            data = Utility.bswap(data);
            data = blowfish.Encrypt_ECB(data);
            data = Utility.bswap(data);
            return data;
        }

        public byte[] Decipher(byte[] data)
        {
            data = Utility.bswap(data);
            data = blowfish.Decrypt_ECB(data);
            data = Utility.bswap(data);
            return data;
        }
    }
}
