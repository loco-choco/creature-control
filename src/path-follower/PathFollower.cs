using System.Collections.Generic;

namespace CreatureControl;

public interface PathFollower<Node, TargetState, CurrentState> 
{
  TargetState FollowPath(List<Node> graph, CurrentState current_state);
}
  
