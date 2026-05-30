using System;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalInput : MonoBehaviour {

  private PlayerInput _playerInput;

  private void OnEnable() {
    QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
    _playerInput = GetComponent<PlayerInput>();
  }

  public void PollInput(CallbackPollInput callback) {
    Quantum.Input i = new Quantum.Input();
    i.Direction = _playerInput.actions["Move"].ReadValue<Vector2>().ToFPVector2();
    callback.SetInput(i, DeterministicInputFlags.Repeatable);
  }
}
