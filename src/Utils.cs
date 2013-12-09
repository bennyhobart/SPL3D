using System;

namespace SPL3D
{
	public class Rectangle 
	{
		public int X,Y,Width,Height;
		public Rectangle(int X,int Y,int Width, int Height)
		{
			this.Width = Width;
			this.Height = Height;
			this.X = X;
			this.Y = Y;
		}

	}
	public class Vector3 {
		public static Vector3 UnitX { 
			get 
			{
				return new Vector3 (1, 0, 0);
			}
		}
		public static Vector3 UnitY { 
			get 
			{
				return new Vector3 (0, 1, 0);
			}
		}
		public static Vector3 UnitZ { 
			get 
			{
				return new Vector3 (0, 0, 1);
			}
		}
		public static Vector3 Zero {
			get {
				return new Vector3 (0, 0, 0);
			}
		}
		public float X;
		public float Y;
		public float Z;
		public Vector3(float X, float Y, float Z)
		{
			this.X = X;
			this.Y = Y;
			this.Z = Z;
		}
		public static float Dot(Vector3 a, Vector3 b) {
			return (a.X * b.X + a.Y * b.Y + a.Z * b.Z);
		}
		public float Length()
		{
			return (float)Math.Sqrt(Length2());
		}
		public float Length2() {
			return X * X + Y * Y + Z * Z;
		}
		public float Normalize()
		{
			float length = Length ();
			X = X / length;
			Y = Y / length;
			Z = Z / length;
			return length;
		}
		public static Vector3 operator +(Vector3 v1, Vector3 v2) {
			return new Vector3 (v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
		}
		public static Vector3 operator -(Vector3 v1) {
			return new Vector3 (-v1.X, -v1.Y, -v1.Z);
		}
		public static Vector3 operator -(Vector3 v1, Vector3 v2) {
			return new Vector3 (v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
		}
		public static Vector3 operator /(Vector3 v, float a) {
			return new Vector3 (v.X/a, v.Y /a, v.Z/a);
		}
		public static Vector3 operator /(float a,Vector3 v) {
			return new Vector3 (v.X/a, v.Y /a, v.Z/a);
		}
		public static Vector3 operator *(Vector3 v, float a) {
			return new Vector3 (v.X*a, v.Y*a, v.Z*a);
		}
		public static Vector3 operator *(float a,Vector3 v) {
			return new Vector3 (v.X*a, v.Y*a, v.Z*a);
		}


	}
	public enum BodyType {
		terrain,
		passive,
		kinematic,
		dynamic,
		stationary

	}
}
