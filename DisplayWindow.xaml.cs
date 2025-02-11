using Aspose.Slides;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls; // Adicionar esta linha
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MultiScreenApp
{
    public partial class DisplayWindow : Window
    {
        private bool isFullScreen = false;
        private WindowState previousWindowState;
        private double previousWindowHeight;
        private double previousWindowWidth;

        public List<BitmapImage> SlideImages { get; private set; }
        public int CurrentSlideIndex { get; set; }

        public DisplayWindow()
        {
            InitializeComponent();
            SlideImages = new List<BitmapImage>();
            CurrentSlideIndex = -1;
            this.SizeChanged += DisplayWindow_SizeChanged;
        }

        private void DisplayWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustContentSize();
        }

        private void AdjustContentSize()
        {
            if (DisplayedVideo.Visibility == Visibility.Visible)
            {
                DisplayedVideo.Width = this.ActualWidth;
                DisplayedVideo.Height = this.ActualHeight;
            }
            else if (DisplayedImage1.Visibility == Visibility.Visible && DisplayedImage2.Visibility == Visibility.Visible)
            {
                DisplayedImage1.Width = this.ActualWidth / 2;
                DisplayedImage1.Height = this.ActualHeight;
                DisplayedImage2.Width = this.ActualWidth / 2;
                DisplayedImage2.Height = this.ActualHeight;
            }
            else if (DisplayedImage1.Visibility == Visibility.Visible)
            {
                DisplayedImage1.Width = this.ActualWidth;
                DisplayedImage1.Height = this.ActualHeight;
            }
        }

        public void DisplayImages(string filePath1, string filePath2)
        {
            DisplayedVideo.Visibility = Visibility.Collapsed;
            DisplayedImage1.Source = new BitmapImage(new Uri(filePath1));
            DisplayedImage1.Visibility = Visibility.Visible;
            Grid.SetColumnSpan(DisplayedImage1, 1); // Certifique-se de definir isso corretamente
            DisplayedImage2.Source = new BitmapImage(new Uri(filePath2));
            DisplayedImage2.Visibility = Visibility.Visible;
            AdjustContentSize();
        }

        public void DisplayImage(string filePath)
        {
            DisplayedVideo.Visibility = Visibility.Collapsed;
            DisplayedImage1.Source = new BitmapImage(new Uri(filePath));
            DisplayedImage1.Visibility = Visibility.Visible;
            Grid.SetColumnSpan(DisplayedImage1, 2); // Certifique-se de definir isso corretamente
            DisplayedImage2.Visibility = Visibility.Collapsed;
            AdjustContentSize();
        }

        public void LoadVideo(string filePath)
        {
            DisplayedImage1.Visibility = Visibility.Collapsed;
            DisplayedImage2.Visibility = Visibility.Collapsed;
            DisplayedVideo.Source = new Uri(filePath);
            DisplayedVideo.Visibility = Visibility.Visible;
            DisplayedVideo.Play();
            AdjustContentSize();
        }

        public void LoadPresentation(string filePath)
        {
            try
            {
                SlideImages.Clear();
                using (var presentation = new Presentation(filePath))
                {
                    foreach (var slide in presentation.Slides)
                    {
                        SlideImages.Add(ConvertSlideToImage(slide));
                    }
                }
                if (SlideImages.Count > 0)
                {
                    CurrentSlideIndex = 0;
                    ShowSlide(CurrentSlideIndex);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar a apresentação: " + ex.Message);
            }
        }

        private BitmapImage ConvertSlideToImage(ISlide slide)
        {
            using (var stream = new MemoryStream())
            {
                slide.GetThumbnail(1.0f, 1.0f).Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public void ShowSlide(int index)
        {
            if (index >= 0 && index < SlideImages.Count)
            {
                DisplayedVideo.Visibility = Visibility.Collapsed;
                DisplayedImage1.Source = SlideImages[index];
                DisplayedImage1.Visibility = Visibility.Visible;
                DisplayedImage2.Visibility = Visibility.Collapsed;
                Grid.SetColumnSpan(DisplayedImage1, 2); // Certifique-se de definir isso corretamente
                AdjustContentSize();
            }
        }

        public void PauseVideo()
        {
            DisplayedVideo.Pause();
        }

        public void PlayVideo()
        {
            DisplayedVideo.Play();
        }

        private void DisplayedVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            DisplayedVideo.Position = TimeSpan.Zero; // Reinicia o vídeo
            DisplayedVideo.Play();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToggleFullScreen();
        }

        private void ToggleFullScreen()
        {
            if (!isFullScreen)
            {
                previousWindowState = this.WindowState;
                previousWindowHeight = this.Height;
                previousWindowWidth = this.Width;
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;
                this.Topmost = true;
                this.WindowState = WindowState.Maximized;
                isFullScreen = true;
            }
            else
            {
                this.Topmost = false;
                this.WindowState = previousWindowState;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.ResizeMode = ResizeMode.CanResize;
                this.Height = previousWindowHeight;
                this.Width = previousWindowWidth;
                isFullScreen = false;
            }
        }
    }
}
