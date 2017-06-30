using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.interfaces;
using Cybers.Infrustructure.models;
using Prism.Mvvm;

namespace Cybers.Infrustructure.controls.UserDetails
{
    public class UserDetailsViewModel: BindableBase, IViewModel
    {
        private User _user;

        public User User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public UserDetailsViewModel(User user)
        {
            User = user;
        }
    }
}
