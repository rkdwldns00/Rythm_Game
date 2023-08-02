using UnityEngine;
using System.IO;
using System;

//이 코드는 Chat gpt의 코드를 개선한 버전입니다!
public static class SpriteExporter
{
    /// <summary>
    /// 원하는 스프라이트를 지정된 경로에 png파일로 내보냅니다.
    /// </summary>
    /// <param name="sprite">내보낼 스프라이트</param>
    /// <param name="filePath">파일을 저장할 경로</param>
    /// <returns>내보내기 성공여부</returns>
    public static bool ExportSpriteToPNG(Sprite sprite, string filePath)
    {
        if (sprite == null)
        {
            return false;
        }

        return ExportTextureToPNG(sprite.texture, filePath);
    }

    /// <summary>
    /// 원하는 텍스쳐를 지정된 경로에 png파일로 내보냅니다.
    /// </summary>
    /// <param name="texture">내보낼 텍스쳐</param>
    /// <param name="filePath">파일을 저장할 경로</param>
    /// <returns>내보내기 성공여부</returns>
    public static bool ExportTextureToPNG(Texture2D texture, string filePath)
    {
        byte[] pngBytes = TextureToByte(texture);

        if (pngBytes != null)
        {
            //지정된 경로에 파일저장
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

        //원본 스프라이트와 똑같은 규격의 텍스쳐 생성
        Texture2D copyTexture = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
        //렌더텍스쳐의 텍스쳐를 복사해오기
        copyTexture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        copyTexture.Apply();

        //메모리 해제
        RenderTexture.active = null;
        renderTexture.Release();
        UnityEngine.Object.Destroy(renderTexture);

        //텍스쳐를 PNG로 인코딩 시도
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