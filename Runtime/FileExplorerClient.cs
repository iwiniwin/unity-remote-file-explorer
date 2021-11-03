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

        private void Start()
        {
            m_Client = new Client();
            m_Robot = new Robot(m_Client);
            m_Client.OnReceiveCommand += OnReceiveCommand;
            m_Client.OnConnectStatusChanged += OnConnectStatusChanged;
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

        private bool showWindow = false;
        public void OpenConnectWindow()
        {
            showWindow = true;
        }
        
        private static int windowWidth = Screen.width / 3;
        private static int windowHeight = windowWidth / 2;
        Rect windowRect = new Rect (Screen.width/2 - windowWidth / 2, Screen.height/2 - windowHeight / 2, windowWidth, windowHeight);
        void OnGUI()
        {
            if(showWindow)
            {
                windowRect = GUILayout.Window(0, windowRect, DoMyWindow, "");
            }
        }

        private string hostText;
        private IPAddress address;
        void DoMyWindow(int windowID)
        {
            int fontSize = 20;
            var labelStyle = new GUIStyle(GUI.skin.label){
                fontSize = fontSize,
            };
            labelStyle.normal.textColor = Color.white;
            var textFieldStyle = new GUIStyle(GUI.skin.textField){
                fontSize = fontSize,
            };
            var buttonStyle = new GUIStyle(GUI.skin.button){
                fontSize = fontSize,
            };

            int groupHeight = windowHeight / 5;
            GUILayout.Label("Enter Host to Connect: ", labelStyle);
            hostText = GUILayout.TextField(hostText ?? host, textFieldStyle);

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Cancel", buttonStyle)){
                showWindow = false;
            }
            if(GUILayout.Button("Connect", buttonStyle)){
                if(IPAddress.TryParse(hostText, out address))
                {
                    host = hostText;
                    showWindow = false;
                    this.StartConnect();
                }
                else
                {
                    hostText = "";
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}