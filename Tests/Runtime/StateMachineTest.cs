using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Fiasqo.FluentStateMachine.Tests {
[SuppressMessage("ReSharper", "InconsistentNaming")]
[TestOf(typeof(StateMachine<Context>))]
public class StateMachineTest {
    [Test]
    public void CurrentStateGet_ReturnsCorrectState() {
        var stateMachine = CreateStateMachine(out var state);

        Assert.AreSame(state, stateMachine.CurrentState);
    }

    [TestCaseSource(nameof(SpecifiedStateMachineMethod_StateMachineIsEnabled_InvokesSpecifiedStateMethod_Cases))]
    public void SpecifiedStateMachineMethod_StateMachineIsEnabled_InvokesSpecifiedStateMethod(string stateMachineMethodName,
        string stateMethodName) {
        var stateMachine = CreateStateMachine(out var state);
        stateMachine.Enable();
        state.CallOrder.Clear();
        var specifiedStateMachineMethod = GetStateMachineMethod<StateMachine<Context>>(stateMachineMethodName);

        specifiedStateMachineMethod(stateMachine);

        Assert.IsTrue(state.CallOrder.SequenceEqual(new[] { stateMethodName }));
    }

    [TestCaseSource(nameof(SpecifiedStateMachineMethod_StateMachineIsDisabled_DoNothing_Cases))]
    public void SpecifiedStateMachineMethod_StateMachineIsDisabled_DoNothing(string stateMachineMethodName) {
        var stateMachine = CreateStateMachine(out var state);
        stateMachine.Enable();
        stateMachine.Disable();
        state.CallOrder.Clear();
        var specifiedStateMachineMethod = GetStateMachineMethod<StateMachine<Context>>(stateMachineMethodName);

        specifiedStateMachineMethod(stateMachine);

        Assert.IsTrue(state.CallOrder.Count == 0);
    }

    [Test]
    public void Enable_StateMachineWasNeverBeEnabled_InvokesStateOnEnterThenStateOnEnable() {
        var expectedCallOrder = new[] {
            nameof(State<Context>.OnEnter),
            nameof(State<Context>.OnEnable)
        };
        var stateMachine = CreateStateMachine(out var state);

        stateMachine.Enable();

        Assert.IsTrue(state.CallOrder.SequenceEqual(expectedCallOrder));
    }

    [Test]
    public void Enable_StateMachineIsEnabled_DoNothing() {
        var stateMachine = CreateStateMachine(out var state);
        stateMachine.Enable();
        state.CallOrder.Clear();

        stateMachine.Enable();

        Assert.IsTrue(state.CallOrder.Count == 0);
    }

    [Test]
    public void Enable_StateMachineIsDisabled_InvokesStateOnEnable() {
        var expectedCallOrder = new[] { nameof(State<Context>.OnEnable) };
        var stateMachine = CreateStateMachine(out var state);
        stateMachine.Enable();
        stateMachine.Disable();
        state.CallOrder.Clear();

        stateMachine.Enable();

        Assert.IsTrue(state.CallOrder.SequenceEqual(expectedCallOrder));
    }

    [Test]
    public void LateUpdate_ValidTransitionExists_ChangeState() {
        string[] initStateExpectedCallOrder = {
                     nameof(State<Context>.OnLateUpdate),
                     nameof(State<Context>.OnDisable),
                     nameof(State<Context>.OnExit)
                 },
                 nextStateExpectedCallOrder = {
                     nameof(State<Context>.OnEnter),
                     nameof(State<Context>.OnEnable)
                 };
        var stateMachine = CreateStateMachine(out var initState, out var nextState);
        stateMachine.Enable();
        initState.CallOrder.Clear();

        stateMachine.LateUpdate();

        Assert.IsTrue(initState.CallOrder.SequenceEqual(initStateExpectedCallOrder));
        Assert.IsTrue(nextState.CallOrder.SequenceEqual(nextStateExpectedCallOrder));
        Assert.AreSame(nextState, stateMachine.CurrentState);
    }

#region Test Cases

    public static IEnumerable<TestCaseData> SpecifiedStateMachineMethod_StateMachineIsEnabled_InvokesSpecifiedStateMethod_Cases {
        get {
            yield return new TestCaseData(nameof(StateMachine<object>.Disable), nameof(State<object>.OnDisable));
            yield return new TestCaseData(nameof(StateMachine<object>.Update), nameof(State<object>.OnUpdate));
            yield return new TestCaseData(nameof(StateMachine<object>.FixedUpdate), nameof(State<object>.OnFixedUpdate));
            yield return new TestCaseData(nameof(StateMachine<object>.LateUpdate), nameof(State<object>.OnLateUpdate));
        }
    }

    public static IEnumerable<TestCaseData> SpecifiedStateMachineMethod_StateMachineIsDisabled_DoNothing_Cases {
        get {
            yield return new TestCaseData(nameof(StateMachine<object>.Disable));
            yield return new TestCaseData(nameof(StateMachine<object>.Update));
            yield return new TestCaseData(nameof(StateMachine<object>.FixedUpdate));
            yield return new TestCaseData(nameof(StateMachine<object>.LateUpdate));
        }
    }

#endregion

#region Private Members

    private static StateMachine<Context> CreateStateMachine(out MoqState initState, out MoqState nextState) {
        var nextStateContainer = new MoqStateContainer();
        var initStateContainer = new MoqStateContainer(nextStateContainer);

        initState = initStateContainer.StateAsMoqState;
        nextState = nextStateContainer.StateAsMoqState;

        return new StateMachine<Context>(new Context(), initStateContainer);
    }

    private static StateMachine<Context> CreateStateMachine(out MoqState state) {
        var stateContainer = new MoqStateContainer();

        state = stateContainer.StateAsMoqState;

        return new StateMachine<Context>(new Context(), stateContainer);
    }

    private static Action<T> GetStateMachineMethod<T>(string methodName)
        where T : class, IStateMachine {
        var methodInfo = typeof(T).GetMethod
        (
            methodName,
            BindingFlags.Public | BindingFlags.Instance,
            null,
            CallingConventions.HasThis,
            Type.EmptyTypes,
            null
        ) ?? throw new ArgumentException($"StateMachine of Type <{typeof(T).Name}> doesn't contains " +
                                         $"parameterless public method (\"{methodName}\")", nameof(methodName));

        return stateMachine => methodInfo.Invoke(stateMachine, Array.Empty<object>());
    }

    private class Context { }

    private class MoqState : State<Context> {
        public List<string> CallOrder { get; } = new List<string>();

        public override void OnEnter(Context context) {
            CallOrder.Add(nameof(OnEnter));
        }

        public override void OnExit(Context context) {
            CallOrder.Add(nameof(OnExit));
        }

        public override void OnEnable(Context context) {
            CallOrder.Add(nameof(OnEnable));
        }

        public override void OnDisable(Context context) {
            CallOrder.Add(nameof(OnDisable));
        }

        public override void OnUpdate(Context context) {
            CallOrder.Add(nameof(OnUpdate));
        }

        public override void OnFixedUpdate(Context context) {
            CallOrder.Add(nameof(OnFixedUpdate));
        }

        public override void OnLateUpdate(Context context) {
            CallOrder.Add(nameof(OnLateUpdate));
        }
    }

    private class MoqStateContainer : IStateContainer<Context> {
        public MoqState StateAsMoqState { get; } = new MoqState();

        public State<Context> State => StateAsMoqState;

        public MoqStateContainer() {
            returnNextState = false;
        }

        public MoqStateContainer(MoqStateContainer nextStateContainer) {
            _nextStateContainer = nextStateContainer ?? throw new ArgumentNullException(nameof(nextStateContainer));
            returnNextState = true;
        }

        public bool TryGetNextState(Context context, out IStateContainer<Context> nextStateContainer) {
            nextStateContainer = returnNextState ? _nextStateContainer : null;
            return returnNextState;
        }

        private readonly MoqStateContainer _nextStateContainer;
        private readonly bool returnNextState;
    }

#endregion
}
}