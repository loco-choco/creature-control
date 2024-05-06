using System;
using UnityEngine;

namespace CreatureControl;

public abstract class PathingAI<Node, Edge, TargetState, Command, CurrentState> : MonoBehaviour
  where Node : IEquatable<Node>
  where Edge : IComparable<Edge>
{
  [SerializeField]
  private PathGenerator<Node, Edge> path_generator;
  [SerializeField]
  private PathFollower<Node, TargetState, CurrentState> path_follower;
  [SerializeField]
  private CharacterController<TargetState, Command, CurrentState> character_controller;
}
  
