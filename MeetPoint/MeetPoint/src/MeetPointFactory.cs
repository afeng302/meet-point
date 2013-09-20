using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;
using System.Reflection;

namespace MeetPoint.src
{
    public static class MeetPointFactory
    {
        static Dictionary<string, IMeetPoint> MEET_POINT_MAP = new Dictionary<string, IMeetPoint>();

        public static IMeetPoint Create(string pointID, out bool createdNew)
        {
            return Create(pointID, 2, 0, Timeout.Infinite, out createdNew);
        }

        public static IMeetPoint Create(string pointID, int preCondNumber, int postCondNumber,
            out bool createdNew)
        {
            return Create(pointID, preCondNumber, postCondNumber, Timeout.Infinite, out createdNew);
        }

        public static IMeetPoint Create(string pointID, int preCondNumber, int postCondNumber,
            int millisecondsTimeout, out bool createdNew)
        {
            createdNew = true;

            if (string.IsNullOrEmpty(pointID))
            {
                Log.ErrorFormat("Empty pointID [{0}] in create().", pointID);
                return null;
            }

            if (preCondNumber == 0)
            {
                Log.ErrorFormat("Not support zero preCondNumber meetpoint. pointID[{0}]", pointID);
                return null;
            }

            lock (MEET_POINT_MAP)
            {
                if (MEET_POINT_MAP.ContainsKey(pointID))
                {
                    Log.InfoFormat("meetpoint[{0}] was reused.", pointID);

                    createdNew = false;
                    return MEET_POINT_MAP[pointID];
                }

                IMeetPoint meetPoint = new MeetPointImpl(pointID, preCondNumber, postCondNumber,
                    millisecondsTimeout);
                MEET_POINT_MAP[pointID] = meetPoint;

                Log.InfoFormat("meetpoint[{0}] was created.", pointID);

                return meetPoint;
            }
        }

        public static bool HasMeetPoint(string pointID)
        {
            if (string.IsNullOrEmpty(pointID))
            {
                Log.ErrorFormat("Empty pointID [{0}] in HasMeetPoint().", pointID);
                return false;
            }

            lock (MEET_POINT_MAP)
            {
                return MEET_POINT_MAP.ContainsKey(pointID);
            }
        }

        public static bool Remove(string pointID)
        {
            if (string.IsNullOrEmpty(pointID))
            {
                Log.ErrorFormat("Empty pointID [{0}] in Remove().", pointID);
                return false;
            }

            lock (MEET_POINT_MAP)
            {
                return MEET_POINT_MAP.Remove(pointID);
            }
        }

        public static int MeetPointCount
        {
            get
            {
                lock (MEET_POINT_MAP)
                {
                    return MEET_POINT_MAP.Count;
                }
            }
        }

        public static string[] MeetPointIDs
        {
            get
            {
                lock (MEET_POINT_MAP)
                {
                    return MEET_POINT_MAP.Keys.ToArray<string>();
                }
            }
        }

        public static IMeetPoint GetMeetPoint(string pointID)
        {
            if (string.IsNullOrEmpty(pointID))
            {
                Log.ErrorFormat("Empty pointID [{0}] in GetMeetPoint().", pointID);
                return null;
            }

            lock (MEET_POINT_MAP)
            {
                if (MEET_POINT_MAP.ContainsKey(pointID))
                {
                    return MEET_POINT_MAP[pointID];
                }

                return null;
            }
        }

        public static void Cleanup()
        {
            List<string> IDList = new List<string>();

            lock (MEET_POINT_MAP)
            {
                Log.DebugFormat("Cleanup() invoked. [{0}] meetpoint in map.", MEET_POINT_MAP.Count);

                foreach (IMeetPoint nextPoint in MEET_POINT_MAP.Values)
                {
                    if ((nextPoint.PreCondArrivedCount == nextPoint.PreCondNumber)
                        && (nextPoint.PostCondArrivedCount == nextPoint.PostCondNumber))
                    {
                        IDList.Add(nextPoint.ID);
                    }
                }

                foreach (string nextID in IDList)
                {
                    Log.DebugFormat("Cleanup() invoked. meetpoint [{0}] was cleaned.", nextID);
                    MEET_POINT_MAP.Remove(nextID);
                }
            } // lock (meetPointMap)
        }

        public static void Clear()
        {
            lock (MEET_POINT_MAP)
            {
                Log.DebugFormat("Clear() invoked. [{0}] meetpoint in map.", MEET_POINT_MAP.Count);
                MEET_POINT_MAP.Clear();
            }
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
