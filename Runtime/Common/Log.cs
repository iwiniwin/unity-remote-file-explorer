using UnityEngine;

namespace RemoteFileExplorer
{
    public class Log 
    {
        public static void Debug(object message)
        {
            LogToConsole(CreateLogData(LogType.Log, message));
        }

        public static void Error(object message)
        {
            LogToConsole(CreateLogData(LogType.Error, message));
        }

        public static void Warning(object message)
        {
            LogToConsole(CreateLogData(LogType.Warning, message));
        }

        public static LogData CreateLogData(LogType level, object message)
        {
            LogData data = new LogData();
            data.level = level;
            data.message = message;
            return data;
        }

        public static void LogToConsole(LogData data)
        {
            UnityEngine.Debug.unityLogger.Log(data.level, data.message);
        }

        public class LogData 
        {
            public LogType level;
            public object message;
        }
    } 
}