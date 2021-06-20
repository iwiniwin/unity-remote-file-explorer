using System.IO;

namespace RemoteFileExplorer
{
    public class Robot
    {
        private static string[] emptyStringArray = new string[]{};

        private BaseSocket m_Socket;

        public Robot(BaseSocket socket)
        {
            m_Socket = socket;
        }

        public void Execute(Command command)
        {
            if(command is QueryPathInfo.Req)
            {
                var req = command as QueryPathInfo.Req;
                bool exists = Directory.Exists(req.Path);
                string path = req.Path;
                if(exists)
                {
                    path += "/";
                }
                QueryPathInfo.Rsp rsp = new QueryPathInfo.Rsp{
                    Exists = exists,
                    SubDirectories = exists ? Directory.GetDirectories(path) : emptyStringArray,
                    SubFiles = exists ? Directory.GetFiles(path) : emptyStringArray,
                };
                rsp.Ack = req.Seq;
                m_Socket.Send(rsp);
            }
        }
    }
}