using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPL3D
{
    public class Sphere
    {
        public Vector3 position;
        public float radius;
        public Sphere(Vector3 position, float radius)
        {
            this.position = position;
            this.radius = radius;
        }
    }
}
