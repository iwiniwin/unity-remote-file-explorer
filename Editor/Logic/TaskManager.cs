using System.Collections;
using System.Collections.Generic;
using System;

namespace RemoteFileExplorer.Editor
{
    public enum TaskType 
    {
        GoTo,
        Download,
        Upload
    }

    public class Task 
    {
        public TaskType Type;
        public Coroutine Coroutine;
        public DateTime StartTime;
    }

    public class TaskManager
    {
        private static List<Task> m_TaskList = new List<Task>();

        public static void StartTask(TaskType type, IEnumerator routine)
        {
            if(type == TaskType.GoTo)  // 同一时刻只能有一个GoTo task
            {
                // StopTasks(type);
                var list = GetTasks(type);
            }
            Task task = new Task()
            {
                Type = type,
                Coroutine = Coroutines.Start(routine),
                StartTime = DateTime.Now,
            };
            m_TaskList.Add(task);
        }

        public static List<Task> GetTasks(TaskType type)
        {
            UpdateTasks();
            List<Task> list = new List<Task>();
            foreach(var task in m_TaskList)
            {
                if(task.Type == type)
                {
                    list.Add(task);
                }
            }
            return list;
        }

        public static List<Task> GetTasks()
        {
            UpdateTasks();
            return m_TaskList;
        }

        public static void StopTasks(TaskType type)
        {
            UpdateTasks();
            for(int i = m_TaskList.Count - 1; i >= 0; i --)
            {
                if(m_TaskList[i].Type == type)
                {
                    UnityEngine.Debug.Log("stop..........");
                    Coroutines.Stop(m_TaskList[i].Coroutine);
                    m_TaskList.RemoveAt(i);
                }
            }
        }

        public static void UpdateTasks()
        {
            for(int i = m_TaskList.Count - 1; i >= 0; i --)
            {
                // if(m_TaskList[i].Coroutine.Finished)
                // {
                //     m_TaskList.RemoveAt(i);
                // }
            }
        }
    }
}