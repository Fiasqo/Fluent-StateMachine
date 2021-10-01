using System.Collections.Generic;

namespace Fiasqo.StateMachine {
internal class StateFrames<TContext> : List<StateFrame<TContext>>
    where TContext : class {
    public StateFrames() { }

    public StateFrames(int capacity) : base(capacity) { }

    public StateFrames(IEnumerable<StateFrame<TContext>> collection) : base(collection) { }

    public bool TryFindIndexByStateType<TState>(out int index)
        where TState : State<TContext> {
        index = FindIndex(sf => sf.StateContainer.State is TState &&
                                !sf.StateContainer.State.GetType().IsSubclassOf(typeof(TState)));

        return index >= 0;
    }

    public int GetIndexOfExistsOrAdd<TState>()
        where TState : State<TContext>, new() {
        if (TryFindIndexByStateType<TState>(out var stateFrameIndex)) { return stateFrameIndex; }

        stateFrameIndex = Count;
        Add(new StateFrame<TContext>(new TState()));

        return stateFrameIndex;
    }
}
}