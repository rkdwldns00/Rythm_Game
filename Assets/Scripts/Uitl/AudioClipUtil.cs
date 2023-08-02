using System;
using System.IO;
using UnityEngine;

public static class AudioClipUtil
{
    public static void ExportAudioClipToWAV(AudioClip audioClip, string filePath)
    {
        var clipData = ExtractionAudioClipData(audioClip);
        if (!clipData.isConvertSuccessed) { return; }

        WriteWAV(clipData.chunkSize, clipData.channels, clipData.sampleRate, clipData.subchunk2Size, clipData.bytesData, filePath);
    }

    public static string AudioClipToJSON(AudioClip audioClip)
    {
        var data = ExtractionAudioClipData(audioClip);
        if (!data.isConvertSuccessed)
        {
            return null;
        }
        return JsonUtility.ToJson(new WAV_JSON(data.chunkSize, data.channels, data.sampleRate, data.subchunk2Size, data.bytesData));
    }

    public static bool ExportJSONToAudioClip(string jsonData, string filePath)
    {
        WAV_JSON converted = JsonUtility.FromJson<WAV_JSON>(jsonData);
        WriteWAV(converted.chunkSize, converted.channels, converted.sampleRate, converted.subchunk2Size, converted.bytesData, filePath);
        return true;
    }

    static void WriteWAV(int chunkSize, short channels, int sampleRate, int subchunk2Size, byte[] bytesData, string filePath)
    {
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

    static (bool isConvertSuccessed, int chunkSize, short channels, int sampleRate, int subchunk2Size, byte[] bytesData) ExtractionAudioClipData(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            return (false, 0, 0, 0, 0, null);
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

        return (true, chunkSize, channels, sampleRate, subchunk2Size, bytesData);
    }

    [Serializable]
    struct WAV_JSON
    {
        [SerializeField]
        public int chunkSize;
        [SerializeField]
        public short channels;
        [SerializeField]
        public int sampleRate;
        [SerializeField]
        public int subchunk2Size;
        [SerializeField]
        public byte[] bytesData;

        public WAV_JSON(int chunkSize, short channels, int sampleRate, int subchunk2Size, byte[] bytesData)
        {
            this.chunkSize = chunkSize;
            this.channels = channels;
            this.sampleRate = sampleRate;
            this.subchunk2Size = subchunk2Size;
            this.bytesData = bytesData;
        }
    }
}

