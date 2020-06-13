using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Plugins.Imaging.SkiaSharp
{
    public class SkiaResizerDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var overrideOptions = RegistrationOptions.Override();

            container
                .Register<IImageAssetFileService, SkiaSharpImageAssetFileService>(overrideOptions)
                .Register<IResizedImageAssetFileService, SkiaSharpResizedImageAssetFileService>(overrideOptions)
                .Register<ISkiaSharpResizeSettingsValidator, SkiaSharpResizeSettingsValidator>()
                .Register<IResizeSpecificationFactory, ResizeSpecificationFactory>()
                .Register<ISkiaSharpImageResizer, SkiaSharpImageResizer>()
                ;
        }
    }
}
