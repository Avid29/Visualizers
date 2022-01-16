using ComputeSharp;
using System;

namespace Visualizers
{
    [AutoConstructor]
    [EmbeddedBytecode(DispatchAxis.XY)]
    public partial struct TestShader : IPixelShader<float4>
    {
        private ReadOnlyBuffer<float> pcm;

        private float Sample(float u)
        {
            int length = pcm.Length;
            float x = u * length;

            int lowIndex = (int)x;
            int highIndex = (int)Math.Ceiling(x);
            float deci = x - lowIndex;

            float lowValue = pcm[lowIndex];
            float highValue = pcm[highIndex];
            float difference = highValue - lowValue;

            return lowValue + (difference * deci);
        }

        public float4 Execute()
        {
            float u = ThreadIds.Normalized.X;
            float v = ThreadIds.Normalized.Y;
            v = (v * -2) + 1;

            float point = Sample(u);

            float4 color = new float4(0, 0.1f, 0.8f, 1f);

            float4 pixel = float4.Zero;
            if (v == 0) pixel = float4.One * 0.5f;
            if (v < point + .01 && v > point - .01) pixel = color;
            return pixel;
        }
    }
}
