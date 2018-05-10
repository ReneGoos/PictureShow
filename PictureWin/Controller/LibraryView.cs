using PictureLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureWin.Controller
{
    public class LibraryView
    {
        public static EventHandler<OnPictureChangedArgs> onPictureChanged;

        private static async Task<BitmapImage> LoadImage(StorageFile file)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);
            bitmapImage.SetSource(stream);
            return bitmapImage;
        }

        private static async Task<PagePicture> LoadFile(PageFile aPic, double dScrnHeight, double dScrnWidth)
        {
            PagePicture lPic = new PagePicture(aPic, dScrnHeight, dScrnWidth);
            string sFileName = lPic.FullName;
            if (lPic.BMImage == null)
            {
                StorageFile sampleFile = await StorageFile.GetFileFromPathAsync(sFileName);
                lPic.BMImage = await LoadImage(sampleFile);
            }
            return lPic;
        }

        private static async Task PicturesLoad(List<PageFile> lPic, GridView imageFiles)
        {
            //Pictures lPics = lPic[26].Items[1].Items[0].Value;
            // load file from document library. Note: select document library in capabilities and declare .png file type
            foreach (PageFile pFile in lPic)
            {
                PagePicture prFile = await LoadFile(pFile, 0, 0);
                imageFiles.Items.Add(prFile);
            }
        }

        public static async Task<BookShelf> FolderOpenLoad()
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");
            StorageFolder sfFolder = await picker.PickSingleFolderAsync();
            return await Library.GetSerieSets(sfFolder);
        }

        public static async Task PictureImageLoad(PageFile asPic, ItemsControl agvImages)
        {
            double dScrnHeight = 0, dScrnWidth = 0;
            FrameworkElement dSizeFind = agvImages.Parent as FrameworkElement;
            while (dSizeFind as Page == null && dSizeFind.Parent != null)
                dSizeFind = dSizeFind.Parent as FrameworkElement;
            if (dSizeFind as Page != null)
            {
                dScrnHeight = dSizeFind.ActualHeight;
                dScrnWidth = dSizeFind.ActualWidth;

                if (DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.Landscape ||
                    DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.LandscapeFlipped)
                {
                    if (dScrnHeight > dScrnWidth)
                    {
                        double dSwap = dScrnWidth;
                        dScrnWidth = dScrnHeight;
                        dScrnHeight = dSwap;
                    }
                }
                else if (dScrnHeight < dScrnWidth)
                {
                    double dSwap = dScrnWidth;
                    dScrnWidth = dScrnHeight;
                    dScrnHeight = dSwap;
                }

            }

            PagePicture lPic = await LoadFile(asPic, dScrnHeight, dScrnWidth);
            agvImages.Items.Clear();
            agvImages.Items.Add(lPic);
        }

        public static async Task LoadTopGrid(BookShelf asPic, ItemsControl agvImages)
        {
            agvImages.Items.Clear();
            await PicturesLoad(asPic.Files, agvImages as GridView);
        }

        public static async Task LoadBottomGrid(BookShelf asPic, string series, ItemsControl agvImages)
        {
            agvImages.Items.Clear();
            await PicturesLoad(asPic[series].Files, agvImages as GridView);
        }

        public static async Task LoadNewPicture(BookShelf asPic, string series, string seriesNumber, ItemsControl agvImages)
        {
            agvImages.Items.Clear();
            await PictureImageLoad(asPic[series][seriesNumber].First, agvImages);
        }
    }

    public class OnPictureChangedArgs : EventArgs
    {
        public OnPictureChangedArgs(PageFile pagefile, string type)
        {
            File = pagefile;
            FileType = type;
        }

        public PageFile File { get; private set; }
        public string FileType { get; private set; }
    }
}
