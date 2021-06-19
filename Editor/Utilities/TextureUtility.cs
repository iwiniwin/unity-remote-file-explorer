using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental;

namespace RemoteFileExplorer.Editor
{
    public class TextureUtility
    {
        private static Dictionary<string, Texture2D> m_TextureCache = new Dictionary<string, Texture2D>();

        private static Texture2D m_DefaultTexture;

        public static Texture2D GetTexture(string key)
        {
            if(m_TextureCache.ContainsKey(key))
            {
                return m_TextureCache[key];
            }
            Texture2D icon = null;
            switch (key)
            {
                case "folder":
                    icon = EditorGUIUtility.IconContent(EditorResources.folderIconName).image as Texture2D;
                    break;
                case ".cs":
                    icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
                    break;
                case ".cs active":
                    Texture2D texture = GetTexture(".cs");
                    texture = TextureUtility.TextureToTexture2D(texture);

                    Color c;
                    ColorUtility.TryParseHtmlString("#91c9f7", out c);
                    c.a = 0.2f;
                    texture = TextureUtility.CloneTexture2D_2(texture, c);
                    icon = texture;
                    break;
                case ".shader":
                    icon = EditorGUIUtility.IconContent("shader Icon").image as Texture2D;
                    break;
            }
            if(icon == null)
            {
                if(m_DefaultTexture == null)
                    m_DefaultTexture = EditorReflection.InvokeStaticMethod<EditorGUIUtility>("FindTextureByType", typeof(TextAsset)) as Texture2D;
                icon = m_DefaultTexture;
            }
            else
            {
                m_TextureCache.Add(key, icon);
            }
            return icon;
        }

        public static Texture2D TextureToTexture2D(Texture texture)
        {
            Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
            Graphics.Blit(texture, renderTexture);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(renderTexture);

            return texture2D;
        }

        public static Texture2D CloneTexture2D(Texture2D target, Color color)
        {
            Texture2D texture2D = new Texture2D(target.width, target.height);
            for(int y = 0; y < texture2D.height; y ++)
            {
                for(int x = 0; x < texture2D.width; x ++)
                {
                    color.a = target.GetPixel(x, y).a;
                    texture2D.SetPixel(x, y, color);
                }
            }
            texture2D.Apply();
            return texture2D;
        }


        public static Texture2D CloneTexture2D_2(Texture2D target, Color color)
        {
            Texture2D texture2D = new Texture2D(target.width, target.height);
            for(int y = 0; y < texture2D.height; y ++)
            {
                for(int x = 0; x < texture2D.width; x ++)
                {
                    Color c = target.GetPixel(x, y);
                    if(c.a != 0)
                    {
                        texture2D.SetPixel(x, y, (c * (1 - color.a)) + (color * color.a));
                    }else
                    {
                        texture2D.SetPixel(x, y, c);
                    }
                }
            }
            texture2D.Apply();
            return texture2D;
        }

    }
}