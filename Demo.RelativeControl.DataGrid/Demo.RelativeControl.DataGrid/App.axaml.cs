using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Demo.RelativeControl.DataGrid.Views;
using Demo.RelativeControl.ViewModels;

namespace Demo.RelativeControl;

public class App : Application {
    public override void Initialize() { AvaloniaXamlLoader.Load(this); }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow() {DataContext = new RelativeDataGridViewModel()};

        base.OnFrameworkInitializationCompleted();
    }
}