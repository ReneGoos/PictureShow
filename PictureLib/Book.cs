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
        private Book _previous;
        private Book _next;

        public string Number { get; private set; }
        public string Name { get; private set; }

        public Book(string sSeriesNumber, string sSeriesName, Series sLocation, Book previous)
            : base()
        {
            UniqueId = sLocation.UniqueId + "+" + sSeriesNumber;
            Number = sSeriesNumber;
            Name = sSeriesName;
            Series = sLocation;
            Previous = previous;
            mpfFirst = mpfLast = null;
        }

        public string UniqueId { get; set; }

        public Series Series { get; }

        public Book Previous
        {
            get => _previous;
            protected set
            {
                _previous = value;
                if (_previous != null)
                    _previous.Next = this;
            }
        }

        public Book Next
        {
            get => _next;

            protected set
            {
                _next = value;
            }
        }

        public PageFile First
        {
            get => mpfFirst;
            internal set
            {
                if (mpfFirst == null)
                    mpfLast = value;
                mpfFirst = value;
            }
        }

        public PageFile Last
        {
            get => mpfLast;
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
