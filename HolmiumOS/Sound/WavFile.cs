using System;
using HolmiumOS.Shell;

namespace HolmiumOS.Sound
{
    public class WavFile
    {
        public byte[] Data { get; private set; }

        public ushort Channels { get; private set; }
        public uint SampleRate { get; private set; }
        public ushort BitsPerSample { get; private set; }

        public ushort AudioFormat { get; private set; }

        public static WavFile Load(string path)
        {
            byte[] file = FileSystemManager.ReadBytes(path);

            if (file == null || file.Length < 44)
                throw new Exception("Gecersiz veya bozuk WAV dosyasi.");

            if (file[0] != 'R' ||
                file[1] != 'I' ||
                file[2] != 'F' ||
                file[3] != 'F')
            {
                throw new Exception("WAV dosyasi RIFF formatinda degil.");
            }

            if (file[8] != 'W' ||
                file[9] != 'A' ||
                file[10] != 'V' ||
                file[11] != 'E')
            {
                throw new Exception("WAV dosyasi WAVE formatinda degil.");
            }

            WavFile wav = new WavFile();

            int position = 12;

            bool foundFmt = false;
            bool foundData = false;

            int dataPosition = 0;
            int dataSize = 0;

            while (position + 8 <= file.Length)
            {
                byte c0 = file[position];
                byte c1 = file[position + 1];
                byte c2 = file[position + 2];
                byte c3 = file[position + 3];

                int chunkSize = BitConverter.ToInt32(file, position + 4);

                if (chunkSize < 0)
                    throw new Exception("Gecersiz WAV chunk boyutu.");

                int chunkData = position + 8;

                if (chunkData > file.Length)
                    throw new Exception("Bozuk WAV chunk.");

                if (c0 == 'f' &&
                    c1 == 'm' &&
                    c2 == 't' &&
                    c3 == ' ')
                {
                    if (chunkSize < 16 || chunkData + 16 > file.Length)
                        throw new Exception("Gecersiz WAV fmt chunk.");

                    wav.AudioFormat = BitConverter.ToUInt16(file, chunkData);
                    wav.Channels = BitConverter.ToUInt16(file, chunkData + 2);
                    wav.SampleRate = BitConverter.ToUInt32(file, chunkData + 4);
                    wav.BitsPerSample = BitConverter.ToUInt16(file, chunkData + 14);

                    foundFmt = true;
                }
                else if (c0 == 'd' &&
                         c1 == 'a' &&
                         c2 == 't' &&
                         c3 == 'a')
                {
                    if (chunkData + chunkSize > file.Length)
                        throw new Exception("WAV data chunk dosya sinirlarini asiyor.");

                    dataPosition = chunkData;
                    dataSize = chunkSize;

                    foundData = true;
                    break;
                }

                position = chunkData + chunkSize;

                if ((position & 1) != 0)
                    position++;
            }

            if (!foundFmt)
                throw new Exception("WAV fmt chunk bulunamadi.");

            if (!foundData)
                throw new Exception("WAV data chunk bulunamadi.");

            if (wav.AudioFormat != 1)
                throw new Exception("Yalnizca PCM WAV dosyalari destekleniyor.");

            if (wav.Channels != 2)
                throw new Exception("AC97 yalnizca stereo WAV destekliyor.");

            if (wav.BitsPerSample != 16)
                throw new Exception("AC97 yalnizca 16-bit WAV destekliyor.");

            if (wav.SampleRate == 0)
                throw new Exception("Gecersiz WAV sample rate.");

            wav.Data = new byte[dataSize];

            Array.Copy(
                file,
                dataPosition,
                wav.Data,
                0,
                dataSize
            );

            return wav;
        }
    }
}