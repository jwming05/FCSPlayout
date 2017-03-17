using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace FCSPlayout.WPF.Core
{
    public sealed class CustomInvokeCommandAction : TriggerAction<DependencyObject>
    {
        private string commandName;

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CustomInvokeCommandAction), null);

        public string CommandName
        {
            get
            {
                base.ReadPreamble();
                return this.commandName;
            }
            set
            {
                if (this.CommandName != value)
                {
                    base.WritePreamble();
                    this.commandName = value;
                    base.WritePostscript();
                }
            }
        }

        public ICommand Command
        {
            get
            {
                return (ICommand)base.GetValue(CustomInvokeCommandAction.CommandProperty);
            }
            set
            {
                base.SetValue(CustomInvokeCommandAction.CommandProperty, value);
            }
        }

        protected override void Invoke(object parameter)
        {
            if (base.AssociatedObject != null)
            {
                ICommand command = this.ResolveCommand();
                if (command != null && command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
        }

        private ICommand ResolveCommand()
        {
            ICommand result = null;
            if (this.Command != null)
            {
                result = this.Command;
            }
            else if (base.AssociatedObject != null)
            {
                Type type = base.AssociatedObject.GetType();
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                //PropertyInfo[] array = properties;
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo propertyInfo = properties[i];
                    if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType) && string.Equals(propertyInfo.Name, this.CommandName, StringComparison.Ordinal))
                    {
                        result = (ICommand)propertyInfo.GetValue(base.AssociatedObject, null);
                    }
                }
            }
            return result;
        }
    }
}
