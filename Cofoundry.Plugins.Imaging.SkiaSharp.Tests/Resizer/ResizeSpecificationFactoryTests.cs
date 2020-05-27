using Cofoundry.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Cofoundry.Plugins.Imaging.SkiaSharp.Tests
{
    public class ResizeSpecificationFactoryTests
    {
        public static List<object[]> AllImageParameters = new List<object[]>() { new object[] { TestImages.Landscape_2048x1375 }, new object[] { TestImages.Portrait_1536x2048 } };

        [Theory]
        [MemberData(nameof(AllImageParameters))]
        public void Create_WhenNotResized_DoesNotAlter(TestImage testImage)
        {
            var resizeSettings = new ImageResizeSettings();
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(testImage.Width, result.CanvasWidth);
            Assert.Equal(testImage.Width, result.UncroppedImageWidth);
            Assert.Equal(testImage.Width, result.VisibleImageWidth);
            Assert.Equal(testImage.Height, result.CanvasHeight);
            Assert.Equal(testImage.Height, result.UncroppedImageHeight);
            Assert.Equal(testImage.Height, result.VisibleImageHeight);
        }

        #region crop

        [Theory]
        [InlineData(ImageScaleMode.DownscaleOnly)]
        [InlineData(ImageScaleMode.UpscaleCanvas)]
        [InlineData(ImageScaleMode.Both)]
        public void Create_WhenCropWidthOnly_CropsCorrectly(ImageScaleMode imageScaleMode)
        {
            var resizeSettings = new ImageResizeSettings()
            {
                Mode = ImageFitMode.Crop,
                Scale = imageScaleMode,
                Width = 500
            };

            var testImage = TestImages.Landscape_2048x1375;
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(500, result.CanvasWidth);
            Assert.Equal(500, result.UncroppedImageWidth);
            Assert.Equal(500, result.VisibleImageWidth);
            Assert.Equal(336, result.CanvasHeight);
            Assert.Equal(336, result.UncroppedImageHeight);
            Assert.Equal(336, result.VisibleImageHeight);
        }

        [Theory]
        [InlineData(ImageScaleMode.DownscaleOnly)]
        [InlineData(ImageScaleMode.UpscaleCanvas)]
        [InlineData(ImageScaleMode.Both)]
        public void Create_WhenCropHeightOnly_CropsCorrectly(ImageScaleMode imageScaleMode)
        {
            var resizeSettings = new ImageResizeSettings()
            {
                Mode = ImageFitMode.Crop,
                Scale = imageScaleMode,
                Height = 500
            };

            var testImage = TestImages.Landscape_2048x1375;
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(745, result.CanvasWidth);
            Assert.Equal(745, result.UncroppedImageWidth);
            Assert.Equal(745, result.VisibleImageWidth);
            Assert.Equal(500, result.CanvasHeight);
            Assert.Equal(500, result.UncroppedImageHeight);
            Assert.Equal(500, result.VisibleImageHeight);
        }

        [Theory]
        [InlineData(ImageScaleMode.DownscaleOnly)]
        [InlineData(ImageScaleMode.UpscaleCanvas)]
        [InlineData(ImageScaleMode.Both)]
        public void Create_WhenCropWithShortWidth_WidthCutoff(ImageScaleMode imageScaleMode)
        {
            var resizeSettings = new ImageResizeSettings()
            {
                Mode = ImageFitMode.Crop,
                Scale = imageScaleMode,
                Width = 500,
                Height = 1000
            };

            var testImage = TestImages.Landscape_2048x1375;
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(500, result.CanvasWidth);
            Assert.Equal(1489, result.UncroppedImageWidth);
            Assert.Equal(500, result.VisibleImageWidth);
            Assert.Equal(1000, result.CanvasHeight);
            Assert.Equal(1000, result.UncroppedImageHeight);
            Assert.Equal(1000, result.VisibleImageHeight);
        }

        [Theory]
        [InlineData(ImageScaleMode.DownscaleOnly)]
        [InlineData(ImageScaleMode.UpscaleCanvas)]
        [InlineData(ImageScaleMode.Both)]
        public void Create_WhenCropWithShortHeight_HeightCutoff(ImageScaleMode imageScaleMode)
        {
            var resizeSettings = new ImageResizeSettings()
            {
                Mode = ImageFitMode.Crop,
                Scale = imageScaleMode,
                Width = 1000,
                Height = 500
            };

            var testImage = TestImages.Landscape_2048x1375;
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(1000, result.CanvasWidth);
            Assert.Equal(1000, result.UncroppedImageWidth);
            Assert.Equal(1000, result.VisibleImageWidth);
            Assert.Equal(500, result.CanvasHeight);
            Assert.Equal(671, result.UncroppedImageHeight);
            Assert.Equal(500, result.VisibleImageHeight);
        }

        #endregion

        #region pad

        [Fact]
        public void Create_WhenPadWidthOnly_DoesNotPad()
        {
            var resizeSettings = new ImageResizeSettings()
            {
                Mode = ImageFitMode.Pad,
                Width = 500
            };

            var testImage = TestImages.Landscape_2048x1375;
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(500, result.CanvasWidth);
            Assert.Equal(500, result.UncroppedImageWidth);
            Assert.Equal(500, result.VisibleImageWidth);
            Assert.Equal(336, result.CanvasHeight);
            Assert.Equal(336, result.UncroppedImageHeight);
            Assert.Equal(336, result.VisibleImageHeight);
        }

        [Fact]
        public void Create_WhenPadHeightOnly_DoesNotPad()
        {
            var resizeSettings = new ImageResizeSettings()
            {
                Mode = ImageFitMode.Pad,
                Height = 500
            };

            var testImage = TestImages.Landscape_2048x1375;
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(745, result.CanvasWidth);
            Assert.Equal(745, result.UncroppedImageWidth);
            Assert.Equal(745, result.VisibleImageWidth);
            Assert.Equal(500, result.CanvasHeight);
            Assert.Equal(500, result.UncroppedImageHeight);
            Assert.Equal(500, result.VisibleImageHeight);
        }

        [Fact]
        public void Create_WhenPadWithShortWidth_HeightPadded()
        {
            var resizeSettings = new ImageResizeSettings()
            {
                Mode = ImageFitMode.Pad,
                Width = 500,
                Height = 1000
            };

            var testImage = TestImages.Landscape_2048x1375;
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(500, result.CanvasWidth);
            Assert.Equal(500, result.UncroppedImageWidth);
            Assert.Equal(500, result.VisibleImageWidth);
            Assert.Equal(1000, result.CanvasHeight);
            Assert.Equal(336, result.UncroppedImageHeight);
            Assert.Equal(336, result.VisibleImageHeight);
        }

        [Fact]
        public void Create_WhenPadWithShortHeight_WidthPadded()
        {
            var resizeSettings = new ImageResizeSettings()
            {
                Mode = ImageFitMode.Pad,
                Width = 1000,
                Height = 500
            };

            var testImage = TestImages.Landscape_2048x1375;
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(1000, result.CanvasWidth);
            Assert.Equal(745, result.UncroppedImageWidth);
            Assert.Equal(745, result.VisibleImageWidth);
            Assert.Equal(500, result.CanvasHeight);
            Assert.Equal(500, result.UncroppedImageHeight);
            Assert.Equal(500, result.VisibleImageHeight);
        }

        #endregion

        #region max

        [Fact]
        public void Create_WhenMaxWithWidthOnly_Resizes()
        {
            var resizeSettings = new ImageResizeSettings()
            {
                Mode = ImageFitMode.Max,
                Width = 500
            };

            var testImage = TestImages.Landscape_2048x1375;
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(500, result.CanvasWidth);
            Assert.Equal(500, result.UncroppedImageWidth);
            Assert.Equal(500, result.VisibleImageWidth);
            Assert.Equal(336, result.CanvasHeight);
            Assert.Equal(336, result.UncroppedImageHeight);
            Assert.Equal(336, result.VisibleImageHeight);
        }

        [Fact]
        public void Create_WhenMaxWithHeightOnly_Resizes()
        {
            var resizeSettings = new ImageResizeSettings()
            {
                Mode = ImageFitMode.Max,
                Height = 500
            };

            var testImage = TestImages.Landscape_2048x1375;
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(745, result.CanvasWidth);
            Assert.Equal(745, result.UncroppedImageWidth);
            Assert.Equal(745, result.VisibleImageWidth);
            Assert.Equal(500, result.CanvasHeight);
            Assert.Equal(500, result.UncroppedImageHeight);
            Assert.Equal(500, result.VisibleImageHeight);
        }

        [Fact]
        public void Create_WhenMaxWithShortWidth_Resizes()
        {
            var resizeSettings = new ImageResizeSettings()
            {
                Mode = ImageFitMode.Max,
                Width = 500,
                Height = 1000
            };

            var testImage = TestImages.Landscape_2048x1375;
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(500, result.CanvasWidth);
            Assert.Equal(500, result.UncroppedImageWidth);
            Assert.Equal(500, result.VisibleImageWidth);
            Assert.Equal(336, result.CanvasHeight);
            Assert.Equal(336, result.UncroppedImageHeight);
            Assert.Equal(336, result.VisibleImageHeight);
        }

        [Fact]
        public void Create_WhenMaxWithShortHeight_Resizes()
        {
            var resizeSettings = new ImageResizeSettings()
            {
                Mode = ImageFitMode.Max,
                Width = 1000,
                Height = 500
            };

            var testImage = TestImages.Landscape_2048x1375;
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(745, result.CanvasWidth);
            Assert.Equal(745, result.UncroppedImageWidth);
            Assert.Equal(745, result.VisibleImageWidth);
            Assert.Equal(500, result.CanvasHeight);
            Assert.Equal(500, result.UncroppedImageHeight);
            Assert.Equal(500, result.VisibleImageHeight);
        }

        #endregion

        #region ImageScaleMode.DownscaleOnly
        
        [Theory]
        [InlineData(ImageFitMode.Crop)]
        [InlineData(ImageFitMode.Pad)]
        [InlineData(ImageFitMode.Max)]
        public void Create_WhenDownscaleOnlyAndWidthOnly_DoesNotUpscale(ImageFitMode fitMode)
        {
            var resizeSettings = new ImageResizeSettings()
            {
                Mode = fitMode,
                Scale = ImageScaleMode.DownscaleOnly,
                Width = 2500
            };

            var testImage = TestImages.Landscape_2048x1375;
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(2048, result.CanvasWidth);
            Assert.Equal(2048, result.UncroppedImageWidth);
            Assert.Equal(2048, result.VisibleImageWidth);
            Assert.Equal(1375, result.CanvasHeight);
            Assert.Equal(1375, result.UncroppedImageHeight);
            Assert.Equal(1375, result.VisibleImageHeight);
        }

        [Theory]
        [InlineData(ImageFitMode.Crop)]
        [InlineData(ImageFitMode.Pad)]
        [InlineData(ImageFitMode.Max)]
        public void Create_WhenDownscaleOnlyAndHeightOnly_DoesNotUpscale(ImageFitMode fitMode)
        {
            var resizeSettings = new ImageResizeSettings()
            {
                Mode = fitMode,
                Scale = ImageScaleMode.DownscaleOnly,
                Height = 2500
            };

            var testImage = TestImages.Landscape_2048x1375;
            var result = CreateSpecification(testImage, resizeSettings);

            Assert.Equal(2048, result.CanvasWidth);
            Assert.Equal(2048, result.UncroppedImageWidth);
            Assert.Equal(2048, result.VisibleImageWidth);
            Assert.Equal(1375, result.CanvasHeight);
            Assert.Equal(1375, result.UncroppedImageHeight);
            Assert.Equal(1375, result.VisibleImageHeight);
        }

        #endregion

        // + ImageScaleMode.Both, ImageScaleMode.DownscaleOnly, ImageScaleMode.UpscaleCanvas, ImageScaleMode.UpscaleOnly

        private ResizeSpecification CreateSpecification(TestImage image, ImageResizeSettings imageResizeSettings)
        {
            var factory = new ResizeSpecificationFactory();

            using (var imageFile = image.Load())
            {
                return factory.Create(imageFile.Codec, imageFile.Bitmap, imageResizeSettings);
            }
        }
    }
}
