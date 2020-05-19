using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Plugins.Imaging.SkiaSharp
{
    public class TempDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var overrideOptions = RegistrationOptions.Override();

            container
                .Register<IImageResizeSettingsValidator, ImageResizeSettingsValidator>()
                ;
        }
    }
}
