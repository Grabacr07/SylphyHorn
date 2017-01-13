using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SylphyHorn.Services
{
    public class DigitDrawer : IDisposable
    {
        public DigitDrawer(int width, Color color)
        {
            Width = width;
            Color = color;
        }

        private Image[] _cachedDigits = new Image[10];
        private Color _color;
        private int _width;

        public int Width
        {
            get { return _width; }
            set
            {
                ClearCache();
                _width = value;
            }
        }

        // TODO: change color of created digits instead of clearing them
        public Color Color
        {
            get { return _color; }
            set
            {
                ClearCache();
                _color = value;
            }
        }

        public Image GetDigit(int digit, Size digitSize)
        {
            if (digit < 0 || digit > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(digit));
            }

            var digitImage = _cachedDigits[digit];

            if (digitImage == null)
            {
                digitImage = Draw(digit, digitSize);

                _cachedDigits[digit] = digitImage;
            }

            return digitImage;
        }

        private Image Draw(int digit, Size digitSize)
        {
            var digitImage = new Bitmap(digitSize.Width, digitSize.Height);

            using (var digitGraphics = Graphics.FromImage(digitImage))
            {
                var commonPositions = GetCommonPositions(digitSize);
                var drawPath = GetDrawPath(digit, commonPositions);

                using (var pen = new Pen(this.Color, this.Width))
                {
                    digitGraphics.DrawLines(pen, drawPath);
                }
            }

            return digitImage;
        }

        private Point[] GetDrawPath(int digit, Point[] commonPositions)
        {
            switch (digit)
            {
                case 0:
                    return new Point[]
                    {
                        commonPositions[0],
                        commonPositions[1],
                        commonPositions[5],
                        commonPositions[4],
                        commonPositions[0],
                    };
                case 1:
                    return new Point[]
                    {
                        commonPositions[1],
                        commonPositions[5],
                    };
                case 2:
                    return new Point[]
                    {
                        commonPositions[0],
                        commonPositions[1],
                        commonPositions[3],
                        commonPositions[2],
                        commonPositions[4],
                        commonPositions[5],
                    };
                case 3:
                    return new Point[]
                    {
                        commonPositions[0],
                        commonPositions[1],
                        commonPositions[3],
                        commonPositions[2],
                        commonPositions[3],
                        commonPositions[5],
                        commonPositions[4],
                    };
                case 4:
                    return new Point[]
                    {
                        commonPositions[0],
                        commonPositions[2],
                        commonPositions[3],
                        commonPositions[1],
                        commonPositions[5],
                    };
                case 5:
                    return new Point[]
                    {
                        commonPositions[1],
                        commonPositions[0],
                        commonPositions[2],
                        commonPositions[3],
                        commonPositions[5],
                        commonPositions[4],
                    };
                case 6:
                    return new Point[]
                    {
                        commonPositions[1],
                        commonPositions[0],
                        commonPositions[4],
                        commonPositions[5],
                        commonPositions[3],
                        commonPositions[2],
                    };
                case 7:
                    return new Point[]
                    {
                        commonPositions[0],
                        commonPositions[1],
                        commonPositions[5],
                    };
                case 8:
                    return new Point[]
                    {
                        commonPositions[2],
                        commonPositions[3],
                        commonPositions[1],
                        commonPositions[0],
                        commonPositions[4],
                        commonPositions[5],
                        commonPositions[3],
                    };
                case 9:
                    return new Point[]
                    {
                        commonPositions[4],
                        commonPositions[5],
                        commonPositions[1],
                        commonPositions[0],
                        commonPositions[2],
                        commonPositions[3],
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(digit));
            }
        }

        private Point[] GetCommonPositions(Size targetSize)
        {
            return new Point[6]
            {
                new Point(0, 0),
                new Point(targetSize.Width, 0),
                new Point(0, targetSize.Height/2),
                new Point(targetSize.Width, targetSize.Height/2),
                new Point(0, targetSize.Height),
                new Point(targetSize.Width, targetSize.Height),
            };
        }

        public void Dispose()
        {
            ClearCache();
        }

        private void ClearCache()
        {
            _cachedDigits.Where(digit => digit != null).ToList().ForEach(digit => digit.Dispose());
        }
    }
}
