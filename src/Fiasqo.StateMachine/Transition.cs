using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fiasqo.StateMachine.Tests")]

namespace Fiasqo.StateMachine {
internal sealed class Transition<TContext>
    : ITransition<TContext>
    where TContext : class {
    internal Transition(Func<TContext, bool> condition, IStateContainer<TContext> stateContainer) {
        _condition = condition ?? throw new ArgumentNullException(nameof(condition));
        _stateContainer = stateContainer ?? throw new ArgumentNullException(nameof(stateContainer));
    }

    IStateContainer<TContext> ITransition<TContext>.StateContainer => _stateContainer;

    bool ITransition<TContext>.IsValid(TContext context) {
        return _condition.Invoke(context);
    }

    private readonly Func<TContext, bool> _condition;
    private readonly IStateContainer<TContext> _stateContainer;
}
}