using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatapp
{
    class User
    {
        public string name;
        public string ipString;

        public User(string name, string ipString)
        {
            this.name = name;
            this.ipString = ipString;
        }
    }
}
