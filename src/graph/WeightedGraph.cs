using System;
using System.Collections.Generic;

namespace CreatureControl;

public class WeightedGraph<T,U> 
  where T : IEquatable<T>
  where U : IComparable<U>
{
  private Dictionary<T, List<GraphEdge<T, U>> > graph;
  
  public WeightedGraph()
  {
    graph = new Dictionary<T, List<GraphEdge<T, U>> >();
  }
  
  public bool TryAddNode(T node){
    if(graph.ContainsKey(node)) return false;

    graph.Add(node, new List<GraphEdge<T, U>>());
    return true;
  }

  public bool TryAddEdge(T node_a, T node_b, U weight){
    if(!graph.TryGetValue(node_a, out var edges_node_a)) return false;
    if(!graph.TryGetValue(node_b, out var edges_node_b)) return false;
    if(edges_node_a.Exists((edge)=>edge.NodeA.Equals(node_b) || edge.NodeB.Equals(node_b))) return false;
    
    var edge = new GraphEdge<T,U>(node_a, node_b, weight);
    graph[node_a].Add(edge);
    graph[node_b].Add(edge);
    return true;
  }

  public bool TryGetEdge(T node_a, T node_b, out GraphEdge<T,U> edge){
    edge = null;
    if(graph.ContainsKey(node_a)) return false;
    
    edge = graph[node_a].Find((edge)=>edge.NodeA.Equals(node_b) || edge.NodeB.Equals(node_b));
    if(edge == null) return false;
    return true;
  }

  public bool TryGetEdgeList(T node, out List<GraphEdge<T,U>> edges) => graph.TryGetValue(node, out edges);
  public List<T> GetNodeList() => new List<T>(graph.Keys);
}

public class GraphEdge<T,U> 
  where T : IEquatable<T>
  where U : IComparable<U>
{
  public T NodeA;
  public T NodeB;
  public U Weight;

  public GraphEdge(T node_a, T node_b, U edge_weight)
  {
    NodeA = node_a;
    NodeB = node_b;
    Weight = edge_weight;
  }
}
