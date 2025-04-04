using System;

namespace qASIC.Parsing
{
    public class EnumParser : ValueParser
    {
        public override Type ValueType => typeof(Enum);

        public override bool TypeValid(Type type) =>
            type.IsEnum;

        public override string ConvertToString(object obj) =>
            obj?.ToString() ?? string.Empty;

        public override bool TryParse(Type type, string s, out object obj)
        {
            obj = null;
            if (type == ValueType)
                return false;

            var vals = Enum.GetValues(type);
            foreach (var item in vals)
            {
                if (item.ToString() != s) continue;
                obj = item;
                return true;
            }

            return false;
        }
    }

    public class BoolParser : ValueParser<bool>
    {
        public override bool TryParse(string s, out bool result)
        {
            switch (s?.ToLower() ?? string.Empty)
            {
                case "true":
                case "1":
                case "yes":
                case "on":
                    result = true;
                    return true;
                case "false":
                case "0":
                case "no":
                case "off":
                    result = false;
                    return true;
                default:
                    result = false;
                    return false;
            }
            ;
        }
    }

    public class ByteParser : ValueParser<byte>
    {
        public override bool TryParse(string s, out byte result) =>
            byte.TryParse(s, out result);
    }

    public class SByteParser : ValueParser<sbyte>
    {
        public override bool TryParse(string s, out sbyte result) =>
            sbyte.TryParse(s, out result);
    }

    public class IntParser : ValueParser<int>
    {
        public override bool TryParse(string s, out int result) =>
            int.TryParse(s, out result);
    }

    public class UIntParser : ValueParser<uint>
    {
        public override bool TryParse(string s, out uint result) =>
            uint.TryParse(s, out result);
    }

    public class ShortParser : ValueParser<short>
    {
        public override bool TryParse(string s, out short result) =>
            short.TryParse(s, out result);
    }

    public class UShortParser : ValueParser<ushort>
    {
        public override bool TryParse(string s, out ushort result) =>
            ushort.TryParse(s, out result);
    }

    public class FloatParser : ValueParser<float>
    {
        public override bool TryParse(string s, out float result)
        {
            s = s?.ToLower() ?? string.Empty;

            if (s.EndsWith("f"))
                s = s.Substring(0, s.Length - 1);

            return float.TryParse(s, out result);
        }
    }

    public class DoubleParser : ValueParser<double>
    {
        public override bool TryParse(string s, out double result) =>
            double.TryParse(s, out result);
    }

    public class DecimalParser : ValueParser<decimal>
    {
        public override bool TryParse(string s, out decimal result) =>
            decimal.TryParse(s, out result);
    }

    public class LongParser : ValueParser<long>
    {
        public override bool TryParse(string s, out long result) =>
            long.TryParse(s, out result);
    }

    public class UlongParser : ValueParser<ulong>
    {
        public override bool TryParse(string s, out ulong result) =>
            ulong.TryParse(s, out result);
    }

    public class CharParser : ValueParser<char>
    {
        public override bool TryParse(string s, out char result) =>
            char.TryParse(s, out result);
    }

    public class StringParser : ValueParser<string>
    {
        public override bool TryParse(string s, out string result)
        {
            result = s;
            return s != null;
        }
    }
}