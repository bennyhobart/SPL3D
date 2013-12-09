using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPL3D
{
    class Quadtree
    {
        private int level;
        private Rectangle bounds;
        private List<PhysicsModel> objects;
        private Quadtree[] nodes;
        private readonly int MAX_OBJECTS = 10;
        private readonly int MAX_LEVELS = 10;  

        public Quadtree(int level,Rectangle bounds)
        {
            this.level = level;
            this.bounds = bounds;
            objects = new List<PhysicsModel>();
            nodes = new Quadtree[4];
        }
        public void Clear()
        {
            objects.Clear();
            for (int i = 0; i < 4; ++i)
            {
                if (nodes[i] != null)
                {
                    nodes[i].Clear();
                    nodes[i] = null;
                }
            }
        }
        private void Split()
        {
            int subWidth = (bounds.Width/4);
            int subHeight = (bounds.Height/4);
            int x = bounds.X;
            int y = bounds.Y;
            //Top Left
            nodes[0] = new Quadtree(level + 1, new Rectangle(x - subWidth, y + subHeight, subWidth, subHeight));
            //Top Right
            nodes[1] = new Quadtree(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
            //Bottom Left
            nodes[2] = new Quadtree(level + 1, new Rectangle(x - subWidth, y - subHeight, subWidth, subHeight));
            //Bottom Right
            nodes[3] = new Quadtree(level + 1, new Rectangle(x + subWidth, y - subHeight, subWidth, subHeight));
        }
        /*
         * Determine which node the object belongs to. -1 means
         * object cannot completely fit within a child node and is part
         * of the parent node
         */
        private int GetIndex(PhysicsModel obj)
        {
            int index = -1;
            double verticalMidpoint = bounds.Y;
            double horizontalMidpoint = bounds.X;
            int subWidth = (obj.Width / 2);
            int subHeight = (obj.Height / 2);

            // Object can completely fit within the top quadrants
            bool topQuadrant = (obj.position.Z - subHeight > verticalMidpoint) && (obj.position.Z + subHeight > verticalMidpoint);
            // Object can completely fit within the bottom quadrants
            bool bottomQuadrant = (obj.position.Z + subHeight < verticalMidpoint) && (obj.position.Z - subHeight < verticalMidpoint);
            // Object can completely fit within the left quadrants
            bool leftQuadrant = (obj.position.X - subWidth < horizontalMidpoint) && (obj.position.X + subWidth < horizontalMidpoint);
            // Object can completely fit within the right quadrants
            bool rightQuadrant = (obj.position.X - subWidth > horizontalMidpoint) && (obj.position.X + subWidth > horizontalMidpoint);
            if (topQuadrant)
            {
                if (leftQuadrant)
                {
                    index = 0;
                }
                else if (rightQuadrant)
                {
                    index = 1;
                }
            }
            else if (bottomQuadrant)
            {
                if (leftQuadrant)
                {
                    index = 2;
                }
                else if (rightQuadrant)
                {
                    index = 3;
                }
            }

            return index;
        }
        /*
         * Insert the object into the quadtree. If the node
         * exceeds the capacity, it will split and add all
         * objects to their corresponding nodes.
         */
        public void Insert(PhysicsModel obj)
        {
            if (nodes[0] != null)
            {
                int index = GetIndex(obj);

                if (index != -1)
                {
                    nodes[index].Insert(obj);

                    return;
                }
            }

            objects.Add(obj);

            if (objects.Count > MAX_OBJECTS && level<MAX_LEVELS)
            {
                if (nodes[0] == null)
                {
                    Split();
                }

                int i = 0;
                while (i < objects.Count)
                {
                    int index = GetIndex(objects[i]);
                    if (index != -1)
                    {
                        nodes[index].Insert(objects[i]);
                        objects.Remove(objects[i]);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }
        /*
         * Return all objects that could collide with the given object
         */
        public List<PhysicsModel> Retrieve(List<PhysicsModel> returnObjects,PhysicsModel obj)
        {
            int index = GetIndex(obj);            
            if (index != -1 && nodes[0] != null)
            {
                nodes[index].Retrieve(returnObjects, obj);
            }
            foreach(PhysicsModel o in objects) {
                returnObjects.Add(o);
            }
            
            return returnObjects;
        }
        public override string ToString()
        {
            String result = "\n";
            for (int i = 0; i < level; ++i)
            {
                result += "\t";
            }
            result += "level: " + level;
            result += " x: " + bounds.X + " y: " + bounds.Y;
            foreach(PhysicsModel obj in objects) 
            {
                result += "\n";
                for (int i = 0; i < level; ++i)
                {
                    result += "\t";
                }
                result += obj.ToString();
            }
            if (nodes[0] != null)
            {
                for (int i = 0; i < 4; ++i)
                {
                    result+=nodes[i].ToString();
                }
            }
            return result;
            
        }

        
    }

}
