using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace botof37s.services
{
    public class Authorisationcheck
    {
        
        public bool Check(ulong id, IConfiguration _config)
        {
            if (id.ToString() == _config["AdminUserID"])
                return true;
            if (File.Exists($"authorized/{id}.37"))
                return true;
            return false;
        }
    }
}
