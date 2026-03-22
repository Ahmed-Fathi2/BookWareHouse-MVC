using BookWarehouse.Application.Comman;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.Abstractions
{
    public interface IFileService
    {
        Task<Result<string>> Upload(string webRootPath, string fileName, Stream imageStream);
        Result Delete(string webRootPath, string relativePath);
    }
}
