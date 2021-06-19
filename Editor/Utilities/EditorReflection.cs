using UnityEngine;
using System.Reflection;
using UnityEditor;

namespace URFS.Editor
{
    public class EditorReflection
    {
        public static object InvokeStaticMethod<T>(string methodName, params object[] param)
        {
            return InvokeStaticMethod(typeof(T), methodName, param);
        }

        public static object InvokeStaticMethod(System.Type type, string methodName, params object[] param)
        {
            return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Invoke(null, param);
        }
    }
}