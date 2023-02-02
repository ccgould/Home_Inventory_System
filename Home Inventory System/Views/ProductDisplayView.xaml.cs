using Home_Inventory_System.ViewModels;
using SharpHook;

namespace Home_Inventory_System.Views;

public partial class ProductDisplayView : ContentPage
{
    private readonly ProductDisplayViewModel _vm;

    public ProductDisplayView(ProductDisplayViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        _vm = vm;
    }

    protected override async void OnAppearing()
    {
        var hook = new SimpleGlobalHook();
        hook.KeyReleased += Hook_KeyReleased;
        await hook.RunAsync();
        base.OnAppearing();
    }

    private void Hook_KeyReleased(object sender, KeyboardHookEventArgs e)
    {
        //Not used anymore
        //_vm.KeyPressed(e);
    }
}