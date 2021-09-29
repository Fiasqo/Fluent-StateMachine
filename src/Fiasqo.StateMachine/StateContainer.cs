using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fiasqo.StateMachine.Tests")]

namespace Fiasqo.StateMachine {
internal sealed class StateContainer<TContext>
    : IStateContainer<TContext>
    where TContext : class {
    internal StateContainer(State<TContext> state, IReadOnlyList<ITransition<TContext>> transitions) {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        _transitions = transitions ?? throw new ArgumentNullException(nameof(transitions));
    }

    State<TContext> IStateContainer<TContext>.State => _state;

    bool IStateContainer<TContext>.TryGetNextState(TContext context, out IStateContainer<TContext> nextStateContainer) {
        nextStateContainer = null;

        foreach (var transition in _transitions) {
            if (!transition.IsValid(context)) { continue; }

            nextStateContainer = transition.StateContainer;
            return true;
        }

        return false;
    }

    private readonly State<TContext> _state;
    private readonly IReadOnlyList<ITransition<TContext>> _transitions;
}
}