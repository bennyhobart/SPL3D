using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPL3D
{
    public class TerrainBody : PhysicsBody
    {
        public float[,] points;
        public float xzScale;
        public Vector3 up;
        
		public TerrainBody(PhysicsModel parent,float[,] points, float xzScale) : base(BodyType.terrain, parent)
        {
            bodyType = BodyType.terrain;
            this.points = points;
            this.up = new Vector3(0, 1, 0);
            this.xzScale = xzScale;
        }

        internal Vector3 GetClosestPoint(Vector3 lowestPoint)
        {
            float[,] segments = points;
            int i = (int)((0.5f + lowestPoint.X / xzScale / 2) * segments.GetLength(0));
            int j = (int)((0.5f + lowestPoint.Z / xzScale / 2) * segments.GetLength(1));
            if (i >= segments.GetLength(0) || j >= segments.GetLength(1) || j < 0 || i < 0)
            {
                return new Vector3(float.NaN, float.NaN, float.NaN);
            }

            return new Vector3(lowestPoint.X, segments[i, j], lowestPoint.Z);
        }
    }
}
