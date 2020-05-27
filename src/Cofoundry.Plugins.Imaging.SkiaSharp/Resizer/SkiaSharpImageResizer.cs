﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cofoundry.Plugins.Imaging.SkiaSharp
{
    public class SkiaSharpImageResizer : ISkiaSharpImageResizer
    {
        public SKImage Resize(SKBitmap sourceImage, ResizeSpecification resizeSpecification)
        {
            var canvasSpecification = sourceImage.Info.WithSize(resizeSpecification.CanvasWidth, resizeSpecification.CanvasHeight);

            using (var surface = SKSurface.Create(canvasSpecification))
            {
                var canvas = surface.Canvas;

                if ((resizeSpecification.RequiresPadding() || resizeSpecification.UsesTransparency) 
                    && resizeSpecification.BackgroundColor.HasValue)
                {
                    canvas.Clear(resizeSpecification.BackgroundColor.Value);
                }

                var newSize = new SKSizeI(resizeSpecification.UncroppedImageWidth, resizeSpecification.UncroppedImageHeight);
                using (var resizedBitmap = sourceImage.Resize(newSize, SKFilterQuality.High))
                using (var resizedImage = SKImage.FromBitmap(resizedBitmap))
                {
                    surface.Canvas.DrawImage(resizedImage, resizeSpecification.AnchorAt);
                }
                surface.Canvas.Flush();

                return surface.Snapshot();
            }
        }
    }
}