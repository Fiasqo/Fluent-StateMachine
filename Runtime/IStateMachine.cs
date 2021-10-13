namespace Fiasqo.FluentStateMachine {
public interface IStateMachine {
    void Enable();
    void Disable();

    void Update();
    void FixedUpdate();
    void LateUpdate();
}
}