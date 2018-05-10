using PictureLib;
using PictureWin.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PictureWin
{
    public sealed partial class MainPage : Page
    {
        BookShelf mlPic;

        private async Task OpenPicturesFolder()
        {
            mlPic = await LibraryView.FolderOpenLoad();

            if (mlPic.Files.Count() == 0)
                return;
            var pagePic = mlPic.First().First;
            await LibraryView.LoadTopGrid(mlPic, imageHeads);
            await LibraryView.LoadBottomGrid(mlPic, pagePic.Series, imageGroups);
            await LibraryView.LoadNewPicture(mlPic, pagePic.Series, pagePic.SeriesNumberTag, mainImage);
        }

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
            LibraryView.onPictureChanged += MainPage_OnPictureChanged;
        }

        private async void MainPage_OnPictureChanged(object sender, OnPictureChangedArgs e)
        {
            var selTop = imageHeads.SelectedIndex;
            if (selTop < 0)
                selTop = 0;
            var selItem = imageHeads.Items[selTop] as PagePicture;
            if (!selItem.Series.Equals(e.File.Series))
            {//move next, previous
                foreach (var item in imageHeads.Items)
                {
                    var pagePic = item as PagePicture;
                    if (pagePic.Series.Equals(e.File.Series))
                    {
                        imageHeads.SelectedItem = item;
                        imageHeads.ScrollIntoView(item);

                        await LibraryView.LoadBottomGrid(mlPic, pagePic.Series, imageGroups);
                        if (!e.FileType.Equals("grid"))
                            await LibraryView.LoadNewPicture(mlPic, pagePic.Series, pagePic.SeriesNumberTag, mainImage);
                        return;
                    }
                }
            }

            var selBottom = imageGroups.SelectedIndex;
            if (selBottom < 0)
                selBottom = 0;
            selItem = imageGroups.Items[selBottom] as PagePicture;
            if (!selItem.Series.Equals(e.File.Series) || !selItem.SeriesNumberTag.Equals(e.File.SeriesNumberTag))
            {//move next, previous
                foreach (var item in imageGroups.Items)
                {
                    var pagePic = item as PagePicture;
                    if (pagePic.Series.Equals(e.File.Series) && pagePic.SeriesNumberTag.Equals(e.File.SeriesNumberTag))
                    {
                        imageGroups.SelectedItem = item;
                        imageGroups.ScrollIntoView(item);
                        if (!e.FileType.Equals("grid"))
                            await LibraryView.LoadNewPicture(mlPic, pagePic.Series, pagePic.SeriesNumberTag, mainImage);
                    }
                }
            }
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayInformation.GetForCurrentView().OrientationChanged += OnOrientationChanged;
            await OpenPicturesFolder();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void OnTopItemTapped(object sender, TappedRoutedEventArgs e)
        {
            var gridView = sender as GridView;
            var pagePic = gridView.SelectedItem as PagePicture;
            await LibraryView.LoadBottomGrid(mlPic, pagePic.Series, imageGroups);
            await LibraryView.LoadNewPicture(mlPic, pagePic.Series, pagePic.SeriesNumberTag, mainImage);
        }

        private async void OnBottomItemTapped(object sender, TappedRoutedEventArgs e)
        {
            var gridView = sender as GridView;
            var pagePic = gridView.SelectedItem as PagePicture;
            await LibraryView.LoadNewPicture(mlPic, pagePic.Series, pagePic.SeriesNumberTag, mainImage);
        }

        private Point initialpoint;

        private void mainImage_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            initialpoint = e.Position;
            e.Handled = true;
        }

        private async void mainImage_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            //if (e.IsInertial)
            {
                PagePicture lpCurrent = (PagePicture)mainImage.Items[0];
                if (lpCurrent != null)
                {
                    Point currentpoint = e.Position;
                    if (currentpoint.X - initialpoint.X >= 5 && lpCurrent.Previous != null)//500 is the threshold value, where you want to trigger the swipe right event
                    {
                        await LibraryView.PictureImageLoad(lpCurrent.Previous, mainImage);
                        LibraryView.onPictureChanged(sender, new OnPictureChangedArgs(lpCurrent.Previous, "grid"));
                        e.Complete();
                    }
                    else if (currentpoint.X - initialpoint.X <= -5 && lpCurrent.Next != null)//500 is the threshold value, where you want to trigger the swipe right event
                    {
                        await LibraryView.PictureImageLoad(lpCurrent.Next, mainImage);
                        LibraryView.onPictureChanged(sender, new OnPictureChangedArgs(lpCurrent.Next, "grid"));
                        e.Complete();
                    }
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await OpenPicturesFolder();
        }

        private async void mainImage_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(e.Velocities.Linear.X) > .1)
            {
                PagePicture lpCurrent = (PagePicture)mainImage.Items[0];
                PageFile lpfFile = null;
                if (lpCurrent != null)
                {
                    if (e.Velocities.Linear.X > 0)//500 is the threshold value, where you want to trigger the swipe right event
                        lpfFile = StepPrevious((e.Velocities.Linear.X > 2), lpCurrent);
                    else if (e.Velocities.Linear.X < 0)
                        lpfFile = StepNext((e.Velocities.Linear.X < -2), lpCurrent);

                    if (lpfFile != null)
                    {
                        await LibraryView.PictureImageLoad(lpfFile, mainImage);
                        LibraryView.onPictureChanged(sender, new OnPictureChangedArgs(lpfFile, "grid"));
                    }
                    else
                        e.Handled = false;
                }
            }
        }

        private static PageFile StepPrevious(bool fastStep, PagePicture lpCurrent)
        {
            PageFile lpfFile = lpCurrent.Previous;
            if (fastStep)
            {
                for (int i = 0; i < 9 && lpfFile.Previous != null; i++)
                    lpfFile = lpfFile.Previous;
            }

            return lpfFile;
        }

        private static PageFile StepNext(bool fastStep, PagePicture lpCurrent)
        {
            PageFile lpfFile = lpCurrent.Next;
            if (fastStep)
            {
                for (int i = 0; i < 9 && lpfFile.Next != null; i++)
                    lpfFile = lpfFile.Next;
            }

            return lpfFile;
        }

        async void OnOrientationChanged(DisplayInformation sender, object args)
        {
            //scrollView.Height = this.Height;
            //scrollView.Width = this.Width;

            if (mainImage.Items.Count > 0)
            {
                PageFile lpfFile = ((PagePicture)mainImage.Items[0]).PictureFile;

                mainImage.Items.Clear();

                if (lpfFile != null)
                {
                    await LibraryView.PictureImageLoad(lpfFile, mainImage);
                    LibraryView.onPictureChanged(sender, new OnPictureChangedArgs(lpfFile, "grid"));
                }
            }
        }

        async void gridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            {
                PagePicture lpCurrent = (PagePicture)mainImage.Items[0];
                PageFile lpfFile = null;
                if (lpCurrent != null)
                {
                    var position = e.GetPosition(mainImage);

                    if (position.X < 50)
                        lpfFile = StepPrevious((position.X < 25), lpCurrent);
                    else if (position.X > mainImage.ActualWidth - 75)
                        lpfFile = StepNext((position.X > mainImage.ActualWidth - 50), lpCurrent);

                    if (lpfFile != null)
                    {
                        await LibraryView.PictureImageLoad(lpfFile, mainImage);
                        LibraryView.onPictureChanged(sender, new OnPictureChangedArgs(lpfFile, "grid" ));
                    }
                    else
                        e.Handled = false;
                }
            }
        }
    }
}
