using Cofoundry.Web;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Plugins.Imaging.SkiaSharp
{
    public class SkiaSharpConfigurationStartupTask : IStartupConfigurationTask
    {
        private readonly ISkiaSharpInitializer _imageSharpConfiguration;

        public SkiaSharpConfigurationStartupTask(
            ISkiaSharpInitializer imageSharpConfiguration
            )
        {
            _imageSharpConfiguration = imageSharpConfiguration;
        }

        public int Ordering => (int)StartupTaskOrdering.Early;

        public void Configure(IApplicationBuilder app)
        {
            // Setup some initial config
            //var imgConfig = SixLabors.ImageSharp.Configuration.Default;

            //_imageSharpConfiguration.Initialize(imgConfig);
        }
    }
}
