using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureLib
{
    public class BookShelf
    {
        Dictionary<string, Series> mlsPictures;

        public BookShelf(Library lbLocation, string sLocation)
            : base()
        {
            UniqueId = sLocation;
            Name = sLocation;
            mlsPictures = new Dictionary<string, Series>();
        }

        public string UniqueId { get; set; }

        public string Name { get; private set; }

        public Series this[string sName]
        {
            get
            {
                if (!mlsPictures.ContainsKey(sName))
                    mlsPictures[sName] = new Series(sName, this);
                return mlsPictures[sName];
            }
        }

        public Book First()
        {
            return mlsPictures.First().Value.First;
        }

        public List<PageFile> Files
        {
            get
            {
                List<PageFile> lsPictures = new List<PageFile>();
                foreach (KeyValuePair<string, Series> sp in mlsPictures)
                    lsPictures.Add(sp.Value.First.First);
                return lsPictures;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
