using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AVKN;

namespace AVKNTests
{
    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void Constructor_PropertiesNotNull_Tests()
        {
            Message message = new Message();

            Assert.IsNotNull(message.DomainUrl);
            Assert.IsNotNull(message.MsgText);
            Assert.IsNotNull(message.MsgType);
            Assert.IsNotNull(message.MsgUrl);
            Assert.IsNotNull(message.SenderName);

            Assert.AreEqual(message.DomainUrl, "");
            Assert.AreEqual(message.MsgText, "");
            Assert.AreEqual(message.MsgType, AVKN.MsgTypes.Private);
            Assert.AreEqual(message.MsgUrl, "");
            Assert.AreEqual(message.SenderName, "");
        }

        [TestMethod]
        public void Correct_Property_Values()
        {
            Message message = new Message();
            string domainUrl = "1";
            string msgText = "2";
            MsgTypes msgType = MsgTypes.Group;
            string msgUrl = "4";
            string senderName = "5";

            message.DomainUrl = domainUrl;
            message.MsgText = msgText;
            message.MsgType = msgType;
            message.MsgUrl = msgUrl;
            message.SenderName = senderName;

            Assert.AreEqual(message.DomainUrl, domainUrl);
            Assert.AreEqual(message.MsgText, msgText);
            Assert.AreEqual(message.MsgType, msgType);
            Assert.AreEqual(message.MsgUrl, msgUrl);
            Assert.AreEqual(message.SenderName, senderName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DomainUrl_ExceptionOnNull()
        {
            Message message = new Message();

            message.DomainUrl = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MsgText_ExceptionOnNull()
        {
            Message message = new Message();

            message.MsgText = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MsgUrl_ExceptionOnNull()
        {
            Message message = new Message();

            message.MsgUrl = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SenderName_ExceptionOnNull()
        {
            Message message = new Message();

            message.SenderName = null;
        }
    }
}
