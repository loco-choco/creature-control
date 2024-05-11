using System.Collections.Generic;
using UnityEngine;
using OWML.Common;

namespace CreatureControl;

public class PathGeneratorManipulatorItem : OWItem 
{
  public IGizmosAPI GizmosAPI;
  public static IModConsole ModConsole;

  private PathGenerator<Vector3, float> path_generator;
  private List<Vector3> path;

  bool was_picked_up = false;

  public override string GetDisplayName() => "Path Generator Manipulator";

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

  public void Start(){
    path_generator = GetComponent<PathGenerator<Vector3, float>>();
  }

  bool was_primary_action = false;
  bool was_secondary_action = false;
  Vector3 selected_node;
  bool selected_a_node = false;
  private void Update(){
    if(GraphManipulatorItem.graph == null) return;

    bool is_using_item = Locator.GetToolModeSwapper().IsInToolMode(ToolMode.Item) && was_picked_up;
    bool is_primary_action = OWInput.IsPressed(InputLibrary.toolActionPrimary, InputMode.Character, 0f) && is_using_item;
    bool is_secondary_action = OWInput.IsPressed(InputLibrary.toolActionSecondary, InputMode.Character, 0f) && is_using_item;
      
    if(is_primary_action && !was_primary_action){
      Transform camera = Locator.GetPlayerCamera().transform;
      if(Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, 1000f, OWLayerMask.physicalMask)){
        Vector3 local_pos_of_hit = hit.transform.InverseTransformPoint(hit.point);
  
        var nodes = GraphManipulatorItem.graph.GetNodeList();
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
          ModConsole.WriteLine($"Calculating Path from {selected_node}->{closest_node}");
          path = path_generator.CalculatePath(GraphManipulatorItem.graph, selected_node, closest_node);
          if(path != null) ModConsole.WriteLine($"Path calculated! {path.Count} steps");
          else ModConsole.WriteLine($"Path not found");
        }
      }
    }

    was_primary_action = is_primary_action;
    was_secondary_action = is_secondary_action;
  }

  private void OnRenderObject(){
    if(GizmosAPI == null || GraphManipulatorItem.graph == null || !was_picked_up) return;
    Transform graph_transform = GraphManipulatorItem.graph_transform;

    GizmosAPI.SetDefaultMaterialPass();
    GizmosAPI.DrawWithReference(graph_transform,()=>{
      if(selected_node != null) GizmosAPI.DrawSimpleWireframeSphere(0.4f, selected_node, Color.green, 6);
      if(path != null) foreach(var node in path) GizmosAPI.DrawSimpleWireframeSphere(0.6f, node, Color.yellow, 6);
    
      var nodes = GraphManipulatorItem.graph.GetNodeList();
      foreach(var node in nodes){
        GizmosAPI.DrawSimpleWireframeSphere(0.2f, node, Color.red, 6);
        if(GraphManipulatorItem.graph.TryGetEdgeList(node, out var edges))
          foreach(var edge in edges){
            GizmosAPI.DrawVector((edge.NodeB - edge.NodeA), 0.1f, edge.NodeA, Color.yellow);
          }
      }
    });
  }
}
  
