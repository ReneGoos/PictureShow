using System;
using System.IO;
using System.Collections;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;

namespace PictureLib
{
    public enum FileNodeEnum
    {
        Drive,
        Dir,
        Series,
        SeriesNumber,
        File
    }

	/// <summary>
	/// Summary description for PictureFile.
	/// </summary>
    public class PageFile
    {
        public PageFile(StorageFile asFileName, String asSeries, String asSeriesNumber, int aiNumber, Book abParent, PageFile apPrev)
            : base()
        {
            UniqueId = asFileName.Path;

            File = asFileName;

            Book = abParent;
            Previous = abParent.Last;
            if (Previous == null)
            {
                Current = 1;
                Previous = apPrev;
            }
            else
            {
                Current = Previous.Current + 1;
                Debug.Assert(apPrev == Previous);
            }

            if (Previous != null)
                Previous.Next = this;
            abParent.Last = this;

            Series = asSeries;
            SeriesNumberTag = asSeriesNumber;
            Number = aiNumber;
        }

        public string UniqueId { get; set; }

        public Book Book { get; }

        public string Series { get; } = "";

        public int Number { get; } = -1;

        public int Current { get; } = -1;

        public PageFile Next { get; protected set; } = null;
        public PageFile Previous { get; protected set; } = null;

        public int Total => Book.Last.Current;

        public string SeriesNumberTag { get; } = "";
        public StorageFile File { get; } = null;

        public string FullName => File.Path;

        public string FileName => File.DisplayName;
    }
}
