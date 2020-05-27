using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cofoundry.Plugins.Imaging.SkiaSharp.Tests
{
    public class TestImage
    {
        public string FileName { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public ImageFileSource Load()
        {
            return EmbeddedResourceImageFileLoader.Load(FileName);
        }
    }
}
