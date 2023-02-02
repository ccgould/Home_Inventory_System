using CommunityToolkit.Maui.Views;
using Home_Inventory_System.ViewModels;
using SharpHook;

namespace Home_Inventory_System.Views;

public partial class MainPage : ContentPage
{
    private MainPageViewModel _vm;

    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
               
    }
}