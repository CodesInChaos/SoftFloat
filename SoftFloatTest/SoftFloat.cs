using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftFloatTest
{
	// Internal representation for finite normalized values corresponds to the IEEE binary32 representation
	public struct SoftFloat
	{
		private readonly uint _raw;

		internal SoftFloat(uint raw)
		{
			_raw = raw;
		}

		private uint RawMantissa { get { return _raw & 0x7FFFFF; } }
		public int Mantissa
		{
			get
			{
				if (RawExponent != 0)
				{
					if ((int)_raw >= 0)
						return (int)(RawMantissa | 0x800000);
					else
						return -(int)(RawMantissa | 0x800000);
				}
				else
				{
					if ((int)_raw >= 0)
						return (int)RawMantissa;
					else
						return -(int)RawMantissa;
				}
			}
		}

		public sbyte Exponent { get { return (sbyte)(RawExponent - ExponentBias); } }

		private byte RawExponent { get { return (byte)(_raw >> MantissaBits); } }


		private const uint SignMask = 0x80000000;
		private const int MantissaBits = 23;
		private const int ExponentBias = 127;

		private const uint RawZero = 0;
		private const uint RawNaN = 0xFFC00000;//same as float.NaN
		private const uint RawPositiveInfinity = 0x7F800000;
		private const uint RawNegativeInfinity = RawPositiveInfinity ^ SignMask;
		private const uint RawOne = 0x3F800000;
		private const uint RawMinusOne = RawOne ^ SignMask;

		public static SoftFloat Zero { get { return new SoftFloat(); } }
		public static SoftFloat PositiveInfinity { get { return new SoftFloat(RawPositiveInfinity); } }
		public static SoftFloat NegativeInfinity { get { return new SoftFloat(RawNegativeInfinity); } }
		public static SoftFloat NaN { get { return new SoftFloat(RawNaN); } }
		public static SoftFloat One { get { return new SoftFloat(RawOne); } }
		public static SoftFloat MinusOne { get { return new SoftFloat(RawMinusOne); } }

		public override string ToString()
		{
			return ((float)this).ToString();
		}

		public static explicit operator SoftFloat(float f)
		{
			uint raw = ReinterpretFloatToInt32(f);
			return new SoftFloat(raw);
		}

		public static explicit operator float(SoftFloat f)
		{
			uint raw = f._raw;
			return ReinterpretIntToFloat32(raw);
		}

		public static SoftFloat operator -(SoftFloat f)
		{
			return new SoftFloat(f._raw ^ 0x80000000);
		}

		public bool IsFinite()
		{
			return RawExponent != 255;
		}

		public static SoftFloat operator +(SoftFloat f1, SoftFloat f2)
		{
			byte rawExp1 = f1.RawExponent;
			byte rawExp2 = f2.RawExponent;
			int deltaExp = rawExp1 - rawExp2;
			if (deltaExp >= 0)
			{
				if (rawExp1 != 255)
				{//Finite
					if (deltaExp > 25)
						return f1;
					int man1;
					int man2;
					if (rawExp2 != 0)
					{
						//man1 = f1.Mantissa
						//http://graphics.stanford.edu/~seander/bithacks.html#ConditionalNegate
						uint sign1 = (uint)((int)f1._raw >> 31);
						man1 = (int)(((f1.RawMantissa | 0x800000) ^ sign1) - sign1);
						//man2 = f2.Mantissa
						uint sign2 = (uint)((int)f2._raw >> 31);
						man2 = (int)(((f2.RawMantissa | 0x800000) ^ sign2) - sign2);
					}
					else
					{//Subnorm
						//man1 = f1.Mantissa
						uint sign1 = (uint)((int)f1._raw >> 31);
						man1 = (int)((f1.RawMantissa ^ sign1) - sign1);
						//man2 = f2.Mantissa
						uint sign2 = (uint)((int)f2._raw >> 31);
						man2 = (int)((f2.RawMantissa ^ sign2) - sign2);

						rawExp2 = 1;
						if (rawExp1 == 0)
							rawExp1 = 1;
						deltaExp = rawExp1 - rawExp2;
					}
					int man = (man1 << 6) + ((man2 << 6) >> deltaExp);
					uint absMan = (uint)Math.Abs(man);
					if (absMan == 0)
						return Zero;
					uint msb = absMan >> 23;
					int rawExp = rawExp1 - 6;
					while (msb == 0)
					{
						rawExp -= 8;
						absMan <<= 8;
					}
					int msbIndex= BitScanReverse8(msb);
					rawExp += msbIndex;
					absMan >>= msbIndex;
					if ((uint)(rawExp - 1) < 254)
					{
						uint raw = (uint)man & 0x80000000 | (uint)rawExp << 23 | (absMan & 0x7FFFFF);
						return new SoftFloat(raw);
					}
					else
					{
						if (rawExp >= 255)
						{//Overflow
							if (man >= 0)
								return PositiveInfinity;
							else
								return NegativeInfinity;
						}
						if (rawExp >= -24)
						{
							uint raw = (uint)man & 0x80000000 | absMan >> (-rawExp);
							return new SoftFloat(raw);
						}
						return Zero;
					}
				}
				else
				{//special

					if (rawExp1 != 0)//f2 is NaN, +Inf, -Inf and f1 is finite
						return f2;
					// Both not finite
					if (f1._raw == f2._raw)
						return f1;
					else
						return NaN;
				}

			}
			else
			{
				//ToDo manually write this code
				return f2 + f1;//flip operands
			}
		}

		public static SoftFloat operator -(SoftFloat f1, SoftFloat f2)
		{
			return f1 + (-f2);
		}

		public static SoftFloat operator *(SoftFloat f1, SoftFloat f2)
		{
			int man1;
			byte rawExp1 = f1.RawExponent;
			if (rawExp1 == 0)
			{//SubNorm
				man1 = f1.Mantissa;
				rawExp1 = 1;
			}
			else if (rawExp1 != 255)
			{//Norm
				man1 = f1.Mantissa;
			}
			else
			{//Non finite
				throw new NotImplementedException();
			}

			int man2;
			byte rawExp2 = f2.RawExponent;
			if (rawExp2 == 0)
			{//SubNorm
				man2 = f2.Mantissa;
				rawExp2 = 1;
			}
			else if (rawExp2 != 255)
			{//Norm
				man2 = f2.Mantissa;
			}
			else
			{//Non finite
				throw new NotImplementedException();
			}

			long longMan = (long)man1 * (long)man2;
			int man = (int)(longMan >> 23);
			uint absMan = (uint)Math.Abs(man);
			int rawExp = rawExp1 + rawExp2 - ExponentBias;
			if ((absMan & 0x1000000) != 0)
			{
				absMan >>= 1;
				rawExp++;
			}
			uint raw = (uint)man & 0x80000000 | (uint)rawExp << 23 | absMan & 0x7FFFFF;
			return new SoftFloat(raw);
		}

		private static readonly sbyte[] msb = new sbyte[256];
		private static int BitScanReverse8(uint b)
		{
			return msb[b];
		}

		private static unsafe uint ReinterpretFloatToInt32(float f)
		{
			return *((uint*)&f);
		}

		private static unsafe float ReinterpretIntToFloat32(uint i)
		{
			return *((float*)&i);
		}

		static SoftFloat()
		{
			//Init MostSignificantBit table
			for (int i = 0; i < 256; i++)
			{
				sbyte value = 7;//128-255
				if (i < 128)//64-127
					value = 6;
				if (i < 64)//32-63
					value = 5;
				if (i < 32)//16-31
					value = 4;
				if (i < 16)//8-15
					value = 3;
				if (i < 8)//4-7
					value = 2;
				if (i < 4)//2-3
					value = 1;
				if (i < 2)//1
					value = 0;
				if (i < 1)//0
					value = -1;
				msb[i] = value;
			}
		}
	}
}
