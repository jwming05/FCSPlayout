using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCSPlayout.Entities;

namespace FCSPlayout.WPF.Core
{
    public class LoadPlaybillConfirmation:Confirmation
    {
        public IEnumerable<BindablePlaybill> Playbills { get; set; }

        public BindablePlaybill SelectedPlaybill { get; set; }
    }

    public class BindablePlaybill
    {
        private PlaybillEntity _entity;

        public Guid Id { get { return _entity.Id; } }

        public DateTime StartTime { get { return _entity.StartTime; } }
        public DateTime StopTime { get { return _entity.StopTime; } }
        public TimeSpan Duration { get { return TimeSpan.FromSeconds(_entity.Duration); } }


        public BindablePlaybill(PlaybillEntity entity)
        {
            this._entity = entity;
        }

        public PlaybillEntity Entity
        {
            get
            {
                return _entity;
            }
        }
    }
}
