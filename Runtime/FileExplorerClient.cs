using UnityEngine;
using System.Collections;
using System.Net;

namespace RemoteFileExplorer
{
    public class FileExplorerClient : MonoBehaviour
    {
        private static FileExplorerClient instance = null;
        public static FileExplorerClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<FileExplorerClient>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject(typeof(FileExplorerClient).Name);
                        instance = go.AddComponent<FileExplorerClient>();
                        DontDestroyOnLoad(instance);
                    }
                }
                return instance;
            }
        }

        private Client m_Client;
        private Robot m_Robot;
        public string host;
        public readonly int port = 8243;
        public bool connectOnStart;
        [Tooltip("Whether to automatically reconnect after disconnection")]
        public bool autoReconnect = false;
        public float reconnectInterval = 1;

        private void Awake()
        {
            m_Client = new Client();
            m_Robot = new Robot(m_Client);
            m_Client.OnReceiveCommand += OnReceiveCommand;
            m_Client.OnConnectStatusChanged += OnConnectStatusChanged;
        }

        private void Start()
        {
            if (connectOnStart)
            {
                StartConnect();
            }
        }

        public void StartConnect()
        {
            Log.Debug(string.Format("Connect on start, host = {0}, port = {1}", host, port));
            m_Client.StartConnect(host, port);
        }

        public void StartConnect(float delay)
        {
            Coroutines.Start(Internal_StartConnect(delay), this);
        }

        private IEnumerator Internal_StartConnect(float delay)
        {
            yield return new YieldWaitForSeconds(delay);
            StartConnect();
        }

        private void Update() {
            if(m_Client != null)
            {
                m_Client.Update();
            }
            Coroutines.Update();
        }

        public void OnReceiveCommand(Command command)
        {
            m_Robot.Execute(command);
        }

        public void OnConnectStatusChanged(ConnectStatus status)
        {
            Log.Debug("Clinet connect status changed : " + status);
            if(status == ConnectStatus.Disconnect)
            {
                if(autoReconnect)
                {
                    StartConnect(reconnectInterval);
                }
            }
        }

        private void OnDestroy()
        {
            if (m_Client != null)
            {
                Coroutines.StopAll(this);
                m_Client.OnReceiveCommand -= OnReceiveCommand;
                m_Client.OnConnectStatusChanged -= OnConnectStatusChanged;
                m_Client.Close();
            }
        }
    }
}