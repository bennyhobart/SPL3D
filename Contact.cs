using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPL3D
{
    public class Contact
    {
        public Vector3 normal;
        public float distance;
        public PhysicsBody x;
        public PhysicsBody y;



        public Contact(SpheresBody target,SpheresBody body,Vector3 direction,float distance)
        {
            this.x = target;
            this.y = body;
            normal = direction;
            this.distance = distance;

        }

        public Contact(TerrainBody target, Vector3 targetPoint, SpheresBody body, Vector3 lowestPoint)
        {
            x = target;
            y = body;
            normal = targetPoint - lowestPoint;
            distance = normal.Length();
            normal = -Vector3.UnitY;
        }
        public override String ToString()
        {
            return ("Normal :" + normal.ToString() + " Distance :" + distance);
        }
    }
}
