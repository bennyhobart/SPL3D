using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPL3D
{	
    public class PhysicsBody
    {
        public BodyType bodyType;
        public PhysicsModel parent;
        public PhysicsBody(BodyType a, PhysicsModel p)
        {
            parent = p;
            bodyType = a;
        }
    }
}
