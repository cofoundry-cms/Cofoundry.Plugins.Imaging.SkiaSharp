using Cofoundry.Domain;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Plugins.Imaging.SkiaSharp
{
    public class ResizeSpecificationFactory : IResizeSpecificationFactory
    {
        public ResizeSpecification Create(
            SKCodec sourceCodec,
            SKBitmap sourceImage,
            IImageResizeSettings resizeSettings
            )
        {
            var spec = new ResizeSpecification();
            // Currently it's not that advantageous to know if there is actually any transparency 
            // used in the image, as the encoder isn't configurable, so let's not bother checking in detail.
            spec.UsesTransparency = SupportsTransparency(sourceCodec);

            // Is resizing required
            if (!resizeSettings.IsWidthSet() && !resizeSettings.IsHeightSet())
            {
                spec.CanvasWidth = sourceImage.Width;
                spec.CanvasHeight = sourceImage.Height;
                spec.AnchorAt = SKPoint.Empty;
                spec.ImagePaintWidth = sourceImage.Width;
                spec.ImagePaintHeight = sourceImage.Height;
                spec.ImageWidth = sourceImage.Width;
                spec.ImageHeight = sourceImage.Height;

                return spec;
            }

            // TODO: implement this properly and write some tests!
            if (!resizeSettings.IsWidthSet() || resizeSettings.Height > resizeSettings.Width)
            {
                // Scale to height
                spec.CanvasHeight = spec.ImagePaintHeight = spec.ImageHeight = resizeSettings.Height;

                var heightRatio = (float)resizeSettings.Height / (float)sourceImage.Height;
                var scaledWidth = Convert.ToInt32(Math.Ceiling(sourceImage.Width * heightRatio));
                spec.CanvasWidth = spec.ImagePaintWidth = spec.ImageWidth = scaledWidth;

                //if (resizeSettings.IsWidthSet())
                //{
                //    switch (resizeSettings.Mode)
                //    {
                //        case ImageFitMode.Crop:
                //            break;
                //    }
                //}
                //else
                //{
                //    // No width specified, so just use scaled height
                //    spec.CanvasWidth = spec.ImagePaintWidth = spec.ImageWidth = scaledHeight;
                //}
            }
            else
            {
                // Scale to width 
                spec.CanvasWidth = spec.ImagePaintWidth = spec.ImageWidth = resizeSettings.Width;

                var widthRatio = (float)resizeSettings.Width / (float)sourceImage.Width;
                var scaledHeight = Convert.ToInt32(Math.Ceiling(sourceImage.Height * widthRatio));
                spec.CanvasHeight = spec.ImagePaintHeight = spec.ImageHeight = scaledHeight;
            }

            SetBackgroundColor(spec, sourceCodec, resizeSettings);

            return spec;
        }

        private static void SetBackgroundColor(
            ResizeSpecification spec,
            SKCodec sourceCodec,
            IImageResizeSettings resizeSettings
            )
        {
            if (!string.IsNullOrWhiteSpace(resizeSettings.BackgroundColor))
            {
                if (SKColor.TryParse(resizeSettings.BackgroundColor, out SKColor color))
                {
                    spec.BackgroundColor = color;
                    return;
                }
            }

            if (spec.UsesTransparency || SupportsTransparency(sourceCodec))
            {
                spec.UsesTransparency = true;
                spec.BackgroundColor = SKColor.Empty.WithAlpha(0);
            }
            else
            {
                // Default to white background
                spec.BackgroundColor = new SKColor(255, 255, 255);
            }
        }

        private ImageFitMode GetFitMode(IImageResizeSettings resizeSettings)
        {
            if (resizeSettings.Mode == ImageFitMode.Default)
            {
                return ImageFitMode.Crop;
            }

            return resizeSettings.Mode;
        }

        private static bool SupportsTransparency(SKCodec sourceCodec)
        {
            switch (sourceCodec.EncodedFormat)
            {
                case SKEncodedImageFormat.Gif:
                case SKEncodedImageFormat.Png:
                case SKEncodedImageFormat.Webp:
                    return true;
                default:
                    return false;
            }
        }
    }
}
