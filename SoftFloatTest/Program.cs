using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SoftFloatTest
{
	class Program
	{
		static Random random = new Random();

		static uint RandomUInt32()
		{
			return (uint)random.Next(int.MinValue, int.MaxValue);
		}

		static void Main(string[] args)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			SoftFloat sum = SoftFloat.Zero;
			SoftFloat pro = (SoftFloat)1E30f;
			float sumFloat = 0;
			const int n = 1000000000;

			SoftFloat two = SoftFloat.One + SoftFloat.One;
			SoftFloat factor = (SoftFloat)1f;
			var x = SoftFloat.Epsilon + SoftFloat.Epsilon;
			for (int i = 0; i < n; i++)
			{
				//pro *= factor;
				//sum += factor;
				//sumFloat += 1f;
			}
			Console.WriteLine(""+pro+" "+sumFloat+" "+ sum);
			/*for (int i = 0; i < n; i++)
			{
				uint i1 = RandomUInt32();
				uint i2 = RandomUInt32();

				SoftFloat sf1 = SoftFloat.FromIeeeRaw(i1);
				SoftFloat sf2 = SoftFloat.FromIeeeRaw(i2);
				SoftFloat sf = sf1 * sf2;

				float f1 = (float)sf1;
				float f2 = (float)sf2;
				float f = f1 * f2;
				SoftFloat sfc = (SoftFloat)f;

				long error = SoftFloat.RawDistance(sf, sfc);
				if (error > 1)
				{
					Console.WriteLine(sf1.ToIeeeRaw() + " + " + sf2.ToIeeeRaw() +
						" is " + sf.ToIeeeRaw() +
						" expecting " + sfc.ToIeeeRaw() +
						" error=" + error);
				}
			}*/
			watch.Stop();
			Console.WriteLine(watch.Elapsed + " " + Math.Round(n / watch.Elapsed.TotalSeconds / 1000000, 2) + "M FLOPS");
			Console.ReadLine();
		}
	}
}
