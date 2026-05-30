using System;
using Photon.Deterministic;

namespace Quantum
{
  public unsafe class PlayerInputSystem : SystemMainThreadFilter<PlayerInputSystem.Filter>
  {
    public struct Filter
    {
      public EntityRef Entity;
      public PlayerLink* Link;
      public InputContainer* InputContainer;
    }

    // just copy Input from real player
    public override void Update(Frame f, ref Filter filter)
    {
      if (f.Unsafe.TryGetPointerSingleton<TeamProgression>(out var progression))
      {
        if (progression->State != GameState.Playing)
        {
          filter.InputContainer->Input = default;
          UpdateBotSelection(f, ref filter);
          return;
        }
      }

      if (UpdateBot(f, ref filter) == false)
      {
        filter.InputContainer->Input = *f.GetPlayerInput(filter.Link->Player);
        if (filter.InputContainer->Input.Direction != default)
        {
          filter.Link->LastInputDirection = filter.InputContainer->Input.Direction;
        }
      }
    }

    private void UpdateBotSelection(Frame f, ref Filter filter)
    {
      if (f.Unsafe.TryGetPointer<BotData>(filter.Entity, out var data))
      {
        if (f.Unsafe.TryGetPointer<PowerUpSelection>(filter.Entity, out var selection))
        {
          // AI always select first suggestion
          f.Signals.OnPlayerChoosePowerUp(filter.Link->Player, 0);
        }
      }
    }

    private Boolean UpdateBot(Frame f, ref Filter filter)
    {
      var flags = f.GetPlayerInputFlags(filter.Link->Player);
      var playerDisconnected =
        (flags & DeterministicInputFlags.PlayerNotPresent) == DeterministicInputFlags.PlayerNotPresent;
      var localGameBot = f.SessionConfig.PlayerCount == 1 && filter.Link->Player != 0;
      if (playerDisconnected || localGameBot)
      {
        if (f.Has<BotData>(filter.Entity) == false)
        {
          f.Add<BotData>(filter.Entity);
        }
        UpdateBotInput(f, ref filter);
        return true;
      }
      else if (f.Has<BotData>(filter.Entity))
      {
        f.Remove<BotData>(filter.Entity);
      }

      return false;
    }

    private void UpdateBotInput(Frame f, ref Filter filter)
    {
      //filter.InputContainer->Input = default;
      if (f.Unsafe.TryGetPointer<BotData>(filter.Entity, out var data))
      {
        // no leader, or leader is now a bot
        var notAlone = data->State != BotStates.Alone;
        var noLeader = f.Exists(data->Leader) == false || f.Has<BotData>(data->Leader);
        if (notAlone && noLeader)
        {
          EntityRef leader = default;
          if (FindLeader(f, out leader))
          {
            data->Leader = leader;
            FindOther(f, leader, filter.Entity, out data->Other);
            data->State = BotStates.Following;
          }
          else
          {
            data->State = BotStates.Alone;
          }
        }

        switch (data->State)
        {
          case BotStates.Following:
            UpdateFollowing(f, ref filter, data);
            break;
          case BotStates.Alone:
            break;
        }

      }
    }

    private void UpdateFollowing(Frame f, ref Filter filter, BotData* data)
    {
      if (f.Number % 10 != 0) { return; }
      if (f.Exists(data->Leader) == false)
      {
        data->State = BotStates.None;
        data->Leader = default;
        return;
      }
      var leaderTransform = f.Get<Transform2D>(data->Leader);
      var myTransform = f.Get<Transform2D>(filter.Entity);
      var direction = leaderTransform.Position - myTransform.Position;
      FPVector2 inputDirection = FPVector2.Zero;
      var distance = direction.Magnitude;
      if (distance < 2)
      {
        inputDirection += -direction.Normalized;
      }
      else if (distance > 8)
      {
        inputDirection += direction.Normalized;
      }

      if (f.Unsafe.TryGetPointer<Transform2D>(data->Other, out var otherTransform))
      {
        direction = otherTransform->Position - myTransform.Position;
        distance = direction.Magnitude;
        if (distance < 8)
        {
          inputDirection += -direction.Normalized;
        }
      }

      if (f.Unsafe.TryGetPointer<Transform2D>(data->CloseMonster, out var monsterTransform))
      {
        direction = monsterTransform->Position - myTransform.Position;
        distance = direction.Magnitude;
        if (distance < 4)
        {
          inputDirection += -direction.Normalized;
        }
      }

      filter.InputContainer->Input.Direction = inputDirection;
      if (inputDirection != default)
      {
        filter.Link->LastInputDirection = inputDirection;
      }
    }

    private bool FindLeader(Frame f, out EntityRef leader)
    {
      leader = default;
      if (f.Unsafe.TryGetPointerSingleton<TeamProgression>(out var team))
      {
        for (int i = 0; i < team->Characters.Length; i++)
        {
          var character = team->Characters[i];
          // potentially not a bot
          if (f.Has<BotData>(character) == false)
          {
            leader = character;
            return true;
          }
        }
      }
      return false;
    }

    private void FindOther(Frame f, EntityRef leader, EntityRef me, out EntityRef other)
    {
      other = default;
      if (f.Unsafe.TryGetPointerSingleton<TeamProgression>(out var team))
      {
        for (int i = 0; i < team->Characters.Length; i++)
        {
          var character = team->Characters[i];
          // potentially not a bot
          if (character != leader && character != me)
          {
            other = character;
          }
        }
      }
    }

  }
}