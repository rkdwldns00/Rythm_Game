using UnityEngine;
using System.IO;
using System;

//�� �ڵ�� Chat gpt�� �ڵ带 ������ �����Դϴ�!
public static class SpriteExporter
{
    /// <summary>
    /// ���ϴ� ��������Ʈ�� ������ ��ο� png���Ϸ� �������ϴ�.
    /// </summary>
    /// <param name="sprite">������ ��������Ʈ</param>
    /// <param name="filePath">������ ������ ���</param>
    /// <returns>�������� ��������</returns>
    public static bool ExportSpriteToPNG(Sprite sprite, string filePath)
    {
        if (sprite == null)
        {
            return false;
        }

        return ExportTextureToPNG(sprite.texture, filePath);
    }

    /// <summary>
    /// ���ϴ� �ؽ��ĸ� ������ ��ο� png���Ϸ� �������ϴ�.
    /// </summary>
    /// <param name="texture">������ �ؽ���</param>
    /// <param name="filePath">������ ������ ���</param>
    /// <returns>�������� ��������</returns>
    public static bool ExportTextureToPNG(Texture2D texture, string filePath)
    {
        byte[] pngBytes = TextureToByte(texture);

        if (pngBytes != null)
        {
            //������ ��ο� ��������
            File.WriteAllBytes(filePath, pngBytes);
            return true;
        }

        return false;
    }

    public static bool ExportJSONToPNG(string jsonData, string fliePath)
    {
        PNG_JSON converted = JsonUtility.FromJson<PNG_JSON>(jsonData);
        if (converted.data == null) return false;
        File.WriteAllBytes(fliePath, converted.data);
        return true;
    }

    public static string TextureToJSON(Texture2D texture)
    {
        if(texture == null)
        {
            return null;
        }
        return JsonUtility.ToJson(new PNG_JSON(TextureToByte(texture)));
    }

    static byte[] TextureToByte(Texture2D texture)
    {
        if(texture == null) { return null; }

        RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
        RenderTexture.active = renderTexture;
        Graphics.Blit(texture, renderTexture);

        //���� ��������Ʈ�� �Ȱ��� �԰��� �ؽ��� ����
        Texture2D copyTexture = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
        //�����ؽ����� �ؽ��ĸ� �����ؿ���
        copyTexture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        copyTexture.Apply();

        //�޸� ����
        RenderTexture.active = null;
        renderTexture.Release();
        UnityEngine.Object.Destroy(renderTexture);

        //�ؽ��ĸ� PNG�� ���ڵ� �õ�
        byte[] pngBytes = copyTexture.EncodeToPNG();
        UnityEngine.Object.Destroy(copyTexture);

        return pngBytes;
    }

    [Serializable]
    struct PNG_JSON
    {
        [SerializeField]
        public byte[] data;

        public PNG_JSON(byte[] data)
        {
            this.data = data;
        }
    }
}