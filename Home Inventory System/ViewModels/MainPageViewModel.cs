using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Home_Inventory_System.Models;
using Home_Inventory_System.Views;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO.Ports;
using static Home_Inventory_System.Models.BarCodeLookup;
using System.Net;
using System.Runtime.Serialization;
using CommunityToolkit.Maui.Views;


namespace Home_Inventory_System.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        /// <summary>
        /// Used to send a Notification while Filter Changed
        /// </summary>
        internal delegate void FilterChanged();

        /// <summary>
        /// Gets or sets the value of FilterText and notifies user when value gets changed 
        /// </summary>
        public string? FilterText
        {
            get
            {
                return this.filtertext;
            }

            set
            {
                this.filtertext = value;
                this.OnFilterTextChanged();
                this.OnPropertyChanged("FilterText");
            }
        }

        [ObservableProperty]
        private ObservableCollection<object>? dataGridSelectedItems;
        string filterText;

        internal FilterChanged? Filtertextchanged;

        #region private variables

        private string? filtertext = string.Empty;
        private string? selectedcolumn = "All Columns";
        private string? selectedcondition = "Contains";

        private List<DateTime>? orderedDates;
        private Random random = new Random();


        #endregion

        #region MainGrid DataSource
        private string[] genders = new string[]
        {
            "Male",
            "Female",
            "Female",
            "Female",
            "Male",
            "Male",
            "Male",
            "Male",
            "Male",
            "Male",
            "Male",
            "Male",
            "Female",
            "Female",
            "Female",
            "Male",
            "Male",
            "Male",
            "Female",
            "Female",
            "Female",
            "Male",
            "Male",
            "Male",
            "Male"
        };

        private string[] firstNames = new string[]
        {
            "Kyle",
            "Gina",
            "Irene",
            "Katie",
            "Michael",
            "Oscar",
            "Ralph",
            "Torrey",
            "William",
            "Bill",
            "Daniel",
            "Frank",
            "Brenda",
            "Danielle",
            "Fiona",
            "Howard",
            "Jack",
            "Larry",
            "Holly",
            "Jennifer",
            "Liz",
            "Pete",
            "Steve",
            "Vince",
            "Zeke"
        };
        private string[] lastNames = new string[]
        {
            "Adams",
            "Crowley",
            "Ellis",
            "Gable",
            "Irvine",
            "Keefe",
            "Mendoza",
            "Owens",
            "Rooney",
            "Waddell",
            "Thomas",
            "Betts",
            "Doran",
            "Holmes",
            "Jefferson",
            "Landry",
            "Newberry",
            "Perez",
            "Spencer",
            "Vargas",
            "Grimes",
            "Edwards",
            "Stark",
            "Cruise",
            "Fitz",
            "Chief",
            "Blanc",
            "Perry",
            "Stone",
            "Williams",
            "Lane",
            "Jobs"
        };

        private string[] customerID = new string[]
        {
            "Alfki",
            "Frans",
            "Merep",
            "Folko",
            "Simob",
            "Warth",
            "Vaffe",
            "Furib",
            "Seves",
            "Linod",
            "Riscu",
            "Picco",
            "Blonp",
            "Welli",
            "Folig"
        };

        private string[] shipCountry = new string[]
        {
            "Argentina",
            "Austria",
            "Belgium",
            "Brazil",
            "Canada",
            "Denmark",
            "Finland",
            "France",
            "Germany",
            "Ireland",
            "Italy",
            "Mexico",
            "Norway",
            "Poland",
            "Portugal",
            "Spain",
            "Sweden",
            "UK",
            "USA",
        };

        public event PropertyChangedEventHandler? PropertyChanged;

        private Dictionary<string, string[]> shipCity = new Dictionary<string, string[]>();

        #endregion

        /// Gets or sets the value of OrdersInfo and notifies user when value gets changed
        [ObservableProperty]
        ObservableCollection<OrderInfo>? ordersInfo = new();
        public MySqlConnection connect;

        private SerialPort serialPort;
        private string _data;
        private string _currentMode = "0";
        private bool _isCreatingProduct;
        private AddNewProductPage _popup;

        public MainPageViewModel()
        {

            this.DataGridSelectedItems = new ObservableCollection<object>();

            CollectionData();


            try
            {
                serialPort = new SerialPort("COM3");
                serialPort.Open();
                serialPort.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            }
            catch (Exception)
            {
                //Show message
            }
        }



        private async void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Show all the incoming data in the port's buffer
            _data = serialPort.ReadExisting();

            if(!_isCreatingProduct)
            {
                var result = ordersInfo.FirstOrDefault(x => x.Barcode.Equals(_data));

                if (result is not null)
                {
                    IncrementItem(result);
                }
                else
                {
                    App.Current.MainPage.Dispatcher.Dispatch(async () =>
                    {
                        var anwser = await App.Current.MainPage.DisplayAlert("Product Not Known", "The Product you scanned is not in the database, would you like to add it?", "YES", "NO");

                        if (anwser)
                        {
                            await CreateProduct();
                        }
                    });
                 

                }
            }
            else
            {
                _popup.Barcode = _data;
            }
        }

        private void OpenConnection()
        {
            if(connect.State == ConnectionState.Closed)
            {
                connect.Open();
            }
        }

        [RelayCommand]
        private void CollectionData()
        {
            try
            {
                ordersInfo.Clear();
                Con();
                OpenConnection();


                MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM tbl_products", connect);
                DataTable data = new DataTable();
                adapter.Fill(data);

                foreach (DataRow row in data.Rows)
                {

                    var ord = new OrderInfo()
                    {
                        OrderID = Convert.ToInt32(row.ItemArray[0]),
                        Barcode = Convert.ToString(row.ItemArray[1]),
                        CustomerID = Convert.ToString(row.ItemArray[2]),
                        ShipCity = Convert.ToInt32(row.ItemArray[3]),
                    };
                    this.OrdersInfo.Add(ord);
                }
            }
            catch (Exception ex)
            {
                //TODO Show Error Message On Screen
                throw ex;
            }
            finally
            {
                connect.Close();
            }
        }

        private void IncrementItem(OrderInfo info)
        {
            if (connect.State == ConnectionState.Open) connect.Close();
            connect.Open();

            string updateQuery = $"UPDATE db_price_checker.tbl_products SET amount = \"{++info.ShipCity}\" WHERE id= {info.OrderID}";

            MySqlCommand insertCommand = new MySqlCommand(updateQuery, connect);

            if (insertCommand.ExecuteNonQuery() == 1)
            {
                //MessageBox.Show("Data Inserted");
            }
            else
            {
                //MessageBox.Show("Data Not Inserted");
            }

            connect.Close();
        }

        private void DecrementItem(OrderInfo info)
        {
            if (connect.State == ConnectionState.Open) connect.Close();
            connect.Open();

            string updateQuery = $"UPDATE db_price_checker.tbl_products SET amount = \"{--info.ShipCity}\" WHERE id= {info.OrderID}";

            MySqlCommand insertCommand = new MySqlCommand(updateQuery, connect);

            if (insertCommand.ExecuteNonQuery() == 1)
            {
                //MessageBox.Show("Data Inserted");
            }
            else
            {
                //MessageBox.Show("Data Not Inserted");
            }
        }

        public void Con()
        {
            connect = new MySqlConnection(
                "server=localhost; username=root; database=db_price_checker;password=wWw##4125");
        }

        private void AddToDataBase(string name)
        {
            if (connect.State == ConnectionState.Open) connect.Close();
            connect.Open();
            string insertQuery = "INSERT INTO tbl_products(barcode,label,amount) VALUES('" + _data + "','" + name + "','" + 1 + "')";

            MySqlCommand insertCommand = new MySqlCommand(insertQuery, connect);

            if (insertCommand.ExecuteNonQuery() == 1)
            {
                CollectionData();               
            }
            else
            {
                //MessageBox.Show("Data Not Inserted");
            }

            connect.Close();
        }

        [RelayCommand]
        async Task CreateProduct()
        {
            _popup = new AddNewProductPage();

            _isCreatingProduct = true;

            var result = await App.Current.MainPage.ShowPopupAsync(_popup);

            AddToDataBase((string)result);

            _isCreatingProduct = false;
        }

        private async Task<string> GetProductNameAsync()
        {

            var task = await Application.Current.MainPage.ShowPopupAsync(new AddNewProductPage());
            return (string)task;
        }

        /// <summary>
        /// used to decide generate records or not
        /// </summary>
        /// <param name="o">object type parameter</param>
        /// <returns>true or false value</returns>
        public bool FilerRecords(object o)
        {
            double res;

            var item = o as OrderInfo;

            if (item.OrderID!.ToString().ToLower().Contains(this.FilterText!.ToLower()) ||
                           item.CustomerID!.ToString().ToLower().Contains(this.FilterText.ToLower()) ||
                           item.ShipCity!.ToString().ToLower().Contains(this.FilterText.ToLower()))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Used to call the filter text changed()
        /// </summary>
        private void OnFilterTextChanged()
        {
            if (this.Filtertextchanged != null)
            {
                this.Filtertextchanged();
            }
        }
    }
}
