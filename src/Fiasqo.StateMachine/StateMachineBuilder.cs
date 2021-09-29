using System;

namespace Fiasqo.StateMachine {
public class StateMachineBuilder<TContext>
    : StateMachineBuilder<TContext>.IDeclareFirstStateStage,
      StateMachineBuilder<TContext>.IDeclareSecondStateStage,
      StateMachineBuilder<TContext>.IDeclareMultipleStatesStage,
      StateMachineBuilder<TContext>.ISetTransitionConditionStage,
      StateMachineBuilder<TContext>.ISetTransitionDestinationStage,
      StateMachineBuilder<TContext>.IBuildStage
    where TContext : class {
    public static IDeclareFirstStateStage Create(TContext context) {
        return new StateMachineBuilder<TContext>(context);
    }

    IDeclareSecondStateStage IDeclareStateStage<IDeclareSecondStateStage>.In<TState>(Action<ISetTransitionConditionStage> cfg) {
        AddState<TState>(cfg);
        return this;
    }

    IDeclareMultipleStatesStage IDeclareStateStage<IDeclareMultipleStatesStage>.In<TState>(Action<ISetTransitionConditionStage> cfg) {
        AddState<TState>(cfg);
        return this;
    }

    ISetTransitionDestinationStage ISetTransitionConditionStage.If(Func<TContext, bool> condition) {
        _selectedStateFrameTransitionCondition = condition ?? throw new ArgumentNullException(nameof(condition));
        return this;
    }

    ISetTransitionConditionStage ISetTransitionDestinationStage.Goto<TState>() {
        var condition = _selectedStateFrameTransitionCondition;
        var stateToGo = _stateFrames[_stateFrames.GetIndexOfExistsOrAdd<TState>()].StateContainer;

        _stateFrames[_indexSelectedStateFrame].Transitions.Add(new Transition<TContext>(condition, stateToGo));

        _selectedStateFrameTransitionCondition = null;
        return this;
    }

    IBuildStage IDeclareMultipleStatesStage.InitialStateIs<TState>() {
        if (_stateFrames.TryFindIndexByStateType<TState>(out _indexInitialStateFrame)) { return this; }
        throw new InvalidOperationException($"The specified state {{{typeof(TState).Name}}} was not found. " +
                                            "State must be declared in order to set it as initial");
    }

    StateMachine<TContext> IBuildStage.Build() {
        foreach (var stateFrame in _stateFrames) { stateFrame.Transitions.TrimExcess(); }
        return new StateMachine<TContext>(_context, _stateFrames[_indexInitialStateFrame].StateContainer);
    }

#region Private Methods

    private readonly TContext _context;
    private int _indexInitialStateFrame;
    private int _indexSelectedStateFrame;
    private readonly StateFrames<TContext> _stateFrames;
    private Func<TContext, bool> _selectedStateFrameTransitionCondition;

    private StateMachineBuilder(TContext context) {
        // ReSharper disable once JoinNullCheckWithUsage
        if (context == null) { throw new ArgumentNullException(nameof(context)); }
        _context = context;
        _stateFrames = new StateFrames<TContext>();
    }

    private void AddState<TState>(Action<ISetTransitionConditionStage> cfg)
        where TState : State<TContext>, new() {
        if (cfg == null) { throw new ArgumentNullException(nameof(cfg)); }
        
        _indexSelectedStateFrame = _stateFrames.GetIndexOfExistsOrAdd<TState>();

        cfg.Invoke(this);
    }

#endregion

#region Stages

    public interface IDeclareStateStage<TReturnType> {
        TReturnType In<TState>(Action<ISetTransitionConditionStage> cfg)
            where TState : State<TContext>, new();
    }

    public interface IDeclareFirstStateStage : IDeclareStateStage<IDeclareSecondStateStage> { }

    public interface IDeclareSecondStateStage : IDeclareStateStage<IDeclareMultipleStatesStage> { }

    public interface IDeclareMultipleStatesStage : IDeclareStateStage<IDeclareMultipleStatesStage> {
        IBuildStage InitialStateIs<TState>()
            where TState : State<TContext>;
    }

    public interface ISetTransitionConditionStage {
        ISetTransitionDestinationStage If(Func<TContext, bool> condition);
    }

    public interface ISetTransitionDestinationStage {
        ISetTransitionConditionStage Goto<TState>()
            where TState : State<TContext>, new();
    }

    public interface IBuildStage {
        StateMachine<TContext> Build();
    }

#endregion
}
}