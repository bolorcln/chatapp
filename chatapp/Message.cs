using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatapp
{
    class Message
    {
        public string senderName;
        public string text;

        public Message(string senderName, string text)
        {
            this.senderName = senderName;
            this.text = text;
        }
    }
}
