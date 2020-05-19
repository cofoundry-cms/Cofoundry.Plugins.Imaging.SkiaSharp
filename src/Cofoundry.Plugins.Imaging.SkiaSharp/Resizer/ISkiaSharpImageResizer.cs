using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cofoundry.Plugins.Imaging.SkiaSharp
{
    public interface ISkiaSharpImageResizer
    {
        SKImage Resize(SKBitmap sourceImage, ResizeSpecification resizeSpecification);
    }
}
