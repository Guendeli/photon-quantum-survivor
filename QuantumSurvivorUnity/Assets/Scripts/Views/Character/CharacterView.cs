using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Quantum;
using UnityEngine;
using UnityEngine.Events;

public unsafe class CharacterView : QuantumCallbacks
{
  private EntityView _entityView;
  private Animator _animator;
  private SpriteRenderer _characterSprite;

  private void Start()
  {
    _entityView = GetComponentInParent<EntityView>();
    _animator = GetComponent<Animator>();
    _characterSprite = GetComponent<SpriteRenderer>();
  }
  public override void OnUpdateView(QuantumGame game)
  {
    var f = game.Frames.Verified;

    if (_entityView == null || f.Exists(_entityView.EntityRef) == false)
    {
      return;
    }

    var cc = f.Get<Quantum.CharacterController>(_entityView.EntityRef);

    UpdateAnimator(cc.Velocity.ToUnityVector2());
    UpdateDirection(cc.Velocity.ToUnityVector2());
  }

  private void UpdateDirection(Vector2 direction)
  {
    if (direction.x == 0)
    {
      return;
    }
    _characterSprite.flipX = direction.x < 0;
  }

  private void UpdateAnimator(Vector2 direction)
  {
    _animator.SetFloat("Speed", direction.magnitude);
  }
}