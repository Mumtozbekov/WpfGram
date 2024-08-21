using System.IO.Compression;
using System.IO;
    namespace WpfGram.Utils
{
    public class ZipUtils
    {
        public static string ExtractEntry(string gzipFilePath, string fileNameToExtract)
        {

            var extractPath = Path.GetDirectoryName(gzipFilePath);
            // Create the extraction directory if it doesn't exist

            // Extract the specific file from the ZIP archive
            Directory.CreateDirectory(extractPath);

            try
            {

                // Extract the specific entry from the GZIP archive
                using (FileStream gzipFileStream = File.OpenRead(gzipFilePath))
                using (GZipStream gzipStream = new GZipStream(gzipFileStream, CompressionMode.Decompress))
                {
                    using (FileStream extractFileStream = File.Create(Path.Combine(extractPath, fileNameToExtract)))
                    {
                        gzipStream.CopyTo(extractFileStream);
                    }

                }
            }
            catch { }

            return Path.Combine(extractPath, fileNameToExtract);
        }
    }
}
