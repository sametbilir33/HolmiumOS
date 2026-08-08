using System;
using System.Drawing;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI.Apps
{
    public class BadApple : AppBase
    {
        [ManifestResourceStream(ResourceName = "HolmiumOS.Resources.bad_apple.bad")]
        private static byte[] embeddedBadVideo;

        public BadApple() : base("Bad Apple!! Player")
        {
        }

        public override void Load()
        {
            if (this.Window == null) return;

            this.Window.Title = "Bad Apple!! Player";

            // parentWindow parametresi kaldırıldı, koordinat offset hatası çözüldü
            VideoPlayerControl player = new VideoPlayerControl(5, 5, embeddedBadVideo);
            this.Window.AddControl(player);
        }
    }

    public class VideoPlayerControl : Control
    {
        private byte[] fileData;
        private int fileOffset = 0;
        private ushort width;
        private ushort height;
        private byte fps;
        private Color[] pixelBuffer;
        private bool isLoaded = false;

        public VideoPlayerControl(int x, int y, byte[] videoData) : base(x, y, 160, 120)
        {
            if (videoData == null || videoData.Length < 9) return;

            fileData = videoData;
            fileOffset = 0;

            if (fileData[0] != (byte)'B' || fileData[1] != (byte)'A' ||
                fileData[2] != (byte)'D' || fileData[3] != (byte)'!')
            {
                return;
            }

            fileOffset = 4;

            width = (ushort)(fileData[fileOffset] | (fileData[fileOffset + 1] << 8));
            fileOffset += 2;

            height = (ushort)(fileData[fileOffset] | (fileData[fileOffset + 1] << 8));
            fileOffset += 2;

            fps = fileData[fileOffset];
            fileOffset += 1;

            this.Width = width;
            this.Height = height;

            pixelBuffer = new Color[width * height];

            for (int py = 0; py < height; py++)
            {
                for (int px = 0; px < width; px++)
                {
                    if (fileOffset >= fileData.Length) break;
                    byte val = fileData[fileOffset++];
                    int index = (py * width) + px;
                    pixelBuffer[index] = Color.FromArgb(val, val, val);
                }
            }

            isLoaded = true;
        }

        private void ReadNextFrame()
        {
            if (!isLoaded || fileData == null || fileOffset >= fileData.Length) return;
            if (fileOffset + 4 > fileData.Length) return;

            uint diffCount = (uint)(fileData[fileOffset] |
                                   (fileData[fileOffset + 1] << 8) |
                                   (fileData[fileOffset + 2] << 16) |
                                   (fileData[fileOffset + 3] << 24));
            fileOffset += 4;

            for (uint i = 0; i < diffCount; i++)
            {
                if (fileOffset + 5 > fileData.Length) break;

                ushort px = (ushort)(fileData[fileOffset] | (fileData[fileOffset + 1] << 8));
                fileOffset += 2;

                ushort py = (ushort)(fileData[fileOffset] | (fileData[fileOffset + 1] << 8));
                fileOffset += 2;

                byte val = fileData[fileOffset++];

                if (px < width && py < height)
                {
                    int index = (py * width) + px;
                    pixelBuffer[index] = Color.FromArgb(val, val, val);
                }
            }
        }

        public override void Click()
        {
        }

        public override void Draw(Canvas canvas)
        {
            ReadNextFrame();

            if (!isLoaded || pixelBuffer == null) return;

            // Window.cs çizim yapmadan hemen önce this.X ve this.Y değerlerini
            // pencerenin mutlak ekran konumuna getiriyor.
            // Bu yüzden doğrudan bu koordinatları kullanıyoruz.
            int startX = this.X;
            int startY = this.Y;

            for (int py = 0; py < height; py++)
            {
                int rowOffset = py * width;
                int screenY = startY + py;

                for (int px = 0; px < width; px++)
                {
                    Color pixelColor = pixelBuffer[rowOffset + px];
                    canvas.DrawPoint(pixelColor, startX + px, screenY);
                }
            }
        }
    }
}