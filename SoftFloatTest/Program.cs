using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SoftFloatTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			SoftFloat sum = SoftFloat.Zero;
			float sumFloat = 0;
			const int n = 100000000;

			SoftFloat two = SoftFloat.One + SoftFloat.One;
			for (int i = 0; i < n; i++)
			{
				//sum += SoftFloat.One;
				//if ((i & 0xFFFFF) == 0)
				//	sum = SoftFloat.Zero;
				sumFloat += 1f;
			}
			watch.Stop();
			Console.WriteLine(watch.Elapsed + " " + n / watch.Elapsed.TotalSeconds);
		}
	}
}
