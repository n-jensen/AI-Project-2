using System;
using System.Collections.Generic;

namespace AI_P2
{
    class Program
    {
        static Node[,] maze = new Node[5, 6];
        static List<Position> blockedPositions = new List<Position>
        {
            new Position
            {
                X = 1,
                Y = 1,
            },
            new Position
            {
                X = 2,
                Y = 1
            },
            new Position
            {
                X = 1,
                Y = 2,
            },
            new Position
            {
                X = 1,
                Y = 3
            },
            new Position
            {
                X = 2,
                Y = 3,
            },
            new Position
            {
                X = 1,
                Y = 4
            } 
        };

        static void Main(string[] args)
        {
            FillGraph();
            PrintGraph();
            Filtering(new List<int> { 0, 0, 0, 0 });
            PrintGraph();
            Prediction('W');
            PrintGraph();
            Filtering(new List<int> { 1, 1, 0, 1 });
            PrintGraph();
            Prediction('N');
            PrintGraph();
            Filtering(new List<int> { 1, 1, 0, 1 });
            PrintGraph();
        }
// Curtis' Code!
        static void PrintGraph()
        {
            // controls y-axis
            for (int i = 0; i < 6; i++)
            {
                //controls x-axis
                for (int j = 0; j < 5; j++)
                {
                    var node = maze[j, i];
                    if (node != null)
                    {
                        Console.Write($"{Math.Round(node.Probability * 100, 2)} ");
                    }
                    else
                    {
                        Console.Write("#### ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        // fills the graph full of the Node class
        static void FillGraph()
        {
            // controls y-axis
            for (int i = 0; i < 6; i++)
            {
                //controls x-axis
                for (int j = 0; j < 5; j++)
                {
                    Node node = new Node();
                    node.Position = new Position();
                    node.Position.X = j;
                    node.Position.Y = i;
                    node.Probability = 1.0 / 24.0;
                    maze[j, i] = node;
                }
            }
            FillGraphWithNulls();
        }

        // Puts nulls where the walls should be in the maze
        static void FillGraphWithNulls()
        {
            foreach(var position in blockedPositions)
            {
                maze[position.X, position.Y] = null;
            }
        }

        //.75 = sense obstacle, is obstacle
        //.25 = sense open, is obstacle;wrong
        //.8 = sense open, is open
        //.2 = sense obstacle, is open;wrong
        static void Filtering(List<int> sensor)
        {
            double total = 0;
            
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (maze[j, i] != null)
                    {
                        total += EvidenceConditionalProbability(maze[j, i], sensor);
                    }
                }
            }

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (maze[j, i] != null)
                    {
                        maze[j, i].Probability = maze[j, i].EvidenceProp / total;
                    }
                }
            }
        }


        // Returns the numerator for Bayes' theorem based of the probablty that the node is holding
        // and what the probability is of this node dectectiing walls or opens spaces based off the sensor.
        static double EvidenceConditionalProbability(Node node, List<int> sensor)
        {
            double p = 1;
            if(node.Position.X - 1 < 0 || maze[node.Position.X - 1, node.Position.Y] == null)
            {
                if(sensor[0] == 0)
                {
                    p *= .25;
                }
                else
                {
                    p *= .75;
                }
            }
            else
            {
                if (sensor[0] == 0)
                {
                    p *= .8;
                }
                else
                {
                    p *= .2;
                }
            }

            if(node.Position.X + 1 > 4 || maze[node.Position.X + 1, node.Position.Y] == null)
            {
                if (sensor[2] == 0)
                {
                    p *= .25;
                }
                else
                {
                    p *= .75;
                }
            }
            else
            {
                if (sensor[2] == 0)
                {
                    p *= .8;
                }
                else
                {
                    p *= .2;
                }
            }

            if (node.Position.Y - 1 < 0 || maze[node.Position.X, node.Position.Y - 1] == null)
            {
                if (sensor[1] == 0)
                {
                    p *= .25;
                }
                else
                {
                    p *= .75;
                }
            }
            else
            {
                if (sensor[1] == 0)
                {
                    p *= .8;
                }
                else
                {
                    p *= .2;
                }
            }

            if (node.Position.Y + 1 > 5 || maze[node.Position.X, node.Position.Y + 1] == null)
            {
                if (sensor[3] == 0)
                {
                    p *= .25;
                }
                else
                {
                    p *= .75;
                }
            }
            else
            {
                if (sensor[3] == 0)
                {
                    p *= .8;
                }
                else
                {
                    p *= .2;
                }
            }

            node.EvidenceProp = node.Probability * p;

            return node.EvidenceProp;
        }




//Noel's code!
        //returns the manhattan distance from a node to the target node
            //later: if manhattan != 1, the trans probability will automatically be 0
        static int Manhattan(Node target, Node current)
        {
            int md = Math.Abs(target.Position.X - current.Position.X) + Math.Abs(target.Position.Y - current.Position.Y);
            return md;
        }

        //handles transitional probability for westward motion
        static double West_Transitional_Probability(Node target_node, Node start_node)
        {
            //.7 = direction available
            //.15 = direction unavailable; bounce back
            //checking same box - for west movement, check all but east percentages (.7+.15+.15)
                //for north movement, check all but south (.7+.7+.15)
            //End result for one box = all trans probabilities added up

            double transitional_prob = 0;
            int manhattan = Manhattan(target_node, start_node);
            if (manhattan == 0)
            {
                //three walls = 1 * p
                //no walls = 0
                //walls on top, open rights and down = (.15 + 0 + 0) * p ...
                //west: if x-1 < 0 OR if strt location is null
                int x = start_node.Position.X;
                int y = start_node.Position.Y;
                double bounce = 0.0;
                //if there's a wall/obstacle to the west
                if (((x-1) < 0) || (maze[x-1,y] == null))
                {
                    bounce += .7;
                }
                //if there's a wall/obstacle to the north
                if (((y-1) < 0) || (maze[x, y-1] == null))
                {
                    bounce += .15;
                }
                //if there's a wall/obstacle to the south
                if (((y + 1) > 5) || (maze[x, y+1] == null))
                {
                    bounce += .15;
                }

                transitional_prob = bounce * maze[start_node.Position.X, start_node.Position.Y].Probability;
                
            }
            else if (manhattan == 1)
            {
                //to the right (east)
                if (start_node.Position.X < target_node.Position.X)
                {
                    transitional_prob = 0;
                }
                //to the left (west)
                else if (start_node.Position.X > target_node.Position.X)
                {
                    transitional_prob = .7 * maze[start_node.Position.X, start_node.Position.Y].Probability;
                }
                //upwards (north) or downwards (south)
                else if ((start_node.Position.Y < target_node.Position.Y) || (start_node.Position.Y > target_node.Position.Y))
                {
                    transitional_prob = .15 * maze[start_node.Position.X, start_node.Position.Y].Probability;
                }
            }
            else if (manhattan != 1)
            {
                transitional_prob = 0;
            }

            return transitional_prob;
        }

        //handles transitional probability for northward motion
        static double North_Transitional_Probability(Node target_node, Node start_node)
        {
            double transitional_prob = 0;
            int manhattan = Manhattan(target_node, start_node);
            if (manhattan == 0)
            {
                int x = start_node.Position.X;
                int y = start_node.Position.Y;
                double bounce = 0.0;
                //if there's a wall/obstacle to the North
                if (((y - 1) < 0) || (maze[x, y - 1] == null))
                {
                    bounce += .7;
                }
                //if there's a wall/obstacle to the West
                if (((x - 1) < 0) || (maze[x - 1, y] == null))
                {
                    bounce += .15;
                }
                //if there's a wall/obstacle to the East
                if (((x + 1) > 4) || (maze[x + 1, y] == null))
                {
                    bounce += .15;
                }

                transitional_prob = bounce * maze[start_node.Position.X, start_node.Position.Y].Probability;

            }
            else if (manhattan == 1)
            {
                //downwards (south)
                if (start_node.Position.Y < target_node.Position.Y)
                {
                    transitional_prob = 0;
                }
                //upwards (north)
                else if (start_node.Position.Y > target_node.Position.Y)
                {
                    transitional_prob = .7 * maze[start_node.Position.X, start_node.Position.Y].Probability;
                }
                //to the right (east) or leftwards (west)
                else if ((start_node.Position.X < target_node.Position.X) || (start_node.Position.X > target_node.Position.X))
                {
                    transitional_prob = .15 * maze[start_node.Position.X, start_node.Position.Y].Probability;
                }
            }
            else if (manhattan != 1)
            {
                transitional_prob = 0;
            }

            return transitional_prob;
        }

        //completes addition in transitional probability
        //pass in captial W or N to indicate west/north direction
        static void Prediction(char direction)
        {
            double probability = 0.0;
            var transitional_maze = new Node[5, 6];
            for(int i = 0; i < 6; i++)
            {
                for(int j = 0; j < 5; j++)
                {
                    if(maze[j,i] != null)
                    {
                        var oldNode = maze[j, i];
                        Node temp = new Node();
                        temp.EvidenceProp = oldNode.EvidenceProp;
                        temp.Probability = oldNode.Probability;
                        temp.Position = new Position();
                        temp.Position.X = oldNode.Position.X;
                        temp.Position.Y = oldNode.Position.Y;
                        transitional_maze[j, i] = temp;
                    }
                    else
                    {
                        transitional_maze[j, i] = null;
                    }
                }
            }

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Node target_node = maze[j, i];
                    if (target_node != null)
                    {
                        for (int c = 0; c < 6; c++)
                        {
                            for (int r = 0; r < 5; r++)
                            {
                                Node start_node = maze[r, c];
                                if (start_node != null && direction=='W')
                                {
                                    probability += West_Transitional_Probability(target_node, start_node);
                                }
                                else if (start_node != null && direction == 'N')
                                {
                                    probability += North_Transitional_Probability(target_node, start_node);
                                }
                            }
                        }
                        transitional_maze[j, i].Probability = probability;
                        probability = 0.0;
                    }

                }
            }
            maze = transitional_maze;
        }

    }
}
