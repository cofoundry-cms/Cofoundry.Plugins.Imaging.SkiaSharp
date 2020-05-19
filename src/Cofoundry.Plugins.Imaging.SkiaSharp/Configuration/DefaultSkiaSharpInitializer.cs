using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Plugins.Imaging.SkiaSharp
{
    public class DefaultSkiaSharpInitializer : ISkiaSharpInitializer
    {
        private readonly SkiaSharpSettings _imageSharpSettings;

        public DefaultSkiaSharpInitializer(
            SkiaSharpSettings imageSharpSettings
            )
        {
            _imageSharpSettings = imageSharpSettings;
        }

        //public void Initialize(Configuration configuration)
        //{
        //    configuration.ImageFormatsManager.SetDecoder(JpegFormat.Instance, new JpegDecoder()
        //    {
        //        IgnoreMetadata = _imageSharpSettings.IgnoreMetadata
        //    });

        //    configuration.ImageFormatsManager.SetEncoder(JpegFormat.Instance, new JpegEncoder()
        //    {
        //        Quality = _imageSharpSettings.JpegQuality
        //    });

        //    configuration.ImageFormatsManager.SetDecoder(GifFormat.Instance, new GifDecoder()
        //    {
        //        IgnoreMetadata = _imageSharpSettings.IgnoreMetadata
        //    });

        //    configuration.ImageFormatsManager.SetDecoder(PngFormat.Instance, new PngDecoder()
        //    {
        //        IgnoreMetadata = _imageSharpSettings.IgnoreMetadata
        //    });
        //}
    }
}
