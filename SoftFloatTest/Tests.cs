using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SoftFloatTest
{
	class Tests
	{
		public void Assert(bool expr)
		{
			if (!expr)
			{
				Debugger.Break();
				throw new ApplicationException("Assert failed");
			}
		}

		public bool IsIdentical(SoftFloat f1, SoftFloat f2)
		{
			return f1.ToIeeeRaw() == f2.ToIeeeRaw();
		}

		public bool IsIdentical(SoftFloat f1, uint i)
		{
			return f1.ToIeeeRaw() == i;
		}

		private bool IsIdentical(SoftFloat f1, float f2)
		{
			return f1.ToIeeeRaw() == ((SoftFloat)f2).ToIeeeRaw();
		}

		public void Representation()
		{
			Assert(IsIdentical(SoftFloat.Zero, 0f));
			Assert(IsIdentical(-SoftFloat.Zero, -0f));
			Assert(!IsIdentical(SoftFloat.Zero, -SoftFloat.Zero));
			Assert(IsIdentical(SoftFloat.NaN, float.NaN));
			Assert(IsIdentical(SoftFloat.One, 1f));
			Assert(IsIdentical(SoftFloat.MinusOne, -1f));
			Assert(IsIdentical(SoftFloat.PositiveInfinity, float.PositiveInfinity));
			Assert(IsIdentical(SoftFloat.NegativeInfinity, float.NegativeInfinity));
			Assert(IsIdentical(SoftFloat.Epsilon, float.Epsilon));
			Assert(IsIdentical(SoftFloat.MaxValue, float.MaxValue));
			Assert(IsIdentical(SoftFloat.MinValue, float.MinValue));
		}

		public void Equality()
		{
			Assert(SoftFloat.NaN != SoftFloat.NaN);
			Assert(SoftFloat.NaN.Equals(SoftFloat.NaN));
			Assert(SoftFloat.Zero == -SoftFloat.Zero);
			Assert(SoftFloat.Zero.Equals(-SoftFloat.Zero));
			Assert(!(SoftFloat.NaN > SoftFloat.Zero));
			Assert(!(SoftFloat.NaN >= SoftFloat.Zero));
			Assert(!(SoftFloat.NaN < SoftFloat.Zero));
			Assert(!(SoftFloat.NaN <= SoftFloat.Zero));
			Assert(SoftFloat.NaN.CompareTo(SoftFloat.Zero) == -1);
			Assert(SoftFloat.NaN.CompareTo(SoftFloat.NegativeInfinity) == -1);
			Assert(!(-SoftFloat.Zero < SoftFloat.Zero));
		}


		public void Addition()
		{
			Assert(IsIdentical(SoftFloat.One + SoftFloat.One, 2f));
			Assert(IsIdentical(SoftFloat.One - SoftFloat.One, 0f));
		}

		public void Multiplication()
		{
			Assert(IsIdentical(SoftFloat.PositiveInfinity * SoftFloat.Zero, float.PositiveInfinity * 0f));
			Assert(IsIdentical(SoftFloat.PositiveInfinity * (-SoftFloat.Zero), float.PositiveInfinity * (-0f)));
			Assert(IsIdentical(SoftFloat.PositiveInfinity * SoftFloat.One, float.PositiveInfinity * 1f));
			Assert(IsIdentical(SoftFloat.PositiveInfinity * SoftFloat.MinusOne, float.PositiveInfinity * -1f));

			Assert(IsIdentical(SoftFloat.NegativeInfinity * SoftFloat.Zero, float.NegativeInfinity * 0f));
			Assert(IsIdentical(SoftFloat.NegativeInfinity * (-SoftFloat.Zero), float.NegativeInfinity * (-0f)));
			Assert(IsIdentical(SoftFloat.NegativeInfinity * SoftFloat.One, float.NegativeInfinity * 1f));
			Assert(IsIdentical(SoftFloat.NegativeInfinity * SoftFloat.MinusOne, float.NegativeInfinity * -1f));

			Assert(IsIdentical(SoftFloat.One * SoftFloat.One, 1f));
		}

		public Tests()
		{
			Representation();
			Equality();
			Addition();
			Multiplication();
		}

	}
}
