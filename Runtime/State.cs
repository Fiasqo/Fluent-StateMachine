namespace Fiasqo.FluentStateMachine {
public abstract class State<TContext>
    where TContext : class {
    public virtual void OnEnter(TContext context) { }
    public virtual void OnExit(TContext context) { }

    public virtual void OnEnable(TContext context) { }
    public virtual void OnDisable(TContext context) { }

    public virtual void OnUpdate(TContext context) { }
    public virtual void OnFixedUpdate(TContext context) { }
    public virtual void OnLateUpdate(TContext context) { }
}
}