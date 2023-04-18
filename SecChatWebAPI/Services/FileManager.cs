namespace SecChatWebAPI.Services
{
    public class FileManager
    {
        private readonly IWebHostEnvironment _appEnvironment;

        public FileManager(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public async Task<string> AddFile(IFormFile file)
        {
            if (file != null)
            {
                string path = "/Files/" + file.FileName;
                using (var fileStream = new FileStream(_appEnvironment.ContentRootPath + path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Flush();
                }
                return path;
            }
            return null;
        }
    }
}
