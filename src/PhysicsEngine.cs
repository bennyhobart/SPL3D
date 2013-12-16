using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPL3D
{
    
    public class PhysicsEngine
    {
        enum CollisionType
        {
            Terrain,
            Sphere
        }
        List<Contact> contacts;
        List<PhysicsModel> bodies;
        private const int numberOfResolutionIterations = 8;
        private const float gravity= -9.8f;
		private Octtree<PhysicsModel> collisionTree;

        public PhysicsEngine()
        {
			Cube WorldBounds = new Cube (0, 0, 0, 10, 10, 10);
            bodies = new List<PhysicsModel>();
            contacts = new List<Contact>();
			collisionTree = new Octtree<PhysicsModel>(0,WorldBounds);
        }
        
        public void step(float dt){
            
            CheckForCollisions();
            ResolveCollisions();
            MoveObjects(dt);

        }

        private void ResolveCollisions()
        {
            for (int i = 0; i < numberOfResolutionIterations; ++i)
            {
                foreach (Contact contact in contacts)
                {
                    if (contact.x.bodyType == BodyType.passive || contact.y.bodyType == BodyType.passive)
                    {
                        continue;
                    }
                    ImpulseResolution(contact);
                    PositionalCorrection(contact);
                }
            }
            ApplyFriction();
        }

        private void ApplyFriction()
        {
            foreach (Contact contact in contacts)
            {
                //all terrain contacts have terrain as contact.x
                if (contact.x.bodyType == BodyType.terrain)
                {
                    PhysicsModel body = contact.y.parent;
                    float friction = Math.Min(contact.x.parent.friction, contact.y.parent.friction);
                    Vector3 normal = contact.normal;
                    Vector3 projection = Vector3.Dot(body.velocity, normal) * normal;
                    Vector3 direction = body.velocity - projection;
                    body.ApplyImpulse(-direction * friction);
                }
            }
        }

        private void PositionalCorrection(Contact contact)
        {
                const float percentageOfIntersection = 0.1f;
                float movedist = contact.distance*percentageOfIntersection;
                PhysicsModel body1 = contact.x.parent;
                PhysicsModel body2 = contact.y.parent;
                body1.Move(contact.normal*movedist);
                body2.Move(contact.normal*-movedist);
        }

        private void ImpulseResolution(Contact contact)
        {
           
                PhysicsModel body1 = contact.x.parent;
                PhysicsModel body2 = contact.y.parent;
                Vector3 relativeVelBefore = body2.velocity - body1.velocity;
                float contactVelocity = Vector3.Dot(contact.normal, relativeVelBefore);

                //nothing to solve
                if (contactVelocity < 0)
                {
                    return;
                }
                //Get Resutitution
                float restitution = Math.Min(body1.restitution, body2.restitution);
                //Get Impulse
                float impulseScalar = -(1 + restitution) * contactVelocity;
                

                //Apply Impulses

                body1.ApplyImpulse(-contact.normal*impulseScalar);
                body2.ApplyImpulse(contact.normal*impulseScalar);
        }

        private void MoveObjects(float dt)
        {
            foreach (PhysicsModel body in bodies)
            {  
                UpdateForces(body, dt);
                UpdateVelocity(body, dt);
                MoveBody(body, dt);
            }
        }

        private void UpdateForces(PhysicsModel body, float dt)
        {
            //Apply global forces suchas gravity linear damp etc...
            ApplyGravity(body, dt);
            ApplyLinearDamp(body, dt);
        }

        private void ApplyLinearDamp(PhysicsModel body, float dt)
        {
            body.ApplyImpulse(-body.velocity*(body.linearDamp));
        }

        private void ApplyGravity(PhysicsModel body, float dt)
        {
            body.ApplyImpulse(Vector3.UnitY*dt * (gravity));
        }

        private void MoveBody(PhysicsModel body, float dt)
        {

            body.Move(body.velocity*dt);
        }

        private void UpdateVelocity(PhysicsModel body, float dt)
        {
            Vector3 dv = body.forces * body.invmass;
            body.forces = Vector3.Zero;
            body.ApplyImpulse(dv*dt);
        }

        
        private void CheckForCollisions()
        {
            contacts.Clear();
            collisionTree.Clear();
            foreach(PhysicsModel body in bodies)
            {
                collisionTree.Insert(body);
                body.contacts.Clear();
            }
            List<PhysicsModel> possibleCollisions = new List<PhysicsModel>();
            foreach (PhysicsModel body in bodies)
            {
                possibleCollisions.Clear();
				possibleCollisions = BroadPhaseCollision(possibleCollisions,body);
                
                foreach (PhysicsModel target in possibleCollisions)
                {
                    Contact newContact = CheckCollision(target, body);
                    if (newContact != null)
                    {
                        contacts.Add(newContact);
                        target.contacts.Add(newContact);
                    }
                }
            }                        
        }

		private List<PhysicsModel> BroadPhaseCollision(List<PhysicsModel> possibleCollisions, PhysicsModel body)
        {
            if (body.bodyDefinition.bodyType == BodyType.passive || body.bodyDefinition.bodyType == BodyType.terrain || body.bodyDefinition.bodyType == BodyType.stationary)
            {
                return possibleCollisions;
            }
                
            collisionTree.Retrieve(possibleCollisions, body);
            possibleCollisions.Remove(body);
            return possibleCollisions;
        }



        private Contact CheckCollision(PhysicsModel target, PhysicsModel body)
        {
            
            CollisionType collisionType;
            Contact result = null;
            bool isTargetTerrain = false;

            //ASSUMPTION TERRAIN WON'T COLLIDE WITH OTHER TERRAIN!!!
            if (target.bodyDefinition.bodyType == BodyType.terrain)
            {
                isTargetTerrain=true;
                collisionType = CollisionType.Terrain;
            }
            else if (body.bodyDefinition.bodyType == BodyType.terrain) {
                isTargetTerrain=false;
                collisionType = CollisionType.Terrain;
            }
            else {
                collisionType = CollisionType.Sphere;
            }

            switch(collisionType) 
            {
                case CollisionType.Terrain:
                    TerrainBody a;
                    SpheresBody b;
                    if(isTargetTerrain)
                    {
                        a = (TerrainBody)target.bodyDefinition;
                        b = (SpheresBody)body.bodyDefinition;
                    } else {
                        b = (SpheresBody)target.bodyDefinition;
                        a = (TerrainBody)body.bodyDefinition;

                    }
                    result = CheckTerrainCollision(a, b);
                    break;

                case CollisionType.Sphere:
                    SpheresBody x = (SpheresBody)target.bodyDefinition;
                    SpheresBody y = (SpheresBody)body.bodyDefinition;
                    result = CheckSphereCollision(x, y);               
                    break;

                default:
                    break;
            }
            return result;            
        }

        private Contact CheckTerrainCollision(TerrainBody a, SpheresBody b)
        {
            Sphere s = GetLowestSphere(b);
            Vector3 up = a.up;
            Vector3 lowestPoint = s.position + b.parent.position - (Vector3.UnitY*s.radius);
            Vector3 targetPoint = a.GetClosestPoint(lowestPoint);
            
            Contact result = null;
            if (BelowPoint(lowestPoint,targetPoint))
            {
                result = new Contact(a, targetPoint, b, lowestPoint);
            }

            return result;
        }

       

        private bool BelowPoint(Vector3 a, Vector3 b)
        {
            
            if (a.Y < b.Y)
            {
                return true;
            }
            return false;
        }

        private Sphere GetLowestSphere(SpheresBody body)
        {
            Sphere lowest = body.spheres[0];
            List<Sphere> spheres = body.spheres;
            for (int i = 0; i < spheres.Count; ++i)
            {
                if (lowest.position.Y-lowest.radius > spheres[i].position.Y-spheres[i].radius)
                {
                    lowest = spheres[i];
                }
            }
            return lowest;
        }

        private Contact CheckSphereCollision(SpheresBody x, SpheresBody y)
        {
            Vector3 aTrans = x.parent.position;
            Vector3 bTrans = y.parent.position;
            foreach (Sphere s1 in x.spheres)
            {
                foreach (Sphere s2 in y.spheres)
                {
                    Vector3 aLoc = s1.position + aTrans;
                    Vector3 bLoc = s2.position + bTrans;
                    Vector3 direction = aLoc - bLoc;
					double mag2 = direction.Length2();
                    double rad2 = (s1.radius + s2.radius) * (s1.radius + s2.radius);
                    if (mag2 < rad2)
                    {
                        float temp = (float)(Math.Sqrt(rad2) - Math.Sqrt(mag2));
                        direction.Normalize();
                        if (direction == Vector3.Zero)
                        {
                            direction = Vector3.UnitY;
                        }
                        return new Contact(x,y,direction,temp);
                    }
                }
            }
            return null;
        }

        public void AddBody(PhysicsModel obj) 
        {
            bodies.Add(obj);
        }

        public void RemoveBody(PhysicsModel obj)
        {
            bodies.Remove(obj);
        }
        
    }
}
