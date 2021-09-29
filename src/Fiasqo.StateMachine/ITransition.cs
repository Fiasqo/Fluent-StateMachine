using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fiasqo.StateMachine.Tests")]

namespace Fiasqo.StateMachine {
internal interface ITransition<TContext>
    where TContext : class {
    IStateContainer<TContext> StateContainer { get; }
    bool IsValid(TContext context);
}
}