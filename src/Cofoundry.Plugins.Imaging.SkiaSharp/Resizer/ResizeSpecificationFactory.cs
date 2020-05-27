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
                spec.VisibleImageWidth = sourceImage.Width;
                spec.VisibleImageHeight = sourceImage.Height;
                spec.UncroppedImageWidth = sourceImage.Width;
                spec.UncroppedImageHeight = sourceImage.Height;
                spec.Origin = sourceCodec.EncodedOrigin;

                return spec;
            }

            switch (resizeSettings.Mode)
            {
                case ImageFitMode.Crop:
                case ImageFitMode.Default:
                    SetCrop(spec, sourceImage, resizeSettings);
                    break;
                case ImageFitMode.Max:
                    SetMax(spec, sourceImage, resizeSettings);
                    break;
                case ImageFitMode.Pad:
                    SetPad(spec, sourceImage, resizeSettings);
                    break;
                default:
                    throw new NotSupportedException($"ImagefitMode {resizeSettings.Mode} not supported.");
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

        /// <summary>
        /// Width and height are exact values - cropping is used if there is an
        /// aspect ratio difference.
        /// </summary>
        private void SetCrop(
            ResizeSpecification spec,
            SKBitmap sourceImage,
            IImageResizeSettings resizeSettings
            )
        {

            if (!resizeSettings.IsWidthSet() || resizeSettings.Height > resizeSettings.Width)
            {
                // Scale to height
                spec.CanvasHeight = spec.VisibleImageHeight = spec.UncroppedImageHeight = resizeSettings.Height;

                var heightRatio = GetResizeRatio(resizeSettings, resizeSettings.Height, sourceImage.Height);
                var scaledWidth = RoundPixels(sourceImage.Width * heightRatio);

                spec.CanvasWidth = spec.VisibleImageWidth = resizeSettings.IsWidthSet() ? resizeSettings.Width : scaledWidth;
                spec.UncroppedImageWidth = scaledWidth;
            }
            else
            {
                // Scale to width 
                spec.CanvasWidth = spec.VisibleImageWidth = spec.UncroppedImageWidth = resizeSettings.Width;

                var widthRatio = GetResizeRatio(resizeSettings, resizeSettings.Width, sourceImage.Width);
                var scaledHeight = RoundPixels(sourceImage.Height * widthRatio);

                spec.CanvasHeight = spec.VisibleImageHeight = resizeSettings.IsHeightSet() ? resizeSettings.Height : scaledHeight;
                spec.UncroppedImageHeight = scaledHeight;
            }
        }

        /// <summary>
        /// Width and height are considered maximum values. The resulting image may be smaller
        //  to maintain its aspect ratio.
        /// </summary>
        private void SetMax(
            ResizeSpecification spec,
            SKBitmap sourceImage,
            IImageResizeSettings resizeSettings
            )
        {
            if (!resizeSettings.IsWidthSet() || (resizeSettings.IsHeightSet() && resizeSettings.Height < resizeSettings.Width))
            {
                // Scale to height
                spec.CanvasHeight = spec.VisibleImageHeight = spec.UncroppedImageHeight = resizeSettings.Height;

                var heightRatio = GetResizeRatio(resizeSettings, resizeSettings.Height, sourceImage.Height);
                var scaledWidth = RoundPixels(sourceImage.Width * heightRatio);

                spec.CanvasWidth = spec.UncroppedImageWidth = spec.VisibleImageWidth = scaledWidth;
            }
            else
            {
                // Scale to width 
                spec.CanvasWidth = spec.VisibleImageWidth = spec.UncroppedImageWidth = resizeSettings.Width;

                var widthRatio = GetResizeRatio(resizeSettings, resizeSettings.Width, sourceImage.Width);
                var scaledHeight = RoundPixels(sourceImage.Height * widthRatio);

                spec.CanvasHeight = spec.UncroppedImageHeight = spec.VisibleImageHeight = scaledHeight;
            }
        }

        /// <summary>
        /// Width and height are considered exact values - padding is used if there is an
        /// aspect ratio difference.
        /// </summary>
        private void SetPad(
            ResizeSpecification spec,
            SKBitmap sourceImage,
            IImageResizeSettings resizeSettings
            )
        {
            if (!resizeSettings.IsWidthSet() || (resizeSettings.IsHeightSet() && resizeSettings.Height < resizeSettings.Width))
            {
                // Scale to height
                spec.CanvasHeight = spec.VisibleImageHeight = spec.UncroppedImageHeight = resizeSettings.Height;

                var heightRatio = GetResizeRatio(resizeSettings, resizeSettings.Height, sourceImage.Height);
                var scaledWidth = RoundPixels(sourceImage.Width * heightRatio);

                spec.CanvasWidth = resizeSettings.IsWidthSet() ? resizeSettings.Width : scaledWidth;
                spec.UncroppedImageWidth = spec.VisibleImageWidth = scaledWidth;
            }
            else
            {
                // Scale to width 
                spec.CanvasWidth = spec.VisibleImageWidth = spec.UncroppedImageWidth = resizeSettings.Width;

                var widthRatio = GetResizeRatio(resizeSettings, resizeSettings.Width, sourceImage.Width);
                var scaledHeight = RoundPixels(sourceImage.Height * widthRatio);

                spec.CanvasHeight = resizeSettings.IsHeightSet() ? resizeSettings.Height : scaledHeight;
                spec.UncroppedImageHeight = spec.VisibleImageHeight = scaledHeight;
            }
        }

        private int RoundPixels(float pixels)
        {
            return Convert.ToInt32(Math.Round(pixels, MidpointRounding.AwayFromZero));
        }

        private float GetResizeRatio(IImageResizeSettings resizeSettings, int resizeValue, int sourceValue)
        {
            var ratio = (float)resizeValue / (float)sourceValue;

            if ((ratio > 1 && resizeSettings.Scale == ImageScaleMode.DownscaleOnly)
                || (ratio < 1 && resizeSettings.Scale == ImageScaleMode.UpscaleOnly))
            {
                ratio = 1;
            }

            return ratio;
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
