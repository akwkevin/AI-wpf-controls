using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AIStudio.Wpf.Controls
{
    public class GifFrame : Image
    {

        public int delayTime;

        public int disposalMethod;

        public int left;

        public int top;

        public int width;

        public int height;
    }

    public class GifAnimation : Viewbox
    {
        private Canvas canvas = null;

        private List<GifFrame> frameList = null;

        private int frameCounter = 0;
        private int numberOfFrames = 0;

        private int numberOfLoops = -1;
        private int currentLoop = 0;

        private int logicalWidth = 0;
        private int logicalHeight = 0;

        private DispatcherTimer frameTimer = null;

        private GifFrame currentParseGifFrame;

        public GifAnimation()
        {
            canvas = new Canvas();
            this.Child = canvas;
        }

        private void Reset()
        {
            if (frameList != null)
            {
                frameList.Clear();
            }
            frameList = null;
            frameCounter = 0;
            numberOfFrames = 0;
            numberOfLoops = -1;
            currentLoop = 0;
            logicalWidth = 0;
            logicalHeight = 0;
            if (frameTimer != null)
            {
                frameTimer.Stop();
                frameTimer = null;
            }
        }

        #region PARSE
        private void ParseGif(byte[] gifData)
        {
            frameList = new List<GifFrame>();
            currentParseGifFrame = new GifFrame();
            ParseGifDataStream(gifData, 0);
        }


        private int ParseBlock(byte[] gifData, int offset)
        {
            switch (gifData[offset])
            {
                case 0x21:
                    if (gifData[offset + 1] == 0xF9)
                    {
                        return ParseGraphicControlExtension(gifData, offset);
                    }
                    else
                    {
                        return ParseExtensionBlock(gifData, offset);
                    }
                case 0x2C:
                    offset = ParseGraphicBlock(gifData, offset);
                    frameList.Add(currentParseGifFrame);
                    currentParseGifFrame = new GifFrame();
                    return offset;
                case 0x3B:
                    return -1;
                default:
                    throw new Exception("GIF format incorrect: missing graphic block or special-purpose block. ");
            }
        }

        private int ParseGraphicControlExtension(byte[] gifData, int offset)
        {
            int returnOffset = offset;
            int length = gifData[offset + 2];
            returnOffset = offset + length + 2 + 1;

            byte packedField = gifData[offset + 3];
            currentParseGifFrame.disposalMethod = (packedField & 0x1C) >> 2;

            int delay = BitConverter.ToUInt16(gifData, offset + 4);
            currentParseGifFrame.delayTime = delay;
            while (gifData[returnOffset] != 0x00)
            {
                returnOffset = returnOffset + gifData[returnOffset] + 1;
            }

            returnOffset++;

            return returnOffset;
        }

        private int ParseLogicalScreen(byte[] gifData, int offset)
        {
            logicalWidth = BitConverter.ToUInt16(gifData, offset);
            logicalHeight = BitConverter.ToUInt16(gifData, offset + 2);

            byte packedField = gifData[offset + 4];
            bool hasGlobalColorTable = (int)(packedField & 0x80) > 0 ? true : false;

            int currentIndex = offset + 7;
            if (hasGlobalColorTable)
            {
                int colorTableLength = packedField & 0x07;
                colorTableLength = (int)Math.Pow(2, colorTableLength + 1) * 3;
                currentIndex = currentIndex + colorTableLength;
            }
            return currentIndex;
        }

        private int ParseGraphicBlock(byte[] gifData, int offset)
        {
            currentParseGifFrame.left = BitConverter.ToUInt16(gifData, offset + 1);
            currentParseGifFrame.top = BitConverter.ToUInt16(gifData, offset + 3);
            currentParseGifFrame.width = BitConverter.ToUInt16(gifData, offset + 5);
            currentParseGifFrame.height = BitConverter.ToUInt16(gifData, offset + 7);
            if (currentParseGifFrame.width > logicalWidth)
            {
                logicalWidth = currentParseGifFrame.width;
            }
            if (currentParseGifFrame.height > logicalHeight)
            {
                logicalHeight = currentParseGifFrame.height;
            }
            byte packedField = gifData[offset + 9];
            bool hasLocalColorTable = (int)(packedField & 0x80) > 0 ? true : false;

            int currentIndex = offset + 9;
            if (hasLocalColorTable)
            {
                int colorTableLength = packedField & 0x07;
                colorTableLength = (int)Math.Pow(2, colorTableLength + 1) * 3;
                currentIndex = currentIndex + colorTableLength;
            }
            currentIndex++;

            currentIndex++;

            while (gifData[currentIndex] != 0x00)
            {
                int length = gifData[currentIndex];
                currentIndex = currentIndex + gifData[currentIndex];
                currentIndex++;
            }
            currentIndex = currentIndex + 1;
            return currentIndex;
        }

        private int ParseExtensionBlock(byte[] gifData, int offset)
        {
            int returnOffset = offset;
            int length = gifData[offset + 2];
            returnOffset = offset + length + 2 + 1;
            if (gifData[offset + 1] == 0xFF && length > 10)
            {
                string netscape = System.Text.ASCIIEncoding.ASCII.GetString(gifData, offset + 3, 8);
                if (netscape == "NETSCAPE")
                {
                    numberOfLoops = BitConverter.ToUInt16(gifData, offset + 16);
                    if (numberOfLoops > 0)
                    {
                        numberOfLoops++;
                    }
                }
            }
            while (gifData[returnOffset] != 0x00)
            {
                returnOffset = returnOffset + gifData[returnOffset] + 1;
            }

            returnOffset++;

            return returnOffset;
        }

        private int ParseHeader(byte[] gifData, int offset)
        {
            string str = System.Text.ASCIIEncoding.ASCII.GetString(gifData, offset, 3);
            if (str != "GIF")
            {
                throw new Exception("Not a proper GIF file: missing GIF header");
            }
            return 6;
        }

        private void ParseGifDataStream(byte[] gifData, int offset)
        {
            offset = ParseHeader(gifData, offset);
            offset = ParseLogicalScreen(gifData, offset);
            while (offset != -1)
            {
                offset = ParseBlock(gifData, offset);
            }
        }

        #endregion

        public void CreateGifAnimation(MemoryStream memoryStream)
        {
            Reset();

            byte[] gifData = memoryStream.GetBuffer();  // Use GetBuffer so that there is no memory copy

            GifBitmapDecoder decoder = new GifBitmapDecoder(memoryStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            numberOfFrames = decoder.Frames.Count;

            try
            {
                ParseGif(gifData);
            }
            catch
            {
                throw new FileFormatException("Unable to parse Gif file format.");
            }

            for (int i = 0; i < decoder.Frames.Count; i++)
            {
                frameList[i].Source = decoder.Frames[i];
                frameList[i].Visibility = Visibility.Hidden;
                canvas.Children.Add(frameList[i]);
                Canvas.SetLeft(frameList[i], frameList[i].left);
                Canvas.SetTop(frameList[i], frameList[i].top);
                Canvas.SetZIndex(frameList[i], i);
            }
            canvas.Height = logicalHeight;
            canvas.Width = logicalWidth;

            frameList[0].Visibility = Visibility.Visible;

            for (int i = 0; i < frameList.Count; i++)
            {
                Console.WriteLine(frameList[i].disposalMethod.ToString() + " " + frameList[i].width.ToString() + " " + frameList[i].delayTime.ToString());
            }

            if (frameList.Count > 1)
            {
                if (numberOfLoops == -1)
                {
                    numberOfLoops = 1;
                }
                frameTimer = new System.Windows.Threading.DispatcherTimer();
                frameTimer.Tick += NextFrame;
                frameTimer.Interval = new TimeSpan(0, 0, 0, 0, frameList[0].delayTime * 10);
                frameTimer.Start();
            }
        }

        public void NextFrame()
        {
            NextFrame(null, null);
        }

        public void NextFrame(object sender, EventArgs e)
        {
            frameTimer.Stop();
            if (numberOfFrames == 0) return;
            if (frameList[frameCounter].disposalMethod == 2)
            {
                frameList[frameCounter].Visibility = Visibility.Hidden;
            }
            if (frameList[frameCounter].disposalMethod >= 3)
            {
                frameList[frameCounter].Visibility = Visibility.Hidden;
            }
            frameCounter++;

            if (frameCounter < numberOfFrames)
            {
                frameList[frameCounter].Visibility = Visibility.Visible;
                frameTimer.Interval = new TimeSpan(0, 0, 0, 0, frameList[frameCounter].delayTime * 10);
                frameTimer.Start();
            }
            else
            {
                if (numberOfLoops != 0)
                {
                    currentLoop++;
                }
                if (currentLoop < numberOfLoops || numberOfLoops == 0)
                {
                    for (int f = 0; f < frameList.Count; f++)
                    {
                        frameList[f].Visibility = Visibility.Hidden;
                    }
                    frameCounter = 0;
                    frameList[frameCounter].Visibility = Visibility.Visible;
                    frameTimer.Interval = new TimeSpan(0, 0, 0, 0, frameList[frameCounter].delayTime * 10);
                    frameTimer.Start();
                }
            }
        }
    }
}
