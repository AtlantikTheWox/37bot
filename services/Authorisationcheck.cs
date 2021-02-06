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
            if (!File.Exists("db/authorized.37"))
                return false;
            var allowed = File.ReadAllLines("db/authorized.37");
            foreach(string user in allowed)
            {
                if (ulong.Parse(user) == id)
                    return true;
            }
            return false;
        }
    }
}
