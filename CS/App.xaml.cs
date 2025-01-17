using CollectionViewWithActionButtons.Views;

namespace CollectionViewWithActionButtons;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        Routing.RegisterRoute("completeList", typeof(CompleteListPage));
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        return new Window(new AppShell());
    }
}

