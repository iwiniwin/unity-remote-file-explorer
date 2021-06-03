using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace URFS
{
    public sealed class Coroutines
    {
        private static Coroutines instance = null;
        private static int lastFrameCount = 0;

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
            if (instance == null)
            {
                instance = new Coroutines();
            }
            instance.Start(coroutine);
            return coroutine;
        }

        public static void Stop(IEnumerator routine, object target)
        {

        }

        public static void Stop(Coroutine routine)
        {

        }

        public static void Stop(string methodName, object target)
        {

        }

        public static void StopAll(object target)
        {

        }

        public static void StopAll()
        {

        }

        public static void Update()
        {
            if (instance == null) return;
            if(lastFrameCount == Time.frameCount)
            {
                Debug.LogError("Coroutines.Update is called multiple times in one frame");
            }
            lastFrameCount = Time.frameCount;
            instance.OnUpdate(Time.deltaTime);
        }

        private void Start(Coroutine coroutine)
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

        internal bool Finished = false;

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
            this.m_Key = this.m_TargetKey + routine.GetHashCode().ToString();
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