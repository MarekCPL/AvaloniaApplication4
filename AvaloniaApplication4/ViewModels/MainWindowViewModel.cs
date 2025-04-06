using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScottPlot;
using System;

namespace AvaloniaApplication4.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [RelayCommand]
        private void GeneratePlot()
        {
            // Dane zostaną wygenerowane w code-behind
        }
    }
}