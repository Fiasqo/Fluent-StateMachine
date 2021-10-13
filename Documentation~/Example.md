# Unity Example

```c#
using Fiasqo.StateMachine;
using UnityEngine;

namespace Fiasqo.GameName.Runtime.Game {
public class HorizontalMovement : MonoBehaviour {
    public float Speed => _speed;
    public Transform Transform { get; private set; }

    [SerializeField] private float _speed = 2F;
    private StateMachine<HorizontalMovement> _sm;

    private void Awake() {
        Transform = transform;

        _sm = StateMachineBuilder<HorizontalMovement>
            .Create(this)
            .In<Idle>(tb => {
                tb.If(x => Input.GetAxis("Vertical") > 0F).Goto<Jump>()
                  .If(x => Input.GetAxis("Horizontal") != 0F).Goto<HorizontalMove>();
            })
            .In<Jump>(tb => {
                tb.If(x => Transform.position.y <= 0F).Goto<Idle>();
            })
            .In<HorizontalMove>(tb => {
                tb.If(x => Input.GetAxis("Horizontal") == 0F).Goto<Idle>()
                  .If(x => Input.GetAxis("Vertical") > 0F).Goto<Jump>();
            })
            .InitialStateIs<Idle>()
            .Build();
    }

    private void OnEnable() {
        _sm.Enable();
    }

    private void OnDisable() {
        _sm.Disable();
    }

    private void Update() {
        _sm.Update();
        Debug.Log(_sm.CurrentState.GetType().Name);
    }

    private void FixedUpdate() {
        _sm.FixedUpdate();
    }

    private void LateUpdate() {
        _sm.LateUpdate();
    }
}

public class Idle : State<HorizontalMovement> { }

public class HorizontalMove : State<HorizontalMovement> {
    public override void OnUpdate(HorizontalMovement context) {
        var direction = Input.GetAxis("Horizontal");
        context.Transform.position += new Vector3(direction * context.Speed * Time.deltaTime, 0F, 0F);
    }
}

public class Jump : State<HorizontalMovement> {
    private const float Gravity = -9.81F;
    private const float GravityScale = 5F;
    private const float JumpHeight = 3F;
    private float _velocity;

    public override void OnEnter(HorizontalMovement context) {
        _velocity = Mathf.Sqrt(JumpHeight * -2F * (Gravity * GravityScale));
    }

    public override void OnUpdate(HorizontalMovement context) {
        _velocity += Gravity * GravityScale * Time.deltaTime;

        if (context.Transform.position.y <= 0F && _velocity < 0) {
            _velocity = 0;
            context.Transform.position = new Vector3(context.Transform.position.x, 0F, 0F);
        }

        context.Transform.Translate(new Vector3(0, _velocity, 0) * Time.deltaTime);
    }

    public override void OnExit(HorizontalMovement context) {
        context.Transform.position = new Vector3(context.Transform.position.x, 0F, 0F);
        _velocity = 0;
    }
}
}
```