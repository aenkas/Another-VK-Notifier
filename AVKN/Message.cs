using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVKN
{
    public enum MsgTypes
    {
        Personal = 1,
        Dialog = 2,
        Group = 4
    }

    public class Message
    {
        MsgTypes msgType;
        string senderName;
        string msgText;
        string msgUrl;
        string domainUrl;

        public MsgTypes MsgType
        {
            get
            {
                return msgType;
            }

            set
            {
                msgType = value;
            }
        }

        public string SenderName
        {
            get
            {
                return senderName;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                senderName = value;
            }
        }

        public string MsgText
        {
            get
            {
                return msgText;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                msgText = value;
            }
        }

        public string MsgUrl
        {
            get
            {
                return msgUrl;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                msgUrl = value;
            }
        }

        public string DomainUrl
        {
            get
            {
                return domainUrl;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                domainUrl = value;
            }
        }

        public Message()
        {
            msgType = MsgTypes.Personal;
            senderName = "";
            msgText = "";
            msgUrl = "";
            domainUrl = "";
        }
    }
}
