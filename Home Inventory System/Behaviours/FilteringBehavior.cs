using Home_Inventory_System.ViewModels;

namespace Home_Inventory_System.Behaviours
{
    public class FilteringBehavior : Behavior<ContentPage>
    {
        private MainPageViewModel? viewModel;
        private Syncfusion.Maui.DataGrid.SfDataGrid? dataGrid;
        private SearchBar? filterText;
        /// <summary>
        /// Triggers while filter text was changed 
        /// </summary>
        /// <param name="sender">OnFilterTextChanged event sender</param>
        /// <param name="e">OnFilterTextChanged event args e</param>
        public void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue == null)
            {
                this.viewModel!.FilterText = string.Empty;
            }
            else
            {
                this.viewModel!.FilterText = e.NewTextValue;
            }
        }


        /// <summary>
        /// Refreshes the filter.
        /// </summary>
        public void OnFilterChanged()
        {
            if (this.dataGrid!.View != null)
            {
                this.dataGrid.View.Filter = this.viewModel!.FilerRecords;
                this.dataGrid.View.RefreshFilter();
            }
        }

        /// <summary>
        /// You can override this method to subscribe to AssociatedObject events and initialize properties.
        /// </summary>
        /// <param name="bindAble">SampleView type of bindAble</param>
        protected override void OnAttachedTo(ContentPage bindAble)
        {
            this.viewModel = new MainPageViewModel();
            this.dataGrid = bindAble.FindByName<Syncfusion.Maui.DataGrid.SfDataGrid>("dataGrid");
            bindAble.BindingContext = this.viewModel;
            this.filterText = bindAble.FindByName<SearchBar>("filterText");


            this.viewModel.Filtertextchanged = this.OnFilterChanged;
            this.filterText.TextChanged += this.OnFilterTextChanged!;
            base.OnAttachedTo(bindAble);
        }

        /// <summary>
        /// You can override this method while View was detached from window
        /// </summary>
        /// <param name="bindAble">SampleView type of bindAble parameter</param>
        protected override void OnDetachingFrom(ContentPage bindAble)
        {
            this.filterText!.TextChanged -= this.OnFilterTextChanged!;
            this.dataGrid = null;
            this.filterText = null;
            base.OnDetachingFrom(bindAble);
        }
    }
}
