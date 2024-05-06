using System;
using System.Collections.Generic;

namespace CreatureControl;

public class WeightedGraph<Node,Edge> 
  where Node : IEquatable<Node>
  where Edge : IComparable<Edge>
{
  private Dictionary<Node, List<GraphEdge<Node, Edge>> > graph;
  
  public WeightedGraph()
  {
    graph = new Dictionary<Node, List<GraphEdge<Node, Edge>> >();
  }
  
  public bool TryAddNode(Node node){
    if(graph.ContainsKey(node)) return false;

    graph.Add(node, new List<GraphEdge<Node, Edge>>());
    return true;
  }

  public bool TryAddEdge(Node node_a, Node node_b, Edge weight){
    if(!graph.TryGetValue(node_a, out var edges_node_a)) return false;
    if(!graph.TryGetValue(node_b, out var edges_node_b)) return false;
    if(edges_node_a.Exists((edge)=>edge.NodeA.Equals(node_b) || edge.NodeB.Equals(node_b))) return false;
    
    var edge = new GraphEdge<Node,Edge>(node_a, node_b, weight);
    graph[node_a].Add(edge);
    graph[node_b].Add(edge);
    return true;
  }

  public bool TryGetEdge(Node node_a, Node node_b, out GraphEdge<Node,Edge> edge){
    edge = null;
    if(graph.ContainsKey(node_a)) return false;
    
    edge = graph[node_a].Find((edge)=>edge.NodeA.Equals(node_b) || edge.NodeB.Equals(node_b));
    if(edge == null) return false;
    return true;
  }

  public bool TryGetEdgeList(Node node, out List<GraphEdge<Node,Edge>> edges) => graph.TryGetValue(node, out edges);
  public List<Node> GetNodeList() => new List<Node>(graph.Keys);
}

public class GraphEdge<Node,Edge> 
  where Node : IEquatable<Node>
  where Edge : IComparable<Edge>
{
  public Node NodeA;
  public Node NodeB;
  public Edge Weight;

  public GraphEdge(Node node_a, Node node_b, Edge edge_weight)
  {
    NodeA = node_a;
    NodeB = node_b;
    Weight = edge_weight;
  }
}
