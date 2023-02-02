using Home_Inventory_System.Views;

namespace Home_Inventory_System;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(ProductDisplayView), typeof(ProductDisplayView));
        Routing.RegisterRoute(nameof(AddNewProductPage), typeof(AddNewProductPage));
    }
}
