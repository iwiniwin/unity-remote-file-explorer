using UnityEngine;
using System.Collections;
using System.Net;

namespace RemoteFileExplorer
{
    public class FileExplorerWindow : MonoBehaviour
    {
        public ConnectWindowOrientation orientation = ConnectWindowOrientation.UseProjectSettings;

        private void Start()
        {
            OpenConnectWindow();
        }

        private bool showWindow = false;
        private string cacheKey = "FileExplorerClient_Host";

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
                bool protrait = orientation == ConnectWindowOrientation.Portrait;
                if(orientation == ConnectWindowOrientation.UseProjectSettings)
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

            hostText = GUILayout.TextField(hostText, textFieldStyle);

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Cancel", buttonStyle)){
                showWindow = false;
            }
            if(GUILayout.Button("Connect", buttonStyle)){
                if(IPAddress.TryParse(hostText, out address))
                {
                    showWindow = false;
                    PlayerPrefs.SetString(cacheKey, hostText);
                    PlayerPrefs.Save();  // 缓存上次输入
                    FileExplorerClient.Instance.host = hostText;
                    FileExplorerClient.Instance.StartConnect();
                }
                else
                {
                    hostText = "";
                }
            }
            GUILayout.EndHorizontal();
        }
    }

    public enum ConnectWindowOrientation
    {
        UseProjectSettings,
        Landscape,
        Portrait
    }
}