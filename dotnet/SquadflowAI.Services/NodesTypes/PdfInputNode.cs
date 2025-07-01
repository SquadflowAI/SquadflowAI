using SquadflowAI.Contracts;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Services.NodesTypes.Base;
using System.Diagnostics;
using System.Text;
using UglyToad.PdfPig;

namespace SquadflowAI.Services.NodesTypes
{
    public class PdfInputNode : INode
    {
        public string Id { get; private set; }

        public void Initialize(string id, IDictionary<string, string> parameters)
        {
            Id = id;
        }

        public async Task<ExecutionInputOutputDto> ExecuteAsync(ExecutionInputOutputDto input, IDictionary<string, string> parameters, UIFlowDto uIFlow, IDictionary<string, byte[]> parametersByte)
        {
            var output = new ExecutionInputOutputDto();

            byte[] pdfBytes = null;
            if (input.ByteInputs != null && input.ByteInputs.Any())
            {
                pdfBytes = input.ByteInputs.First().ByteInput; // only process one. If more use loop
            } else if (parametersByte["pdf"] != null)
            {
                pdfBytes = parametersByte["pdf"];
            }
               
            string tempPdfPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
            await File.WriteAllBytesAsync(tempPdfPath, pdfBytes);

            try
            {
                // Try to extract text with PdfPig
                string text = TryPdfPig(tempPdfPath);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    Console.WriteLine("✅ Text extracted with PdfPig");
                    
                    output.Input = text;
                    return output;
                }

                Console.WriteLine("⚠️ PdfPig failed or returned empty. Falling back to OCR...");

                output.Input = await ExtractTextWithOcrFallbackAsync(tempPdfPath);
                return output;
            }
            finally
            {
                File.Delete(tempPdfPath);
            }
        }

        private string TryPdfPig(string path)
        {
            try
            {
                var sb = new StringBuilder();
                using (var document = PdfDocument.Open(path))
                {
                    foreach (var page in document.GetPages())
                    {
                        sb.AppendLine(page.Text);
                    }
                }
                return sb.ToString().Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PdfPig failed: {ex.Message}");
                return null;
            }
        }

        private async Task<string> ExtractTextWithOcrFallbackAsync(string pdfPath)
        {
            var images = ConvertPdfToImages(pdfPath);
            var sb = new StringBuilder();

            foreach (var imagePath in images)
            {
                string text = await RunTesseractAsync(imagePath);
                sb.AppendLine(text);
                File.Delete(imagePath);
            }

            return sb.ToString().Trim();
        }

        private List<string> ConvertPdfToImages(string pdfPath)
        {
            string outputPrefix = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            string args = $"-png \"{pdfPath}\" \"{outputPrefix}\"";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "pdftoppm",
                    Arguments = args,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            string pattern = Path.GetFileName(outputPrefix) + "-*.png";
            return Directory.GetFiles(Path.GetTempPath(), pattern).ToList();
        }

        private async Task<string> RunTesseractAsync(string imagePath)
        {
            string outputBase = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "tesseract",
                    Arguments = $"\"{imagePath}\" \"{outputBase}\" -l eng",
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            string outputPath = outputBase + ".txt";
            if (!File.Exists(outputPath))
                throw new Exception("Tesseract failed to generate output");

            string text = await File.ReadAllTextAsync(outputPath);
            File.Delete(outputPath);

            return text;
        }
    }
}
