using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PictureLib
{
    public class Book
    {
        private PageFile mpfFirst, mpfLast;
        private Series msLocation;

        public string Number { get; private set; }
        public string Name { get; private set; }

        public Book(string sSeriesNumber, string sSeriesName, Series sLocation)
            : base()
        {
            UniqueId = sLocation.UniqueId + "+" + sSeriesNumber;
            Number = sSeriesNumber;
            Name = sSeriesName;
            msLocation = sLocation;
            mpfFirst = mpfLast = null;
        }

        public string UniqueId { get; set; }

        public PageFile First
        {
            get
            {
                return mpfFirst;
            }
            internal set
            {
                if (mpfFirst == null)
                    mpfLast = value;
                mpfFirst = value;
            }
        }

        public PageFile Last
        {
            get
            {
                return mpfLast;
            }
            internal set
            {
                if (mpfFirst == null)
                    mpfFirst = value;

                mpfLast = value;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
