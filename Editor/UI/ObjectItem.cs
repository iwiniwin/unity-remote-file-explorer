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

        public Vector2 Size {
            set {
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
        }

        public void UpdateView()
        {
            ObjectIcon.image = GetTexture("aaa.cs");
        }

        public Texture2D GetTexture(string fileName)
        {
            Texture2D icon = null;
            switch (Path.GetExtension(fileName))
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
    }
}