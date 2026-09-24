using System;
using System.Drawing;
using System.IO;
using Cosmos.Kernel.System.Graphics;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI.Apps
{
    public class BadApple : AppBase
    {
        public BadApple()
            : base("Bad Apple!! Player")
        {
        }

        public override void Load()
        {
            if (this.Window == null)
            {
                return;
            }

            this.Window.Title =
                "Bad Apple!! Player";

            byte[]? videoData =
                LoadVideoData();

            if (videoData is null)
            {
                return;
            }

            VideoPlayerControl player =
                new VideoPlayerControl(
                    5,
                    5,
                    videoData);

            this.Window.AddControl(player);
        }

        private static byte[]? LoadVideoData()
        {
            string[] paths =
            {
                "/mnt/boot/bad_apple.bad",
                "/mnt/bad_apple.bad",
                "/mnt/resources/bad_apple.bad",
                "/mnt/Resources/bad_apple.bad",
                "/mnt/resources/bad_apple/bad",
                "/mnt/Resources/bad_apple/bad"
            };

            foreach (string path in paths)
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        continue;
                    }

                    return File.ReadAllBytes(path);
                }
                catch
                {
                    // Bir sonraki yolu dene.
                }
            }

            return null;
        }
    }

    public class VideoPlayerControl : Control
    {
        private readonly byte[] fileData;

        private int fileOffset;

        private ushort width;

        private ushort height;

        private byte fps;

        private Color[]? pixelBuffer;

        private bool isLoaded;

        public VideoPlayerControl(
            int x,
            int y,
            byte[] videoData)
            : base(x, y, 160, 120)
        {
            fileData = videoData;

            if (fileData.Length < 9)
            {
                return;
            }

            fileOffset = 0;

            if (fileData[0] != (byte)'B' ||
                fileData[1] != (byte)'A' ||
                fileData[2] != (byte)'D' ||
                fileData[3] != (byte)'!')
            {
                return;
            }

            fileOffset = 4;

            width =
                (ushort)(
                    fileData[fileOffset] |
                    (fileData[fileOffset + 1] << 8));

            fileOffset += 2;

            height =
                (ushort)(
                    fileData[fileOffset] |
                    (fileData[fileOffset + 1] << 8));

            fileOffset += 2;

            fps =
                fileData[fileOffset];

            fileOffset++;

            Width = width;
            Height = height;

            pixelBuffer =
                new Color[width * height];

            for (int py = 0;
                 py < height;
                 py++)
            {
                for (int px = 0;
                     px < width;
                     px++)
                {
                    if (fileOffset >= fileData.Length)
                    {
                        break;
                    }

                    byte value =
                        fileData[fileOffset++];

                    int index =
                        (py * width) + px;

                    pixelBuffer[index] =
                        Color.FromArgb(
                            value,
                            value,
                            value);
                }
            }

            isLoaded = true;
        }

        private void ReadNextFrame()
        {
            if (!isLoaded ||
                pixelBuffer is null ||
                fileOffset >= fileData.Length)
            {
                return;
            }

            if (fileOffset + 4 > fileData.Length)
            {
                return;
            }

            uint diffCount =
                (uint)(
                    fileData[fileOffset] |
                    (fileData[fileOffset + 1] << 8) |
                    (fileData[fileOffset + 2] << 16) |
                    (fileData[fileOffset + 3] << 24));

            fileOffset += 4;

            for (uint i = 0;
                 i < diffCount;
                 i++)
            {
                if (fileOffset + 5 > fileData.Length)
                {
                    break;
                }

                ushort px =
                    (ushort)(
                        fileData[fileOffset] |
                        (fileData[fileOffset + 1] << 8));

                fileOffset += 2;

                ushort py =
                    (ushort)(
                        fileData[fileOffset] |
                        (fileData[fileOffset + 1] << 8));

                fileOffset += 2;

                byte value =
                    fileData[fileOffset++];

                if (px < width &&
                    py < height)
                {
                    int index =
                        (py * width) + px;

                    pixelBuffer[index] =
                        Color.FromArgb(
                            value,
                            value,
                            value);
                }
            }
        }

        public override void Click()
        {
        }

        public override void Draw(Canvas canvas)
        {
            ReadNextFrame();

            if (!isLoaded ||
                pixelBuffer is null)
            {
                return;
            }

            int startX = X;
            int startY = Y;

            for (int py = 0;
                 py < height;
                 py++)
            {
                int rowOffset =
                    py * width;

                int screenY =
                    startY + py;

                for (int px = 0;
                     px < width;
                     px++)
                {
                    Color pixelColor =
                        pixelBuffer[rowOffset + px];

                    canvas.DrawPoint(
                        pixelColor,
                        startX + px,
                        screenY);
                }
            }
        }
    }
}