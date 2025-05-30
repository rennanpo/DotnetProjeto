using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Application = Microsoft.Office.Interop.PowerPoint.Application;
using System.Runtime.InteropServices;
using DocumentFormat.OpenXml.Math;

namespace MultiScreenApp
{
    public partial class DisplayWindow : Window
    {
        private bool isFullScreen = true;
        public List<BitmapImage> SlideImages { get; private set; }
        public int CurrentSlideIndex { get; set; }

        private Application pptApp; // Aplicativo PowerPoint
        private Presentation pptPresentation; // Apresentação atual

        public DisplayWindow()
        {
            InitializeComponent();
            SlideImages = new List<BitmapImage>();
            CurrentSlideIndex = 0;
            this.SizeChanged += DisplayWindow_SizeChanged;
            InitializePowerPoint(); // Inicializando o PowerPoint
        }
        private void InitializePowerPoint()
        {
            try
            {
                pptApp = new Application();
                pptApp.Visible = MsoTriState.msoFalse; // PowerPoint invisível
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Erro ao inicializar o PowerPoint: " + ex.Message);
            }
        }

        private void DisplayWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustContentSize();
        }

        private void AdjustContentSize()
        {
            double windowWidth = this.ActualWidth;
            double windowHeight = this.ActualHeight;

            if (DisplayedVideo.Visibility == Visibility.Visible && DisplayedImage1.Visibility == Visibility.Visible)
            {
                DisplayedImage1.Width = windowWidth / 2;
                DisplayedImage1.Height = windowHeight;
                DisplayedVideo.Width = windowWidth / 2;
                DisplayedVideo.Height = windowHeight;
                Grid.SetColumnSpan(DisplayedImage1, 1);
                Grid.SetColumnSpan(DisplayedVideo, 1);
                Grid.SetColumn(DisplayedImage1, 0);
                Grid.SetColumn(DisplayedVideo, 1);
            }
            else if (DisplayedVideo.Visibility == Visibility.Visible)
            {
                DisplayedVideo.Width = windowWidth;
                DisplayedVideo.Height = windowHeight;
                Grid.SetColumnSpan(DisplayedVideo, 2);
                Grid.SetColumn(DisplayedVideo, 0);
            }
            else if (DisplayedImage1.Visibility == Visibility.Visible && DisplayedImage2.Visibility == Visibility.Visible)
            {
                DisplayedImage1.Width = windowWidth / 2;
                DisplayedImage1.Height = windowHeight;
                DisplayedImage2.Width = windowWidth / 2;
                DisplayedImage2.Height = windowHeight;
            }
            else if (DisplayedImage1.Visibility == Visibility.Visible)
            {
                DisplayedImage1.Width = windowWidth;
                DisplayedImage1.Height = windowHeight;
                Grid.SetColumnSpan(DisplayedImage1, 2);
                Grid.SetColumn(DisplayedImage1, 0);
            }
        }

        public void DisplayImages(string filePath1, string filePath2)
        {
            // Troca os elementos de coluna
            if (DisplayedVideo.Visibility == Visibility.Visible)
            {
                Grid.SetColumn(DisplayedImage1, 0);
                Grid.SetColumn(DisplayedVideo, 2);
                Grid.SetColumn(DisplayedImage2, 1);
            }
            DisplayedVideo.Visibility = Visibility.Collapsed;
            DisplayedImage1.Source = new BitmapImage(new Uri(filePath1));
            DisplayedImage1.Visibility = Visibility.Visible;
            Grid.SetColumnSpan(DisplayedImage1, 1);
            DisplayedImage2.Source = new BitmapImage(new Uri(filePath2));
            DisplayedImage2.Visibility = Visibility.Visible;
            AdjustContentSize();
        }

        public void DisplayImage(string filePath)
        {
            DisplayedVideo.Visibility = Visibility.Collapsed;
            DisplayedImage1.Source = new BitmapImage(new Uri(filePath));
            DisplayedImage1.Visibility = Visibility.Visible;
            Grid.SetColumnSpan(DisplayedImage1, 2);
            DisplayedImage2.Visibility = Visibility.Collapsed;
            AdjustContentSize();
        }

        public void DisplayImageAndVideo(string imagePath, string videoPath)
        {
            DisplayedImage1.Visibility = Visibility.Visible;
            DisplayedVideo.Visibility = Visibility.Visible;
            DisplayedImage1.Source = new BitmapImage(new Uri(imagePath));
            DisplayedVideo.Source = new Uri(videoPath);
            AdjustContentSize();
            DisplayedVideo.Play();
        }

        public void LoadVideo(string filePath)
        {
            DisplayedImage1.Visibility = Visibility.Collapsed;
            DisplayedImage2.Visibility = Visibility.Collapsed;
            DisplayedVideo.Source = new Uri(filePath);
            DisplayedVideo.Visibility = Visibility.Visible;
            AdjustContentSize();
            DisplayedVideo.Play();
        }

        public void LoadPresentation(string filePath)
        {
            try
            {
                // Fechar apresentação anterior e liberar recursos
                ClosePresentation();

                // Recriando uma nova instância do PowerPoint
                InitializePowerPoint();

                // Abre a nova apresentação
                var pptPresentations = pptApp.Presentations;
                pptPresentation = pptPresentations.Open(filePath);

                // Converte os slides da apresentação em imagens
                SlideImages.Clear();
                foreach (Slide slide in pptPresentation.Slides)
                {
                    BitmapImage slideImage = ConvertSlideToImage(slide);
                    if (slideImage != null)
                    {
                        SlideImages.Add(slideImage);
                    }
                }

                // Exibe o primeiro slide
                if (SlideImages.Count > 0)
                {
                    DisplayedImage1.Source = SlideImages[0];
                    DisplayedImage1.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Erro ao carregar a apresentação: " + ex.Message);
            }
        }

        private BitmapImage ConvertSlideToImage(Slide slide)
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), $"slide_{slide.SlideIndex}.png");

                // Exportando o slide como imagem PNG
                slide.Export(tempPath, "PNG", 960, 540);

                if (File.Exists(tempPath))
                {
                    var bitmapImage = new BitmapImage(new Uri(tempPath));
                    return bitmapImage;
                }
                else
                {
                    throw new Exception($"Falha ao exportar o slide {slide.SlideIndex}.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao converter o slide: {ex.Message}");
                return null;
            }
        }



        // Método para fechar a apresentação atual
        public void ClosePresentation()
        {
            try
            {
                if (pptPresentation != null)
                {
                    // Fechar apresentação
                    pptPresentation.Close();
                    Marshal.ReleaseComObject(pptPresentation); // Libera o objeto COM
                    pptPresentation = null;

                    // Fechar o aplicativo PowerPoint
                    if (pptApp != null)
                    {
                        pptApp.Quit(); // Fecha o PowerPoint
                        Marshal.ReleaseComObject(pptApp); // Libera o objeto COM do PowerPoint
                        pptApp = null;
                    }

                    // Esconde a imagem exibida e volta a tela preta
                    DisplayedImage1.Visibility = Visibility.Collapsed;
                    DisplayedVideo.Visibility = Visibility.Collapsed;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao fechar a apresentação: " + ex.Message);
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
                Grid.SetColumnSpan(DisplayedImage1, 2);
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
            DisplayedVideo.Position = TimeSpan.Zero;
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
            try
            {
                if (!isFullScreen)
                {
                    this.WindowState = WindowState.Normal;
                    this.ResizeMode = ResizeMode.NoResize;
                    this.Top = 0;
                    this.Left = 0;
                    this.Width = SystemParameters.PrimaryScreenWidth;
                    this.Height = SystemParameters.PrimaryScreenHeight;
                    this.WindowState = WindowState.Maximized;
                    isFullScreen = true;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                    this.ResizeMode = ResizeMode.CanResize;
                    this.Width = 800;
                    this.Height = 600;
                    this.Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2;
                    this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
                    isFullScreen = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao alternar entre tela cheia e modo janela: " + ex.Message);
            }
        }

        public void UpdateLeftImage(string filePath)
        {
            if (DisplayedImage1 != null)
            {
                DisplayedImage1.Source = new BitmapImage(new Uri(filePath));
                DisplayedImage1.Visibility = Visibility.Visible;  // Torna a imagem visível
            }
        }

        // Método para atualizar a imagem da direita
        public void UpdateRightImage(string filePath)
        {
            if (DisplayedImage2 != null)
            {
                DisplayedImage2.Source = new BitmapImage(new Uri(filePath));
                DisplayedImage2.Visibility = Visibility.Visible;  // Torna a imagem visível
            }
        }

        // Método para trocar as imagens de posição
        public void SwapImagePositions()
        {
            if (DisplayedImage1 != null && DisplayedImage2 != null)
            {
                var temp = DisplayedImage1.Source;
                DisplayedImage1.Source = DisplayedImage2.Source;
                DisplayedImage2.Source = temp;
            }
        }

        // Método para esconder as imagens
        public void HideImages()
        {
            DisplayedImage1.Visibility = Visibility.Collapsed;
            DisplayedImage2.Visibility = Visibility.Collapsed;
        }

        public void SwapImageAndVideo()
        {
            if (DisplayedImage1.Visibility == Visibility.Visible && DisplayedVideo.Visibility == Visibility.Visible)
            {
                int imageColumn = Grid.GetColumn(DisplayedImage1);
                int videoColumn = Grid.GetColumn(DisplayedVideo);

                // Troca os elementos de coluna
                Grid.SetColumn(DisplayedImage1, videoColumn);
                Grid.SetColumn(DisplayedVideo, imageColumn);
            }
        }


    }
}
