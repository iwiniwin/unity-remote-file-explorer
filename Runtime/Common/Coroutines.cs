/*
 * @Author: iwiniwin
 * @Date: 2021-06-04 19:33:53
 * @Description: 自定义协程
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace RemoteFileExplorer
{
    public sealed class Coroutines
    {
        private static Coroutines instance = null;
        public const int TickToSecond = 10000000;
        private long previousTicks = 0;

        private Coroutines() { }

        private Dictionary<string, List<Coroutine>> m_CoroutineDict = new Dictionary<string, List<Coroutine>>();

        public static Coroutine Start(IEnumerator routine, object target = null)
        {
            return InternalStart(new Coroutine(routine, target));
        }

        public static Coroutine Start(string methodName, object target)
        {
            return Start(methodName, null, target);
        }

        public static Coroutine Start(string methodName, object value, object target)
        {
            MethodInfo methodInfo = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
            {
                Debug.LogError($"Coroutine '{methodName}' couldn't be started!");
                return null;
            }
            object returnValue;
            if (value == null)
            {
                returnValue = methodInfo.Invoke(target, null);
            }
            else
            {
                returnValue = methodInfo.Invoke(target, new object[] { value });
            }
            if (returnValue is IEnumerator)
            {
                return InternalStart(new Coroutine((IEnumerator)returnValue, methodName, target));
            }
            return null;
        }

        private static Coroutine InternalStart(Coroutine coroutine)
        {
            CreateInstanceIfRequired();
            instance.StartCoroutine(coroutine);
            return coroutine;
        }

        private static void CreateInstanceIfRequired()
        {
            if (instance == null)
            {
                instance = new Coroutines();
                instance.previousTicks = DateTime.Now.Ticks;
            }
        }

        public static void Stop(IEnumerator routine, object target)
        {
            Stop(new Coroutine(routine, target));
        }

        public static void Stop(string methodName, object target)
        {
            Stop(new Coroutine(null, methodName, target));
        }

        public static void Stop(Coroutine routine)
        {
            if(instance == null) return;
            instance.StopCoroutine(routine);
        }

        public static void StopAll(object target)
        {
            if(instance == null) return;
            instance.StopAllCoroutines(new Coroutine(null, target).TargetKey);
        }

        public static void StopAll()
        {
            if(instance == null) return;
            instance.StopAllCoroutines();
        }

        private void StopCoroutine(Coroutine routine)
        {
            if(m_CoroutineDict.ContainsKey(routine.Key))
            {
                m_CoroutineDict.Remove(routine.Key);
            }
        }

        private void StopAllCoroutines(string targetKey)
        {
            List<string> keys = new List<string>(m_CoroutineDict.Keys);
            foreach (string key in keys)
            {
                List<Coroutine> coroutines = m_CoroutineDict[key];
                for (int i = coroutines.Count - 1; i >= 0; i--)
                {
                    if(coroutines[i].TargetKey == targetKey)
                    {
                        coroutines.RemoveAt(i);
                    }
                }
                if(m_CoroutineDict[key].Count == 0)
                {
                    m_CoroutineDict.Remove(key);
                }
            }
        }

        private void StopAllCoroutines()
        {
            m_CoroutineDict.Clear();
        }

        public static void Update()
        {
            CreateInstanceIfRequired();
            float deltaTime = (float)(DateTime.Now.Ticks - instance.previousTicks) / TickToSecond;
            if(deltaTime > 0)  // 避免一帧内触发多次
            {
                instance.OnUpdate(deltaTime);
                instance.previousTicks = DateTime.Now.Ticks;
            }
        }

        private void StartCoroutine(Coroutine coroutine)
        {
            MoveNext(coroutine);
            if (!m_CoroutineDict.ContainsKey(coroutine.Key))
            {
                m_CoroutineDict.Add(coroutine.Key, new List<Coroutine>());
            }
            m_CoroutineDict[coroutine.Key].Add(coroutine);
        }

        private void OnUpdate(float dt)
        {
            if (m_CoroutineDict.Count == 0) return;
            List<string> keys = new List<string>(m_CoroutineDict.Keys);
            foreach (string key in keys)
            {
                List<Coroutine> coroutines = m_CoroutineDict[key];
                for (int i = coroutines.Count - 1; i >= 0; i--)
                {
                    Coroutine coroutine = coroutines[i];
                    if(!coroutine.CurrentYield.IsDone())
                    {
                        continue;
                    }
                    if(!MoveNext(coroutine))
                    {
                        coroutines.RemoveAt(i);
                        coroutine.CurrentYield = null;
                        coroutine.Finished = true;
                    }
                }
                if(m_CoroutineDict[key].Count == 0)
                {
                    m_CoroutineDict.Remove(key);
                }
            }
        }

        private bool MoveNext(Coroutine coroutine)
        {
            if (!coroutine.Routine.MoveNext())
            {
                return false;
            }
            object current = coroutine.Routine.Current;
            if (current == null)
            {
                coroutine.CurrentYield = new YieldDefault();
            }
            else if(current is Coroutine)
            {
                coroutine.CurrentYield = new YieldNestedCoroutine(){
                    coroutine = (Coroutine)current
                };
            }
            else if(current is IEnumerator)
            {
                Coroutine nestedCoroutine = Start((IEnumerator)current);
                coroutine.CurrentYield = new YieldNestedCoroutine(){
                    coroutine = nestedCoroutine
                };
            }
            else if(current is ICoroutineYield)
            {
                coroutine.CurrentYield = (ICoroutineYield)current;
            }
            else
            {
                coroutine.CurrentYield = new YieldDefault();
            }
            return true;
        }
    }

    public sealed class Coroutine : YieldInstruction
    {
        private string m_Key = "";
        private string m_TargetKey = "";

        public string Key => m_Key;
        public string TargetKey => m_TargetKey;

        private IEnumerator m_Routine;
        internal IEnumerator Routine => m_Routine;

        internal bool Finished {get; set;}

        internal ICoroutineYield CurrentYield = new YieldDefault();

        internal Coroutine(IEnumerator routine, string methodName, object target)
        {
            this.m_TargetKey = target.GetHashCode().ToString();
            this.m_Key = this.m_TargetKey + methodName;
            this.m_Routine = routine;
        }

        internal Coroutine(IEnumerator routine, object target)
        {
            if (target != null)
            {
                this.m_TargetKey = target.GetHashCode().ToString();
            }
            if(routine != null)
            {
                this.m_Key = this.m_TargetKey + routine.GetHashCode().ToString();
            }
            this.m_Routine = routine;
        }
    }

    public interface ICoroutineYield
    {
        bool IsDone();
    }

    public class YieldDefault : ICoroutineYield
    {
        public bool IsDone()
        {
            return true;
        }
    }

    public class YieldNestedCoroutine : ICoroutineYield
    {
        public Coroutine coroutine;
        public bool IsDone()
        {
            return coroutine.Finished;
        }
    }
}