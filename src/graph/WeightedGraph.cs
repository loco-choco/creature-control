using System;
using System.Collections.Generic;

namespace CreatureControl;

public class WeightedGraph<Node,Edge> 
  where Node : IEquatable<Node>
  where Edge : IComparable<Edge>
{
  private Dictionary<Node, GraphNode<Node,Edge> > graph;
  
  public WeightedGraph()
  {
    graph = new Dictionary<Node, GraphNode<Node,Edge> >();
  }
  
  public bool TryAddNode(Node node){
    if(graph.ContainsKey(node)) return false;

    graph.Add(node, new GraphNode<Node,Edge>());
    return true;
  }

  public bool TryAddEdge(Node node_a, Node node_b, Edge weight){
    if(!graph.TryGetValue(node_a, out var edges_node_a)) return false;
    if(!graph.TryGetValue(node_b, out var edges_node_b)) return false;
    if(edges_node_a.Edges.Exists((edge)=>edge.NodeA.Equals(node_b) || edge.NodeB.Equals(node_b))) return false;
    
    var edge = new GraphEdge<Node,Edge>(node_a, node_b, weight);
    graph[node_a].Edges.Add(edge);
    graph[node_b].Edges.Add(edge);
    return true;
  }

  public bool TryGetEdge(Node node_a, Node node_b, out GraphEdge<Node,Edge> edge){
    edge = null;
    if(graph.ContainsKey(node_a)) return false;
    
    edge = graph[node_a].Edges.Find((edge)=>edge.NodeA.Equals(node_b) || edge.NodeB.Equals(node_b));
    if(edge == null) return false;
    return true;
  }

  public bool TryGetEdgeList(Node node, out List<GraphEdge<Node,Edge>> edges) {
    if(graph.TryGetValue(node, out var node_info)){
      edges=node_info.Edges;
      return true;
    }
    edges = null;
    return false;
  }
  public List<Node> GetNodeList() => new List<Node>(graph.Keys);

  public void Visit(Node node) => graph[node].Visited = true;

  public bool IsVisited(Node node) => graph[node].Visited;

  public void ClearVisits(){
    foreach(var key_pairs in graph) key_pairs.Value.Visited = false;
  }
}

public class GraphNode<Node,Edge> 
  where Node : IEquatable<Node>
  where Edge : IComparable<Edge>
{
  public List<GraphEdge<Node, Edge>> Edges;
  public bool Visited;

  public GraphNode(){
    Edges = new();
    Visited = false;
  }
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
