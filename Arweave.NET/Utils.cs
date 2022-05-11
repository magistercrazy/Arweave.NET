using Arweave.NET.Models;
using Arweave.NET.Services;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Arweave.NET
{
    public class Utils
    {
        private readonly EncryptionService encription = new EncryptionService();
        public byte[] ConcatBuffers(byte[] first, byte[] second)
        {
            return first.Concat(second).ToArray();
        }

        public byte[] IntToBuffer(int note)
        {
            var buffer = new byte[32];
            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                var oneByte = note % 256;
                buffer[i] = (byte)oneByte;
                note = (note - oneByte) / 256;
            }
            return buffer;
        }
       

        public string Base64Decode(string data)
        {
            data = data.Replace('-', '+').Replace('_', '/');
            var padding = 0;
            if (data.Length % 4 == 0)
            {
                padding = 0;
            }
            else
            {
                padding = 4 - (data.Length % 4);
            }
            data += string.Concat(Enumerable.Repeat("=", padding));
            return data;
        }

        public string Base64Encode(byte[] data)
        {
            var b64str = Convert.ToBase64String(data);
            return b64str.Replace('+', '-').Replace('/', '_').Replace('=', ' ').Trim();
        }


        public List<object> PrepareTags(Tag[] tags)
        {
            var tagsLst = new List<object>();
            if (tags.Length == 0)
                return tagsLst;

            foreach (var tag in tags)
            {
                var byteLst = new List<object>() 
                {
                    Convert.FromBase64String(Base64Decode(tag.Name)),
                    Convert.FromBase64String(Base64Decode(tag.Value))
                };
                tagsLst.Add(byteLst);
            }
            return tagsLst;
        }

        


    }
}
