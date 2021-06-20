using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using UnityEditor.Experimental;

namespace RemoteFileExplorer.Editor.UI
{
    public class ObjectItem : VisualElement
    {
        public Action<ObjectItem> clickItemCallback;
        public Action<ObjectItem> doubleClickItemCallback;
        const string k_UxmlFilesPath = "Packages/com.iwin.remotefileexplorer/Resources/UXML/ObjectItem.uxml";

        public VisualElement objectView { get; }
        public Image objectIcon { get; }
        public Label objectLabel { get; }
        private ObjectData m_Data;
        private int m_NumCharacters;

        private Vector2 m_Size;
        public Vector2 Size
        {
            set
            {
                m_Size = value;
                this.style.width = value.x;
                this.style.height = value.y;
            }
        }

        public ObjectData Data 
        {
            get
            {
                return m_Data;
            }
        }

        public ObjectItem(Vector2 size)
        {
            this.Size = size;
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_UxmlFilesPath);
            tree.CloneTree(this);

            objectView = this.Q<VisualElement>("objectView");
            objectIcon = this.Q<Image>("objectIcon");

            objectLabel = this.Q<Label>("objectLabel");
            this.RegisterCallback<GeometryChangedEvent>(this.OnGeometryChanged);
            this.RegisterCallback<MouseDownEvent>(this.OnMouseDown);
        }

        public void UpdateView(ObjectData data)
        {
            m_Data = data;
            objectLabel.text = GetText(data.path);
            UpdateState(data.state);
        }

        public void UpdateState(ObjectState state)
        {
            m_Data.state = state;
            switch (state)
            {
                case ObjectState.Normal:
                    objectLabel.style.color = Color.white;
                    break;
                case ObjectState.Selected:
                    Color c;
                    ColorUtility.TryParseHtmlString("#5576bd", out c);
                    objectLabel.style.color = c;
                    break;
                default:
                    break;
            }
            objectIcon.image = GetTexture();
        }

        public Texture2D GetTexture()
        {
            string key = null;
            if(m_Data.type == ObjectType.Folder)
            {
                key = "folder";
            }
            else
            {
                key = Path.GetExtension(m_Data.path);
            }
            if(m_Data.state == ObjectState.Selected)
            {
                key += " active";
            }
            return TextureUtility.GetTexture(key);
        }

        public string GetText(string path)
        {
            string name = Path.GetFileName(path);
            if (m_NumCharacters > 2 && m_NumCharacters < name.Length)
            {
                name = name.Substring(0, m_NumCharacters - 2) + "\u2026";
            }
            return name;
        }

        public void OnGeometryChanged(GeometryChangedEvent e)
        {
            if (m_NumCharacters == 0)
            {
                m_NumCharacters = GetNumCharactersThatFitWithinWidth();
            }
            objectLabel.text = GetText(m_Data.path);
        }

        public void OnMouseDown(MouseDownEvent e)
        {
            if (e.button == 0)
            {
                if (clickItemCallback != null)
                {
                    clickItemCallback(this);
                }
                if (e.clickCount == 2)
                {
                    doubleClickItemCallback(this);
                }
            }
        }

        public int GetNumCharactersThatFitWithinWidth()
        {
            var rect = objectLabel.contentRect;
            char testChar = 'a';
            string testStr = "";
            float width = 0;
            while (width < rect.width)
            {
                testStr += testChar;
                width = objectLabel.MeasureTextSize(testStr, rect.width, MeasureMode.AtMost, rect.height, MeasureMode.AtMost).x;
            }
            return testStr.Length;
        }
    }

    public enum ObjectType
    {
        File,
        Folder,
    }

    public enum ObjectState
    {
        Normal,
        Selected,
        Renaming,
        Deleting,
        Downloading,
        Uploading,
    }

    public class ObjectData
    {
        public ObjectType type;
        public string path;
        public ObjectState state;

        public ObjectData() {}

        public ObjectData(ObjectType type, string path, ObjectState state = ObjectState.Normal)
        {
            this.type = type;
            this.path = path;
            this.state = state;
        }
    }
}