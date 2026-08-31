using System;
using System.IO;

namespace Cupi.ResourceLoader.Audio.Decoders
{
    public static class WavDecoder
    {
        public static (float[] samples, int channels, int frequency) Decode(byte[] fileBytes)
        {
            using var stream = new MemoryStream(fileBytes);
            using var reader = new BinaryReader(stream);

            // Header RIFF
            string riff = new string(reader.ReadChars(4)); // "RIFF"
            reader.ReadInt32(); // tamaño del chunk, no lo necesitamos
            string wave = new string(reader.ReadChars(4)); // "WAVE"

            int channels = 0;
            int sampleRate = 0;
            int bitsPerSample = 0;
            float[] samples = null;

            while (stream.Position < stream.Length)
            {
                string chunkId = new string(reader.ReadChars(4));
                int chunkSize = reader.ReadInt32();

                if (chunkId == "fmt ")
                {
                    reader.ReadInt16(); // audio format (1 = PCM)
                    channels = reader.ReadInt16();
                    sampleRate = reader.ReadInt32();
                    reader.ReadInt32(); // byte rate
                    reader.ReadInt16(); // block align
                    bitsPerSample = reader.ReadInt16();

                    int remaining = chunkSize - 16;
                    if (remaining > 0) reader.ReadBytes(remaining);
                }
                else if (chunkId == "data")
                {
                    byte[] rawData = reader.ReadBytes(chunkSize);
                    samples = ConvertToFloat(rawData, bitsPerSample);
                }
                else
                {
                    // saltar chunks que no nos interesan (LIST, fact, etc)
                    if (chunkSize > 0 && stream.Position + chunkSize <= stream.Length)
                        reader.ReadBytes(chunkSize);
                }
            }

            return (samples, channels, sampleRate);
        }

        private static float[] ConvertToFloat(byte[] rawData, int bitsPerSample)
        {
            int bytesPerSample = bitsPerSample / 8;
            int sampleCount = rawData.Length / bytesPerSample;
            float[] result = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                int offset = i * bytesPerSample;

                switch (bitsPerSample)
                {
                    case 16:
                        short val16 = BitConverter.ToInt16(rawData, offset);
                        result[i] = val16 / 32768f;
                        break;
                    case 24:
                        int val24 = (rawData[offset + 2] << 16) | (rawData[offset + 1] << 8) | rawData[offset];
                        if ((val24 & 0x800000) != 0) val24 |= unchecked((int)0xFF000000); // signo
                        result[i] = val24 / 8388608f;
                        break;
                    case 32:
                        int val32 = BitConverter.ToInt32(rawData, offset);
                        result[i] = val32 / 2147483648f;
                        break;
                    default:
                        throw new NotSupportedException($"WAV de {bitsPerSample} bits no soportado");
                }
            }

            return result;
        }
    }
}