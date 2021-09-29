using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Fiasqo.StateMachine.Tests {
[SuppressMessage("ReSharper", "InconsistentNaming")]
[TestOf(typeof(Transition<Context>))]
public class TransitionTests {
    [Test]
    public void StateContainerGet_ReturnsCorrectState() {
        var stateContainer = new MoqStateContainer();
        var transition = (ITransition<Context>) new Transition<Context>(context => true, stateContainer);

        Assert.AreSame(stateContainer, transition.StateContainer);
    }

    [Test]
    public void IsValid_ConditionIsFalse_ReturnsFalse() {
        var transition = (ITransition<Context>) new Transition<Context>(context => false, new MoqStateContainer());

        Assert.IsFalse(transition.IsValid(new Context()));
    }

    [Test]
    public void IsValid_ConditionIsTrue_ReturnsTrue() {
        var transition = (ITransition<Context>) new Transition<Context>(context => true, new MoqStateContainer());

        Assert.IsTrue(transition.IsValid(new Context()));
    }

#region Private Members

    private class Context { }

    private class MoqStateContainer : IStateContainer<Context> {
        public State<Context> State => default;

        public bool TryGetNextState(Context context, out IStateContainer<Context> nextStateContainer) {
            nextStateContainer = default;
            return true;
        }
    }

#endregion
}
}