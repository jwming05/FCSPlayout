using FCSPlayout.Domain;
using MPLATFORMLib;
using System;

namespace FCSPlayout.PlayEngine
{
    public interface IMPlaylistFactory
    {
        MPlaylistClass Create(MPlaylistSettings settings);
    }

    public abstract class MPlaylistFactory : IMPlaylistFactory
    {
        private static IMPlaylistFactory _current;

        public static IMPlaylistFactory Current
        {
            get
            {
                return _current ?? DefaultMPlaylistFactory.Instance;
            }

            set
            {
                if (_current != null)
                {
                    throw new InvalidOperationException();
                }

                _current = value;
            }
        }

        public abstract MPlaylistClass Create(MPlaylistSettings settings);
    }

    public class DefaultMPlaylistFactory : MPlaylistFactory
    {
        private static readonly DefaultMPlaylistFactory _instance = new DefaultMPlaylistFactory();

        public static DefaultMPlaylistFactory Instance
        {
            get
            {
                return _instance;
            }
        }

        public override MPlaylistClass Create(MPlaylistSettings settings)
        {
            var mplaylist = new MPlaylistClass();
            MPlaylistInitializer.Initialize(mplaylist, settings);
            return mplaylist;
        }
    }
}
