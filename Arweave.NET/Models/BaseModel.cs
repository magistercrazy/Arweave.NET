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
    }
}
