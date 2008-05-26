namespace Sexp {
    public abstract class Cell<T> {
        T m_contents;

        T value
        {
            get { return m_contents; }
            set { m_contents = value; }
        }

        public Cell() { }

        public Cell(T o)
        {
            m_contents = o;
        }
        
        public delegate object Thunk();

        public object bind(Thunk t, T o)
        {
            T m_saved = m_contents;
            m_contents = o;
            object rv = t();
            m_contents = m_saved;
            return rv;
        }
    }
}
