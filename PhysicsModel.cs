using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPL3D
{
    public class PhysicsModel
    {
        public PhysicsBody bodyDefinition;
        public float invmass;
        public float mass;
        public float linearDamp;
        public Vector3 velocity;
        public Vector3 forces;
        public Vector3 position;
        public float restitution;
        public List<Contact> contacts;
        public float friction;
        public int Width;
        public int Height;
        public object extraData;

        public void Initialize(float mass, float restitution, float linearDamp, float friction, Vector3 iPosition, Vector3 iVelocity, PhysicsBody bodyDefinition)
        {
            this.mass = mass;
            this.restitution = restitution;
            position = iPosition;
            velocity = iVelocity;
            this.bodyDefinition = bodyDefinition;
            if (bodyDefinition.GetType() == typeof(SpheresBody))
            {
                Height = 0;
                Width = 0;
                foreach (Sphere sphere in ((SpheresBody)bodyDefinition).spheres)
                {
                    int sphereWidth;
                    int sphereHeight;
                    if (sphere.position.X < 0)
                    {
                        sphereWidth = (int)(sphere.radius - sphere.position.X);
                    }
                    else
                    {
                        sphereWidth = (int)(sphere.radius + sphere.position.X);

                    }
                    if (sphere.position.Y < 0)
                    {
                        sphereHeight = (int)(sphere.radius - sphere.position.Y);
                    }
                    else
                    {
                        sphereHeight = (int)(sphere.radius + sphere.position.Y);
                    }
                    if (Height < sphereHeight)
                    {

                        Height = sphereHeight;
                    }
                    if (Width < sphereWidth)
                    {
                        Width = sphereWidth;
                    }
                }
            }
            else
            {
                TerrainBody target= (TerrainBody)bodyDefinition;
                Width = (int)(target.xzScale*2);
                Height = (int)(target.xzScale*2);
            }
            if (mass == 0)
            {
                invmass = 0;
            }
            else 
            {
                invmass = 1 / mass;
            }
            this.linearDamp = linearDamp;
            this.friction = friction;
            contacts = new List<Contact>();
        }

        public void ApplyForce(Vector3 force)
        {

            if (bodyDefinition.bodyType == BodyType.stationary || bodyDefinition.bodyType == BodyType.terrain)
            {
                return;
            }
            forces += (force);            
        }
        public void ApplyImpulse(Vector3 impulse)
        {
            if (bodyDefinition.bodyType == BodyType.stationary || bodyDefinition.bodyType == BodyType.terrain)
            {
                return;
            }
            velocity += (impulse);
        }
            
        public void Move(Vector3 amount)
        {
            if (bodyDefinition.bodyType == BodyType.stationary||bodyDefinition.bodyType==BodyType.terrain)
            {
                return;
            }
            position += (amount);
        }

    }
}
