using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Fiasqo.StateMachine.Tests {
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class StateMachineBuilderTests {
    [Test]
    public void Create_ContextIsNull_ThrowsException() {
        TestDelegate code = () => StateMachineBuilder<Context>.Create(null);

        Assert.Throws<ArgumentNullException>(code);
    }

    [Test]
    public void In_CfgIsNull_ThrowsException() {
        TestDelegate code = () => StateMachineBuilder<Context>.Create(new Context()).In<MoqStateA>(null);

        Assert.Throws<ArgumentNullException>(code);
    }

    [Test]
    public void In_CfgIsNotNull_AddsState() {
        var stateMachine = BuildUncompletedStateMachineBuilder
        (
            StateMachineBuilder<Context>.Create(new Context())
                .In<MoqStateA>(_ => { })
        );

        Assert.IsInstanceOf<MoqStateA>(stateMachine.CurrentState);
    }

    [Test]
    public void In_UsesOneInstanceOfSpecifiedState() {
        var passedStates = new List<State<Context>>();
        var stateAGotoSwitcher = true;
        var stateMachine = BuildUncompletedStateMachineBuilder
        (
            StateMachineBuilder<Context>.Create(new Context())
                // ReSharper disable twice AccessToModifiedClosure
                .In<MoqStateA>(tb => { tb.If(c => stateAGotoSwitcher).Goto<MoqStateB>(); })
                .In<MoqStateB>(tb => { tb.If(c => true).Goto<MoqStateA>(); })
                .In<MoqStateA>(tb => { tb.If(c => !stateAGotoSwitcher).Goto<MoqStateC>(); })
                .In<MoqStateC>(tb => { tb.If(c => true).Goto<MoqStateA>(); })
        );
        stateMachine.Enable();

        stateMachine.LateUpdate();
        for (var i = 0; i < 3; i++) {
            passedStates.Add(stateMachine.CurrentState);
            stateMachine.LateUpdate();
            stateAGotoSwitcher = !stateAGotoSwitcher;
            stateMachine.LateUpdate();
        }

        Assert.IsInstanceOf<MoqStateB>(passedStates[0]);
        Assert.IsInstanceOf<MoqStateC>(passedStates[1]);
        Assert.IsInstanceOf<MoqStateB>(passedStates[2]);
    }

    [Test]
    public void If_ConditionIsNull_ThrowsException() {
        TestDelegate code = () => StateMachineBuilder<Context>.Create(new Context()).In<MoqStateA>(tb => tb.If(null));

        Assert.Throws<ArgumentNullException>(code);
    }

    [Test]
    public void Goto_UsesOneInstanceOfSpecifiedState() {
        var passedStates = new List<State<Context>>(4);
        var stateMachine = BuildUncompletedStateMachineBuilder
        (
            StateMachineBuilder<Context>.Create(new Context())
                .In<MoqStateA>(tb => { tb.If(c => true).Goto<MoqStateB>(); })
                .In<MoqStateB>(tb => { tb.If(c => true).Goto<MoqStateA>(); })
        );
        stateMachine.Enable();

        for (var i = 0; i < 4; i++) {
            passedStates.Add(stateMachine.CurrentState);
            stateMachine.LateUpdate();
        }

        Assert.AreSame(passedStates[0], passedStates[2]);
        Assert.AreSame(passedStates[1], passedStates[3]);
    }

    [Test]
    public void InitialStateIs_StateWasNotAdded_ThrowsException() {
        TestDelegate code = () => StateMachineBuilder<Context>.Create(new Context())
            .In<MoqStateA>(tb => tb.If(c => true).Goto<MoqStateB>())
            .In<MoqStateB>(tb => tb.If(c => true).Goto<MoqStateA>())
            .InitialStateIs<MoqStateC>();

        var ex = Assert.Throws<InvalidOperationException>(code);
        Assert.IsNotNull(ex);
        Assert.That(ex.Message, Contains.Substring(nameof(MoqStateC)));
    }

    [TestCaseSource(nameof(InitialStateIs_StateWasAdded_SetsInitialState_Cases))]
    public void InitialStateIs_StateWasAdded_SetsInitialState(Type initialStateType) {
        var stateMachineBuilder = StateMachineBuilder<Context>.Create(new Context())
            .In<MoqStateA>(tb => tb.If(c => true).Goto<MoqStateB>())
            .In<MoqStateB>(tb => tb.If(c => true).Goto<MoqStateC>())
            .In<MoqStateC>(tb => tb.If(c => true).Goto<MoqStateA>());

        var stateMachine = (initialStateType.Name switch {
            nameof(MoqStateA) => stateMachineBuilder.InitialStateIs<MoqStateA>(),
            nameof(MoqStateB) => stateMachineBuilder.InitialStateIs<MoqStateB>(),
            nameof(MoqStateC) => stateMachineBuilder.InitialStateIs<MoqStateC>(),
            var _             => throw new ArgumentOutOfRangeException()
        }).Build();

        Assert.IsInstanceOf(initialStateType, stateMachine.CurrentState);
    }

#region Test Cases

    public static IEnumerable<TestCaseData> InitialStateIs_StateWasAdded_SetsInitialState_Cases {
        get {
            yield return new TestCaseData(typeof(MoqStateA));
            yield return new TestCaseData(typeof(MoqStateB));
            yield return new TestCaseData(typeof(MoqStateC));
        }
    }

#endregion

#region Private Members

    private static StateMachine<Context> BuildUncompletedStateMachineBuilder(StateMachineBuilder<Context> stateMachineBuilder) {
        if (stateMachineBuilder == null) { throw new ArgumentNullException(nameof(stateMachineBuilder)); }
        return ((StateMachineBuilder<Context>.IBuildStage) stateMachineBuilder).Build();
    }
    
    private static StateMachine<Context> BuildUncompletedStateMachineBuilder(StateMachineBuilder<Context>.IDeclareSecondStateStage stateMachineBuilder) {
        return BuildUncompletedStateMachineBuilder((StateMachineBuilder<Context>) stateMachineBuilder);
    }

    private static StateMachine<Context> BuildUncompletedStateMachineBuilder(StateMachineBuilder<Context>.IDeclareMultipleStatesStage stateMachineBuilder) {
        return BuildUncompletedStateMachineBuilder((StateMachineBuilder<Context>) stateMachineBuilder);
    }

    private class Context { }

    private class MoqStateA : State<Context> { }

    private class MoqStateB : State<Context> { }

    private class MoqStateC : State<Context> { }

#endregion
}
}