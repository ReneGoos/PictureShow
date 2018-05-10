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
        private StorageFile isFullName = null;//, isName = "", isPath = "";
		private string isSeries = "", isSeriesNumber = "";
        private int iiNumber = -1;
        private int iiCurrent = -1;
        private PageFile ipPrev = null, ipNext = null;
        private Book ibParent = null;

        public PageFile(StorageFile asFileName, String asSeries, String asSeriesNumber, int aiNumber, Book abParent, PageFile apPrev)
            : base()
        {
            UniqueId = asFileName.Path;

            isFullName = asFileName;

            ibParent = abParent;
            ipPrev = abParent.Last;
            if (ipPrev == null)
            {
                iiCurrent = 1;
                ipPrev = apPrev;
            }
            else
            {
                iiCurrent = ipPrev.iiCurrent + 1;
                Debug.Assert(apPrev == ipPrev);
            }

            if (ipPrev != null)
                ipPrev.Next = this;
            abParent.Last = this;

            isSeries = asSeries;
            isSeriesNumber = asSeriesNumber;
            iiNumber = aiNumber;
        }

        public string UniqueId { get; set; }

        public string Series
		{
			get
			{
				return isSeries;
			}
		}

		public int Number
		{
			get
			{
				return iiNumber;
			}
		}

        public int Current
        {
            get
            {
                return iiCurrent;
            }
        }

        public PageFile Next
        {
            get
            {
                return ipNext;
            }
            protected set
            {
                ipNext = value;
            }
        }

        public PageFile Previous
        {
            get
            {
                return ipPrev;
            }
            protected set 
            {
                ipPrev = value;
            }
        }

        public int Total
        {
            get
            {
                return ibParent.Last.Current;
            }
        }

		public string SeriesNumberTag
		{
			get
			{
				return isSeriesNumber;
			}
		}

        public StorageFile File
        {
            get
            {
                return isFullName;
            }
        }

        public string FullName
        {
            get
            {
                return isFullName.Path;
            }
        }

        public string FileName
        {
            get
            {
                return isFullName.DisplayName;
            }
        }
	}
}
