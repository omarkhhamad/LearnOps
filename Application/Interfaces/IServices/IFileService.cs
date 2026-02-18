using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);
        void DeleteFile(string fileName, string folderName);
    }
}
