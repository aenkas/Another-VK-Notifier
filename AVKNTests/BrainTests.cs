using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AVKN;

namespace AVKNTests
{
    [TestClass]
    public class BrainTests
    {
        [TestMethod]
        public void Constructor_PropertiesNotNull_Tests()
        {
            Brain brain = new Brain();

            Assert.IsNotNull(brain.NotifyAboutPersonal);
            Assert.IsNotNull(brain.NotifyAboutDialogs);
            Assert.IsNotNull(brain.NotifyAboutGroups);
            Assert.IsNotNull(brain.Login);
            Assert.IsNotNull(brain.Password);

            Assert.AreEqual(brain.NotifyAboutPersonal, true);
            Assert.AreEqual(brain.NotifyAboutDialogs, true);
            Assert.AreEqual(brain.NotifyAboutGroups, true);
            Assert.AreEqual(brain.Login, "");
            Assert.AreEqual(brain.Password, "");
        }

        [TestMethod]
        public void Correct_Property_Values()
        {
            Brain brain = new Brain();
            string login = "1";
            string password = "2";

            brain.Login = login;
            brain.Password = password;

            Assert.AreEqual(brain.Login, login);
            Assert.AreEqual(brain.Password, password);

            for (int i = 0; i < 8; i++)
            {
                bool notifyAboutPersonal = (i & 1) > 0;
                bool notifyAboutDialogs = (i & 2) > 0;
                bool notifyAboutGroups = (i & 4) > 0;

                brain.NotifyAboutPersonal = notifyAboutPersonal;
                brain.NotifyAboutDialogs = notifyAboutDialogs;
                brain.NotifyAboutGroups = notifyAboutGroups;

                Assert.AreEqual(brain.NotifyAboutPersonal, notifyAboutPersonal);
                Assert.AreEqual(brain.NotifyAboutDialogs, notifyAboutDialogs);
                Assert.AreEqual(brain.NotifyAboutGroups, notifyAboutGroups);
            }
            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Login_ExceptionOnNull()
        {
            Brain brain = new Brain();

            brain.Login = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Password_ExceptionOnNull()
        {
            Brain brain = new Brain();

            brain.Password = null;
        }
    }
}
