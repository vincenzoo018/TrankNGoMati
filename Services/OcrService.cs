using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Tesseract;

namespace TrackNGoMati.Services
{
    public class OcrService
    {
        private readonly IWebHostEnvironment _env;

        public OcrService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string ExtractTextFromImage(string filePath)
        {
            if (!File.Exists(filePath)) return string.Empty;
            return PerformOcr(filePath);
        }

        public string ExtractTextFromImage(Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0) return string.Empty;

            var tempFile = Path.GetTempFileName() + Path.GetExtension(file.FileName);
            using (var stream = new FileStream(tempFile, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var text = PerformOcr(tempFile);
            File.Delete(tempFile);
            return text;
        }

        private string PerformOcr(string imagePath)
        {
            var tessDataPath = Path.Combine(_env.WebRootPath, "tessdata");
            
            try
            {
                using var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default);
                using var img = Pix.LoadFromFile(imagePath);
                using var page = engine.Process(img);
                
                return page.GetText()?.Trim() ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OCR Error: {ex.Message}");
                return string.Empty;
            }
        }

        public object ParseDocumentFields(string rawText)
        {
            // Dummy parser for demonstration. Extract keywords based on common form layouts.
            return new
            {
                title = rawText.Split('\n').FirstOrDefault() ?? "",
                sender = "Extracted Sender Name",
                date = DateTime.Now.ToString("yyyy-MM-dd")
            };
        }
    }
}
