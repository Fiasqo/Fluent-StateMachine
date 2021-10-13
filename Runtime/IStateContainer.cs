namespace Fiasqo.FluentStateMachine {
internal interface IStateContainer<TContext>
    where TContext : class {
    State<TContext> State { get; }
    bool TryGetNextState(TContext context, out IStateContainer<TContext> nextStateContainer);
}
}