using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PictureLib
{
    public class Series
    {
        Dictionary<string, Book> mlsPictures;
        private BookShelf mbsLocation = null;

        private Series _previous;
        private Series _next;


        public Series(string sSeriesName, BookShelf sfLocation, Series previous)
            : base()
        {
            UniqueId = sfLocation.UniqueId + "+" + sSeriesName;
            Name = sSeriesName;
            mbsLocation = sfLocation;
            mlsPictures = new Dictionary<string, Book>();
            Previous = previous;
        }

        public string UniqueId { get; set; }

        public string Name { get; private set; }


        public Book this[string sBook]
        {
            get
            {
                 return mlsPictures[sBook];
            }
            set
            {
                if (!mlsPictures.ContainsKey(sBook))
                    mlsPictures[sBook] = value;
            }
        }

        public Series Previous
        {
            get => _previous;
            protected set
            {
                _previous = value;
                if (_previous != null)
                    _previous.Next = this;
            }
        }

        public Series Next
        {
            get => _next;

            protected set
            {
                _next = value;
            }
        }

        public Book First
        {
            get
            {
                return mlsPictures.First().Value;
            }
        }

        public Book Last
        {
            get
            {
                return mlsPictures.Last().Value;
            }
        }

        public List<PageFile> Files
        {
            get
            {
                List<PageFile> lpFirst = new List<PageFile>();
                foreach (Book lbBook in mlsPictures.Values)
                    lpFirst.Add(lbBook.First);
                return lpFirst;
            }
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
