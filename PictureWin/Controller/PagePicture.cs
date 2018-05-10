using PictureLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureWin.Controller
{
    /// <summary>
	/// Summary description for PictureFile.
	/// </summary>
    public class PagePicture
    {
        private PageFile ipfName = null;//, isName = "", isPath = "";

        public string UniqueId { get; set; }

        public BitmapImage BMImage { get; set;  }

        public double Height { get; private set; }
        public double Width { get; private set; }

        public PagePicture(PageFile pfFile, double dScrnHeight, double dScrnWidth)
            : base()
        {
            ipfName = pfFile;
            Height = dScrnHeight;
            Width = dScrnWidth;
        }

        public string Series
        {
            get
            {
                return ipfName.Series;
            }
        }

        public int Number
        {
            get
            {
                return ipfName.Number;
            }
        }

        public int Current
        {
            get
            {
                return ipfName.Current;
            }
        }

        public int Total
        {
            get
            {
                return ipfName.Total;
            }
        }

        public string SeriesNumberTag
        {
            get
            {
                return ipfName.SeriesNumberTag;
            }
        }

        public PageFile PictureFile
        {
            get
            {
                return ipfName;
            }
        }

        public StorageFile File
        {
            get
            {
                return ipfName.File;
            }
        }

        public string FileName
        {
            get
            {
                return ipfName.FileName;
            }
        }

        public string FullName
        {
            get
            {
                return ipfName.FullName;
            }
        }

        public PageFile Next
        {
            get
            {
                return ipfName.Next;
            }
        }

        public PageFile Previous
        {
            get
            {
                return ipfName.Previous;
            }
        }
    }
}
