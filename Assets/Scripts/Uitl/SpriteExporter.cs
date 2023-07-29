using UnityEngine;
using System.IO;

//�� �ڵ�� Chat gpt�� �ڵ带 ������ �����Դϴ�!
public static class SpriteExporter
{
    /// <summary>
    /// ���ϴ� ��������Ʈ�� ������ ��ο� png���Ϸ� �������ϴ�.
    /// </summary>
    /// <param name="sprite">������ ��������Ʈ</param>
    /// <param name="filePath">������ ������ ���</param>
    /// <returns>�������� ��������</returns>
    public static bool ExportSpriteToPNG(Sprite sprite,string filePath)
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
        if(texture == null)
        {
            return false;
        }

        //���ο� �����ؽ��Ŀ� ��������Ʈ ����
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
        Object.Destroy(renderTexture);

        //�ؽ��ĸ� PNG�� ���ڵ� �õ�
        byte[] pngBytes = copyTexture.EncodeToPNG();
        Object.Destroy(copyTexture);

        if (pngBytes != null)
        {
            //������ ��ο� ��������
            File.WriteAllBytes(filePath, pngBytes);
            return true;
        }

        return false;
    }
}