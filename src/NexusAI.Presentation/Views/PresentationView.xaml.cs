using NexusAI.Presentation.ViewModels;

namespace NexusAI.Presentation.Views;

public partial class PresentationView : System.Windows.Controls.UserControl
{
    public PresentationView()
    {
        InitializeComponent();
    }

    private PresentationViewModel? ViewModel => DataContext as PresentationViewModel;
}
