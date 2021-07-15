using System;

namespace RemoteFileExplorer.Editor
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BeforeUploadAttribute : System.Attribute
    {
        public string description;
        public int priority;
        public string IncludeName = null;

        public BeforeUploadAttribute(string description) : this(description, 0) { }

        public BeforeUploadAttribute(string description, int priority)
        {
            this.description = description;
            this.priority = priority;
        }
    }
}