using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MultiScreenApp
{
    public partial class MainWindow : Window
    {
        private bool isPlaying;
        private DisplayWindow displayWindow;

        public MainWindow()
        {
            InitializeComponent();
            isPlaying = true;
            displayWindow = new DisplayWindow();

            // Abrir a DisplayWindow na tela secundária, se disponível
            var allScreens = System.Windows.Forms.Screen.AllScreens;
            if (allScreens.Length > 1)
            {
                var secondaryScreen = allScreens[1];
                displayWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                displayWindow.Left = secondaryScreen.WorkingArea.Left;
                displayWindow.Top = secondaryScreen.WorkingArea.Top;
                displayWindow.Width = secondaryScreen.WorkingArea.Width;
                displayWindow.Height = secondaryScreen.WorkingArea.Height;
                displayWindow.WindowState = WindowState.Maximized;
            }

            displayWindow.Show();

            // Adicionando o evento Closed para fechar a DisplayWindow
            this.Closed += (s, e) => displayWindow.Close();
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            //openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|Video files (*.mp4)|*.mp4|PowerPoint files (*.pptx)|*.pptx";
            openFileDialog.Multiselect = true; // Permitir seleção de múltiplos arquivos
            PreviousButton.Visibility = Visibility.Collapsed;
            NextButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Collapsed;
            CloseButton.Visibility = Visibility.Collapsed;
            hubimg.Visibility = Visibility.Collapsed;

            if (openFileDialog.ShowDialog() == true)
            {
                var filePaths = openFileDialog.FileNames;
                if (filePaths.Length == 1)
                {
                    var filePath = filePaths[0];
                    if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg") || filePath.EndsWith(".jpeg"))
                    {
                        displayWindow.DisplayImage(filePath);
                    }
                    else if (filePath.EndsWith(".mp4"))
                    {
                        displayWindow.LoadVideo(filePath);
                        PauseButton.Visibility = Visibility.Visible; // Mostra o botão de pausa
                    }
                    else if (filePath.EndsWith(".pptx"))
                    {
                        displayWindow.LoadPresentation(filePath);
                        PreviousButton.Visibility = Visibility.Visible; // Mostra os botões de navegação
                        NextButton.Visibility = Visibility.Visible; // Mostra os botões de navegação
                        CloseButton.Visibility = Visibility.Visible; // Mostra o botão fechar
                    }
                }
                else if (filePaths.Length == 2)
                {
                    string imagePath = null;
                    string videoPath = null;

                    // Verifica os tipos de arquivos selecionados
                    foreach (var filePath in filePaths)
                    {
                        if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg") || filePath.EndsWith(".jpeg"))
                        {
                            imagePath = filePath;
                        }
                        else if (filePath.EndsWith(".mp4"))
                        {
                            SwapButton.Visibility = Visibility.Visible;
                            PauseButton.Visibility = Visibility.Visible;
                            videoPath = filePath;
                        }
                        else
                        {
                            MessageBox.Show("Por favor, selecione apenas imagens e vídeos.");
                            return;
                        }
                    }

                    // Exibe dois arquivos de imagem
                    if (imagePath != null && videoPath == null)
                    {
                        displayWindow.DisplayImages(filePaths[0], filePaths[1]);
                        hubimg.Visibility = Visibility.Visible; // Exibe a área para imagens lado a lado
                    }
                    // Exibe uma imagem e um vídeo
                    else if (imagePath != null && videoPath != null)
                    {
                        displayWindow.DisplayImageAndVideo(imagePath, videoPath);
                        PauseButton.Visibility = Visibility.Visible; // Mostra o botão de pausa para o vídeo
                    }
                    else
                    {
                        MessageBox.Show("Selecione uma imagem e um vídeo, ou duas imagens.");
                    }
                }
                else
                {
                    MessageBox.Show("Selecione apenas até dois arquivos de imagem para exibição lado a lado.");
                }
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (displayWindow != null)
            {
                if (isPlaying)
                {
                    displayWindow.PauseVideo();
                    PauseButton.Content = "Play ";
                }
                else
                {
                    displayWindow.PlayVideo();
                    PauseButton.Content = "Pause";
                }
                isPlaying = !isPlaying;
            }
        }

        private void PreviousSlide_Click(object sender, RoutedEventArgs e)
        {
            if (displayWindow != null && displayWindow.CurrentSlideIndex > 0)
            {
                displayWindow.CurrentSlideIndex--;
                displayWindow.ShowSlide(displayWindow.CurrentSlideIndex);
            }
        }

        private void NextSlide_Click(object sender, RoutedEventArgs e)
        {
            if (displayWindow != null && displayWindow.CurrentSlideIndex < displayWindow.SlideImages.Count - 1)
            {
                displayWindow.CurrentSlideIndex++;
                displayWindow.ShowSlide(displayWindow.CurrentSlideIndex);
            }
        }

        private void ClosePresentationButton_Click(object sender, RoutedEventArgs e)
        {
            displayWindow.ClosePresentation(); // Chama o método para fechar a apresentação na DisplayWindow
        }

        // Novo método para alterar a imagem da esquerda
        private void ChangeLeftImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                displayWindow.UpdateLeftImage(filePath);
            }
        }

        // Novo método para alterar a imagem da direita
        private void ChangeRightImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                displayWindow.UpdateRightImage(filePath);
            }
        }

        // Novo método para trocar as imagens
        private void SwapImages_Click(object sender, RoutedEventArgs e)
        {
            displayWindow.SwapImagePositions();
        }

        private void SwapButton_Click(object sender, RoutedEventArgs e)
        {
            displayWindow.SwapImageAndVideo();
        }

    }
}