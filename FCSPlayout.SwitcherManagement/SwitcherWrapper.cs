using BMDSwitcherAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.SwitcherManagement
{
    public class SwitcherWrapper
    {
        private static Dictionary<string, SwitcherWrapper> _cache=new Dictionary<string, SwitcherWrapper>();
        public static SwitcherWrapper Get(string address)
        {
            SwitcherWrapper result = null;
            if(!_cache.TryGetValue(address,out result))
            {
                result = new SwitcherWrapper(address);
                _cache.Add(address, result);
            }
            return result;
        }

        private string _address;
        private SwitcherWrapper(string address)
        {
            _address = address;
            EnsureManagerCreated();
            _manager.SetAFVMixOption();
        }

        private BMDSwitcherManagement _manager;
        
        public StringObjectPair<long> CurrentProgram
        {
            get { return _manager.CurrentProgram; }
        }

        public void SetProgramInput(string name)
        {
            EnsureManagerCreated();
            _manager.SetProgramInput(name);
        }

        private void EnsureManagerCreated()
        {
            if (_manager == null || !_manager.Connected)
            {
                if (_manager != null)
                {
                    _manager.CurrentProgramChanged -= Manager_CurrentProgramChanged;
                }
                _manager = new BMDSwitcherManagement(_address);
                _manager.CurrentProgramChanged += Manager_CurrentProgramChanged;
            }
        }

        public event EventHandler CurrentProgramChanged;

        private void Manager_CurrentProgramChanged(object sender, EventArgs e)
        {
            if (CurrentProgramChanged != null)
            {
                CurrentProgramChanged(this, EventArgs.Empty);
            }
        }

        //private static string GetAddress()
        //{
        //    return "192.168.10.240";
        //}

        public IEnumerable<Tuple<long, string>> GetPrograms()
        {
            EnsureManagerCreated();
            return _manager.Programs.Select(p => new Tuple<long, string>(p.value, p.name));
        }
    }
}
