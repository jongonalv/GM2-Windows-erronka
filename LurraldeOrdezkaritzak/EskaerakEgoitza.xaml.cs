using LurraldeOrdezkaritzak.ViewModels;

namespace LurraldeOrdezkaritzak;

public partial class EskaerakEgoitza : ContentPage
{
    EgoitzaViewModel _viewModel;
    public EskaerakEgoitza()
	{
		InitializeComponent();
        _viewModel = new EgoitzaViewModel();
        BindingContext = _viewModel;
    }
}