using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParallelTaskScheduler.test
{
    public class Result
    {
        public DateTime TimeStamp
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }
    }

    public static class KeyValuePool
    {
        static Dictionary<string, Result> MAP = new Dictionary<string, Result>();

        public static void Set(string key, Result relsut)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            lock (MAP)
            {
                MAP[key] = relsut;
            }
        }

        public static Result Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            lock (MAP)
            {
                if (MAP.ContainsKey(key))
                {
                    return MAP[key];
                }

                return null;
            }
        }

        public static void Clear()
        {
            lock (MAP)
            {
                MAP.Clear();
            }
        }
    }
}
