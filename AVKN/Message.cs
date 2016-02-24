using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVKN
{
    enum MsgTypes
    {
        Private = 1,
        Dialog = 2,
        Group = 4
    }

    class Message
    {
        public MsgTypes MsgType { get; set; }

        public string SenderName { get; set; }

        public string MsgText { get; set; }

        public string MsgUrl { get; set; }

        public string DomainUrl { get; set; }

        public Message() { }
    }
}
