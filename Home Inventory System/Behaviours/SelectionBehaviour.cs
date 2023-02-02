using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home_Inventory_System.Behaviours
{
    public class SelectionBehavior : Behavior<ContentPage>
    {

        private Syncfusion.Maui.DataGrid.SfDataGrid? datagrid;

        protected override void OnAttachedTo(ContentPage bindable)
        {
            this.datagrid = bindable.FindByName<Syncfusion.Maui.DataGrid.SfDataGrid>("dataGrid");
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            this.datagrid = null;
            base.OnDetachingFrom(bindable);
        }
    }
}
