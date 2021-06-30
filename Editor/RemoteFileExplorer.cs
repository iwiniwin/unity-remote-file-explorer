using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using RemoteFileExplorer.Editor.UI;

namespace RemoteFileExplorer.Editor
{
    public class RemoteFileExplorerWindow : EditorWindow
    {
        bool m_WindowInitialized = false;
        private static string m_Host = NetworkUtility.GetLocalHost();
        private static int m_Port = 8243;

        const string k_PackageResourcesPath = "Packages/com.iwin.remotefileexplorer/Resources/";
        const string k_UxmlFilesPath = k_PackageResourcesPath + "UXML/";
        const string k_WindowUxmlPath = k_UxmlFilesPath + "RemoteFileExplorer.uxml";
        const string k_StyleSheetsPath = k_PackageResourcesPath + "StyleSheets/";
        const string k_WindowCommonStyleSheetPath = k_StyleSheetsPath + "RemoteFileExplorer_style.uss";
        const string k_WindowLightStyleSheetPath = k_StyleSheetsPath + "RemoteFileExplorer_style_light.uss";
        const string k_WindowDarkStyleSheetPath = k_StyleSheetsPath + "RemoteFileExplorer_style_dark.uss";

        private static Texture2D m_EstablishedTexture;

        private Manipulator m_Manipulator;
        public Label m_ConnectStateLabel;
        public Label m_ConnectHostLabel;
        public Label m_ConnectPortLabel;
        public Label m_DeviceNameLabel;
        public Label m_DeviceModelLabel;
        public Label m_DeviceSystemLabel;
        public ToolbarButton m_PrevButton;
        public ToolbarButton m_NextButton;
        public Image m_PrevImage;
        public Image m_NextImage;

        [MenuItem("Window/Remote File Explorer")]
        public static void ShowWindow()
        {
            GetWindow<RemoteFileExplorerWindow>();
        }

        int tag = 0;
        void OnGUI()
        {
            if (tag == 4)
            {
                // System.Collections.Generic.List<ObjectData> data = new System.Collections.Generic.List<ObjectData>();
                // for(int i = 0; i < 10; i ++)
                // {
                //     data.Add(new ObjectData(ObjectType.File, "vv/aa.cs"));
                // }
                // m_ObjectListArea.UpdateView(data);
            }
            tag++;
            if (m_WindowInitialized)
            {
                return;
            }
            Init();
        }

        public ObjectListArea m_ObjectListArea;

        void Init()
        {
            m_WindowInitialized = true;
            titleContent = EditorGUIUtility.TrTextContentWithIcon("Remote File Explorer", "Project");
            titleContent.image = TextureUtility.GetTexture("project");
            minSize = new Vector2(500, 300);
            InitContent();
        }

        public Server m_Server;

        IMGUIContainer m_BreadCrumbsContainer;
        ToolbarToggle m_StatsToggle;

        private void InitContent()
        {
            m_Manipulator = new Manipulator(this);
            var root = this.rootVisualElement;
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_WindowCommonStyleSheetPath));
            if (EditorGUIUtility.isProSkin)
            {
                root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_WindowDarkStyleSheetPath));
            }
            else
            {
                root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_WindowLightStyleSheetPath));
            }
            var windowTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_WindowUxmlPath);
            windowTree.CloneTree(root);

            var objectListPlaceHolder = root.Q<VisualElement>("objectListPlaceHolder");
            m_ObjectListArea = new ObjectListArea();
            m_ObjectListArea.doubleClickItemCallback += m_Manipulator.GoTo;
            m_ObjectListArea.clickItemCallback += m_Manipulator.Select;
            m_ObjectListArea.clickEmptyAreaCallback += m_Manipulator.Select;
            m_ObjectListArea.rightClickItemCallback += OpenRightClickMenu;
            m_ObjectListArea.rightClickEmptyAreaCallback += OpenRightClickEmptyAreaMenu;
            m_ObjectListArea.receiveDragPerformCallback += m_Manipulator.Upload;
            objectListPlaceHolder.Add(m_ObjectListArea);

            m_StatsToggle = root.Q<ToolbarToggle>("statsToggle");
            var statsPanel = root.Q<Box>("statsPanel");
            m_StatsToggle.RegisterValueChangedCallback(e =>
            {
                if (e.newValue)
                {
                    statsPanel.style.display = DisplayStyle.Flex;
                }
                else
                {
                    statsPanel.style.display = DisplayStyle.None;
                }
            });

            var goToMenu = root.Q<ToolbarMenu>("goToMenu");
            goToMenu.RegisterCallback<MouseUpEvent>((MouseUpEvent e) =>
            {
                var menu = new GenericMenu();
                foreach (var key in Robot.PathKeyMap.Keys)
                {
                    menu.AddItem(new GUIContent(key), false, () =>
                    {
                        m_Manipulator.GoToByKey(key);
                    });
                }
                menu.DropDown(GetRect(goToMenu));
            });

            m_ConnectStateLabel = root.Q<Label>("connectState");
            m_ConnectHostLabel = root.Q<Label>("connectHost");
            m_ConnectHostLabel.text = m_Host;
            m_ConnectPortLabel = root.Q<Label>("connectPort");
            m_ConnectPortLabel.text = m_Port.ToString();

            m_DeviceNameLabel = root.Q<Label>("deviceName");
            m_DeviceModelLabel = root.Q<Label>("deviceModel");
            m_DeviceSystemLabel = root.Q<Label>("deviceSystem");

            m_PrevButton = root.Q<ToolbarButton>("backwardsInHistoryButton");
            m_PrevButton.SetEnabled(false);
            m_NextButton = root.Q<ToolbarButton>("forwardsInHistoryButton");
            m_NextButton.SetEnabled(false);

            m_PrevButton.RegisterCallback<MouseUpEvent>(e => {
                m_Manipulator.BackwardGoTo();
            });
            m_NextButton.RegisterCallback<MouseUpEvent>(e => {
                m_Manipulator.ForwardGoTo();
            });

            var captureButton = root.Q<Button>("snapshot-control-area__capture-button");

            var backwardsBtn = root.Q<ToolbarButton>("backwardsInHistoryButton");
            var forwardsBtn = root.Q<ToolbarButton>("forwardsInHistoryButton");

            var breadCrumbEdit = root.Q<TextField>("breadCrumbEdit");
            var breadCrumbRoot = root.Q<VisualElement>("breadCrumbRoot");
            var breadCrumbView = root.Q<VisualElement>("breadCrumbView");
            m_BreadCrumbsContainer = root.Q<IMGUIContainer>("breadCrumbContainer");
            breadCrumbRoot.RegisterCallback<MouseUpEvent>((MouseUpEvent e) =>
            {
                if(e.button != 0) return;
                breadCrumbEdit.style.display = DisplayStyle.Flex;
                breadCrumbEdit.value = m_Manipulator.curPath;
                var l = breadCrumbEdit.Q<VisualElement>("unity-text-input");
                l.Focus();
                breadCrumbEdit.SelectAll();

                breadCrumbView.style.display = DisplayStyle.None;
            });
            breadCrumbEdit.RegisterCallback<FocusOutEvent>((FocusOutEvent e) =>
            {
                breadCrumbEdit.style.display = DisplayStyle.None;
                breadCrumbView.style.display = DisplayStyle.Flex;
            });
            breadCrumbEdit.RegisterCallback<KeyDownEvent>(e =>
            {
                if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter)
                {
                    if(breadCrumbEdit.value != m_Manipulator.curPath)
                    {
                        m_Manipulator.GoTo(breadCrumbEdit.value);
                    }
                }
            });

            m_BreadCrumbsContainer.onGUIHandler = BreadCrumbBar;
        }

        void OpenRightClickMenu(ObjectItem item)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Download"), false, () =>
            {
                m_Manipulator.Download(item);
            });
            menu.AddSeparator("");
            // menu.AddItem(new GUIContent("Rename"), false, () =>
            // {
            //     m_Manipulator.Rename(item);
            // });
            menu.AddItem(new GUIContent("Delete"), false, () =>
            {
                m_Manipulator.Delete(item);
            });
            menu.ShowAsContext();
        }

        void OpenRightClickEmptyAreaMenu()
        {
            if(string.IsNullOrEmpty(m_Manipulator.curPath)) return;
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Refresh"), false, () =>
            {
                m_Manipulator.Refresh();
            });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Upload File"), false, () =>
            {
                m_Manipulator.UploadFile();
            });
            menu.AddItem(new GUIContent("Upload Folder"), false, () =>
            {
                m_Manipulator.UploadFolder();
            });
            menu.ShowAsContext();
        }

        void BreadCrumbBar()
        {
            if (m_Manipulator.curPath == null || m_Manipulator.curPath.Length == 0) return;
            char separator = '/';
            string[] names = m_Manipulator.curPath.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            Rect rect = new Rect(m_BreadCrumbsContainer.contentRect);
            bool startWithSeparator = m_Manipulator.curPath[0] == separator;
            string targetPath = startWithSeparator ? separator.ToString() : "";
            for (int i = 0; i < names.Length; i++)
            {
                GUIContent content = new GUIContent(names[i]);
                bool last = i == names.Length - 1;
                targetPath += names[i] + separator;
                GUIStyle style = last ? EditorStyles.boldLabel : EditorStyles.label;
                Vector2 size = style.CalcSize(content);
                rect.y -= 1;
                rect.width = size.x;
                if (GUI.Button(rect, content, style))
                {
                    if (!last)
                    {
                        m_Manipulator.GoTo(targetPath);
                    }
                }
                rect.y += 1;
                rect.x += size.x;
                if (!last)
                {
                    GUIStyle separatorStyle = "ArrowNavigationRight";
                    Rect buttonRect = new Rect(rect.x, rect.y + (rect.height - separatorStyle.fixedHeight) / 2, separatorStyle.fixedWidth, separatorStyle.fixedHeight);
                    if (GUI.Button(buttonRect, GUIContent.none, separatorStyle))
                    {

                    }
                    rect.x += separatorStyle.fixedWidth;
                }
            }
        }

        public Rect GetRect(VisualElement element)
        {
            return new Rect(element.LocalToWorld(element.contentRect.position), element.contentRect.size);
        }

        public void OnConnectStatusChanged(ConnectStatus status)
        {
            if(m_Manipulator != null)
            {
                m_Manipulator.UpdateStatusInfo();
            }
        }

        private void Update()
        {
            Coroutines.Update();
            if (m_Server == null)
            {
                m_Server = new Server();
                if (m_Server.Status == ConnectStatus.Disconnect)
                {
                    m_Server.OnConnectStatusChanged += OnConnectStatusChanged;
                    m_Server.Start(m_Host, m_Port);
                }
            }
            m_Server.Update();
        }

        private void OnDisable()
        {
            m_WindowInitialized = false;
            tag = 0;
            m_Server.Stop();
        }
    }
}