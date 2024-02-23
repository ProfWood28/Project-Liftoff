using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Core
{
	public struct Vector2Double
	{
		public double x;
		public double y;

		public Vector2Double(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		override public string ToString()
		{
			return "[Vector2Double " + x + ", " + y + "]";
		}

		public double Magnitude()
		{
			return Math.Abs(Math.Sqrt(x * x + y * y));
		}

		public Vector2Double Normalize()
		{
			double magnitude = Magnitude();
			if (magnitude > 0)
			{
				return new Vector2Double(Math.Round(x / magnitude), Math.Round(y / magnitude));
			}
			else
			{
				//Console.WriteLine("Error: Magnitude is 0!");
				return new Vector2Double(0, 0);
			}
		}
		public static Vector2Double operator +(Vector2Double a, Vector2Double b)
		{
			return new Vector2Double(a.x + b.x, a.y + b.y);
		}

		public static Vector2Double operator -(Vector2Double a, Vector2Double b)
		{
			return new Vector2Double(a.x - b.x, a.y - b.y);
		}

		public static Vector2Double operator *(Vector2Double vec, double scalar)
		{
			return new Vector2Double(vec.x * scalar, vec.y * scalar);
		}

		public static Vector2Double operator *(float scalar, Vector2Double vec)
		{
			return vec * scalar;
		}
		public static Vector2Double operator /(Vector2Double vec, double scalar)
		{
			return new Vector2Double(vec.x / scalar, vec.y / scalar);
		}

		public static Vector2Double operator /(float scalar, Vector2Double vec)
		{
			return vec / scalar;
		}
	}
}
