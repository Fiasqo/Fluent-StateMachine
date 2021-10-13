using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fiasqo.FluentStateMachine.Tests")]

namespace Fiasqo.FluentStateMachine {
internal interface IStateContainer<TContext>
    where TContext : class {
    State<TContext> State { get; }
    bool TryGetNextState(TContext context, out IStateContainer<TContext> nextStateContainer);
}
}