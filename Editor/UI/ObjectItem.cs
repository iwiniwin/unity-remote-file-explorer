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
        public Action<ObjectItem> rightClickItemCallback;
        const string k_UxmlFilesPath = "Packages/com.iwin.remotefileexplorer/Resources/UXML/ObjectItem.uxml";

        public VisualElement objectView { get; }
        public Image objectIcon { get; }
        public Label objectLabel { get; }
        private ObjectData m_Data;
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
            this.RegisterCallback<MouseUpEvent>(this.OnMouseUp);
        }

        public void UpdateView(ObjectData data)
        {
            m_Data = data;
            UpdateText(data.path);
            UpdateState(data.state);
        }

        public void UpdateState(ObjectState state)
        {
            m_Data.state = state;
            switch (state)
            {
                case ObjectState.Normal:
                    if(EditorGUIUtility.isProSkin)
                    {
                        objectLabel.style.color = Color.white;
                    }
                    else
                    {
                        objectLabel.style.color = Color.black;
                    }
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
                if(key.StartsWith("."))
                {
                    key = key.Substring(1, key.Length - 1);
                }
            }
            if(m_Data.state == ObjectState.Selected)
            {
                key += " active";
            }
            return TextureUtility.GetTexture(key);
        }

        public void UpdateText(string path)
        {
            string name = Path.GetFileName(path);
            string text = name;
            if(!FitWithinWidth(text))
            {
                string ellipsis = "\u2026";
                text = "";
                for(int i = 0; i < name.Length; i ++)
                {
                    if(!FitWithinWidth(text + ellipsis))
                    {
                        break;
                    }
                    text += name[i];
                }
                text += ellipsis;
            }
            objectLabel.text = text;
        }

        public bool FitWithinWidth(string text)
        {
            var rect = objectLabel.contentRect;
            if(float.NaN.Equals(rect.width))
            {
                return true;  // Label仍未初始化完成，将在OnGeometryChanged时重新计算
            }
            float width = objectLabel.MeasureTextSize(text, rect.width, MeasureMode.AtMost, rect.height, MeasureMode.AtMost).x;
            Debug.Log(width + "       saa " + rect.width);
            return width < rect.width;
        }

        public void OnGeometryChanged(GeometryChangedEvent e)
        {
            UpdateText(m_Data.path);
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
            else if(e.button == 1)
            {
                if(clickItemCallback != null)
                {
                    clickItemCallback(this);
                }
            }
            e.StopImmediatePropagation();
        }

        public void OnMouseUp(MouseUpEvent e)
        {
            if(e.button == 1)
            {
                if(rightClickItemCallback != null)
                {
                    rightClickItemCallback(this);
                }
            }
            e.StopImmediatePropagation();
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