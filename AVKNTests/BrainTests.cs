using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AVKN;
using Moq;

namespace AVKNTests
{
    [TestClass]
    public class BrainTests
    {
        [TestMethod]
        public void BrainDrain_Clears_Message_Ids_And_No_Doubling_Test()
        {
            Brain brain = new Brain();
            var mr = new Mock<IMsgReceiver>();
            var notifier = new Mock<INotifier>();
            Message msg1 = new Message(), msg2 = new Message(), msg3 = new Message();
            int messages_count = 0;
            bool showNotificationCalled = false;
            string desiredNotificationText = "";

            Assert.IsTrue(brain.InitBrain(mr.Object, notifier.Object));

            msg1.Id = 1;
            msg2.Id = 2;
            msg3.Id = 3;

            mr.Setup(foo => foo.IsLogged()).Returns(true);
            mr.Setup(foo => foo.GetMessagesCount()).Returns(() => { return messages_count; });
            mr.Setup(foo => foo.PopFirstMsg()).Returns(() => 
                {
                    if (messages_count == 3)
                        return msg3;
                    else if (messages_count == 2)
                        return msg2;
                    else
                        return msg1;
                } ).Callback(() => { messages_count--; });
            notifier.Setup(foo => foo.ShowNotification(It.IsAny<Notification>())).Returns((Notification n) =>
                {
                    Assert.AreEqual(desiredNotificationText, n.NotificationText);

                    showNotificationCalled = true;

                    return true;
                });

            // Получаем 2 новых сообщения
            showNotificationCalled = false;
            messages_count = 2;
            desiredNotificationText = "У вас 2 непрочитанных сообщений";
            Assert.AreEqual(2, mr.Object.GetMessagesCount());
            Assert.IsTrue(brain.IncreaseEntropy(), "brain.IncreaseEntropy(), Получаем 2 новых сообщения");
            Assert.IsTrue(showNotificationCalled, "showNotificationCalled, Получаем 2 новых сообщения");
            Assert.AreEqual(0, mr.Object.GetMessagesCount());

            // Те же 2 новых сообщения
            showNotificationCalled = false;
            messages_count = 2;
            Assert.AreEqual(2, mr.Object.GetMessagesCount());
            Assert.IsTrue(brain.IncreaseEntropy(), "brain.IncreaseEntropy(), Те же 2 новых сообщения");
            Assert.IsFalse(showNotificationCalled, "showNotificationCalled, Те же 2 новых сообщения");
            Assert.AreEqual(0, mr.Object.GetMessagesCount());

            // Те же 2 + 1 новое сообщение
            showNotificationCalled = false;
            messages_count = 3;
            desiredNotificationText = "У вас 3 непрочитанных сообщений";
            Assert.AreEqual(3, mr.Object.GetMessagesCount());
            Assert.IsTrue(brain.IncreaseEntropy(), "brain.IncreaseEntropy(), Те же 2 + 1 новое сообщение");
            Assert.IsTrue(showNotificationCalled, "showNotificationCalled, Те же 2 + 1 новое сообщение");
            Assert.AreEqual(0, mr.Object.GetMessagesCount());

            // Те же 3 новых сообщения
            showNotificationCalled = false;
            messages_count = 3;
            Assert.AreEqual(3, mr.Object.GetMessagesCount());
            Assert.IsTrue(brain.IncreaseEntropy(), "brain.IncreaseEntropy(), Те же 3 новых сообщения");
            Assert.IsFalse(showNotificationCalled, "showNotificationCalled, Те же 3 новых сообщения");
            Assert.AreEqual(0, mr.Object.GetMessagesCount());

            // Те же 3 новых сообщения после вызова BrainDrain()
            showNotificationCalled = false;
            messages_count = 3;
            Assert.AreEqual(3, mr.Object.GetMessagesCount());
            brain.BrainDrain();
            Assert.IsTrue(brain.IncreaseEntropy(), "brain.IncreaseEntropy(), Те же 3 новых сообщения после вызова BrainDrain()");
            Assert.IsTrue(showNotificationCalled, "showNotificationCalled, Те же 3 новых сообщения после вызова BrainDrain()");
            Assert.AreEqual(0, mr.Object.GetMessagesCount());
        }

        [TestMethod]
        public void InitBrain_Returns_True_If_Logged_And_False_If_Not()
        {
            Brain brain = new Brain();
            var mr = new Mock<IMsgReceiver>();
            var notifier = new Mock<Notifier>();

            Assert.IsTrue(brain.InitBrain(mr.Object, notifier.Object));

            mr.Setup(foo => foo.GetMessagesCount()).Returns(0);

            // Не залогинен
            mr.Setup(foo => foo.IsLogged()).Returns(false);
            
            Assert.IsFalse(brain.IncreaseEntropy());

            // Залогинен
            mr.Setup(foo => foo.IsLogged()).Returns(true);
            Assert.IsTrue(brain.IncreaseEntropy());
        }

        [TestMethod]
        public void Unimplemented_Methods_Returns_False()
        {
            Brain brain = new Brain();

            Assert.IsFalse(brain.LoadSettings());
            Assert.IsFalse(brain.SaveSettings());
        }

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
            var mr = new Mock<IMsgReceiver>();
            var notifier = new Mock<Notifier>();

            Assert.IsFalse(brain.InitBrain(null, null));
            Assert.IsFalse(brain.InitBrain(mr.Object, null));
            Assert.IsFalse(brain.InitBrain(null, notifier.Object));
            Assert.IsTrue(brain.InitBrain(mr.Object, notifier.Object));
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
