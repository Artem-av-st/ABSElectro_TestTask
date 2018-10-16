using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace ABSElectro_TestTask
{
    internal class FileProcessor
    {   
        private ILogger _logger;

        public FileProcessor(ILogger logger)
        {            
            _logger = logger;
        }

        /// <summary>
        /// Lookups for files in current directory using search options and writes them using logger method "Message"
        /// </summary>
        /// <param name="fileNamePatterns">Collection of file search patterns in format *.*</param>
        /// <param name="recursievly">Flag that controls the search in subfolders</param>
        /// <param name="regularExpression">Regular expression that should be matched in file.  If you use this option, then
        /// search string parameter must be null or empty</param>
        /// <param name="searchString">Substring that should be found in file. If you use this option, then
        /// regular expression parameter must be null or empty</param>
        /// <param name="sortPatterns">Patterns that controls order of output results</param>
        /// <param name="token">Cancellation token</param>
        public void FindFiles(IEnumerable<string> fileNamePatterns, bool recursievly, string regularExpression, 
            string searchString, IEnumerable<SortPattern> sortPatterns, CancellationToken token)
        {
            if(!String.IsNullOrWhiteSpace(regularExpression) && !String.IsNullOrWhiteSpace(searchString))
            {
                throw new ArgumentException("Cannot use both search options: regular expression and search string");
            }

            _logger.Message(
                $"\nПоиск во вложенных папках: {recursievly}\n" +
                $"Регулярное выражение: {regularExpression}\n" +
                $"Текст поиска: {searchString}\n" +
                $"Шаблоны имени фала: {String.Join(" | ", fileNamePatterns)}\n" +
                $"Шаблоны сортировки: {String.Join(" | ", sortPatterns.Select(sp => (sp.Type.ToString() + " - По убыванию: " + sp.IsDescending)))}");


            var files = GetFilesFromCurrentDirectory(fileNamePatterns, recursievly);

            var filteredFiles = FilterFilesByContetns(files, regularExpression, searchString, token);

            filteredFiles = SortFiles(filteredFiles, sortPatterns);
            
            LogFileNames(filteredFiles);

            // Вывод количества обработанных файлов, после того, как получена отмена операции.
            _logger.Message($"Всего найдено файлов: {files.Count()}. Из них соответствуют критериям поиска {filteredFiles.Count()}");
        }
        
        private void LogFileNames(IEnumerable<FileInfo> files)
        {
            foreach (var file in files)
            {
                _logger.Debug(file.FullName);
            }
        }

        /// <summary>
        /// Returns collection of files whose contents match the regular expression or contains search string
        /// </summary>        
        private IEnumerable<FileInfo> FilterFilesByContetns(IEnumerable<FileInfo> files, string regex, string searchString, CancellationToken token)
        {
            var textProcessor = new TextFileProcessor(files);

            if (!String.IsNullOrWhiteSpace(regex))
            {
                return textProcessor.RegexMatches(regex, token);
            }
            else if (!String.IsNullOrWhiteSpace(searchString))
            {
                return textProcessor.StringMatches(searchString, token);
            }
            else
            {
                return files;
            }
        }

        /// <summary>
        /// Returns collection of files contained in current directory and subfolders
        /// </summary>
        /// <param name="fileNamePatterns">Collection of file search patterns in format *.*</param>
        /// <param name="recursievly">Flag that controls the search in subfolders</param>        
        private IEnumerable<FileInfo> GetFilesFromCurrentDirectory(IEnumerable<string> fileNamePatterns, bool recursievly)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var dirInfo = new DirectoryInfo(currentDirectory);

            var fileSearchOption = recursievly ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            // If fileName patters collection does not contain any element then get all files
            return fileNamePatterns.Any() ?
                fileNamePatterns.SelectMany(p => dirInfo.GetFiles(p, fileSearchOption)) :
                dirInfo.GetFiles("*.*", fileSearchOption);
        }

        /// <summary>
        /// Sorts files using sort patterns collection
        /// </summary>        
        private IEnumerable<FileInfo> SortFiles(IEnumerable<FileInfo> files, IEnumerable<SortPattern> patterns)
        {
            if (!patterns.Any())
            {
                return files;                
            }

            // Primary sorting
            var pattern = patterns.ElementAt(0);
            files = pattern.PrimarySort(files);

            // Secondary sorting
            for (int i = 1; i < patterns.Count(); i++)
            {
                var secondaryPattern = patterns.ElementAt(i);
                files = secondaryPattern.SecondarySort(files);
            }

            return files;
        }
    }
}
