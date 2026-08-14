using System.Collections.Generic;
using System.IO;
using NVorbis;

namespace Cupi.ResourceLoader.Decoders
{
    public static class OggDecoder
    {
        public static (float[] samples, int channels, int frequency) Decode(byte[] fileBytes)
        {
            using var ms = new MemoryStream(fileBytes);
            using var vorbis = new VorbisReader(ms);

            int channels = vorbis.Channels;
            int frequency = vorbis.SampleRate;

            var allSamples = new List<float>((int)(vorbis.TotalSamples * channels));
            float[] buffer = new float[4096];
            int samplesRead;

            while ((samplesRead = vorbis.ReadSamples(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < samplesRead; i++)
                    allSamples.Add(buffer[i]);
            }

            return (allSamples.ToArray(), channels, frequency);
        }
    }
}

