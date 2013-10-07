using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using ParallelTaskScheduler.Src;

namespace ParallelTaskScheduler.Test
{
    [TestFixture]
    class TestParallelTaskScheduler
    {
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            FileInfo fileInfo = new FileInfo("Log4Net.config");
            log4net.Config.XmlConfigurator.Configure(fileInfo);

            KeyValuePool.Clear();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            KeyValuePool.Clear();
        }

        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void SchedulerTest1()
        {
            ITaskItem task1 = new TaskItemImpl("task1", 200, "task1");
            ITaskItem task2 = new TaskItemImpl("task2", 200, "task2");
            ITaskItem task3 = new TaskItemImpl("task3", 500, "task3");
            ITaskItem task4 = new TaskItemImpl("task4", 100, "task4");

            TaskContainer container = new TaskContainer().AddParallel(task1)
                .AddOrdered(task2)
                .AddOrdered(task3)
                .AddOrdered(task4);

            DateTime t0 = DateTime.Now;
            Src.ParallelTaskScheduler.Schedule(container);
            DateTime t1 = DateTime.Now;

            // total time (1000)
            Assert.IsTrue((1000 < (t1 - t0).TotalMilliseconds));
            Assert.IsTrue(((t1 - t0).TotalMilliseconds < 1500));
        }

        [Test]
        public void SchedulerTest2()
        {
            ITaskItem task1 = new TaskItemImpl("task1", 500, "task1");
            ITaskItem task2 = new TaskItemImpl("task2", 500, "task2");
            ITaskItem task3 = new TaskItemImpl("task3", 1000, "task3");
            ITaskItem task4 = new TaskItemImpl("task4", 500, "task4");

            TaskContainer container = new TaskContainer().AddParallel(task1).AddParallel(task2).AddParallel(task3)
                .AddOrdered(task4);

            DateTime t0 = DateTime.Now;
            Src.ParallelTaskScheduler.Schedule(container);
            DateTime t1 = DateTime.Now;

            // total time (1500)
            Assert.IsTrue((1500 < (t1 - t0).TotalMilliseconds));
            Assert.IsTrue(((t1 - t0).TotalMilliseconds < 2000));

            // task1 time (500)
            Result result1 = KeyValuePool.Get(task1.GetHashCode().ToString());
            Assert.AreEqual("task1", result1.Value);
            Assert.IsTrue(500 < (result1.TimeStamp - t0).TotalMilliseconds);
            Assert.IsTrue((result1.TimeStamp - t0).TotalMilliseconds < 1000);

            // task2 time (500)
            Result result2 = KeyValuePool.Get(task2.GetHashCode().ToString());
            Assert.AreEqual("task2", result2.Value);
            Assert.IsTrue(500 < (result2.TimeStamp - t0).TotalMilliseconds);
            Assert.IsTrue((result2.TimeStamp - t0).TotalMilliseconds < 1000);

            // task3 time (1000)
            Result result3 = KeyValuePool.Get(task3.GetHashCode().ToString());
            Assert.AreEqual("task3", result3.Value);
            Assert.IsTrue(1000 < (result3.TimeStamp - t0).TotalMilliseconds);
            Assert.IsTrue((result3.TimeStamp - t0).TotalMilliseconds < 1500);

            // task4 time (500)
            Result result4 = KeyValuePool.Get(task4.GetHashCode().ToString());
            Assert.AreEqual("task4", result4.Value);
            Assert.IsTrue(500 < (result4.TimeStamp - result3.TimeStamp).TotalMilliseconds);
            Assert.IsTrue((result3.TimeStamp - result3.TimeStamp).TotalMilliseconds < 1000);
        }

        [Test]
        public void SchedulerTest3()
        {
            ITaskItem task1 = new TaskItemImpl("task1", 500, "task1");
            ITaskItem task2 = new TaskItemImpl("task2", 500, "task2");
            ITaskItem task3 = new TaskItemImpl("task3", 500, "task3");
            ITaskItem task4 = new TaskItemImpl("task4", 500, "task4");

            TaskContainer container = new TaskContainer().AddOrdered(task1)
                .AddOrdered(task2).AddParallel(task3)
                .AddOrdered(task4);

            DateTime t0 = DateTime.Now;
            Src.ParallelTaskScheduler.Schedule(container);
            DateTime t1 = DateTime.Now;

            // total time (1500)
            Assert.IsTrue((1500 < (t1 - t0).TotalMilliseconds));
            Assert.IsTrue(((t1 - t0).TotalMilliseconds < 2000));

            // task1 time (500)
            Result result1 = KeyValuePool.Get(task1.GetHashCode().ToString());
            Assert.AreEqual("task1", result1.Value);
            Assert.IsTrue(500 < (result1.TimeStamp - t0).TotalMilliseconds);
            Assert.IsTrue((result1.TimeStamp - t0).TotalMilliseconds < 1000);

            // task2 time (500)
            Result result2 = KeyValuePool.Get(task2.GetHashCode().ToString());
            Assert.AreEqual("task2", result2.Value);
            Assert.IsTrue(500 < (result2.TimeStamp - result1.TimeStamp).TotalMilliseconds);
            Assert.IsTrue((result2.TimeStamp - result1.TimeStamp).TotalMilliseconds < 1000);

            // task3 time (500)
            Result result3 = KeyValuePool.Get(task3.GetHashCode().ToString());
            Assert.AreEqual("task3", result3.Value);
            Assert.IsTrue(500 < (result3.TimeStamp - result1.TimeStamp).TotalMilliseconds);
            Assert.IsTrue((result3.TimeStamp - result1.TimeStamp).TotalMilliseconds < 1000);

            // task4 time (500)
            Result result4 = KeyValuePool.Get(task4.GetHashCode().ToString());
            Assert.AreEqual("task4", result4.Value);
            Assert.IsTrue(500 < (result4.TimeStamp - result3.TimeStamp).TotalMilliseconds);
            Assert.IsTrue((result3.TimeStamp - result3.TimeStamp).TotalMilliseconds < 1000);
        }
    }
}
