using System;

namespace RemoteFileExplorer.Editor
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BeforeUploadAttribute : System.Attribute
    {
        public string description;
        public int priority;
        public string IncludeSrc = null;
        public string IncludeDest = null;

        public BeforeUploadAttribute(string description) : this(description, 0) { }

        public BeforeUploadAttribute(string description, int priority)
        {
            this.description = description;
            this.priority = priority;
        }

        public bool Validate(string src, string dest)
        {
            if (!string.IsNullOrEmpty(this.IncludeSrc))
            {
                src = FileUtil.FixedPath(src);
                if (!src.Contains(FileUtil.FixedPath(this.IncludeSrc)))
                {
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(this.IncludeDest))
            {
                dest = FileUtil.FixedPath(dest);
                if (!dest.Contains(FileUtil.FixedPath(this.IncludeDest)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}