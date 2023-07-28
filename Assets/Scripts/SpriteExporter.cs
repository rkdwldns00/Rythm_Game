using UnityEngine;
using System.IO;

public static class SpriteExporter
{
    /// <summary>
    /// 원하는 스프라이트를 지정된 경로에 png파일로 내보냅니다.
    /// </summary>
    /// <param name="sprite">내보낼 스프라이트</param>
    /// <param name="filePath">파일을 저장할 경로</param>
    /// <returns>내보내기 성공여부</returns>
    public static bool ExportSpriteToFile(Sprite sprite,string filePath)
    {
        if (sprite == null)
        {
            return false;
        }

        return ExportTextureToFile(sprite.texture, filePath);
    }

    /// <summary>
    /// 원하는 텍스쳐를 지정된 경로에 png파일로 내보냅니다.
    /// </summary>
    /// <param name="texture">내보낼 텍스쳐</param>
    /// <param name="filePath">파일을 저장할 경로</param>
    /// <returns>내보내기 성공여부</returns>
    public static bool ExportTextureToFile(Texture2D texture, string filePath)
    {
        if(texture == null)
        {
            return false;
        }

        //새로운 렌더텍스쳐에 스프라이트 복사
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
        Object.Destroy(renderTexture);

        //텍스쳐를 PNG로 인코딩 시도
        byte[] pngBytes = copyTexture.EncodeToPNG();
        Object.Destroy(copyTexture);

        if (pngBytes != null)
        {
            //지정된 경로에 파일저장
            File.WriteAllBytes(filePath, pngBytes);
            return true;
        }

        return false;
    }
}