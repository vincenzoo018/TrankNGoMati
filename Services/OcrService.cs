// ============================================================
//  TrackNGo Mati — OCR Service using Tesseract.NET
//  Reads image or PDF-converted images and extracts text
// ============================================================
using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace TrackNGoMati.Services
{
    public class OcrService
    {
        private readonly string _tessDataPath;

        public OcrService(string tessDataPath)
        {
            _tessDataPath = tessDataPath;
        }

        /// <summary>
        /// Extracts text from an uploaded image file (JPG/PNG/BMP/TIFF).
        /// Returns extracted text or throws an exception on failure.
        /// </summary>
        public string ExtractTextFromImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("No file provided.");

            // Save to temp path
            var tmpPath = Path.Combine(Path.GetTempPath(), $"ocr_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}");
            try
            {
                using (var fs = new FileStream(tmpPath, FileMode.Create))
                {
                    imageFile.CopyTo(fs);
                }

                return ExtractTextFromFile(tmpPath);
            }
            finally
            {
                if (File.Exists(tmpPath)) File.Delete(tmpPath);
            }
        }

        /// <summary>
        /// Extracts text from a file path.
        /// </summary>
        public string ExtractTextFromFile(string filePath)
        {
            // Check if Tesseract data directory exists; if not, return a graceful message
            if (!Directory.Exists(_tessDataPath))
            {
                return "[OCR Engine not configured. Please install tessdata. Showing preview with file name only.]";
            }

            try
            {
                using var engine = new Tesseract.TesseractEngine(_tessDataPath, "eng", Tesseract.EngineMode.Default);
                using var img    = Tesseract.Pix.LoadFromFile(filePath);
                using var page   = engine.Process(img);
                return page.GetText().Trim();
            }
            catch (Exception ex)
            {
                return $"[OCR extraction failed: {ex.Message}]";
            }
        }

        /// <summary>
        /// Parses OCR text and attempts to extract common document metadata.
        /// Returns a dictionary of field-to-extracted-value pairs.
        /// </summary>
        public Dictionary<string, string> ParseDocumentFields(string rawText)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Very basic heuristic parsing — looks for common patterns
            var lines = rawText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("To:", StringComparison.OrdinalIgnoreCase))
                    result["Recipient"] = trimmed[3..].Trim();
                else if (trimmed.StartsWith("From:", StringComparison.OrdinalIgnoreCase))
                    result["Sender"] = trimmed[5..].Trim();
                else if (trimmed.StartsWith("Subject:", StringComparison.OrdinalIgnoreCase))
                    result["Subject"] = trimmed[8..].Trim();
                else if (trimmed.StartsWith("Date:", StringComparison.OrdinalIgnoreCase))
                    result["Date"] = trimmed[5..].Trim();
                else if (trimmed.StartsWith("Re:", StringComparison.OrdinalIgnoreCase))
                    result["Reference"] = trimmed[3..].Trim();
            }

            // If no structured field was found, take the first non-empty line as the subject
            if (!result.ContainsKey("Subject") && lines.Length > 0)
                result["Subject"] = lines[0].Trim();

            return result;
        }
    }
}
