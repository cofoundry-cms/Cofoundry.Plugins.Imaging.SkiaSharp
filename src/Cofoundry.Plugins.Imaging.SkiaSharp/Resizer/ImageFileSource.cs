using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cofoundry.Plugins.Imaging.SkiaSharp
{
    public class ImageFileSource : IDisposable
    {
        public Stream OriginalStream { get; private set; }

        public SKCodec Codec { get; private set; }

        public SKBitmap Bitmap { get; private set; }

        public bool IsLoaded { get; private set; }

        public static ImageFileSource Load(Stream stream)
        {
            var source = new ImageFileSource();
            source.OriginalStream = stream;

            using (var managedStream = new SKManagedStream(stream))
            {
                source.Codec = SKCodec.Create(managedStream);

                if (source.Codec != null)
                {
                    source.Bitmap = SKBitmap.Decode(source.Codec);
                    source.IsLoaded = true;
                }
            }

            return source;
        }

        public void Dispose()
        {
            Bitmap?.Dispose();
            Codec?.Dispose();
            OriginalStream?.Dispose();
        }
    }
}
