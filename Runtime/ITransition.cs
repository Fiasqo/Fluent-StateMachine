using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fiasqo.FluentStateMachine.Tests")]

namespace Fiasqo.FluentStateMachine {
internal interface ITransition<TContext>
    where TContext : class {
    IStateContainer<TContext> StateContainer { get; }
    bool IsValid(TContext context);
}
}