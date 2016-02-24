using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AVKN;

namespace AVKNTests
{
    [TestClass]
    public class NotificationTests
    {
        [TestMethod]
        public void BuildNotification_Correctly_Works_With_Single_Message_And_Different_Message_Type()
        {
            Notification notification = new Notification();
            Message messagePersonal = new Message();
            Message messageDialog = new Message();
            Message messageGroup = new Message();

            messagePersonal.MsgType = MsgTypes.Personal;
            messagePersonal.SenderName = "PersonA";
            messagePersonal.MsgText = "BLAHblahBLAH";
            messagePersonal.MsgUrl = "http://example.com/A/123";
            messagePersonal.DomainUrl = "http://example.com/A";
            notification.AddMessage(messagePersonal);
            notification.BuildNotification();
            Assert.AreEqual("Сообщения ВКонтакте", notification.NotificationHeader);
            Assert.AreEqual(messagePersonal.SenderName + ": " + messagePersonal.MsgText, notification.NotificationText);
            Assert.AreEqual(messagePersonal.MsgUrl, notification.NotificationUrl);

            messageDialog.MsgType = MsgTypes.Dialog;
            messageDialog.SenderName = "PersonB";
            messageDialog.MsgText = "HALBhalbHALB";
            messageDialog.MsgUrl = "http://example.com/B/456";
            messageDialog.DomainUrl = "http://example.com/B";
            notification.ClearMsgQueue();
            notification.AddMessage(messageDialog);
            notification.BuildNotification();
            Assert.AreEqual("Сообщения ВКонтакте", notification.NotificationHeader);
            Assert.AreEqual(messageDialog.SenderName + ": " + messageDialog.MsgText, notification.NotificationText);
            Assert.AreEqual(messageDialog.MsgUrl, notification.NotificationUrl);

            messageGroup.MsgType = MsgTypes.Group;
            messageGroup.SenderName = "PersonA";
            messageGroup.MsgText = "BLAHblahBLAH";
            messageGroup.MsgUrl = "http://example.com/C/456";
            messageGroup.DomainUrl = "http://example.com/C";
            notification.ClearMsgQueue();
            notification.AddMessage(messageGroup);
            notification.BuildNotification();
            Assert.AreEqual("Сообщения ВКонтакте", notification.NotificationHeader);
            Assert.AreEqual(messageGroup.SenderName + ": " + messageGroup.MsgText, notification.NotificationText);
            Assert.AreEqual(messageGroup.MsgUrl, notification.NotificationUrl);

            notification.ClearMsgQueue();
            notification.AddMessage(messagePersonal);
            notification.AddMessage(messageDialog);
            notification.BuildNotification();
            Assert.AreEqual("Сообщения ВКонтакте", notification.NotificationHeader);
            Assert.AreEqual("У вас 2 непрочитанных сообщений", notification.NotificationText);
            Assert.AreEqual("https://vk.com/", notification.NotificationUrl);

            notification.ClearMsgQueue();
            notification.AddMessage(messagePersonal);
            notification.AddMessage(messageGroup);
            notification.BuildNotification();
            Assert.AreEqual("Сообщения ВКонтакте", notification.NotificationHeader);
            Assert.AreEqual("У вас 2 непрочитанных сообщений", notification.NotificationText);
            Assert.AreEqual("https://vk.com/", notification.NotificationUrl);

            notification.ClearMsgQueue();
            notification.AddMessage(messageDialog);
            notification.AddMessage(messageGroup);
            notification.BuildNotification();
            Assert.AreEqual("Сообщения ВКонтакте", notification.NotificationHeader);
            Assert.AreEqual("У вас 2 непрочитанных сообщений", notification.NotificationText);
            Assert.AreEqual("https://vk.com/", notification.NotificationUrl);

            notification.ClearMsgQueue();
            notification.AddMessage(messageDialog);
            notification.AddMessage(messagePersonal);
            notification.AddMessage(messageGroup);
            notification.BuildNotification();
            Assert.AreEqual("Сообщения ВКонтакте", notification.NotificationHeader);
            Assert.AreEqual("У вас 3 непрочитанных сообщений", notification.NotificationText);
            Assert.AreEqual("https://vk.com/", notification.NotificationUrl);
        }

        [TestMethod]
        public void ClearMsgQueue_Makes_BuildNotification_False()
        {
            Notification notification = new Notification();
            Message message = new Message();

            Assert.IsFalse(notification.BuildNotification());

            message.DomainUrl = "1";
            message.MsgText = "2";
            message.MsgUrl = "3";
            message.SenderName = "4";

            notification.AddMessage(message);
            Assert.IsTrue(notification.BuildNotification());
            Assert.AreNotEqual("", notification.NotificationHeader);
            Assert.AreNotEqual("", notification.NotificationText);
            Assert.AreNotEqual("", notification.NotificationUrl);

            notification.ClearMsgQueue();
            Assert.AreEqual("", notification.NotificationHeader);
            Assert.AreEqual("", notification.NotificationText);
            Assert.AreEqual("", notification.NotificationUrl);
            Assert.IsFalse(notification.BuildNotification());
        }

        [TestMethod]
        public void Constructor_PropertiesNotNull_Tests()
        {
            Notification notification = new Notification();

            Assert.IsNotNull(notification.NotificationHeader);
            Assert.IsNotNull(notification.NotificationText);
            Assert.IsNotNull(notification.NotificationUrl);

            Assert.AreEqual("", notification.NotificationHeader);
            Assert.AreEqual("", notification.NotificationText);
            Assert.AreEqual("", notification.NotificationUrl);
        }

        [TestMethod]
        public void Correct_Property_Values()
        {
            Notification notification = new Notification();
            string notificationHeader = "1";
            string notificationText = "2";
            string notificationUrl = "3";

            notification.NotificationHeader = notificationHeader;
            notification.NotificationText = notificationText;
            notification.NotificationUrl = notificationUrl;

            Assert.AreEqual(notificationHeader, notification.NotificationHeader);
            Assert.AreEqual(notificationText, notification.NotificationText);
            Assert.AreEqual(notificationUrl, notification.NotificationUrl);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NotificationHeader_ExceptionOnNull()
        {
            Notification notification = new Notification();

            notification.NotificationHeader = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NotificationText_ExceptionOnNull()
        {
            Notification notification = new Notification();

            notification.NotificationText = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NotificationUrl_ExceptionOnNull()
        {
            Notification notification = new Notification();

            notification.NotificationUrl = null;
        }
    }
}
