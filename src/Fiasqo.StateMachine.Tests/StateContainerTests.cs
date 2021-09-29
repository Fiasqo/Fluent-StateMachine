using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;

namespace Fiasqo.StateMachine.Tests {
[SuppressMessage("ReSharper", "InconsistentNaming")]
[TestOf(typeof(StateContainer<Context>))]
public class StateContainerTests {
    [TestCaseSource(nameof(StateGet_ReturnsCorrectState_Cases))]
    public void StateGet_ReturnsCorrectState(object stateContainerAsObject, object expectedState) {
        var stateContainer = (IStateContainer<Context>) (StateContainer<Context>) stateContainerAsObject;

        var actualState = stateContainer.State;

        Assert.AreSame(expectedState, actualState);
    }

    [TestCaseSource(nameof(TryGetNextState_ValidTransitionDoesNotExists_ReturnsFalseAndOutParameterIsNull_Cases))]
    public void TryGetNextState_ValidTransitionDoesNotExists_ReturnsFalseAndOutParameterIsNull(object stateContainerAsObject) {
        var stateContainer = (IStateContainer<Context>) (StateContainer<Context>) stateContainerAsObject;

        var isFoundNextState = stateContainer.TryGetNextState(new Context(), out var nextStateContainer);

        Assert.IsFalse(isFoundNextState);
        Assert.IsNull(nextStateContainer);
    }

    [TestCaseSource(nameof(TryGetNextState_ValidTransitionExists_ReturnsTrueAndSetsOutParameter_Cases))]
    public void TryGetNextState_ValidTransitionExists_ReturnsTrueAndSetsOutParameter(object stateContainerAsObject,
        object expectedNextStateContainer) {
        var stateContainer = (IStateContainer<Context>) (StateContainer<Context>) stateContainerAsObject;

        var isFoundNextState = stateContainer.TryGetNextState(new Context(), out var actualNextStateContainer);

        Assert.IsTrue(isFoundNextState);
        Assert.AreSame(expectedNextStateContainer, actualNextStateContainer);
    }

#region Test Cases

    public static IEnumerable<TestCaseData> StateGet_ReturnsCorrectState_Cases {
        get {
            var stateA = new MoqState();
            var stateContainerA = new StateContainer<Context>(stateA, Array.Empty<MoqTransition>());
            yield return new TestCaseData(stateContainerA, stateA);

            var stateB = new MoqState();
            var stateContainerB = new StateContainer<Context>(stateB, Array.Empty<MoqTransition>());
            yield return new TestCaseData(stateContainerB, stateB);

            var stateC = new MoqState();
            var stateContainerC = new StateContainer<Context>(stateC, Array.Empty<MoqTransition>());
            yield return new TestCaseData(stateContainerC, stateC);
        }
    }

    public static IEnumerable<TestCaseData> TryGetNextState_ValidTransitionDoesNotExists_ReturnsFalseAndOutParameterIsNull_Cases {
        get {
            yield return new TestCaseData(CreateStateContainer(Array.Empty<bool>(), out var _));
            yield return new TestCaseData(CreateStateContainer(Enumerable.Repeat(false, 3).ToArray(), out var _));
            yield return new TestCaseData(CreateStateContainer(Enumerable.Repeat(false, 2).ToArray(), out var _));
            yield return new TestCaseData(CreateStateContainer(Enumerable.Repeat(false, 1).ToArray(), out var _));
        }
    }

    public static IEnumerable<TestCaseData> TryGetNextState_ValidTransitionExists_ReturnsTrueAndSetsOutParameter_Cases {
        get {
            yield return new TestCaseData(CreateStateContainer(new[] { true, false, false }, out var scs), scs[0]);
            yield return new TestCaseData(CreateStateContainer(new[] { true, true, false }, out scs), scs[0]);
            yield return new TestCaseData(CreateStateContainer(new[] { true, true, true }, out scs), scs[0]);
            yield return new TestCaseData(CreateStateContainer(new[] { false, true, false }, out scs), scs[1]);
            yield return new TestCaseData(CreateStateContainer(new[] { false, true, true }, out scs), scs[1]);
            yield return new TestCaseData(CreateStateContainer(new[] { false, false, true }, out scs), scs[2]);
        }
    }

#endregion

#region Private Members

    private static StateContainer<Context> CreateStateContainer(IReadOnlyList<bool> transitionConditions,
        out StateContainer<Context>[] stateContainers) {
        if (transitionConditions == null) { throw new ArgumentNullException(nameof(transitionConditions)); }

        var transitions = new MoqTransition[transitionConditions.Count];
        stateContainers = new[] {
            new StateContainer<Context>(new MoqState(), transitions),
            new StateContainer<Context>(new MoqState(), transitions),
            new StateContainer<Context>(new MoqState(), transitions)
        };

        for (var i = 0; i < transitionConditions.Count; i++) {
            transitions[i] = new MoqTransition(transitionConditions[i], stateContainers[i % 3]);
        }

        return stateContainers[0];
    }

    private class Context { }

    private class MoqState : State<Context> { }

    private class MoqTransition : ITransition<Context> {
        public IStateContainer<Context> StateContainer { get; }

        public MoqTransition(bool isValidReturnValue, IStateContainer<Context> stateContainer) {
            _isValidReturnValue = isValidReturnValue;
            StateContainer = stateContainer ?? throw new ArgumentNullException(nameof(stateContainer));
        }

        public bool IsValid(Context context) {
            return _isValidReturnValue;
        }

        private readonly bool _isValidReturnValue;
    }

#endregion
}
}