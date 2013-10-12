using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;
using ParallelTaskScheduler.Src;
using Distributor.Service.Src.Util;

namespace ParallelTaskScheduler.Util
{
    static class TransferHelper
    {
        public static TransferTaskItem Box(ITaskItem taskItem)
        {
            Guard.NotNull<ITaskItem>(() => taskItem);

            TransferTaskItem transferItem = new TransferTaskItem()
            {
                ID = taskItem.ID,
                Name = taskItem.Name,
                TypeName = taskItem.GetType().ToString(),
                Params = taskItem.BoxFlyParams(),
            };

            return transferItem;
        }

        public static ITaskItem Unbox(TransferTaskItem transferTaskItem)
        {
            Guard.ArgumentNotNull(transferTaskItem, "transferTaskItem");

            ITaskItem taskItem = System.Reflection.Assembly.GetAssembly(Type.GetType(transferTaskItem.TypeName))
                .CreateInstance(transferTaskItem.TypeName) as ITaskItem;

            Unbox(transferTaskItem, taskItem);

            return taskItem;
        }

        public static void Unbox(TransferTaskItem transferTaskItem, ITaskItem taskItem)
        {
            Guard.ArgumentNotNull(transferTaskItem, "transferTaskItem");
            Guard.ArgumentNotNull(taskItem, "taskItem");

            // check the ID
            if (taskItem.ID != transferTaskItem.ID)
            {
                throw new Exception("task ID mismatch");
            }

            taskItem.UnboxFlyParams(transferTaskItem.Params);
        }
    }
}
