using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FloatExplorer
{
    public class ShellViewModel:BindableBase
    {
        

        private string _selectedText;
        private readonly DelegateCommand _parseCommand;
        private readonly Dictionary<string, float> _floatConstants;
        private FloatUtils.FloatUnion _floatUnion;
        private byte _rawExponent;
        private uint _rawMantissa;

        public ShellViewModel()
        {
            _floatUnion = default(FloatUtils.FloatUnion);
            _floatConstants = new Dictionary<string, float>();
            var floatType = typeof(float);
            var fields = floatType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach(var field in fields)
            {
                if (field.IsLiteral && field.FieldType== floatType)
                {
                    _floatConstants.Add(field.Name, (float)field.GetValue(null));
                }
            }

            _parseCommand = new DelegateCommand(ExecuteParse, CanExecuteParse);
        }

        private bool CanExecuteParse()
        {
            return true;
        }

        private void ExecuteParse()
        {
            float floatValue;
            if (_floatConstants.ContainsKey(this.SelectedText))
            {
                floatValue = _floatConstants[this.SelectedText];
            }
            else
            {
                if(!float.TryParse(this.SelectedText, out floatValue))
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Parse Error!");
                    return;
                }
            }
            _floatUnion.FloatValue = floatValue;

            this.RawExponent = FloatUtils.GetRawExponent(_floatUnion);
            this.RawMantissa= FloatUtils.GetRawMantissa(_floatUnion);

            this.RaisePropertyChanged(nameof(this.FloatValue));
            this.RaisePropertyChanged(nameof(this.UintValue));
        }

        public IEnumerable<string> ConstantNames
        {
            get
            {
                return _floatConstants.Keys;
            }
        }

        public string SelectedText
        {
            get
            {
                return _selectedText;
            }

            set
            {
                _selectedText = value;
                RaisePropertyChanged(nameof(this.SelectedText));
            }
        }

        public uint UintValue
        {
            get { return _floatUnion.UintValue; }
        }

        public float FloatValue
        {
            get { return _floatUnion.FloatValue; }
        }
        public ICommand ParseCommand
        {
            get
            {
                return _parseCommand;
            }
        }

        public byte RawExponent
        {
            get
            {
                return _rawExponent;
            }

            private set
            {
                _rawExponent = value;
                this.RaisePropertyChanged(nameof(this.RawExponent));
            }
        }

        public uint RawMantissa
        {
            get
            {
                return _rawMantissa;
            }

            private set
            {
                _rawMantissa = value;
                this.RaisePropertyChanged(nameof(this.RawMantissa));
            }
        }
    }
}
