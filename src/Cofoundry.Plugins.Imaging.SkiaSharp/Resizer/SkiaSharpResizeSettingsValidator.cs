using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Plugins.Imaging.SkiaSharp
{
    /// <summary>
    /// Used to validate the resize settings sent from the client prior
    /// to resizing the image.
    /// </summary>
    public class SkiaSharpResizeSettingsValidator : ISkiaSharpResizeSettingsValidator
    {
        private readonly ImageAssetsSettings _imageAssetsSettings;
        private readonly IImageResizeSettingsValidator _imageResizeSettingsValidator;

        public SkiaSharpResizeSettingsValidator(
            ImageAssetsSettings imageAssetsSettings,
            IImageResizeSettingsValidator imageResizeSettingsValidator
            )
        {
            _imageAssetsSettings = imageAssetsSettings;
            _imageResizeSettingsValidator = imageResizeSettingsValidator;
        }

        /// <summary>
        /// Validates that the image asset can be resized using the settings
        /// supplied by the client.
        /// </summary>
        /// <param name="settings">Settings to validate.</param>
        /// <param name="asset">Asset attempting to be resized. Cannot be null.</param>
        public void Validate(IImageResizeSettings settings, IImageAssetRenderable asset)
        {
            _imageResizeSettingsValidator.Validate(settings, asset);

            // Scale mode not supported yet
            settings.Scale = ImageScaleMode.Both;
        }
    }
}
