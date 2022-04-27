using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Arweave.NET.Models
{
    public abstract class BaseModel
    {
        public override string ToString()
        {
            return JsonSerializer.Serialize(this,this.GetType(), new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Used to show "nice" shorten string values with certain amount start and finish symbols to prevent huge responses
        /// </summary>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public string ToExampleString(int maxLength = 64)
        {
            var clone = this.MemberwiseClone();
            var typeFields = clone.GetType().GetProperties();
            foreach (var pi in typeFields)
            {
                if(pi.PropertyType==typeof(string))
                {
                    var str = pi.GetValue(clone).ToString();
                    if(str.Length>maxLength)
                    {
                        var sb = new StringBuilder();
                        sb.Append(str.Substring(0, maxLength / 2));
                        sb.Append("...");
                        sb.Append(str.Substring(str.Length - maxLength / 2 - 1, maxLength / 2));
                        pi.SetValue(clone, sb.ToString());
                    }
                }
            }

            return clone.ToString();
        }

        #region Helper methods
        #endregion
    }
}
