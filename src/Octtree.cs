using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPL3D
{
	class Octtree<T> where T : Cube
    {
        private int level;
        private Cube bounds;
		private List<T> objects;
		private Octtree<T>[] nodes;
        private readonly int MAX_OBJECTS = 10;
        private readonly int MAX_LEVELS = 10;

		public Octtree(int level,Cube bounds)
        {
            this.level = level;
            this.bounds = bounds;
			objects = new List<T>();
			nodes = new Octtree<T>[8];
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
			int subWidth = (bounds.Width / 4);
			int subHeight = (bounds.Height / 4);
			int subDepth = (bounds.Depth / 4);
            int x = bounds.X;
            int y = bounds.Y;
			int z = bounds.Z;
			//Forward Top Left
			nodes[0] = new Octtree<T>(level + 1, new Cube(x - subWidth, y + subHeight, z+subDepth, subWidth, subHeight, subDepth));
			//Forward Top Right
			nodes[1] = new Octtree<T>(level + 1, new Cube(x + subWidth, y + subHeight, z+subDepth, subWidth, subHeight, subDepth));
			//Forward Bottom Left
			nodes[2] = new Octtree<T>(level + 1, new Cube(x - subWidth, y - subHeight, z+subDepth, subWidth, subHeight, subDepth));
			//Forward Bottom Right
			nodes[3] = new Octtree<T>(level + 1, new Cube(x + subWidth, y - subHeight, z+subDepth, subWidth, subHeight, subDepth));
			//Back Top Left
			nodes[4] = new Octtree<T>(level + 1, new Cube(x - subWidth, y + subHeight, z-subDepth, subWidth, subHeight, subDepth));
			//Back Top Right
			nodes[5] = new Octtree<T>(level + 1, new Cube(x + subWidth, y + subHeight, z-subDepth, subWidth, subHeight, subDepth));
			//Back Bottom Left
			nodes[6] = new Octtree<T>(level + 1, new Cube(x - subWidth, y - subHeight, z-subDepth, subWidth, subHeight, subDepth));
			//Back Bottom Right
			nodes[7] = new Octtree<T>(level + 1, new Cube(x + subWidth, y - subHeight, z-subDepth, subWidth, subHeight, subDepth));
        }
        /*
         * Determine which node the object belongs to. -1 means
         * object cannot completely fit within a child node and is part
         * of the parent node
         */
		private int GetIndex(T obj)
        {
            int index = -1;
            double verticalMidpoint = bounds.Y;
            double horizontalMidpoint = bounds.X;
			double depthicalMidpoint = bounds.Z;
            int subWidth = (obj.Width / 2);
            int subHeight = (obj.Height / 2);
			int subDepth = (obj.Depth / 2);
			//Object is above z=0
			bool forwardOctrant = (obj.Z - subDepth > depthicalMidpoint) && (obj.Z + subDepth > depthicalMidpoint);
			//Object in below z=0
			bool backOctrant = (obj.Z - subDepth < depthicalMidpoint) && (obj.Z + subDepth < depthicalMidpoint);
			//Object is above y=0
            bool topQuadrant = (obj.Y - subHeight > verticalMidpoint) && (obj.Y + subHeight > verticalMidpoint);
			//Object is below y=0
            bool bottomQuadrant = (obj.Y + subHeight < verticalMidpoint) && (obj.Y - subHeight < verticalMidpoint);
			//Object is below x=0
            bool leftQuadrant = (obj.X - subWidth < horizontalMidpoint) && (obj.X + subWidth < horizontalMidpoint);
			//Object is above x=0
            bool rightQuadrant = (obj.X - subWidth > horizontalMidpoint) && (obj.X + subWidth > horizontalMidpoint);
			if (backOctrant) {
				if (topQuadrant) {
					if (leftQuadrant) {
						index = 0;
					} else if (rightQuadrant) {
						index = 1;
					}
				} else if (bottomQuadrant) {
					if (leftQuadrant) {
						index = 2;
					} else if (rightQuadrant) {
						index = 3;
					}
				}
			} else if(forwardOctrant) {
				if (topQuadrant) {
					if (leftQuadrant) {
						index = 4;
					} else if (rightQuadrant) {
						index = 5;
					}
				} else if (bottomQuadrant) {
					if (leftQuadrant) {
						index = 6;
					} else if (rightQuadrant) {
						index = 7;
					}
				}
			}

            return index;
        }
        /*
         * Insert the object into the quadtree. If the node
         * exceeds the capacity, it will split and add all
         * objects to their corresponding nodes.
         */
		public void Insert(T obj)
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
		public List<T> Retrieve(List<T> returnObjects,T obj)
        {
            int index = GetIndex(obj);            
            if (index != -1 && nodes[0] != null)
            {
                nodes[index].Retrieve(returnObjects, obj);
            }
			foreach(T o in objects) {
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
			foreach(T obj in objects) 
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
