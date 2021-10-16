using System;

namespace Fiasqo.FluentStateMachine {
internal sealed class Transition<TContext>
    : ITransition<TContext>
    where TContext : class {
    private readonly Func<TContext, bool> _condition;

    public Transition(Func<TContext, bool> condition, IStateContainer<TContext> stateContainer) {
        _condition = condition ?? throw new ArgumentNullException(nameof(condition));
        StateContainer = stateContainer ?? throw new ArgumentNullException(nameof(stateContainer));
    }

    public IStateContainer<TContext> StateContainer { get; }

    public bool IsValid(TContext context) => _condition.Invoke(context);
}
}