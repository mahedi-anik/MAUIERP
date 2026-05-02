using Microsoft.Maui.Controls;
using Serilog;

namespace MAUIERP.BlazorUI;

public partial class App : Application
{
    public App()
    {
        Log.Debug("App constructor - Initializing Component");
        InitializeComponent();
        Log.Debug("App constructor - Initialized successfully");
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Log.Debug("CreateWindow called");
        var window = new Window(new MainPage())
        {
            Title = "MAUI ERP System"
        };
        Log.Debug($"Window created: Title={window.Title}");
        return window;
    }
}