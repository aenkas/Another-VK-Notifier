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
        public void Constructor_PropertiesNotNull_Tests()
        {
            Notification notification = new Notification();

            Assert.IsNotNull(notification.NotificationHeader);
            Assert.IsNotNull(notification.NotificationText);
            Assert.IsNotNull(notification.NotificationUrl);

            Assert.AreEqual(notification.NotificationHeader, "");
            Assert.AreEqual(notification.NotificationText, "");
            Assert.AreEqual(notification.NotificationUrl, "");
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

            Assert.AreEqual(notification.NotificationHeader, notificationHeader);
            Assert.AreEqual(notification.NotificationText, notificationText);
            Assert.AreEqual(notification.NotificationUrl, notificationUrl);
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
