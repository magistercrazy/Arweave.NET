using Arweave.NET.Models;
using Arweave.NET.Services;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Arweave.NET
{
    public class Utils
    {     
        public static byte[] ConcatBuffers(byte[] first, byte[] second)
        {
            return first.Concat(second).ToArray();
        }

        public static byte[] IntToBuffer(int note)
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
       

        public static string Base64Decode(string data)
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

        public static string Base64Encode(byte[] data)
        {
            var b64str = Convert.ToBase64String(data);
            return b64str.Replace('+', '-').Replace('/', '_').Replace('=', ' ').Trim();
        }


        public static List<object> PrepareTags(List<Tag> tags)
        {
            var tagsLst = new List<object>();
            if (tags.Count == 0)
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

        public static async Task<byte[]> ReadDataAsync(string dataPath)
        {            
            using FileStream fstream = File.OpenRead(dataPath);
            var buff = new byte[fstream.Length];
            await fstream.ReadAsync(buff.AsMemory(0, buff.Length));
            return buff;
        }

        public static JsonWebKey LoadJWK(string keyFilePath)
        {            
            using StreamReader sr = File.OpenText(keyFilePath);
            var jwksString = sr.ReadToEnd();
            var formattedString = "{ \"keys\":[" + jwksString + "]}";
            var jwks = new JsonWebKeySet(formattedString);
            if (jwks.Keys.Count > 1)
                throw new NotImplementedException("Key file has more then 1 key, please check");
            return jwks.Keys[0];
        
        }

        public static string GetFileFormat(string pathToFile)
        {
            var arr = pathToFile.Split('.');
            var format = arr.Last().ToLower();
            switch (format)
            {
                case "png":
                    return "image/png";
                case "jpeg":
                    return "image/jpeg";
                case "gif":
                    return "image/gif";
                case "jpg":
                    return "image/jpg";

                case "json":
                    return "application/json";
                case "pdf":
                    return "application/pdf";
                case "xml":
                    return "application/xml";
                case "docs":
                    return "application/msword";

                case "mpeg":
                    return "video/mpeg";
                case "mp4":
                    return "video/mp4";
                case "avi":
                    return "video/x-msvideo";

                case "html":
                    return "text/html";
                case "css":
                    return "text/css";
                case "csv":
                    return "text/csv";
                case "txt":
                    return "text/plain";
            }
            return "";
        }

    }
}
