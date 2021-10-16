using System;
using System.Collections.Generic;

namespace Fiasqo.FluentStateMachine {
internal readonly struct StateFrame<TContext> 
    where TContext : class {
    public StateFrame(State<TContext> state) {
        if (state == null) {
            throw new ArgumentNullException(nameof(state));
        }
        
        Transitions = new List<ITransition<TContext>>();
        StateContainer = new StateContainer<TContext>(state, Transitions);
    }
    
    public IStateContainer<TContext> StateContainer { get; }
    public List<ITransition<TContext>> Transitions { get; }
}
}