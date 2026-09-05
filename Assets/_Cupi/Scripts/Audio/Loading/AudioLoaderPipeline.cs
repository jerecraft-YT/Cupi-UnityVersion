using System.Threading.Tasks;
using UnityEngine;
using CupiEngine.ResourceLoader.Audio.Decoders;

namespace CupiEngine.ResourceLoader.Audio
{
    public static class AudioLoaderPipeline
    {
        public static async Task<AudioClip> LoadAndPrepare(byte[] fileBytes, string clipName)
        {
            // todo el trabajo pesado va en background
            var result = await Task.Run(() => Decode(fileBytes));

            if (result.samples == null)
            {
                Debug.LogWarning($"No se pudo decodificar el audio '{clipName}' con los decoders disponibles.");
                return null;
            }

            // solo esto corre en el hilo principal, y es rápido (memcpy)
            int sampleCountPerChannel = result.samples.Length / result.channels;

            AudioClip Clip = AudioClip.Create(clipName, sampleCountPerChannel, result.channels, result.frequency, false);
            Clip.SetData(result.samples, 0);

            return Clip;
        }

        private static (float[] samples, int channels, int frequency) Decode(byte[] fileBytes)
        {
            SupportedFormat format = DetectFormat(fileBytes);

            float[] samples;
            int channels;
            int frequency;

            switch (format)
            {
                case SupportedFormat.Wav:
                    (samples, channels, frequency) = WavDecoder.Decode(fileBytes);
                    break;
                case SupportedFormat.Ogg:
                    (samples, channels, frequency) = OggDecoder.Decode(fileBytes);
                    break;
                case SupportedFormat.Mp3:
                    (samples, channels, frequency) = Mp3Decoder.Decode(fileBytes);
                    break;
                default:
                    return (null, 0, 0);
            }

            return (samples, channels, frequency);
        }

        public enum SupportedFormat { Wav, Ogg, Mp3, Unknown }

        public static SupportedFormat DetectFormat(byte[] data)
        {
            if (data.Length < 4) return SupportedFormat.Unknown;

            if (data[0] == 'R' && data[1] == 'I' && data[2] == 'F' && data[3] == 'F')
                return SupportedFormat.Wav;

            if (data[0] == 'O' && data[1] == 'g' && data[2] == 'g' && data[3] == 'S')
                return SupportedFormat.Ogg;

            if ((data[0] == 'I' && data[1] == 'D' && data[2] == '3') ||
                (data[0] == 0xFF && (data[1] & 0xE0) == 0xE0))
                return SupportedFormat.Mp3;

            return SupportedFormat.Unknown;
        }
    }
}

