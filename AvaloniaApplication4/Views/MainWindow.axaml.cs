using Avalonia.Controls;
using Avalonia.Interactivity;
using ScottPlot;
using System;
using NAudio.Wave;

namespace AvaloniaApplication4.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializePlot();

            AmplitudeSlider.ValueChanged += UpdatePlot;
            FrequencySlider.ValueChanged += UpdatePlot;
            WaveformSelector.SelectionChanged += UpdatePlot;
        }

        private void InitializePlot()
        {
            PlotControl.Plot.Title("Generator Sygna³u");
            PlotControl.Plot.XLabel("Czas [s]");
            PlotControl.Plot.YLabel("Amplituda");
            UpdatePlot(null, null);
        }

        private void UpdatePlot(object? sender, EventArgs? e)
        {
            double amplitude = AmplitudeSlider.Value;
            double frequency = FrequencySlider.Value;
            string waveformType = (WaveformSelector.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Sinusoidalna";

            double[] xs = new double[1000];
            double[] ys = new double[1000];

            for (int i = 0; i < xs.Length; i++)
            {
                xs[i] = i * 0.001;
                ys[i] = waveformType switch
                {
                    "Sinusoidalna" => amplitude * Math.Sin(2 * Math.PI * frequency * xs[i]),
                    "Trójk¹tna" => amplitude * (2 * Math.Asin(Math.Sin(2 * Math.PI * frequency * xs[i]))) / Math.PI,
                    "Prostok¹tna" => amplitude * Math.Sign(Math.Sin(2 * Math.PI * frequency * xs[i])),
                    _ => 0
                };
            }

            PlotControl.Plot.Clear();
            var scatter = PlotControl.Plot.Add.Scatter(xs, ys);
            scatter.LineWidth = 2;
            scatter.Color = ScottPlot.Colors.Blue;

            PlotControl.Plot.Axes.AutoScale();
            PlotControl.Refresh();
        }

        private void OnPlaySoundClick(object? sender, RoutedEventArgs e)
        {
            double amplitude = AmplitudeSlider.Value;
            double frequency = FrequencySlider.Value;
            string waveformType = (WaveformSelector.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Sinusoidalna";

            PlaySignal(amplitude, frequency, waveformType);
        }

        public void PlaySignal(double amplitude, double frequency, string waveformType, int durationSeconds = 2)
        {
            int sampleRate = 44100;
            int sampleCount = sampleRate * durationSeconds;
            float[] buffer = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                double t = (double)i / sampleRate;
                double value = waveformType switch
                {
                    "Sinusoidalna" => amplitude * Math.Sin(2 * Math.PI * frequency * t),
                    "Trójk¹tna" => amplitude * (2 * Math.Asin(Math.Sin(2 * Math.PI * frequency * t))) / Math.PI,
                    "Prostok¹tna" => amplitude * Math.Sign(Math.Sin(2 * Math.PI * frequency * t)),
                    _ => 0,
                };

                buffer[i] = (float)Math.Clamp(value, -1.0, 1.0);
            }

            var waveFormat = new WaveFormat(sampleRate, 1);
            var waveProvider = new BufferedWaveProvider(waveFormat);
            byte[] byteBuffer = new byte[buffer.Length * sizeof(float)];
            Buffer.BlockCopy(buffer, 0, byteBuffer, 0, byteBuffer.Length);

            waveProvider.AddSamples(byteBuffer, 0, byteBuffer.Length);

            var waveOut = new WaveOutEvent();
            waveOut.Init(waveProvider);
            waveOut.Play();
        }
    }
}
