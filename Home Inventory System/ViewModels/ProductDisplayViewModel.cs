using System.Data;
using System.IO.Ports;
using System.Net;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SharpHook;
using SharpHook.Native;
using static Home_Inventory_System.Models.BarCodeLookup;

namespace Home_Inventory_System.ViewModels
{
    public partial class ProductDisplayViewModel : ObservableObject
    {
        [ObservableProperty] private string productName = "Please Scan A Barcode";

        [ObservableProperty] private string productAmount = "0";
        [ObservableProperty] private string viewMode = "Add Mode";
        private SerialPort serialPort;

        public MySqlConnection connect;
        private string _currentMode = "0";
        IDispatcherTimer timer;
        private string _data;


        public ProductDisplayViewModel()
        {

            try
            {
                serialPort = new SerialPort("COM3");
                serialPort.Open();

                timer = Application.Current.Dispatcher.CreateTimer();
                timer.Interval = TimeSpan.FromMilliseconds(1000);
                timer.Tick += (s, e) =>
                {
                    if (serialPort.IsOpen)
                    {

                        var data = serialPort.ReadExisting();

                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            _data = data;

                            MySqlCommand cmd = new MySqlCommand();
                            string sql = "Select * from tbl_products where barcode = '" + data + "'; ";

                            MySqlDataReader reader;
                            Con();
                            connect.Open();
                            cmd.CommandText = sql;
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = connect;
                            reader = cmd.ExecuteReader();


                            switch (_currentMode)
                            {
                                case "0":
                                    AddItem(reader);
                                    break;
                                case "1":
                                    RemoveItem(reader);
                                    break;
                                case "2":
                                    GetInfo(reader);
                                    break;
                            }
                        }
                    }
                };
                timer.Start();
            }
            catch (Exception)
            {
                //Show message
            }
        }

        private void GetInfo(MySqlDataReader reader)
        {
            if (reader.Read())
            {
                ProductName = reader.GetString("label");
                ProductAmount = reader.GetString("amount");
                connect.Close();
            }
            else if (!reader.Read())
            {
                ProductName = "Item Not Found";
                ProductAmount = "0";
            }
        }

        private void AddItem(MySqlDataReader reader)
        {
            if (reader.Read())
            {
                ProductName = reader.GetString("label");
                ProductAmount = reader.GetString("amount");
                IncrementItem(reader.GetString("id"));
                connect.Close();
            }
            else if (!reader.Read())
            {
                ProductName = string.Empty;
                AddToDataBase(FindData());
            }
        }

        private void RemoveItem(MySqlDataReader reader)
        {
            if (reader.Read())
            {
                DecrementItem(reader.GetString("id"));
                connect.Close();
            }
        }

        private void IncrementItem(string id)
        {
            if (connect.State == ConnectionState.Open) connect.Close();
            connect.Open();
            
            var amount = Convert.ToInt32(productAmount);

            string updateQuery = $"UPDATE db_price_checker.tbl_products SET amount = \"{++amount}\" WHERE id= {int.Parse(id)}";

            ProductAmount = amount.ToString();

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

        private void DecrementItem(string id)
        {
            if (connect.State == ConnectionState.Open) connect.Close();
            connect.Open();

            var amount = Convert.ToInt32(productAmount);

            if (amount == 0) return;

            string updateQuery = $"UPDATE db_price_checker.tbl_products SET amount = \"{--amount}\" WHERE id= {int.Parse(id)}";

            ProductAmount = amount.ToString();

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

        private void AddToDataBase(RootObject rootObject)
        {
            if (connect.State == ConnectionState.Open) connect.Close();
            connect.Open();
            string insertQuery = "INSERT INTO tbl_products(barcode,label,amount) VALUES('" + _data + "','" + rootObject?.products[0].title.Replace("'", "''") + "','" + 1 + "')";

            MySqlCommand insertCommand = new MySqlCommand(insertQuery, connect);

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

        private RootObject FindData()
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    WebClient n = new WebClient();
                    string api_key = "ri4ej0d1oizike4g4dpprg8lm10dq6";
                    string url = $"https://api.barcodelookup.com/v3/products?barcode={_data}&formatted=y&key={api_key}";
                    var data = n?.DownloadString(url);
                    return JsonConvert.DeserializeObject<RootObject>(data);
                }
                catch (WebException ex)
                {
                    return null;
                }

            }
        }

        [RelayCommand]
        void ChangeMode(string mode)
        {
            _currentMode = mode;
            switch (_currentMode)
            {
                case "0":
                    ViewMode = "Add Mode";
                    break;
                case "1":
                    ViewMode = "Remove Mode";
                    break;
                case "2":
                    ViewMode = "View Mode";
                    break;
            }
        }
    }
}
