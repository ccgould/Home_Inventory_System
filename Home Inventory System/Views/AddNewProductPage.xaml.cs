using CommunityToolkit.Maui.Views;

namespace Home_Inventory_System.Views;

public partial class AddNewProductPage : Popup
{

	public string Barcode;

	public AddNewProductPage()
	{
		InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

        if(string.IsNullOrWhiteSpace(this.ProductName.Text))
        {
            App.Current.MainPage.DisplayAlert("Missing Required Data","Poduct Name Cannot Be Empty,","OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Barcode))
        {
            App.Current.MainPage.DisplayAlert("Missing Required Data", "Please Scan item before continuing.,", "OK");
            return;
        }

        this.Close(this.ProductName.Text);
    }
}