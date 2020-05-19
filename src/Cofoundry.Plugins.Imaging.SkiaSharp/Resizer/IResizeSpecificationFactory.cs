using Cofoundry.Domain;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Plugins.Imaging.SkiaSharp
{
    public interface IResizeSpecificationFactory
    {
        ResizeSpecification Create(
            SKCodec sourceCodec,
            SKBitmap sourceImage,
            IImageResizeSettings resizeSettings
            );
    }
}
