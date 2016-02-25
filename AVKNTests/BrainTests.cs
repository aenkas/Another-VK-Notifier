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
        public void IncreaseEntropy_Doesnt_Work_Without_Initialization()
        {
            Brain brain = new Brain();

            Assert.IsFalse(brain.IncreaseEntropy());
        }

        [TestMethod]
        public void InitBrain_Arguments_Testing()
        {
            Brain brain = new Brain();
            MsgReceiver mr = new MsgReceiver();
            Notifier notifier = new Notifier();

            Assert.IsFalse(brain.InitBrain(null, null));
            Assert.IsFalse(brain.InitBrain(mr, null));
            Assert.IsFalse(brain.InitBrain(null, notifier));
            Assert.IsTrue(brain.InitBrain(mr, notifier));
        }

        [TestMethod]
        public void Constructor_PropertiesNotNull_Tests()
        {
            Brain brain = new Brain();

            Assert.IsNotNull(brain.NotifyAboutPersonal);
            Assert.IsNotNull(brain.NotifyAboutDialogs);
            Assert.IsNotNull(brain.NotifyAboutGroups);
            Assert.IsNotNull(brain.Login);
            Assert.IsNotNull(brain.Password);

            Assert.AreEqual(true, brain.NotifyAboutPersonal);
            Assert.AreEqual(true, brain.NotifyAboutDialogs);
            Assert.AreEqual(true, brain.NotifyAboutGroups);
            Assert.AreEqual("", brain.Login);
            Assert.AreEqual("", brain.Password);
        }

        [TestMethod]
        public void Correct_Property_Values()
        {
            Brain brain = new Brain();
            string login = "1";
            string password = "2";

            brain.Login = login;
            brain.Password = password;

            Assert.AreEqual(login, brain.Login);
            Assert.AreEqual(password, brain.Password);

            for (int i = 0; i < 8; i++)
            {
                bool notifyAboutPersonal = (i & 1) > 0;
                bool notifyAboutDialogs = (i & 2) > 0;
                bool notifyAboutGroups = (i & 4) > 0;

                brain.NotifyAboutPersonal = notifyAboutPersonal;
                brain.NotifyAboutDialogs = notifyAboutDialogs;
                brain.NotifyAboutGroups = notifyAboutGroups;

                Assert.AreEqual(notifyAboutPersonal, brain.NotifyAboutPersonal);
                Assert.AreEqual(notifyAboutDialogs, brain.NotifyAboutDialogs);
                Assert.AreEqual(notifyAboutGroups, brain.NotifyAboutGroups);
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
