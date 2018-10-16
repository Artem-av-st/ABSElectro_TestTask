using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSElectro_TestTask
{
    internal class SortPattern
    {        
        public SortPatternType Type { get; }
        public bool IsDescending { get; }

        public SortPattern(SortPatternType type, bool isDescending)
        {
            Type = type;
            IsDescending = isDescending;
        }
        public Func<FileInfo, Object> ToOrderFunc()
        {
            switch (Type)
            {
                case SortPatternType.Name: return x => x.Name;
                case SortPatternType.Time: return x => x.LastWriteTime;
                case SortPatternType.Size: return x => x.Length;
                case SortPatternType.Extension: return x => x.Extension;
                default: throw new ArgumentException("Invalid value of Type property");
            }
        }

        public IEnumerable<FileInfo> PrimarySort(IEnumerable<FileInfo> files)
        {
            return !this.IsDescending ?
                files.OrderBy(this.ToOrderFunc()) :
                files.OrderByDescending(this.ToOrderFunc());
        }

        public IEnumerable<FileInfo> SecondarySort(IEnumerable<FileInfo> files)
        {
            return !this.IsDescending ?
                    (files as IOrderedEnumerable<FileInfo>).ThenBy(this.ToOrderFunc()) :
                    (files as IOrderedEnumerable<FileInfo>).ThenByDescending(this.ToOrderFunc());
        }

        internal enum SortPatternType
        {
            Time,
            Name,
            Extension,
            Size
        }
    }
}
