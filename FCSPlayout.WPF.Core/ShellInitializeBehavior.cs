using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace FCSPlayout.WPF.Core
{
    public class ShellInitializeBehavior:Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            ShellModelBase vm = this.AssociatedObject.DataContext as ShellModelBase;
            if (vm != null)
            {
                vm.LoginCommand.Execute();
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }
    }
}
