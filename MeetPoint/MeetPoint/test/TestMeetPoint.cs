using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MeetPoint.src;
using System.IO;

namespace MeetPoint.test
{
    [TestFixture]
    class TestMeetPoint
    {
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            FileInfo fileInfo = new FileInfo("Log4Net.config");
            log4net.Config.XmlConfigurator.Configure(fileInfo);
            MeetPointFactory.Clear();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            MeetPointFactory.Clear();
        }

        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
            MeetPointFactory.Cleanup();
        }

        [Test]
        public void CreatedMeetPointTest()
        {
            bool createdNew = false;

            IMeetPoint meetPoint = MeetPointFactory.Create("", out createdNew);
            Assert.IsNull(meetPoint);

            IMeetPoint meetPoint1 = MeetPointFactory.Create("CreatedNewTest", out createdNew);
            Assert.AreEqual(true, createdNew);

            IMeetPoint meetPoint2 = MeetPointFactory.Create("CreatedNewTest", out createdNew);
            Assert.AreEqual(false, createdNew);

            Assert.AreEqual(meetPoint1, meetPoint2);
        }

        [Test]
        public void PreCondTest1()
        {
            bool createdNew = false;
            string pointID = Guid.NewGuid().ToString();
            IMeetPoint meetPoint = MeetPointFactory.Create(pointID, out createdNew);

            PreCondTask preTask1 = new PreCondTask(meetPoint, 1);
            PreCondTask preTask2 = new PreCondTask(meetPoint, 1);
            Assert.IsTrue(meetPoint.IsBlocked);
            Assert.IsTrue(preTask1.IsBlocked);
            Assert.IsTrue(preTask2.IsBlocked);

            preTask1.RunAsync();
            Assert.IsTrue(preTask1.WaitArrive(1000));
            Assert.IsTrue(meetPoint.IsBlocked);
            Assert.IsTrue(preTask1.IsBlocked);
            Assert.IsTrue(preTask2.IsBlocked);

            preTask2.RunAsync();
            Assert.IsTrue(preTask2.WaitArrive(1000));
            Assert.IsFalse(meetPoint.IsBlocked);

            Assert.IsTrue(preTask1.WaitTaskUnblock(1000));
            Assert.IsFalse(preTask1.IsBlocked);

            Assert.IsTrue(preTask2.WaitTaskUnblock(1000));
            Assert.IsFalse(preTask2.IsBlocked);
        }

        [Test]
        public void PreAndPostCondTest1()
        {
            bool createdNew = false;
            string pointID = Guid.NewGuid().ToString();
            IMeetPoint meetPoint = MeetPointFactory.Create(pointID, 2, 1, out createdNew);

            PreCondTask preTask1 = new PreCondTask(meetPoint, 1);
            PreCondTask preTask2 = new PreCondTask(meetPoint, 1);
            PostCondTask postTask1 = new PostCondTask(meetPoint, 1);
            Assert.IsTrue(meetPoint.IsBlocked);
            Assert.IsTrue(preTask1.IsBlocked);
            Assert.IsTrue(preTask2.IsBlocked);
            Assert.IsTrue(postTask1.IsBlocked);

            preTask1.RunAsync();
            Assert.IsTrue(preTask1.WaitArrive(1000));
            Assert.IsTrue(meetPoint.IsBlocked);

            Assert.IsTrue(preTask1.IsBlocked);
            Assert.IsTrue(preTask2.IsBlocked);
            Assert.IsTrue(postTask1.IsBlocked);

            preTask2.RunAsync();
            Assert.IsTrue(preTask2.WaitArrive(1000));
            Assert.IsFalse(meetPoint.IsBlocked);

            Assert.IsTrue(preTask1.WaitTaskUnblock(1000));
            Assert.IsFalse(preTask1.IsBlocked);

            Assert.IsTrue(preTask2.WaitTaskUnblock(1000));
            Assert.IsFalse(preTask2.IsBlocked);

            Assert.IsTrue(postTask1.IsBlocked);

            postTask1.RunAsync();
            Assert.IsTrue(postTask1.WaitArrive(1000));
            Assert.IsTrue(postTask1.WaitTaskUnblock(1000));
            Assert.IsFalse(postTask1.IsBlocked);
        }

        [Test]
        public void PreAndPostCondTest2()
        {
            bool createdNew = false;
            string pointID = Guid.NewGuid().ToString();
            IMeetPoint meetPoint = MeetPointFactory.Create(pointID, 3, 1, out createdNew);

            PreCondTask preTask1 = new PreCondTask(meetPoint, 2);
            PreCondTask preTask2 = new PreCondTask(meetPoint, 1);
            PostCondTask postTask1 = new PostCondTask(meetPoint, 1);
            Assert.IsTrue(meetPoint.IsBlocked);
            Assert.IsTrue(preTask1.IsBlocked);
            Assert.IsTrue(preTask2.IsBlocked);
            Assert.IsTrue(postTask1.IsBlocked);

            preTask1.RunAsync();
            Assert.IsTrue(preTask1.WaitArrive(1000));
            Assert.IsTrue(meetPoint.IsBlocked);

            Assert.IsTrue(preTask1.IsBlocked);
            Assert.IsTrue(preTask2.IsBlocked);
            Assert.IsTrue(postTask1.IsBlocked);

            preTask2.RunAsync();
            Assert.IsTrue(preTask2.WaitArrive(1000));
            Assert.IsFalse(meetPoint.IsBlocked);

            Assert.IsTrue(preTask1.WaitTaskUnblock(1000));
            Assert.IsFalse(preTask1.IsBlocked);

            Assert.IsTrue(preTask2.WaitTaskUnblock(1000));
            Assert.IsFalse(preTask2.IsBlocked);

            Assert.IsTrue(postTask1.IsBlocked);

            postTask1.RunAsync();
            Assert.IsTrue(postTask1.WaitArrive(1000));
            Assert.IsTrue(postTask1.WaitTaskUnblock(1000));
            Assert.IsFalse(postTask1.IsBlocked);
        }

        [Test]
        public void PreCondIsZeroTest3()
        {
            bool createdNew = false;
            string pointID = Guid.NewGuid().ToString();
            IMeetPoint meetPoint = MeetPointFactory.Create(pointID, 0, 2, out createdNew);

            Assert.IsNull(meetPoint);
        }

        [Test]
        public void MeetPointCleanupTest()
        {
            int pointCount = MeetPointFactory.MeetPointCount;

            bool createdNew = false;
            string pointID = Guid.NewGuid().ToString();
            IMeetPoint meetPoint = MeetPointFactory.Create(pointID, 2, 1, out createdNew);

            PreCondTask preTask1 = new PreCondTask(meetPoint, 1);
            PreCondTask preTask2 = new PreCondTask(meetPoint, 1);
            PostCondTask postTask1 = new PostCondTask(meetPoint, 1);

            preTask1.RunAsync();
            Assert.IsTrue(preTask1.WaitArrive(1000));

            preTask2.RunAsync();
            Assert.IsTrue(preTask2.WaitArrive(1000));

            postTask1.RunAsync();
            Assert.IsTrue(postTask1.WaitArrive(1000));

            // meet point count
            Assert.AreEqual(pointCount + 1, MeetPointFactory.MeetPointCount);
            MeetPointFactory.Cleanup();
            Assert.AreEqual(pointCount, MeetPointFactory.MeetPointCount);
        }
    }
}
