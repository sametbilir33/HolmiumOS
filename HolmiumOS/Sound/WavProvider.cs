using Cosmos.HAL.Audio;

namespace HolmiumOS.Sound
{
    public class WavProvider : IAudioBufferProvider
    {
        private readonly byte[] data;
        private int position;

        public bool Finished
        {
            get
            {
                return position >= data.Length;
            }
        }

        public WavProvider(byte[] pcmData)
        {
            data = pcmData;
            position = 0;
        }

        public void RequestBuffer(AudioBuffer buffer)
        {
            byte[] output = buffer.RawData;

            for (int i = 0; i < output.Length; i++)
            {
                if (position < data.Length)
                {
                    output[i] = data[position];
                    position++;
                }
                else
                {
                    output[i] = 0;
                }
            }
        }
    }
}