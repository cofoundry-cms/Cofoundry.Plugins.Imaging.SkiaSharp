using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Cofoundry.Plugins.Imaging.SkiaSharp.Tests
{
    public static class EmbeddedResourceImageFileLoader
    {
        private static readonly Assembly _assembly = typeof(EmbeddedResourceImageFileLoader).Assembly;

        public static ImageFileSource Load(string filename)
        {
            var path = _assembly.GetName().Name + ".TestImages." + filename;
            var fileStream = _assembly.GetManifestResourceStream(path);

            return ImageFileSource.Load(fileStream);
        }
    }
}
