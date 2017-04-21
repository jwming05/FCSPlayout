using FCSPlayout.Entities;
using FCSPlayout.WPF.Core;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.WPFApp
{
    public class PlaybillListViewModel: BindableBase
    {
        private List<PlaybillEntity> _playbills;
        private PlaybillEntity _selectedPlaybill;
         
        public PlaybillListViewModel()
        {

        }

        public IList<PlaybillEntity> Playbills
        {
            get
            {
                return _playbills;
            }
        }

        public PlaybillEntity SelectedPlaybill
        {
            get
            {
                return _selectedPlaybill;
            }

            set
            {
                _selectedPlaybill = value;
                OnPropertyChanged(() => this.SelectedPlaybill);
            }
        }

        private void LoadPlaybills()
        {
            using(var ctx=new FCSPlayout.PlayoutDbContext())
            {
                _playbills=ctx.Playbills.ToList();
            }
        }
    }
}
