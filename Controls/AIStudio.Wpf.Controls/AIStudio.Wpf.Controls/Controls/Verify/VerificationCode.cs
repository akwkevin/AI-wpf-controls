using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AIStudio.Wpf.Controls
{
    public class VerificationCode
    {
        /// <summary>
        /// 验证码     
        /// </summary>
        public readonly string IndentifyingCode;
        /// <summary>
        /// 图片
        /// </summary>
        public readonly ImageSource CodeImageSource;

        private VerificationCode(string code, int width, int height)
        {
            this.IndentifyingCode = code;
            this.CodeImageSource = CreateCheckCodeImage(code, width, height);
        }

        public static VerificationCode GetNewIndentifyCode(int strLength, string displayMode, int width, int height)
        {
            return new VerificationCode(CreateCode(strLength, displayMode), width, height);
        }

        private static string CreateCode(int strLength, string displayMode)
        {
            var strCode = string.Empty;
            switch (displayMode)
            {
                case "1":
                    strCode = "abcdefhkmnprstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
                    break;
                case "2":
                    strCode = "1234567890";
                    break;
                case "3":
                    strCode = "abcdefghigklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    break;
                default:
                    strCode = "abcdefhkmnprstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
                    break;
            }
            var _charArray = strCode.ToCharArray();

            var randomCode = "";
            int temp = -1;
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < strLength; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(strCode.Length - 1);
                if (!string.IsNullOrWhiteSpace(randomCode))
                {
                    while (randomCode.ToLower().Contains(_charArray[t].ToString().ToLower()))
                    {
                        t = rand.Next(strCode.Length - 1);
                    }
                }
                if (temp == t)
                {
                    return CreateCode(strLength, displayMode);
                }
                temp = t;

                randomCode += _charArray[t];
            }
            return randomCode;
        }

        private ImageSource CreateCheckCodeImage(string checkCode, int width, int height)
        {
            if (string.IsNullOrWhiteSpace(checkCode))
                return null;
            if (width <= 0 || height <= 0)
                return null;
            DrawingVisual drawingVisual = new DrawingVisual();

            //生成随机生成器
            Random random = new Random(Guid.NewGuid().GetHashCode());

            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                dc.DrawRectangle(Brushes.White, new Pen(Brushes.Silver, 1D), new Rect(new Size(70, 23)));
                FormattedText formattedText = new FormattedText(checkCode,
                    System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyles.Oblique, FontWeights.Bold, FontStretches.Normal),
                    20.001D, new LinearGradientBrush(Colors.Blue, Colors.DarkRed, 1.2D))
                {
                    MaxLineCount = 1,
                    TextAlignment = TextAlignment.Justify,
                    Trimming = TextTrimming.CharacterEllipsis
                };

                dc.DrawText(formattedText, new Point(3D, 0.1D));

                for (int i = 0; i < 10; i++)
                {
                    int x1 = random.Next(width - 1);
                    int y1 = random.Next(height - 1);
                    int x2 = random.Next(width - 1);
                    int y2 = random.Next(height - 1);

                    dc.DrawGeometry(Brushes.Silver, new Pen(Brushes.Silver, 0.5D), new LineGeometry(new Point(x1, y1), new Point(x2, y2)));
                }

                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(width - 1);
                    int y = random.Next(height - 1);
                    SolidColorBrush c = new SolidColorBrush(Color.FromRgb((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)));
                    dc.DrawGeometry(c, new Pen(c, 1D), new LineGeometry(new Point(x - 0.5, y - 0.5), new Point(x + 0.5, y + 0.5)));
                }

                dc.Close();
            }

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(70, 23, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);
            return BitmapFrame.Create(renderBitmap);
        }
    }
}
