using FCSPlayout.Domain;
using MPLATFORMLib;
using System;
using System.Runtime.InteropServices;

namespace FCSPlayout.PlayEngine
{

    public class MRendererManager : IDisposable
    {
        private MRendererClass _mrenderer;
        private MRendererSettings _mrendererInfo;
        //private int _instance;

        public bool ExternalKeyMode
        {
            get { return _mrendererInfo.ExternalKeyMode; }
        }
        public string VideoDevice
        {
            get { return _mrendererInfo.VideoDevice; }
        }

        public MRendererManager(MRendererSettings info/*, int instance*/)
        {
            //_instance = instance;

            _mrendererInfo = info;

            _mrenderer = new MRendererClass();
            if (!string.IsNullOrEmpty(this.VideoDevice))
            {
                _mrenderer.PropsSet("rate-control", "true");
                _mrenderer.DeviceSet("renderer", this.VideoDevice, "");
                if (this.ExternalKeyMode)
                {
                    _mrenderer.DeviceSet("renderer::keying", "external", "");
                }
            }

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
        }

        private void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.Dispose();
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public bool AttachVideoDevice(MPlaylistClass m_objPlaylist)
        {
            if (this._mrenderer == null)
            {
                return false;
            }

            try
            {
                string name = string.Empty;
                string parameters = string.Empty;
                int index = -1;
                this._mrenderer.DeviceGet("renderer", out name, out parameters, out index);
                if (!string.IsNullOrEmpty(name))
                {
                    this._mrenderer.ObjectStart(m_objPlaylist);
                }
            }
            catch (System.Exception ex)
            {
                this._mrenderer.ObjectClose();
                Marshal.ReleaseComObject(_mrenderer);
                _mrenderer = null;
                return false;
            }
            return true;
        }

        public bool DetachVideoDevice()
        {
            if (this._mrenderer != null)
            {
                this._mrenderer.ObjectClose();
                Marshal.ReleaseComObject(_mrenderer);
                _mrenderer = null;
            }
            return true;
        }

        public void Dispose()
        {
            if (this._mrenderer != null)
            {
                this._mrenderer.ObjectClose();
                Marshal.ReleaseComObject(_mrenderer);
                this._mrenderer = null;
            }
        }
    }
}
