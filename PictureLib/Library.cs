using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;

namespace PictureLib
{
    public sealed class Library
    {
        private static Library _Series = new Library();
        private static Dictionary<string, BookShelf> mlsSeries = new Dictionary<string, BookShelf>();

        public Library()
        {
        }

        private static void ParseName
            (string asName, out string asSeries, out string asSeriesNumberTag, out int aiNumber)
        {
            asSeries = "";
            asSeriesNumberTag = "";
            long llNumber = 0;
            int iMode = 0; //0 is reading first characters, 
            // 1 is reading number of sSeriesNumberTag,
            // 2 is reading character of sSeriesNumberTag,
            // 3 is reading number of iNumber
            // 5 is read the dot, skip the rest
            foreach (char c in asName.ToLower())
            {
                if (c.Equals('.'))
                {
                    if (iMode == 1)
                    {
                        llNumber = Convert.ToInt64(asSeriesNumberTag);
                        if (llNumber > 1000)
                        {
                            if (asSeries.Length == 0 && llNumber > 100000)
                                asSeries = Convert.ToString(llNumber / 100000);

                            asSeriesNumberTag = Convert.ToString(100 + ((llNumber % 100000) / 1000)).Substring(1);
                            llNumber = llNumber % 1000;
                        }
                        else
                            asSeriesNumberTag = "";
                    }
                    iMode = 5;
                }
                else if (!Char.IsDigit(c))
                {
                    switch (iMode)
                    {
                        case 0:
                            asSeries += c;
                            break;
                        case 1:
                            iMode = 2;
                            if (asSeries.Length == 0)
                            {
                                llNumber = Convert.ToInt64(asSeriesNumberTag);
                                if (llNumber > 100)
                                {
                                    asSeries = Convert.ToString(llNumber / 100);
                                    asSeriesNumberTag = Convert.ToString(100 + (llNumber % 100)).Substring(1);
                                }
                                llNumber = 0;
                            }
                            asSeriesNumberTag += c;
                            break;
                        default:
                            break;
                    }
                }
                else //if (Char.IsDigit(c))
                {
                    switch (iMode)
                    {
                        case 0:
                            iMode = 1;
                            goto case 1;
                        case 1:
                            asSeriesNumberTag += c;
                            break;
                        case 2:
                            iMode = 3;
                            goto case 3;
                        case 3:
                            llNumber = llNumber * 10 + Convert.ToInt64("" + c);
                            break;
                        default:
                            break;
                    }
                }
            }

            aiNumber = (int)llNumber;
            if (asSeries.Length == 0)
                asSeries = ".";
            if (asSeriesNumberTag.Length == 0)
                asSeriesNumberTag = ".";
        }

        public static async Task FolderInSeries(StorageFolder sfFolder, bool bReload = false)
        {
            if (!bReload && mlsSeries.ContainsKey(sfFolder.Path))
                return;

            BookShelf spWork = new BookShelf(_Series, sfFolder.Path);
            mlsSeries[sfFolder.Path] = spWork;
            List<string> fileTypeFilter = new List<string>();
            fileTypeFilter.Add(".jpeg");
            fileTypeFilter.Add(".jpg");
            fileTypeFilter.Add(".png");
            fileTypeFilter.Add(".bmp");
            fileTypeFilter.Add(".gif");
            fileTypeFilter.Add(".tif");

            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
            queryOptions.FolderDepth = FolderDepth.Shallow;
            string sPrevSeries = "some series name you'll never expect to see", sPrevBook = "some series number tag you'll never expect to see";
            StorageFileQueryResult queryResults = sfFolder.CreateFileQueryWithOptions(queryOptions);
            //IReadOnlyList<StorageFile> lsFiles = await queryResults.GetFilesAsync();

            string sSeries, sBook;
            Series seriesThis = null;
            Book bkThis = null;
            PageFile pfLast = null;
            int iNumber = 0;

            foreach (StorageFile fiFile in await queryResults.GetFilesAsync()) // int ifile = 0; ifile < lsFiles.Count; ifile++)
            {
                //StorageFile fiFile = lsFiles[ifile];
                string sName = fiFile.Name;         // fiFile.Path.Substring(sfFolder.Path.Length);
                ParseName(sName, out sSeries, out sBook, out iNumber);

                if (String.Compare(sSeries, sPrevSeries, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    seriesThis = new Series(sSeries, spWork, seriesThis);
                    spWork[sSeries] = seriesThis;
                }

                if (String.Compare(sSeries, sPrevSeries, StringComparison.OrdinalIgnoreCase) != 0 ||
                    String.Compare(sBook, sPrevBook, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    bkThis = new Book(sSeries, sBook, spWork[sSeries], bkThis);
                    spWork[sSeries][sBook] = bkThis;
                    sPrevSeries = sSeries;
                    sPrevBook = sBook;
                }

                PageFile pcfFile = new PageFile(fiFile, sSeries, sBook, iNumber, bkThis, pfLast);
                pfLast = pcfFile;
                //    sPrevSeries = sSeries;
                //    sPrevSeriesNumber = sSeriesNumber;
                //    int iTotal = 0;

                //    while (true)
                //    {
                //        iTotal++;
                //        if (ifile + iTotal < lsFiles.Count)
                //        {
                //            fiFile = fiFile = lsFiles[ifile + iTotal];
                //            sName = fiFile.Path.Substring(sfFolder.Path.Length);
                //            ParseName(sName, out sSeries, out sSeriesNumber, out iNumber[ifile + iTotal]);

                //            if (String.Compare(sSeries, sPrevSeries, StringComparison.OrdinalIgnoreCase) != 0 ||
                //                String.Compare(sSeriesNumber, sPrevSeriesNumber, StringComparison.OrdinalIgnoreCase) != 0)
                //                break;
                //        }
                //        else
                //            break;
                //    };

                //    PictureFile pcfFile = new PictureFile(lsFiles[ifile], FileNodeEnum.File, sPrevSeries, sPrevSeriesNumber, iNumber[ifile], 1, iTotal);
                //    pfLast = new PictureFile(sPrevSeriesNumber);
                //    pfLast.Add(pcfFile);

                //    for (int iPic = 2; iPic <= iTotal; iPic++)
                //    {
                //        pcfFile = new PictureFile(lsFiles[ifile + iPic - 1], FileNodeEnum.File, sPrevSeries, sPrevSeriesNumber, iNumber[ifile + iPic - 1], iPic, iTotal);
                //        pfLast.Add(pcfFile);
                //    }


                //    if (spLast == null || String.Compare(spLast.Name, sSeries) != 0)
                //    {
                //        if (spLast != null)
                //            mlsSeries[sfFolder.Path].Add(spLast);
                //        spLast = new SeriesPictures(sPrevSeries, sfFolder, sPrevSeriesNumber, pfLast);
                //    }
                //    else
                //        spLast.AddPictures(sPrevSeriesNumber, pfLast);
                //    ifile += iTotal;
                //}

                //if (spLast != null)
                //    mlsSeries[sfFolder.Path].Add(spLast);
            }
        }

        //public static async Task FillSeriesInTree(StorageFolder sfFolder, bool bReload = false)
        //{
        //    if (!bReload && mlsSeries.ContainsKey(sfFolder.Path))
        //        return;

        //    mlsSeries[sfFolder.Path] = new List<SeriesPictures>();
        //    List<string> fileTypeFilter = new List<string>();
        //    fileTypeFilter.Add(".jpg");
        //    fileTypeFilter.Add(".png");
        //    fileTypeFilter.Add(".bmp");
        //    fileTypeFilter.Add(".gif");

        //    QueryOptions queryOptions = new Windows.Storage.Search.QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
        //    queryOptions.FolderDepth = FolderDepth.Shallow;
        //    string sPrevSeries = "some series name you'll never expect to see", sPrevSeriesNumber = "some series number tag you'll never expect to see";
        //    StorageFileQueryResult queryResults = sfFolder.CreateFileQueryWithOptions(queryOptions);
        //    IReadOnlyList<StorageFile> lsFiles = await queryResults.GetFilesAsync();
            
        //    string sSeries, sSeriesNumber;
        //    PictureFile pfLast = null;
        //    SeriesPictures spLast = null;
        //    int[] iNumber = new int[lsFiles.Count];

        //    for (int ifile = 0; ifile < lsFiles.Count; )
        //    {
        //        StorageFile fiFile = lsFiles[ifile];
        //        string sName = fiFile.Path.Substring(sfFolder.Path.Length);
        //        ParseName(sName, out sSeries, out sSeriesNumber, out iNumber[ifile]);
        //        sPrevSeries = sSeries;
        //        sPrevSeriesNumber = sSeriesNumber;
        //        int iTotal = 0;

        //        while (true)
        //        {
        //            iTotal++;
        //            if (ifile + iTotal < lsFiles.Count)
        //            {
        //                fiFile = fiFile = lsFiles[ifile + iTotal];
        //                sName = fiFile.Path.Substring(sfFolder.Path.Length);
        //                ParseName(sName, out sSeries, out sSeriesNumber, out iNumber[ifile + iTotal]);

        //                if (String.Compare(sSeries, sPrevSeries, StringComparison.OrdinalIgnoreCase) != 0 ||
        //                    String.Compare(sSeriesNumber, sPrevSeriesNumber, StringComparison.OrdinalIgnoreCase) != 0)
        //                    break;
        //            }
        //            else
        //                break;
        //        };

        //        PictureFile pcfFile = new PictureFile(lsFiles[ifile], FileNodeEnum.File, sPrevSeries, sPrevSeriesNumber, iNumber[ifile], 1, iTotal);
        //        pfLast = new PictureFile(sPrevSeriesNumber);
        //        pfLast.Add(pcfFile);

        //        for (int iPic = 2; iPic <= iTotal; iPic++)
        //        {
        //            pcfFile = new PictureFile(lsFiles[ifile + iPic - 1], FileNodeEnum.File, sPrevSeries, sPrevSeriesNumber, iNumber[ifile + iPic - 1], iPic, iTotal);
        //            pfLast.Add(pcfFile);
        //        }


        //        if (spLast == null || String.Compare(spLast.Name, sSeries) != 0)
        //        {
        //            if (spLast != null)
        //                mlsSeries[sfFolder.Path].Add(spLast);
        //            spLast = new SeriesPictures(sPrevSeries, sfFolder, sPrevSeriesNumber, pfLast);
        //        }
        //        else
        //            spLast.AddPictures(sPrevSeriesNumber, pfLast);
        //        ifile += iTotal;
        //    }

        //    if (spLast != null)
        //        mlsSeries[sfFolder.Path].Add(spLast);
        //}

        public static BookShelf GetSeriesPictures(string sName)
        {
            string lsFolder = sName.Substring(0, sName.IndexOf('+'));

            if (mlsSeries.ContainsKey(sName))
                return mlsSeries[sName];
            return null;
            //SeriesPictures spFound = null;
            //foreach (SeriesPictures spThis in mlsSeries[lsFolder])
            //    if (spThis.UniqueId.Equals(sName))
            //        spFound = spThis;
            //return spFound;
        }

        public static async Task<BookShelf> GetSerieSets(StorageFolder sf)
        {
            ActiveSet = sf.Path;
            if (!mlsSeries.ContainsKey(sf.Path))
            {
                await FolderInSeries(sf);
            }
            return mlsSeries[sf.Path];
        }

        public static async Task<BookShelf> GetSerieSets(string sfFolder)
        {
            ActiveSet = sfFolder;
            StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(sfFolder);
            await GetSerieSets(sf);
            return mlsSeries[sf.Path];
        }

        public static string ActiveSet { get; private set; } = null;

        //public static List<PictureSets> GetListPictureSets(List<SeriesPictures> sSeries, string sName)
        //{
        //    SeriesPictures spFound = null;
        //    foreach (SeriesPictures spThis in sSeries)
        //        if (spThis.Name.Equals(sName, StringComparison.OrdinalIgnoreCase))
        //            spFound = spThis;
        //    if (spFound == null)
        //        return null;
        //    return spFound.Items;
        //}

        //public static PictureFile GetListPictures(List<SeriesPictures> sSeries, string sName, string sNumber)
        //{
        //    List<PictureSets> spThis = GetListPictureSets(sSeries, sName);
        //    if (spThis == null)
        //        return null;
        //    PictureSets psFound = null;

        //    foreach (PictureSets psThis in spThis)
        //        if (psThis.Items[0].Key.Equals(sNumber, StringComparison.OrdinalIgnoreCase))
        //            psFound = psThis;
        //    return psFound.Items[0].Value;
        //}
    }
}
