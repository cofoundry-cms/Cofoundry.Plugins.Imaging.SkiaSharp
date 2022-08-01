using Cofoundry.Core.Data;
using Cofoundry.Core.Validation;
using Cofoundry.Domain;
using Cofoundry.Domain.Data;
using SkiaSharp;

namespace Cofoundry.Plugins.Imaging.SkiaSharp;

public class SkiaSharpImageAssetFileService : IImageAssetFileService
{
    private const string ASSET_FILE_CONTAINER_NAME = "Images";

    private static readonly Dictionary<SKEncodedImageFormat, string> _permittedFormats = new Dictionary<SKEncodedImageFormat, string>() {
        { SKEncodedImageFormat.Gif, "gif" },
        { SKEncodedImageFormat.Jpeg, "jpg" },
        { SKEncodedImageFormat.Png, "png" },
        { SKEncodedImageFormat.Webp, "webp" }
    };

    private readonly CofoundryDbContext _dbContext;
    private readonly IFileStoreService _fileStoreService;
    private readonly ITransactionScopeManager _transactionScopeManager;
    private readonly ImageAssetsSettings _imageAssetsSettings;
    private readonly SkiaSharpSettings _skiaSharpSettings;

    public SkiaSharpImageAssetFileService(
        CofoundryDbContext dbContext,
        IFileStoreService fileStoreService,
        ITransactionScopeManager transactionScopeManager,
        ImageAssetsSettings imageAssetsSettings,
        SkiaSharpSettings skiaSharpSettings
        )
    {
        _dbContext = dbContext;
        _fileStoreService = fileStoreService;
        _transactionScopeManager = transactionScopeManager;
        _imageAssetsSettings = imageAssetsSettings;
        _skiaSharpSettings = skiaSharpSettings;
    }

    public async Task SaveAsync(
        IFileSource uploadedFile,
        ImageAsset imageAsset,
        string propertyName
        )
    {
        using (var fileSource = ImageFileSource.Load(await uploadedFile.OpenReadStreamAsync()))
        {
            ValidateCodec(fileSource.Codec, uploadedFile, propertyName);
            ValidateImage(fileSource.Bitmap, propertyName);

            DetermineOutputFormat(fileSource.Codec, out SKEncodedImageFormat outputFormat, out bool requiresReEncoding);

            imageAsset.WidthInPixels = fileSource.Bitmap.Width;
            imageAsset.HeightInPixels = fileSource.Bitmap.Height;
            imageAsset.FileExtension = _permittedFormats[outputFormat];
            imageAsset.FileSizeInBytes = fileSource.OriginalStream.Length;

            using (var scope = _transactionScopeManager.Create(_dbContext))
            {
                var fileName = Path.ChangeExtension(imageAsset.FileNameOnDisk, imageAsset.FileExtension);

                if (requiresReEncoding)
                {
                    // Convert the image (Quality setting seems to be ignored for pngs)
                    using (var data = fileSource.Bitmap.Encode(outputFormat, _skiaSharpSettings.JpegQuality))
                    using (var outputStream = data.AsStream())
                    {
                        await _fileStoreService.CreateAsync(ASSET_FILE_CONTAINER_NAME, fileName, outputStream);

                        // recalculate size and save
                        imageAsset.FileSizeInBytes = outputStream.Length;
                    }
                }
                else
                {
                    // Save the raw file directly
                    fileSource.OriginalStream.Position = 0;
                    await _fileStoreService.CreateAsync(ASSET_FILE_CONTAINER_NAME, fileName, fileSource.OriginalStream);
                }

                await _dbContext.SaveChangesAsync();
                await scope.CompleteAsync();
            };
        }
    }

    private void DetermineOutputFormat(SKCodec codec, out SKEncodedImageFormat imageFormat, out bool requiresReEncoding)
    {
        imageFormat = codec.EncodedFormat;
        requiresReEncoding = !_permittedFormats.ContainsKey(codec.EncodedFormat);
        if (!_permittedFormats.ContainsKey(codec.EncodedFormat))
        {
            imageFormat = SKEncodedImageFormat.Jpeg;
        }

        if (imageFormat == SKEncodedImageFormat.Gif)
        {
            if ((_skiaSharpSettings.GifResizeBehaviour == GifResizeBehaviour.Auto && codec.FrameCount < 2)
                || _skiaSharpSettings.GifResizeBehaviour == GifResizeBehaviour.ResizeAsAlternative)
            {
                imageFormat = SKEncodedImageFormat.Png;
                requiresReEncoding = true;
            }
        }
    }

    private void ValidateCodec(
        SKCodec codec,
        IFileSource uploadedFile,
        string propertyName
        )
    {
        if (codec == null)
        {
            var fileExtension = Path.GetExtension(uploadedFile.FileName);

            if ((!string.IsNullOrEmpty(fileExtension) && !ImageAssetConstants.PermittedImageTypes.ContainsKey(fileExtension))
                || (!string.IsNullOrEmpty(uploadedFile.MimeType) && !ImageAssetConstants.PermittedImageTypes.ContainsValue(uploadedFile.MimeType)))
            {
                throw ValidationErrorException.CreateWithProperties("The file is not a supported image type.", propertyName);
            }

            throw new Exception("SkiaSharp does not recognise the file as an image type.");
        }

        ValidateDimension(codec.Info.Width, _imageAssetsSettings.MaxUploadWidth, "width", propertyName);
        ValidateDimension(codec.Info.Height, _imageAssetsSettings.MaxUploadHeight, "height", propertyName);
    }

    private void ValidateImage(
        SKBitmap bitmap,
        string propertyName
        )
    {
        if (bitmap == null)
        {
            throw new ArgumentNullException(nameof(bitmap));
        }

        // bitmap.IsEmpty
        ValidateDimension(bitmap.Width, _imageAssetsSettings.MaxUploadWidth, "width", propertyName);
        ValidateDimension(bitmap.Height, _imageAssetsSettings.MaxUploadHeight, "height", propertyName);
    }

    public void ValidateDimension(int amount, int maximum, string dimensionName, string propertyName)
    {
        if (amount > maximum)
        {
            throw ValidationErrorException.CreateWithProperties($"Image exceeds the {dimensionName} permitted width of {maximum} pixels.", propertyName);
        }
    }
}