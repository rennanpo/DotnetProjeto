using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;

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
            displayWindow.Show();
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|Video files (*.mp4)|*.mp4|PowerPoint files (*.pptx)|*.pptx";
            openFileDialog.Multiselect = true; // Permitir seleção de múltiplos arquivos
            PreviousButton.Visibility = Visibility.Collapsed; 
            NextButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Collapsed;

            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileNames.Length == 1)
                {
                    var filePath = openFileDialog.FileNames[0];
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
                    }
                }
                else if (openFileDialog.FileNames.Length == 2)
                {
                    var filePath1 = openFileDialog.FileNames[0];
                    var filePath2 = openFileDialog.FileNames[1];
                    if ((filePath1.EndsWith(".png") || filePath1.EndsWith(".jpg") || filePath1.EndsWith(".jpeg")) && (filePath2.EndsWith(".png") || filePath2.EndsWith(".jpg") || filePath2.EndsWith(".jpeg")))
                    {
                        displayWindow.DisplayImages(filePath1, filePath2);
                    }
                    else
                    {
                        MessageBox.Show("Por favor, selecione duas imagens para exibição lado a lado.");
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
                    PauseButton.Content = "Play";
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
    }
}
