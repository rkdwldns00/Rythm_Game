using System;
using System.IO;
using UnityEngine;

//이 코드는 Chat gpt의 코드를 개선한 버전입니다!
public static class AudioClipExporter
{
    public static void ExportAudioClipToWAV(AudioClip audioClip, string filePath)
    {
        if (audioClip == null)
        {
            return;
        }

        // Get the audio data from the AudioClip
        float[] data = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(data, 0);

        // Scale the data to short range (16-bit PCM format)
        short[] shortData = new short[data.Length];
        int rescaleFactor = short.MaxValue;
        for (int i = 0; i < data.Length; i++)
        {
            shortData[i] = (short)(data[i] * rescaleFactor);
        }

        // Convert short data to bytes
        byte[] bytesData = new byte[shortData.Length * 2];
        Buffer.BlockCopy(shortData, 0, bytesData, 0, bytesData.Length);

        // Create the WAV file header
        int sampleRate = audioClip.frequency;
        short channels = (short)audioClip.channels;
        int subchunk2Size = bytesData.Length;
        int chunkSize = 36 + subchunk2Size;

        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(fileStream))
            {
                // Write the RIFF header
                writer.Write("RIFF".ToCharArray());
                writer.Write(chunkSize);
                writer.Write("WAVE".ToCharArray());

                // Write the format chunk
                writer.Write("fmt ".ToCharArray());
                writer.Write(16); // Subchunk1Size (16 for PCM)
                writer.Write((short)1); // AudioFormat (1 for PCM)
                writer.Write(channels);
                writer.Write(sampleRate);
                writer.Write(sampleRate * channels * 2); // ByteRate
                writer.Write((short)(channels * 2)); // BlockAlign
                writer.Write((short)16); // BitsPerSample

                // Write the data chunk
                writer.Write("data".ToCharArray());
                writer.Write(subchunk2Size);
                writer.Write(bytesData);
            }
        }

        Debug.Log("WAV파일 내보내기 완료, 경로 : " + filePath);
    }
}

