using PictureLib;
using PictureWin.Common;
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
        public string UniqueId { get; set; }

        public BitmapImage BMImage { get; set;  }

        public double Height { get; private set; }
        public double Width { get; private set; }

        public PagePicture(PageFile pfFile, double dScrnHeight, double dScrnWidth)
            : base()
        {
            PictureFile = pfFile;
            Height = dScrnHeight;
            Width = dScrnWidth;
        }

        public string Series
        {
            get
            {
                return PictureFile.Series;
            }
        }

        public int Number
        {
            get
            {
                return PictureFile.Number;
            }
        }

        public int Current
        {
            get
            {
                return PictureFile.Current;
            }
        }

        public int Total
        {
            get
            {
                return PictureFile.Total;
            }
        }

        public string SeriesNumberTag
        {
            get
            {
                return PictureFile.SeriesNumberTag;
            }
        }

        public PageFile PictureFile { get; } = null;

        public StorageFile File
        {
            get
            {
                return PictureFile.File;
            }
        }

        public string FileName
        {
            get
            {
                return PictureFile.FileName;
            }
        }

        public string FullName
        {
            get
            {
                return PictureFile.FullName;
            }
        }

        public PageFile Next
        {
            get
            {
                return PictureFile.Next;
            }
        }

        public PageFile Previous
        {
            get
            {
                return PictureFile.Previous;
            }
        }

        public PageFile StepPrevious(StepSize step)
        {
            if (this.Previous == null)
                return null;

            //if current is 'first page', first goto previous 'book'
            PageFile lpfFile = this.Previous;
            switch (step)
            {
                case StepSize.small:
                    break;
                case StepSize.large:
                    for (int i = 0; i < 9 && lpfFile.Previous != null && lpfFile.Series.Equals(this.Series) && lpfFile.SeriesNumberTag.Equals(this.SeriesNumberTag); i++)
                        lpfFile = lpfFile.Previous;
                    break;
                case StepSize.seriesTag:
                    lpfFile = lpfFile.Book.First;
                    break;
                case StepSize.series:
                    lpfFile = lpfFile.Book.Series.First.First;
                    break;
            }

            return lpfFile;
        }

        public PageFile StepNext(StepSize step)
        {
            if (this.Next == null)
                return null;

            PageFile lpfFile = this.PictureFile;
            switch (step)
            {
                case StepSize.small:
                    lpfFile = lpfFile.Next;
                    break;
                case StepSize.large:
                    for (int i = 0; i < 10 && lpfFile.Next != null && lpfFile.Series.Equals(this.Series) && lpfFile.SeriesNumberTag.Equals(this.SeriesNumberTag); i++)
                        lpfFile = lpfFile.Next;
                    break;
                case StepSize.seriesTag:
                    if (lpfFile.Book.Next == null)
                        return null;

                    lpfFile = lpfFile.Book.Next.First;
                    for (int i = 0; lpfFile.Next != null && lpfFile.Series.Equals(this.Series) && lpfFile.SeriesNumberTag.Equals(this.SeriesNumberTag); i++)
                        lpfFile = lpfFile.Next;
                    break;
                case StepSize.series:
                    for (int i = 0; lpfFile.Next != null && lpfFile.Series.Equals(this.Series); i++)
                        lpfFile = lpfFile.Next;
                    break;
            }

            return lpfFile;
        }
    }
}
