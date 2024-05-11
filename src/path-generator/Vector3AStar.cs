using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace CreatureControl;

public class Vector3AStar : MonoBehaviour, PathGenerator<Vector3,float>
{
  public List<Vector3> CalculatePath(WeightedGraph<Vector3,float> graph, Vector3 initial_node, Vector3 target_node){
    graph.ClearVisits();
    var nodes = graph.GetNodeList();
    Dictionary<Vector3, Vector3> came_from = new(nodes.Count);
    Queue<(Vector3 node, float distance)> priority_queue = new Queue<(Vector3 node, float distance)>();
    
    Dictionary<Vector3, float> known_distances = new(nodes.Count);
    for(int i = 0; i < nodes.Count; i++){
      if(nodes[i] == initial_node) known_distances[initial_node] = 0f;
      else known_distances[nodes[i]] = float.MaxValue;
    }

    Dictionary<Vector3, float> guessed_distances = new(nodes.Count);
    for(int i = 0; i < nodes.Count; i++){
      if(nodes[i] == initial_node) guessed_distances[initial_node] = (target_node - initial_node).sqrMagnitude;
      else guessed_distances[nodes[i]] = float.MaxValue;
    }

    priority_queue.Enqueue((initial_node, 0f));
    
    while(priority_queue.Count > 0){
      (Vector3 node, float distance) = priority_queue.Dequeue();
      graph.Visit(node);

      if(node == target_node) return CreatePath(initial_node, target_node, came_from);

      if(graph.TryGetEdgeList(node, out var edges)){
        foreach(var edge in edges){
          Vector3 neighbour_node;
          if(edge.NodeA == node) neighbour_node = edge.NodeB;
          else neighbour_node = edge.NodeA;
          float distance_until_now = known_distances[node] + edge.Weight;

          if(known_distances[neighbour_node] > distance_until_now){
            came_from[neighbour_node] = node;
            known_distances[neighbour_node] = distance_until_now;
            guessed_distances[neighbour_node] = distance_until_now + (target_node - initial_node).sqrMagnitude;
            if(!graph.IsVisited(neighbour_node)){ //This might be making some issues where it doesnt get the optimal path, see https://en.wikipedia.org/wiki/A*_search_algorithm
              priority_queue.Enqueue((neighbour_node, distance_until_now));
              priority_queue.OrderBy((element)=>element.distance);
            }
          }
        }
      }
    }
    return null;
  }

  private List<Vector3> CreatePath(Vector3 initial_node, Vector3 target_node, Dictionary<Vector3, Vector3> came_from){
    List<Vector3> path = new List<Vector3>();
    Vector3 node = target_node;
    while(node != initial_node){
      path.Insert(0,node);
      node = came_from[node];
    }
    path.Insert(0, initial_node);
    return path;
  }
}
  
