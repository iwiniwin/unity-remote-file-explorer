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
        public ConnectWindowConfig connectWindow;
        private string cacheKey = "FileExplorerClient_Host";

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
            if(connectWindow.showOnStart)
            {
                OpenConnectWindow();
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
        
        private Vector2 designResolution = new Vector2(1334, 750);
        private Vector2 designLandscapeSize = new Vector2(480, 240);
        private Vector2 designPortraitSize = new Vector2(540, 300);
        private int fontSize = 24;
        void OnGUI()
        {
            if(showWindow)
            {
                bool protrait = connectWindow.orientation == ConnectWindowOrientation.Portrait;
                if(connectWindow.orientation == ConnectWindowOrientation.UseProjectSettings)
                {
                    protrait = Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.Portrait;
                }
                fontSize = protrait ? 30 : 24;
                var size = protrait ? designPortraitSize : designLandscapeSize;
                var scale = Mathf.Min(Screen.width / (protrait ? designResolution.y : designResolution.x), Screen.height / (protrait ? designResolution.x : designResolution.y));
                size *= scale;
                fontSize = (int)(fontSize * scale);
                Rect windowRect = new Rect (Screen.width/2 - size.x / 2, Screen.height/2 - size.y / 2, size.x, size.y);
                GUILayout.Window(0, windowRect, DoMyWindow, "");
            }
        }

        private string hostText;
        private IPAddress address;
        void DoMyWindow(int windowID)
        {
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

            GUILayout.Label("Enter Host to Connect: ", labelStyle);
            
            if(hostText == null && PlayerPrefs.HasKey(cacheKey))
            {
                hostText = PlayerPrefs.GetString(cacheKey);
            }

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
                    PlayerPrefs.SetString(cacheKey, host);
                    PlayerPrefs.Save();  // 缓存上次输入
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

    [System.Serializable]
    public class ConnectWindowConfig
    {
        public bool showOnStart;
        public ConnectWindowOrientation orientation = ConnectWindowOrientation.UseProjectSettings;
    }

    public enum ConnectWindowOrientation
    {
        UseProjectSettings,
        Landscape,
        Portrait
    }
}