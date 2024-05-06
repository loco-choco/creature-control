namespace CreatureControl;

public interface CharacterController<TargetState, Command, CurrentState> 
{
  Command IssueCommand(TargetState target_state, CurrentState current_state);
}
  
