using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Cybers.Infrustructure.models
{
    public class UserAttribute : BindableBase
    {
        private string _value;
        private string _key;
        private double _numberValue;
        private bool _isSelected;

        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public string Key
        {
            get => _key;
            set => SetProperty(ref _key, value);
        }

        public double NumberValue
        {
            get => _numberValue;
            set => SetProperty(ref _numberValue, value);
        }


        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
