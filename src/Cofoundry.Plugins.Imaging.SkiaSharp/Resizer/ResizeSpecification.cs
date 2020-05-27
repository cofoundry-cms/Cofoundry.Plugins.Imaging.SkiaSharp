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

        /// <summary>
        /// The width of the resized canvas to paint the image on to.
        /// </summary>
        public int CanvasWidth { get; set; }

        /// <summary>
        /// The height of the resized canvas to paint the image on to.
        /// </summary>
        public int CanvasHeight { get; set; }

        /// <summary>
        /// The width of the resized but uncropped image, which may extend beyond the canvas.
        /// </summary>
        public int UncroppedImageWidth { get; set; }

        /// <summary>
        /// The height of the resized but uncropped image, which may extend beyond the canvas.
        /// </summary>
        public int UncroppedImageHeight { get; set; }

        /// <summary>
        /// The visible width of the resized and cropped image. This exludes any cropped
        /// width that extends beyond the canvas.
        /// </summary>
        public int VisibleImageWidth { get; set; }

        /// <summary>
        /// The visible height of the resized and cropped image. This exludes any cropped
        /// height that extends beyond the canvas.
        /// </summary>
        public int VisibleImageHeight { get; set; }

        public SKPoint AnchorAt { get; set; }

        /// <summary>
        /// The EXIF rotation value that is used to auto-rotate the image.
        /// </summary>
        public SKEncodedOrigin Origin { get; internal set; }

        public bool RequiresPadding()
        {
            return UncroppedImageHeight < CanvasHeight || UncroppedImageWidth < CanvasWidth;
        }
    }
}
