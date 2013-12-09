using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPL3D
{
    public class SpheresBody : PhysicsBody
    {
        public List<Sphere> spheres;
      
        public SpheresBody(PhysicsModel model, bool passive) : base(BodyType.stationary, model)
        {
            if(!passive) 
            {
                this.bodyType = BodyType.dynamic;
            }
            spheres = new List<Sphere>();
            spheres.Add(new Sphere(Vector3.Zero,1));
        }

        public SpheresBody(PhysicsModel model, bool passive, float radius, Vector3 position)
            : base(BodyType.stationary, model)
        {
            if (!passive)
            {
                this.bodyType = BodyType.dynamic;
            }
            spheres = new List<Sphere>();
            spheres.Add(new Sphere(position, radius));
        }

        public SpheresBody(PhysicsModel model, bool passive, List<Sphere> spheres)
            : base(BodyType.stationary, model)
        {
            if (!passive)
            {
                this.bodyType = BodyType.dynamic;
            }
            this.spheres = spheres;
        }


    }
}
