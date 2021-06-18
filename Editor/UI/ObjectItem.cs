using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace URFS.Editor.UI 
{
    public class ObjectItem : VisualElement 
    {
        const string k_UxmlFilesPath = "Packages/com.iwin.remotefileexplorer/Resources/UXML/ObjectItem.uxml";

        public VisualElement ObjectView {get;}
        public Image ObjectIcon {get;}
        public Label ObjectLabel {get;}
        private ObjectData m_Data;

        private Vector2 m_Size;
        public Vector2 Size {
            set {
                m_Size = value;
                this.style.width = value.x;
                this.style.height = value.y;
            }
        }
        
        public ObjectItem(Vector2 size)
        {
            this.Size = size;
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_UxmlFilesPath);
            tree.CloneTree(this);

            ObjectView = this.Q<VisualElement>("objectView");
            ObjectIcon = this.Q<Image>("objectIcon");
            
            ObjectLabel = this.Q<Label>("objectLabel");
            ObjectLabel.style.backgroundColor = Color.red;
        }

        public void UpdateView(ObjectData data)
        {
            m_Data = data; 
            ObjectIcon.image = GetTexture();
            ObjectLabel.text = GetText();
        }

        public Texture2D GetTexture()
        {
            Texture2D icon = null;
            switch (Path.GetExtension(m_Data.path))
            {
                case ".cs":
                    icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
                    break;
                case ".shader":
                    icon = EditorGUIUtility.IconContent("shader Script Icon").image as Texture2D;
                    break;
                default:
                    icon = EditorGUIUtility.IconContent("txt Icon").image as Texture2D;
                    break;
            }
            return icon;
        }

        public string GetText()
        {
            // Font s = Font.CreateDynamicFontFromOSFont("Fonts/Inter/Inter-Regular  ddd.ttf", 0);
            // Debug.Log(s + "       dddddd");
            // ObjectLabel.
            // Debug.Log(ObjectLabel.resolvedStyle.unityFont + "          VVVVVVVVVVVVV " + ObjectLabel.style.unityFont);
           
            // ObjectLabel.font
            string name = Path.GetFileName(m_Data.path);
            
            Debug.Log(CalcTextWidth(name) + "        " + CalcTextWidth("vv"));
            name += " \u2026";
            return name;
        }

        public float CalcTextWidth(string text)
        {
            Font font = ObjectLabel.resolvedStyle.unityFont;
            float fontSize = ObjectLabel.resolvedStyle.fontSize;
            float width = 0;
            CharacterInfo characterInfo;
            for(int i = 0; i < text.Length; i ++)
            {
                Debug.Log(font + "       SS     " + fontSize);
                font.GetCharacterInfo(text[i], out characterInfo, (int)fontSize);
                width += characterInfo.advance;
            }
            return width;
        }
    }

    public enum ObjectType
    {
        File,
        Folder,
    }

    public class ObjectData
    {
        public ObjectType type;
        public string path;
        public ObjectData(ObjectType type, string path)
        {
            this.type = type;
            this.path = path;
        }
    }
}