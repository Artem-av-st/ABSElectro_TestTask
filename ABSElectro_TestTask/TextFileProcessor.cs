using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;

namespace ABSElectro_TestTask
{
    internal class TextFileProcessor
    {
        private readonly IEnumerable<FileInfo> _files;
        public TextFileProcessor(IEnumerable<FileInfo> files)
        {
            _files = files;
        }

        /// <summary>
        /// Returns collection of files whose contents match the regular expression
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<FileInfo> RegexMatches(string pattern, CancellationToken token)
        {
            foreach(var file in _files)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                EncodingDetector.DetectTextEncoding(file.FullName, out var text);
                
                var regex = new Regex(pattern);
                if (regex.IsMatch(text))
                {
                    yield return file;
                }                
            }
        }

        /// <summary>
        /// Returns collection of files whose contents contains search string
        /// </summary>       
        public IEnumerable<FileInfo> StringMatches(string searchString, CancellationToken token)
        {
            foreach (var file in _files)
            {
                if(token.IsCancellationRequested)
                {
                    break;
                }

                EncodingDetector.DetectTextEncoding(file.FullName, out var text);
                
                if (text.Contains(searchString))
                {
                    yield return file;
                }
            }
        }
    }
}
