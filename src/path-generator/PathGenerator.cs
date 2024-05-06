using System;
using System.Collections.Generic;

namespace CreatureControl;

public interface PathGenerator<Node,Edge> 
  where Node : IEquatable<Node>
  where Edge : IComparable<Edge>
{
  List<Node> CalculatePath(WeightedGraph<Node,Edge> graph, Node initial_node, Node target_node);
}
  
