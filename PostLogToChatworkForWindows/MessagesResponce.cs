using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PostLogToChatworkForWindows
{

    [DataContract]
    class MessagesResponce
    {
        [DataMember(Name = "message_id")]
        public string messageID;
    }
}
