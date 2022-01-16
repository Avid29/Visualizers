using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Media.Render;
using Windows.Storage;

namespace Visualizers.UI.Shared.Audio.Rendering
{
    public class AudioGraphRenderer
    {
        private AudioGraph _graph;
        private AudioDeviceOutputNode _deviceOutNode;
        private AudioFrameOutputNode _frameOutNode;
        private AudioFileInputNode _fileInNode;

        public event EventHandler<float[]> FramePlayed;

        /// <summary>
        /// Initializes the <see cref="AudioRenderer"/>.
        /// </summary>
        /// <returns>An asynchronous task that returns a status indicating the initializion success.</returns>
        public async Task<bool> InitializeAsync()
        {
            var settings = new AudioGraphSettings(AudioRenderCategory.Media);
            settings.PrimaryRenderDevice = await GetDefaultRenderDevice();

            var status = true;

            status = await CreateGraph(settings);
            if (!status) return false;

            status = await CreateOutputNodes();
            if (!status) return false;

            return status;
        }

        public async void PlayFile(StorageFile file)
        {
            CreateAudioFileInputNodeResult result = await _graph.CreateFileInputNodeAsync(file);
            if (result.Status != AudioFileNodeCreationStatus.Success) return;
            _fileInNode = result.FileInputNode;

            _fileInNode.AddOutgoingConnection(_frameOutNode);
            _fileInNode.AddOutgoingConnection(_deviceOutNode);

            _fileInNode.Start();
            _frameOutNode.Start();
            _deviceOutNode.Start();
            _graph.Start();
        }

        private async Task<bool> CreateGraph(AudioGraphSettings settings)
        {
            var result = await AudioGraph.CreateAsync(settings);
            if (result.Status != AudioGraphCreationStatus.Success) return false;
            _graph = result.Graph;

            _graph.QuantumStarted += QuantumStarted;

            return true;
        }

        private async Task<bool> CreateOutputNodes()
        {
            if (_graph == null) return false;

            // Frame output
            _frameOutNode = _graph.CreateFrameOutputNode();

            // Device output
            CreateAudioDeviceOutputNodeResult result = await _graph.CreateDeviceOutputNodeAsync();
            if (result.Status != AudioDeviceNodeCreationStatus.Success) return false;
            _deviceOutNode = result.DeviceOutputNode;

            return true;
        }

        private async Task<DeviceInformation> GetDefaultRenderDevice()
        {
            string id = MediaDevice.GetDefaultAudioRenderId(AudioDeviceRole.Default);
            return await DeviceInformation.CreateFromIdAsync(id);
        }

        private void QuantumStarted(AudioGraph sender, object args)
        {
            ProcessFrameOutput(_frameOutNode.GetFrame());
        }

        private unsafe void ProcessFrameOutput(AudioFrame frame)
        {
            float[] dataInFloats;
            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out byte* dataInBytes, out uint capacityInBytes);

                float* dataInFloat = (float*)dataInBytes;
                dataInFloats = new float[capacityInBytes / sizeof(float)];

                for (int i = 0; i < capacityInBytes / sizeof(float); i++)
                {
                    dataInFloats[i] = dataInFloat[i];
                }
            }

            FramePlayed?.Invoke(null, dataInFloats);
        }
    }
}
