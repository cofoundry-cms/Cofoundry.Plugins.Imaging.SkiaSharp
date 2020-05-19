using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Plugins.Imaging.SkiaSharp
{
    public class ResizeSpecification
    {
        public bool UsesTransparency { get; set; }

        public SKColor? BackgroundColor { get; set; }

        public int CanvasWidth { get; set; }

        public int CanvasHeight { get; set; }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }

        public int ImagePaintWidth { get; set; }

        public int ImagePaintHeight { get; set; }

        public SKPoint AnchorAt { get; set; }

        public bool RequiresPadding()
        {
            return ImageHeight < CanvasHeight || ImageWidth < CanvasWidth;
        }
    }
}
