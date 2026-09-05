using System.Collections.Generic;
using System.IO;
using NLayer;

namespace CupiEngine.ResourceLoader.Audio.Decoders
{
    public static class Mp3Decoder
    {
        public static (float[] samples, int channels, int frequency) Decode(byte[] fileBytes)
        {
            using var ms = new MemoryStream(fileBytes);
            using var mp3 = new MpegFile(ms);

            int channels = mp3.Channels;
            int frequency = mp3.SampleRate;

            var allSamples = new List<float>();
            float[] buffer = new float[4096];
            int samplesRead;

            while ((samplesRead = mp3.ReadSamples(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < samplesRead; i++)
                    allSamples.Add(buffer[i]);
            }

            return (allSamples.ToArray(), channels, frequency);
        }
    }
}

