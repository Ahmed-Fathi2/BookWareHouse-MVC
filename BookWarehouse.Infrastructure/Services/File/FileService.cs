using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.Comman.Settings;
using Microsoft.Extensions.Options;

namespace BookWarehouse.Infrastructure.Services.File
{
    public class FileService : IFileService
    {
        private readonly UploadImageSetting _options;

        public FileService(IOptions<UploadImageSetting> options)
        {
            _options = options.Value;
        }
        public Result Delete(string webRootPath, string relativePath)
        {
            var filePath = Path.Combine(webRootPath, relativePath);

            if (!System.IO.File.Exists(filePath))
            {
                return Result.Failure(new Error("File not found.", "File NotFound"));
            }

            System.IO.File.Delete(filePath);
            return Result.Success();
           

        }

        public async Task<Result<string>> Upload(string webRootPath, string fileName, Stream imageStream)
        {
            //1- Create Folder Path
            var folderPath = Path.Combine(webRootPath, _options.ImageFolderPath);

            //2- Check if folder exist or not
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);


            //Create unique file name to avoid conflicts
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";


            //3- Create file path
            var filePath = Path.Combine(folderPath, uniqueFileName);

            //4- Save the file
            await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            //5- Copy the content of the uploaded file to the new file
            await imageStream.CopyToAsync(stream);

            //6- Return the relative path to be stored in the database
            var relativePath = Path.Combine(_options.ImageFolderPath, uniqueFileName).Replace("\\", "/");

            return Result.Success(relativePath);
        }
    }
}

// In Mvc returning  like this: /images/products/uniqueFileName.jpg
// In Api returning  like this: https://localhost:5001/images/products/uniqueFileName.jpg
