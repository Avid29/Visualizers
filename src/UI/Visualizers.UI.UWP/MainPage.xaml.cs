using System;
using Visualizers.UI.Shared.Audio.Rendering;
using Visualizers.UI.Shared.Render;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace Visualizers.UI.UWP
{
    public sealed partial class MainPage : Page
    {
        private AudioGraphRenderer _audioRenderer;
        private VisualizerRunner _visualizerRunner;

        public MainPage()
        {
            this.InitializeComponent();

            _visualizerRunner = new VisualizerRunner();
            Init();
        }

        private async void Init()
        {
            _audioRenderer = new AudioGraphRenderer();
            await _audioRenderer.InitializeAsync();
            _visualizerRunner.AudioRenderer = _audioRenderer;

            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".mp3");
            var file = await filePicker.PickSingleFileAsync();
            _audioRenderer.PlayFile(file);
        }

        public VisualizerRunner VisualizerRunner => _visualizerRunner;
    }
}
