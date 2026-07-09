using Cosmos.HAL.Audio;

namespace HolmiumOS.Sound
{
    public class WavProvider : IAudioBufferProvider
    {
        private readonly byte[] data;
        private int position;

        public bool Finished => position >= data.Length;

        public WavProvider(byte[] wavFile)
        {
            data = new byte[wavFile.Length - 44];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = wavFile[i + 44];
            }

            position = 0;
        }

        public void RequestBuffer(AudioBuffer buffer)
        {
            byte[] output = buffer.RawData;

            for (int i = 0; i < output.Length; i++)
            {
                if (position < data.Length)
                {
                    output[i] = data[position++];
                }
                else
                {
                    output[i] = 0;
                }
            }
        }
    }
}