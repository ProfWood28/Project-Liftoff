using System;

namespace GXPEngine.Core
{
	public struct Vector2
	{
		public float x;
		public float y;
		
		public Vector2 (float x, float y)
		{
			this.x = x;
			this.y = y;
		}
		
		override public string ToString() {
			return "[Vector2 " + x + ", " + y + "]";
		}

		public float Magnitude()
        {
			return Mathf.Abs(Mathf.Sqrt(x * x + y * y));
        }

		public Vector2 Normalize()
        {
			float magnitude = Magnitude();
			if (magnitude > 0)
			{
				return new Vector2(Mathf.Round(x / magnitude), Mathf.Round(y / magnitude));
			}
			else
            {
				//Console.WriteLine("Error: Magnitude is 0!");
				return new Vector2(0, 0);
			}
        }
	}
}

