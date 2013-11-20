using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;
using ParallelTaskScheduler.Src;
using Distributor.Service.Src.Util;
using System.Reflection;
using System.IO;
using log4net;

namespace ParallelTaskScheduler.Util
{
    static class TransferHelper
    {
        public static string MasterNodeName { get; set; }

        public static TransferTaskItem BoxTask(ITaskItem taskItem)
        {
            Guard.NotNull<ITaskItem>(() => taskItem);

            TransferTaskItem transferItem = new TransferTaskItem()
            {
                ID = taskItem.ID,
                Name = taskItem.Name,
                TypeName = taskItem.GetType().ToString(),
                Params = taskItem.BoxFlyParams(),
                Files = taskItem.BoxFlyFiles(),
            };

            return transferItem;
        }

        public static ITaskItem UnboxTask(TransferTaskItem transferTaskItem)
        {
            Guard.ArgumentNotNull(transferTaskItem, "transferTaskItem");

            ITaskItem taskItem = System.Reflection.Assembly.GetAssembly(Type.GetType(transferTaskItem.TypeName))
                .CreateInstance(transferTaskItem.TypeName) as ITaskItem;

            taskItem.ID = transferTaskItem.ID;
            UnboxTask(transferTaskItem, taskItem);

            return taskItem;
        }

        public static void UnboxTask(TransferTaskItem transferTaskItem, ITaskItem taskItem)
        {
            Guard.ArgumentNotNull(transferTaskItem, "transferTaskItem");
            Guard.ArgumentNotNull(taskItem, "taskItem");

            // check the ID
            if (taskItem.ID != transferTaskItem.ID)
            {
                throw new Exception("task ID mismatch");
            }

            taskItem.UnboxFlyParams(transferTaskItem.Params);

            taskItem.UnboxFlyFiles(transferTaskItem.Files);
        }

        public static string GetMostPowerfulNode()
        {
            return null;
        }

        public static string LocalRootPath
        {
            get
            {
                Assembly ass = Assembly.GetEntryAssembly();
                if (ass == null)
                {
                    ass = Assembly.GetCallingAssembly();
                }
                if (ass == null)
                {
                    ass = Assembly.GetExecutingAssembly();
                }
                if (ass == null)
                {
                    Log.ErrorFormat("Cannot find assembly to get root path.");
                }

                return Path.GetDirectoryName(ass.Location);
            }
        }

        public static void Send2Master(TransferTaskItem transferTaskItem)
        {
        }

        public static TransferTaskItem GetFromMaster(string taskID)
        {

            return null;
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
