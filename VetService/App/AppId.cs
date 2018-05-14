using System;

namespace VetService.App
{
    public class AppId : IConvertible, IEquatable<AppId>
    {
        public ApplicationRole Role { get; set; }

        public int Id { get; set; }

        public bool Equals(AppId other)
        {
            if (!Role.Equals(other.Role))
            {
                return false;
            }
            if (Id != other.Id)
            {
                return false;
            }
            return true;
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public string GetValue()
        {
            return Role.ToString() + "-" + Id;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            if ((Role.ToString() != null) && (Id > -1))
            {
                return true;
            }
            return false;
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(GetValue());
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(GetValue());
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(GetValue());
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(GetValue());
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(GetValue());
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(GetValue());
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(GetValue());
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(GetValue());
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(GetValue());
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(GetValue());
        }

        public string ToString(IFormatProvider provider)
        {
            return GetValue();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(GetValue(), conversionType);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(GetValue());
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(GetValue());
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(GetValue());
        }
    }
}
