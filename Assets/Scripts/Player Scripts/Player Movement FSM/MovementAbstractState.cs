public abstract class MovementAbstractState 
{
   public abstract void EnterState(MovementStateMachineManager state);

   public abstract void ExitState(MovementStateMachineManager state);

   public abstract void UpdateState(MovementStateMachineManager state);

   public abstract void FixedUpdate(MovementStateMachineManager state);

   public abstract void OnCollisionEnter(MovementStateMachineManager state);
}