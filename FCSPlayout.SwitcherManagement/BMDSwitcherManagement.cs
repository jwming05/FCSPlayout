using BMDSwitcherAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.SwitcherManagement
{
    public class BMDSwitcherManagement
    {
        private IBMDSwitcherDiscovery m_switcherDiscovery;
        private IBMDSwitcher m_switcher;
        private IBMDSwitcherMixEffectBlock m_mixEffectBlock1;

        private SwitcherMonitor m_switcherMonitor;
        private List<InputMonitor> m_inputMonitors = new List<InputMonitor>();
        private MixEffectBlockMonitor m_mixEffectBlockMonitor;

        //private System.Threading.SynchronizationContext _synchContext;
        private List<StringObjectPair<long>> _programs = new List<StringObjectPair<long>>();
        private StringObjectPair<long> _currentProgram;
        private string _switcherName;
        private IBMDSwitcherAudioMixer m_audioMixer;

        private List<IBMDSwitcherAudioInput> _audioInputs = new List<IBMDSwitcherAudioInput>();

        public bool Connected { get; private set; }

        internal List<StringObjectPair<long>> Programs
        {
            get
            {
                return _programs;
            }
        }

        

        public BMDSwitcherManagement(string address)
        {
            //_synchContext = System.Threading.SynchronizationContext.Current;

            m_switcherMonitor = new SwitcherMonitor();
            m_switcherMonitor.SwitcherDisconnected += OnSwitcherDisconnected;


            m_mixEffectBlockMonitor = new MixEffectBlockMonitor();
            m_mixEffectBlockMonitor.ProgramInputChanged += OnProgramInputChanged;

            m_switcherDiscovery = new CBMDSwitcherDiscovery();

            if (m_switcherDiscovery == null)
            {
                return;
            }

            _BMDSwitcherConnectToFailure failReason = 0;
            try
            {
                // Note that ConnectTo() can take several seconds to return, both for success or failure,
                // depending upon hostname resolution and network response times, so it may be best to
                // do this in a separate thread to prevent the main GUI thread blocking.
                m_switcherDiscovery.ConnectTo(address, out m_switcher, out failReason);
            }
            catch (COMException)
            {
                // An exception will be thrown if ConnectTo fails. For more information, see failReason.
                switch (failReason)
                {
                    case _BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureNoResponse:
                        //MessageBox.Show("No response from Switcher", "Error");
                        break;
                    case _BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureIncompatibleFirmware:
                        //MessageBox.Show("Switcher has incompatible firmware", "Error");
                        break;
                    default:
                        //MessageBox.Show("Connection failed for unknown reason", "Error");
                        break;
                }
                return;
            }

            // Get the switcher name:
            string switcherName;
            m_switcher.GetProductName(out switcherName);
            _switcherName = switcherName;


            // Install SwitcherMonitor callbacks:
            m_switcher.AddCallback(m_switcherMonitor);

            m_switcher.IterateInput((i) =>
            {
                InputMonitor newInputMonitor = new InputMonitor(i);

                i.AddCallback(newInputMonitor);
                newInputMonitor.LongNameChanged += new SwitcherEventHandler(OnInputLongNameChanged);
                m_inputMonitors.Add(newInputMonitor);
            });

            // We want to get the first Mix Effect block (ME 1). We create a ME iterator,
            // and then get the first one:
            m_mixEffectBlock1 = m_switcher.GetFirstMixEffectBlock();

            if (m_mixEffectBlock1 != null)
            {
                m_mixEffectBlock1.AddCallback(m_mixEffectBlockMonitor);
                UpdatePrograms();
                this.Connected = true;
            }

            m_audioMixer = m_switcher.GetBMDSwitcherAudioMixer();

            m_audioMixer.IterateAudioInput(i => { _audioInputs.Add(i); });

            
        }

        public void SetAFVMixOption()
        {
            foreach (var audioInput in _audioInputs)
            {
                _BMDSwitcherAudioInputType audioInputType;
                audioInput.GetType(out audioInputType);
                if (audioInputType == _BMDSwitcherAudioInputType.bmdSwitcherAudioInputTypeEmbeddedWithVideo)
                {
                    audioInput.SetMixOption(_BMDSwitcherAudioMixOption.bmdSwitcherAudioMixOptionAudioFollowVideo);
                }

                //System.Diagnostics.Debug.WriteLine(ty);

                //_BMDSwitcherExternalPortType ty2;
                //ai.GetCurrentExternalPortType(out ty2);
                //System.Diagnostics.Debug.WriteLine(ty2);

            }
        }

        private void OnProgramInputChanged(object sender, object args)
        {
            UpdateCurrentProgram();
        }

        private void OnSwitcherDisconnected(object sender, object args)
        {
            // Remove all input monitors, remove callbacks
            foreach (InputMonitor inputMon in m_inputMonitors)
            {
                inputMon.Input.RemoveCallback(inputMon);
                inputMon.LongNameChanged -= new SwitcherEventHandler(OnInputLongNameChanged);
            }
            m_inputMonitors.Clear();

            if (m_mixEffectBlock1 != null)
            {
                // Remove callback
                m_mixEffectBlock1.RemoveCallback(m_mixEffectBlockMonitor);

                // Release reference
                m_mixEffectBlock1 = null;
            }

            if (m_switcher != null)
            {
                // Remove callback:
                m_switcher.RemoveCallback(m_switcherMonitor);

                // release reference:
                m_switcher = null;
            }

            this.Connected = false;
        }
        
        private void OnInputLongNameChanged(object sender, object args)
        {
            UpdatePrograms();
        }

        private void UpdatePrograms()
        {
            _programs.Clear();

            m_switcher.IterateInput((i)=> 
            {
                string inputName;
                long inputId;

                i.GetInputId(out inputId);
                i.GetLongName(out inputName);

                // Add items to list:
                _programs.Add(new StringObjectPair<long>(inputName, inputId));
            });
            

            UpdateCurrentProgram();
        }

        private void UpdateCurrentProgram()
        {
            long programId;

            m_mixEffectBlock1.GetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdProgramInput, out programId);

            // Select the program popup entry that matches the input id:
            foreach (StringObjectPair<long> item in _programs)
            {
                if (item.value == programId)
                {
                    this.CurrentProgram = item;
                    break;
                }
            }
        }

        public void SetProgramInput(string name)
        {
            StringObjectPair<long> program = _programs.First(i =>String.Equals(i.name,name,StringComparison.OrdinalIgnoreCase));
            SetProgramInput(program);
        }

        private void SetProgramInput(StringObjectPair<long> program)
        {
            if (m_mixEffectBlock1 != null)
            {
                if (!this.CurrentProgram.Equals(program))
                {
                    m_mixEffectBlock1.SetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdProgramInput,
                    program.value);
                    this.CurrentProgram = program;
                }
            }
        }

        internal event EventHandler CurrentProgramChanged;

        internal StringObjectPair<long> CurrentProgram
        {
            get
            {
                return _currentProgram;
            }

            set
            {
                _currentProgram = value;
                OnCurrentProgramChanged();
            }
        }

        private void OnCurrentProgramChanged()
        {
            if (CurrentProgramChanged != null)
            {
                CurrentProgramChanged(this, EventArgs.Empty);
            }
        }

        //private void SetAFVMixOption(IBMDSwitcherAudioInput audioInput)
        //{
        //    audioInput.SetMixOption(_BMDSwitcherAudioMixOption.bmdSwitcherAudioMixOptionAudioFollowVideo);
        //}
    }
}
