using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fiasqo.StateMachine.Tests")]

namespace Fiasqo.StateMachine {
public sealed class StateMachine<TContext>
    : IStateMachine
    where TContext : class {
    public State<TContext> CurrentState => _currentStateContainer.State;

    public void Enable() {
        if (_isFirstEnable) {
            _currentStateContainer.State.OnEnter(_context);
            _isFirstEnable = false;
        }

        if (_isEnable) { return; }
        _isEnable = true;
        _currentStateContainer.State.OnEnable(_context);
    }

    public void Disable() {
        if (!_isEnable) { return; }
        _isEnable = false;
        _currentStateContainer.State.OnDisable(_context);
    }

    public void Update() {
        if (!_isEnable) { return; }
        _currentStateContainer.State.OnUpdate(_context);
    }

    public void FixedUpdate() {
        if (!_isEnable) { return; }
        _currentStateContainer.State.OnFixedUpdate(_context);
    }

    public void LateUpdate() {
        if (!_isEnable) { return; }
        _currentStateContainer.State.OnLateUpdate(_context);

        if (!_currentStateContainer.TryGetNextState(_context, out var nextStateContainer)) { return; }
        ChangeState(nextStateContainer);
    }

    internal StateMachine(TContext context, IStateContainer<TContext> initStateContainer) {
        // ReSharper disable once JoinNullCheckWithUsage
        if (context == null) { throw new ArgumentNullException(nameof(context)); }
        _context = context;
        _currentStateContainer = initStateContainer ?? throw new ArgumentNullException(nameof(initStateContainer));
    }

    private void ChangeState(IStateContainer<TContext> nextStateContainer) {
        _currentStateContainer.State.OnDisable(_context);
        _currentStateContainer.State.OnExit(_context);
        _currentStateContainer = nextStateContainer;
        _currentStateContainer.State.OnEnter(_context);
        _currentStateContainer.State.OnEnable(_context);
    }
    
    private readonly TContext _context;
    private IStateContainer<TContext> _currentStateContainer;
    private bool _isEnable;
    private bool _isFirstEnable = true;
}
}