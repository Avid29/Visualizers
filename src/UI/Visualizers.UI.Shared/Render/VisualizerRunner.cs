using ComputeSharp;
using ComputeSharp.Uwp;
using System;
using Visualizers.UI.Shared.Audio.Rendering;

namespace Visualizers.UI.Shared.Render
{
    public class VisualizerRunner : IShaderRunner
    {
        private ReadOnlyBuffer<float> _audioBuffer;
        private AudioGraphRenderer _audioRenderer;

        public AudioGraphRenderer AudioRenderer
        {
            get => _audioRenderer;
            set
            {
                if (_audioRenderer != null) _audioRenderer.FramePlayed -= ReadFrame;

                _audioRenderer = value;
                _audioRenderer.FramePlayed += ReadFrame;
            }
        }

        public bool TryExecute(IReadWriteTexture2D<Float4> texture, TimeSpan timespan, object parameter)
        {
            if (_audioBuffer == null) return false;

            GraphicsDevice.Default.ForEach(texture, new TestShader(_audioBuffer));
            return true;
        }

        private void ReadFrame(object sender, float[] e)
        {
            if (e.Length > 0) _audioBuffer = GraphicsDevice.Default.AllocateReadOnlyBuffer(e);
        }
    }
}
