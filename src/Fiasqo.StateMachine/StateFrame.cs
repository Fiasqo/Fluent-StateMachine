using System;
using System.Collections.Generic;

namespace Fiasqo.StateMachine {
internal readonly struct StateFrame<TContext> 
    where TContext : class {
    internal IStateContainer<TContext> StateContainer { get; }
    internal List<ITransition<TContext>> Transitions { get; }

    internal StateFrame(State<TContext> state) {
        if (state == null) throw new ArgumentNullException(nameof(state));
        Transitions = new List<ITransition<TContext>>();
        StateContainer = new StateContainer<TContext>(state, Transitions);
    }
}
}