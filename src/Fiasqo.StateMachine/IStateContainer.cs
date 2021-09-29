using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fiasqo.StateMachine.Tests")]

namespace Fiasqo.StateMachine {
internal interface IStateContainer<TContext>
    where TContext : class {
    State<TContext> State { get; }
    bool TryGetNextState(TContext context, out IStateContainer<TContext> nextStateContainer);
}
}