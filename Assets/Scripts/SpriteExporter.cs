using UnityEngine;
using System.IO;

public class SpriteExporter : MonoBehaviour
{
    public static void ExportSpriteToFile(Sprite sprite,string filePath)
    {
        if (sprite == null)
        {
            Debug.LogWarning("No SpriteRenderer or Sprite assigned!");
            return;
        }

        Texture2D spriteTexture = sprite.texture;

        // Create a temporary RenderTexture to copy the sprite's texture
        //RenderTexture rt = new RenderTexture(spriteTexture.width, spriteTexture.height, 0, RenderTextureFormat.ARGB32);
        //RenderTexture.active = rt;
        //Graphics.Blit(spriteTexture, rt);

        // Create a new Texture2D to save the copy
        Texture2D copyTexture = new Texture2D(spriteTexture.width, spriteTexture.height, TextureFormat.ARGB32, false);
        copyTexture.ReadPixels(new Rect(0, 0, spriteTexture.width, spriteTexture.height), 0, 0);
        copyTexture.Apply();

        // Release the temporary RenderTexture
        RenderTexture.active = null;
        //rt.Release();
        //Destroy(rt);

        // Encode the copyTexture to a PNG file
        byte[] pngBytes = copyTexture.EncodeToPNG();
        Destroy(copyTexture); // Clean up the temporary Texture2D

        if (pngBytes != null)
        {
            File.WriteAllBytes(filePath, pngBytes);
            Debug.Log("스프라이트 내보내기 성공\n저장된 파일 : " + filePath);
        }
        else
        {
            Debug.LogError("Failed to encode sprite to PNG!");
        }
    }
}