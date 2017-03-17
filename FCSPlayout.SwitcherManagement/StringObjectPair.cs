namespace FCSPlayout.SwitcherManagement
{
    /// <summary>
    /// Used for putting other object types into combo boxes.
    /// </summary>
    public struct StringObjectPair<T>
    {
        public string name;
        public T value;

        public StringObjectPair(string name, T value)
        {
            this.name = name;
            this.value = value;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
