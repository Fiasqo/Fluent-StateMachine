using System;
using System.Collections.Generic;

namespace Fiasqo.FluentStateMachine {
internal sealed class StateContainer<TContext>
    : IStateContainer<TContext>
    where TContext : class {
    private readonly IReadOnlyList<ITransition<TContext>> _transitions;
    
    public StateContainer(State<TContext> state, IReadOnlyList<ITransition<TContext>> transitions) {
        State = state ?? throw new ArgumentNullException(nameof(state));
        _transitions = transitions ?? throw new ArgumentNullException(nameof(transitions));
    }

    public State<TContext> State { get; }

    public bool TryGetNextState(TContext context, out IStateContainer<TContext> nextStateContainer) {
        nextStateContainer = null;

        foreach (var transition in _transitions) {
            if (!transition.IsValid(context)) {
                continue;
            }

            nextStateContainer = transition.StateContainer;
            return true;
        }

        return false;
    }
}
}