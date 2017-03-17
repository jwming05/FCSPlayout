using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class PlayoutSettings
    {
        private MRendererSettings _rendererSettings = new MRendererSettings();
        private MPlaylistSettings _playlistSettings; //=new MPlaylistSettings();

        public MRendererSettings RendererSettings
        {
            get
            {
                return _rendererSettings;
            }

            set
            {
                _rendererSettings = value;
            }
        }

        public MPlaylistSettings PlaylistSettings
        {
            get
            {
                if (_playlistSettings == null)
                {
                    _playlistSettings = new MPlaylistSettings();
                }
                return _playlistSettings;
            }

            set
            {
                _playlistSettings = value;
            }
        }
    }
}
