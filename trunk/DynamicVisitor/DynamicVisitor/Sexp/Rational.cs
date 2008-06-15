using System;

namespace Sexp {
    public class Rational :
        IConvertible,
        IComparable<long>,
        IComparable<float>,
        IComparable<double>,
        IComparable<Rational> {

        long m_numer;
        public long numer
        {
            get { return m_numer; }

            set
            {
                m_numer = value;
                reduce();
            }
        }

        long m_denom;
        public long denom
        {
            get { return m_denom; }

            set
            {
                if (value == 0) throw new DivideByZeroException();
                if (value < 0) throw new ArgumentOutOfRangeException();

                m_denom = value;
                reduce();
            }
        }

        public Rational(long numer, long denom)
        {
            this.m_numer = numer;
            this.m_denom = denom;
        }

        public static long gcd(long a, long b)
        {
            long r;

            while (b != 0) {
                r = a % b;
                a = b;
                b = r;
            }

            return a;
        }

        void reduce()
        {
            long d = gcd(m_numer, m_denom);
            m_numer /= d;
            m_denom /= d;
        }

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return m_numer != 0;
        }

        public byte ToByte(IFormatProvider provider)
        {
            return (byte)(m_numer / m_denom);
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return (decimal)(m_numer / m_denom);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return (double)(m_numer / m_denom);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return (short)(m_numer / m_denom);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return (int)(m_numer / m_denom);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return (long)(m_numer / m_denom);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return (sbyte)(m_numer / m_denom);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return (float)(m_numer / m_denom);
        }

        public string ToString(IFormatProvider provider)
        {
            return ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return (ushort)(m_numer / m_denom);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return (uint)(m_numer / m_denom);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return (ulong)(m_numer / m_denom);
        }

        #endregion

        public override string ToString()
        {
            return Literal.format(m_numer) + '/' + Literal.format(m_denom);
        }

        public override bool Equals(object obj)
        {
            if (obj is long) {
                return 1L == m_denom && (long)obj == m_numer;
            } else if (obj is float) {
                return ToSingle(null) == (float)obj;
            } else if (obj is double) {
                return ToDouble(null) == (double)obj;
            } else if (obj is Rational) {
                Rational cr = (Rational)obj;
                return cr.m_denom == m_denom && cr.m_numer == m_numer;
            } else {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IComparable<long> Members

        public int CompareTo(long other)
        {
            long remainder;
            long dividend = Math.DivRem(m_numer, m_denom, out remainder);

            if (dividend > other) {
                return 1;
            } else if (dividend < other) {
                return -1;
            } else if (remainder > 0) {
                return 1;
            } else {
                return 0;
            }
        }

        #endregion

        #region IComparable<float> Members

        public int CompareTo(float other)
        {
            return ToSingle(null).CompareTo(other);
        }

        #endregion

        #region IComparable<double> Members

        public int CompareTo(double other)
        {
            return ToDouble(null).CompareTo(other);
        }

        #endregion

        #region IComparable<Rational> Members

        public int CompareTo(Rational other)
        {
            return (m_numer*other.m_denom).CompareTo(other.numer*m_denom);
        }

        #endregion
    }
}
