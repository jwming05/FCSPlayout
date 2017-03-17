using System;

namespace FCSPlayout.Domain
{
    public abstract class PasswordProcessor
    {
        private static PasswordProcessor _current;

        public static PasswordProcessor Current
        {
            get
            {
                return _current ?? NullPasswordProcessor.Instance;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                _current = value;
            }
        }

        public abstract string Encrypt(string password);
        public abstract string Decrypt(string text);
    }

    internal class NullPasswordProcessor : PasswordProcessor
    {
        private readonly static NullPasswordProcessor _instance = new NullPasswordProcessor();

        public static NullPasswordProcessor Instance { get { return _instance; } }
        private NullPasswordProcessor()
        {

        }
        public override string Decrypt(string text)
        {
            return text;
        }

        public override string Encrypt(string password)
        {
            return password;
        }
    }
}
