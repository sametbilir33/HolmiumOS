using System;
using HolmiumOS.Shell;

namespace HolmiumOS.Sound
{
    public class WavFile
    {
        public byte[] Data;
        public ushort Channels;
        public uint SampleRate;
        public ushort BitsPerSample;

        public static WavFile Load(string path)
        {
            byte[] file = FileSystemManager.ReadBytes(path);

            WavFile wav = new WavFile();

            wav.Channels = BitConverter.ToUInt16(file, 22);
            wav.SampleRate = BitConverter.ToUInt32(file, 24);
            wav.BitsPerSample = BitConverter.ToUInt16(file, 34);

            int dataPosition = -1;

            for (int i = 12; i < file.Length - 4; i++)
            {
                if (file[i] == 'd' &&
                    file[i + 1] == 'a' &&
                    file[i + 2] == 't' &&
                    file[i + 3] == 'a')
                {
                    dataPosition = i + 8;
                    break;
                }
            }

            if (dataPosition == -1)
                throw new Exception("WAV data chunk bulunamadi.");

            int dataSize = BitConverter.ToInt32(file, dataPosition - 4);

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