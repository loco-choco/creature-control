using UnityEngine;
using OWML.Common;

namespace CreatureControl;

public class GraphManipulatorItem : OWItem 
{
  public IGizmosAPI GizmosAPI;
  public IModConsole ModConsole;

  private WeightedGraph<Vector3, float> graph;
  private Transform graph_transform;

  bool was_picked_up = false;

  public override string GetDisplayName() => "Graph Manipulator";

  public override void PickUpItem(Transform holdTransform){
    was_picked_up = true;
    base.PickUpItem(holdTransform);
  }
  public override void DropItem(Vector3 position, Vector3 normal,
      Transform parent, Sector sector,
      IItemDropTarget customDropTarget){
    base.DropItem(position, normal, parent, sector, customDropTarget);
    was_picked_up = false;
  }

  bool was_primary_action = false;
  bool was_secondary_action = false;
  Vector3 selected_node;
  bool selected_a_node = false;
  private void Update(){
    bool is_using_item = Locator.GetToolModeSwapper().IsInToolMode(ToolMode.Item) && was_picked_up;
    bool is_primary_action = OWInput.IsPressed(InputLibrary.toolActionPrimary, InputMode.Character, 0f) && is_using_item;
    bool is_secondary_action = OWInput.IsPressed(InputLibrary.toolActionSecondary, InputMode.Character, 0f) && is_using_item;
    
    //Add Node to graph
    if(is_primary_action && !was_primary_action){
      Transform camera = Locator.GetPlayerCamera().transform;
      if(Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, 1000f, OWLayerMask.physicalMask)){
        Vector3 local_pos_of_hit = hit.transform.InverseTransformPoint(hit.point);
        
        if(hit.transform != graph_transform){
          graph = new();
          graph_transform = hit.transform;
        }
        graph.TryAddNode(local_pos_of_hit);
        ModConsole.WriteLine($"Added node at {local_pos_of_hit}");
      }
    }
    else if(is_secondary_action && !was_secondary_action){
      Transform camera = Locator.GetPlayerCamera().transform;
      if(Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, 1000f, OWLayerMask.physicalMask)){
        Vector3 local_pos_of_hit = hit.transform.InverseTransformPoint(hit.point);

        var nodes = graph.GetNodeList();
        Vector3 closest_node = Vector3.zero;
        float node_dist = float.MaxValue;
        foreach(var node in nodes){
          if((local_pos_of_hit - node).sqrMagnitude < node_dist){
            node_dist = (local_pos_of_hit - node).sqrMagnitude;
            closest_node = node;
          }
        }

        if(!selected_a_node){
          selected_a_node = true;
          selected_node = closest_node;
          ModConsole.WriteLine($"Selected node at {selected_node}");
        }
        else{
          selected_a_node = false;
          graph.TryAddEdge(selected_node, closest_node, (selected_node - closest_node).sqrMagnitude);
          ModConsole.WriteLine($"Created edge at {selected_node}-{closest_node}");
        }
      }
    }

    was_primary_action = is_primary_action;
    was_secondary_action = is_secondary_action;
  }

  private void OnRenderObject(){
    if(GizmosAPI == null || graph == null || !was_picked_up) return;

    GizmosAPI.SetDefaultMaterialPass();
    GizmosAPI.DrawWithReference(graph_transform,()=>{
      var nodes = graph.GetNodeList();
      foreach(var node in nodes){
        GizmosAPI.DrawSimpleWireframeSphere(0.2f, node, Color.red, 6);
        if(selected_a_node && selected_node.Equals(node)) GizmosAPI.DrawSimpleWireframeSphere(0.4f, node, Color.magenta, 6); 
        if(graph.TryGetEdgeList(node, out var edges))
          foreach(var edge in edges){
            GizmosAPI.DrawVector((edge.NodeB - edge.NodeA), 0.1f, edge.NodeA, Color.yellow);
          }
      }
    });
  }

}
  
