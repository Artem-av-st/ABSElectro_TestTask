using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABSElectro_TestTask;
using System.IO;
using Xunit;

namespace ABSElectro_TestTask.Tests.UnitTests
{
    public class SortPatternTests
    {   
        [Fact]         
        internal void PrimarySort_ByNameAscending_equals()
        {
            IEnumerable<FileInfo> files = new[]
            {
                new FileInfo(@"C:\1.txt"),
                new FileInfo(@"C:\3.txt"),
                new FileInfo(@"C:\4.txt"),
                new FileInfo(@"C:\2.txt"),
            };
            IEnumerable<FileInfo> expected = new[]
            {
                new FileInfo(@"C:\1.txt"),
                new FileInfo(@"C:\2.txt"),
                new FileInfo(@"C:\3.txt"),
                new FileInfo(@"C:\4.txt"),
            };
            var pattern = new SortPattern(SortPattern.SortPatternType.Name, false);
            var sorted = pattern.PrimarySort(files);
            for(int i = 0; i < sorted.Count(); i++)
            {
                Assert.Equal(sorted.ElementAt(i).Name, expected.ElementAt(i).Name);
            }
        }

        [Fact]
        internal void PrimarySort_ByNameDescending_equals()
        {
            IEnumerable<FileInfo> files = new[]
            {
                new FileInfo(@"C:\1.txt"),
                new FileInfo(@"C:\3.txt"),
                new FileInfo(@"C:\4.txt"),
                new FileInfo(@"C:\2.txt"),
            };
            IEnumerable<FileInfo> expected = new[]
            {
                new FileInfo(@"C:\4.txt"),
                new FileInfo(@"C:\3.txt"),
                new FileInfo(@"C:\2.txt"),
                new FileInfo(@"C:\1.txt"),
            };
            var pattern = new SortPattern(SortPattern.SortPatternType.Name, true);
            var sorted = pattern.PrimarySort(files);
            for (int i = 0; i < sorted.Count(); i++)
            {
                Assert.Equal(sorted.ElementAt(i).Name, expected.ElementAt(i).Name);
            }
        }
    }
}
