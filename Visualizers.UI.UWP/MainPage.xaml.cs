using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Visualizers.UI.Shared.Audio.Rendering;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Visualizers.UI.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private AudioGraphRenderer _audioRenderer;

        public MainPage()
        {
            this.InitializeComponent();

            Init();
        }

        private async void Init()
        {
            _audioRenderer = new AudioGraphRenderer();
            await _audioRenderer.InitializeAsync();

            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".mp3");
            var file = await filePicker.PickSingleFileAsync();
            _audioRenderer.PlayFile(file);
        }
    }
}
