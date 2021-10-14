using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;

namespace Fiasqo.FluentStateMachine.Tests {
[SuppressMessage("ReSharper", "InconsistentNaming")]
[TestOf(typeof(StateFrames<Context>))]
public class StateFramesTest {
    [Test]
    public void TryFindIndexByStateType_Empty_ReturnsFalseAndSetsIndexMinusOne() {
        // Arrange:
        var stateFrames = new StateFrames<Context>();
        
        // Act:
        var isFind = stateFrames.TryFindIndexByStateType<MoqStateD>(out var index);

        // Assert:
        Assert.AreEqual(false, isFind);
        Assert.AreEqual(-1, index);
    }
    
    [TestCaseSource(nameof(ContainsDesiredState_Cases))]
    public void TryFindIndexByStateType_ContainsDesiredState_ReturnsTrueAndSetsStateFrameIndex<TState>(TState stateToFind,
        int expectedIndex)
        where TState : State<Context> {
        // Arrange:
        var states = GetStates();
        var stateFrames = new StateFrames<Context>(states.Select(s => new StateFrame<Context>(s)));
        
        // Act:
        var isFind = stateFrames.TryFindIndexByStateType<TState>(out var index);

        // Assert:
        Assert.AreEqual(true, isFind);
        Assert.AreEqual(expectedIndex, index);
        Assert.IsInstanceOf<TState>(stateFrames[index].StateContainer.State);
    }
    
    [TestCaseSource(nameof(ContainsNoDesiredState_Cases))]
    public void TryFindIndexByStateType_ContainsDesiredState_ReturnsFalseAndSetsIndexMinusOne<TState>(TState stateToFind) 
        where TState : State<Context> {
        // Arrange:
        var states = GetStates().Where(s => s.GetType() != typeof(TState));
        var stateFrames = new StateFrames<Context>(states.Select(s => new StateFrame<Context>(s)));
        
        // Act:
        var isFind = stateFrames.TryFindIndexByStateType<TState>(out var index);
        
        // Assert:
        Assert.AreEqual(false, isFind);
        Assert.AreEqual(-1, index);
    }

    [Test]
    public void GetIndexOfExistsOrAdded_Empty_ReturnsNewStateFrameIndex() {
        // Arrange:
        var stateFrames = new StateFrames<Context>();
        
        // Act:
        var index = stateFrames.GetIndexOfExistsOrAdded<MoqStateC>();

        // Assert:
        Assert.AreEqual(0, index);
        Assert.AreEqual(1, stateFrames.Count);
        Assert.IsInstanceOf<MoqStateC>(stateFrames[index].StateContainer.State);
    }
    
    [TestCaseSource(nameof(ContainsDesiredState_Cases))]
    public void GetIndexOfExistsOrAdded_ContainsDesiredState_ReturnsExistingStateFrameIndex<TState>(TState stateToGet,
        int excpectedIndex)
        where TState : State<Context>, new() {
        // Arrange:
        var states = GetStates();
        var stateFrames = new StateFrames<Context>(states.Select(s => new StateFrame<Context>(s)));
        var stateFramesInitialCount = stateFrames.Count;
        
        // Act:
        var index = stateFrames.GetIndexOfExistsOrAdded<TState>();

        // Assert:
        Assert.AreEqual(excpectedIndex, index);
        Assert.AreEqual(stateFramesInitialCount, stateFrames.Count);
        Assert.IsInstanceOf<TState>(stateFrames[index].StateContainer.State);
    }

    [TestCaseSource(nameof(ContainsNoDesiredState_Cases))]
    public void GetIndexOfExistsOrAdded_ContainsNoDesiredState_ReturnsNewStateFrameIndex<TState>(TState stateToGet)
        where TState : State<Context>, new() {
        // Arrange:
        var states = GetStates().Where(s => s.GetType() != typeof(TState));
        var stateFrames = new StateFrames<Context>(states.Select(s => new StateFrame<Context>(s)));
        var stateFramesInitialCount = stateFrames.Count;
        
        // Act:
        var index = stateFrames.GetIndexOfExistsOrAdded<TState>();

        // Assert:
        Assert.AreEqual(stateFrames.Count - 1, index);
        Assert.AreEqual(stateFramesInitialCount + 1, stateFrames.Count);
        Assert.IsInstanceOf<TState>(stateFrames[index].StateContainer.State);
    }
    
#region Test Cases

    public static IEnumerable<TestCaseData> ContainsDesiredState_Cases {
        get {
            yield return new TestCaseData(new MoqStateA(), 0);
            yield return new TestCaseData(new MoqStateB(), 1);
            yield return new TestCaseData(new MoqStateC(), 2);
            yield return new TestCaseData(new MoqStateD(), 3);
        }
    }
    
    public static IEnumerable<TestCaseData> ContainsNoDesiredState_Cases {
        get {
            yield return new TestCaseData(new MoqStateA());
            yield return new TestCaseData(new MoqStateB());
            yield return new TestCaseData(new MoqStateC());
            yield return new TestCaseData(new MoqStateD());
        }
    }

#endregion
    
    public class Context { }

#region Private Members

    private static IEnumerable<State<Context>> GetStates() {
        yield return new MoqStateA();
        yield return new MoqStateB();
        yield return new MoqStateC();
        yield return new MoqStateD();
    }
    
    private class MoqStateA : State<Context> { }
    
    private class MoqStateB : State<Context> { }
    
    private class MoqStateC : MoqStateA { }
    
    private class MoqStateD : MoqStateC { }

#endregion
}
}