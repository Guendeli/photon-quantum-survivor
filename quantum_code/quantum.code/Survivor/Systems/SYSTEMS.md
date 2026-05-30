# User Systems Overview

This document describes the user systems registered in `SystemSetup.cs` for the Quantum Survivor simulation. They run inside a single `SystemGroup("gameplay", ...)` and are listed below in execution order.

> Engine-provided systems (`CullingSystem2D`, `PhysicsSystem2D`, `DebugCommand`, `EntityPrototypeSystem`, `PlayerConnectedSystem`) are omitted — only the project's own systems are covered.

---

## Setup / Spawning

### `SpawnCharacterSystem` (`SystemSignalsOnly`)
Runs once on `OnInit`. Loads the `CharacterRoster` referenced by `RuntimeConfig` and spawns one character per player (or just one when `TestOnePlayer` is set).

---

## Input

### `PlayerInputSystem` (`SystemMainThreadFilter<PlayerLink, InputContainer>`)
Copies the deterministic input from the real player into each character's `InputContainer`. Doubles as the bot controller: if a slot is disconnected or unused in a single-player session it attaches a `BotData` component, picks a human leader to follow, and synthesizes movement input that stays near the leader, away from the other ally, and away from nearby monsters. Also auto-picks the first power-up option for bots while the game is in `Selecting` state.

### `MonsterInputSystem` (`SystemThreadedFilter<Monster>`) — thread-unsafe / parallel
For every monster, delegates to `Monster.UpdateInput`, which writes the monster's desired movement direction into its `InputContainer` (typically aim toward the assigned target character). Runs in parallel, so it must not perform frame queries.

---

## Commands (player intents from the view)

### `ChoosePowerUpCommandSystem` (`SystemMainThread`)
Polls each player slot for a `ChoosePowerUpCommand` and forwards it as the `OnPlayerChoosePowerUp` signal with the selected option index.

### `MoreTimeCommandSystem` (`SystemMainThread`)
Polls each player slot for a `MoreTimeCommand` and emits `OnSetMoreChooseTime` so the power-up selection window is extended.

---

## Power-ups & Progression

### `PowerUpSystem` (`SystemSignalsOnly`)
Reacts to three signals:
- `OnPlayerChoosePowerUp` — applies the chosen `PowerUpSpec` to the character and removes the `PowerUpSelection` component.
- `OnSetMoreChooseTime` — adds 10s to `TeamProgression.TimeToSelect`.
- `OnTeamLevelUp` — increments level, resets XP, switches state to `Selecting`, sets the 10s selection timer, and fills a fresh `PowerUpSelection` for every alive character (fires `OnLevelUp` event for the view).

### `ProgressionSystem` (`SystemMainThread`)
Drives the `Selecting` state: ticks down `TimeToSelect`, and once it hits zero (or every player has picked) flips state back to `Playing` and emits `OnHideLevelUpPanel`.

### `ProgressionQueryInjectSystem` (`SystemMainThreadFilter`)
For every character, schedules a circular `OverlapShapeQuery` (radius `1.5`) against monsters via `Physics2D`. The query result is consumed later by the update system.

### `ProgressionQueryUpdateSystem` (`SystemMainThreadFilter`, `ISignalOnCharacterDamage`)
Consumes the character's overlap query: any monster hit becomes that character's `Target`, gets pushed out to avoid overlap, and triggers an `OnCharacterDamage` signal when actually touching. Also tells nearby bots which monster to flee from. The signal handler subtracts HP and destroys the character at zero.

---

## Waves & Enemies

### `WavesManagerSystem` (`SystemMainThread`)
Caches the `MapWaves` asset on init and ticks the singleton `WavesManager` each frame, which decides when to spawn new `RuntimeWave` entities. Pauses when no players are connected or the game isn't in `Playing` state.

### `WaveSystem` (`SystemMainThreadFilter<RuntimeWave>`, `ISignalOnComponentRemoved<Waveable>`)
For every active `RuntimeWave`, asks its `WaveConfig` to spawn up to 5 enemies per frame until either `SpawnCount` is met or the wave's `EndTime` passes (at which point the wave entity is destroyed). The signal handler decrements the wave's live-spawn counter whenever a `Waveable` enemy is removed.

### `MonsterSystem` (`SystemMainThreadFilter<Monster>`)
Per-monster logic: if `ShouldDie` is set, spawn the configured drop at the monster's position and destroy the entity; otherwise, when the monster has lost its target, request a new one via `SetMonsterTarget`.

---

## Combat / Abilities

### `AbiltiySystem` (`SystemMainThreadFilter<Abilities, Transform2D>`) — sic, typo in class name
Ticks each enabled ability's cooldown. When ready, it loads the ability's `ProjectileSpec`, resolves a firing direction, spawns the projectile at the character's position, and resets the timer to `Cooldown`.

### `ProjectileQueryInjectSystem` (`SystemMainThreadFilter<Projectile>`)
Each frame, asks the projectile's `ProjectileSpec` to register a physics query (e.g. an overlap shape) tied to that projectile, recording its `QueryID`.

### `ProjectileQueryUpdateSystem` (`SystemMainThreadFilter<Projectile>`, `ISignalOnComponentAdded<Projectile>`)
Counterpart to the inject system. Consumes the query results via `Spec.UpdateQuery` (hit resolution, damage etc.), moves the projectile via `Spec.Move`, and calls `Spec.OnDestroy` once `TTL` hits zero. The `OnAdded` signal initializes `QueryID = -1`.

---

## Collectibles (XP & loot)

### `CollectibleSystem` (`SystemMainThread`)
Iterates every `Collectible` on a rolling 10-frame schedule (load-balanced by entity index). Decays each collectible's `TTL` (scaled by the live collectible count to clear the field faster when it's crowded) and destroys it when it expires. For each one, checks whether any character's `CollectibleArea` overlaps it, and if so attaches a `MagneticLock` so the item is pulled in.

### `MagneticLockSystem` (`SystemMainThreadFilter<MagneticLock, Transform2D>`)
Moves a magnetically-locked entity toward its target character (with a brief initial repulsion phase while `Time > 0` to create a "pop out" effect). When close enough, fetches the `CollectibleSpec`, applies the collectible effect to the target (XP gain, etc.), and destroys the entity.

---

## Movement

### `CharacterMovementSystem` (`SystemThreadedFilter<CharacterController>`) — thread-unsafe / parallel
Runs last, in parallel across characters. Delegates to `CharacterController.Update`, which integrates the input from `InputContainer` into the character's transform. Skipped while the game is not in `Playing` state. Cannot query the frame because it runs on worker threads.

---

## Execution order summary

1. Query injection (Projectile, Progression) — schedule physics queries early
2. Physics2D — solves collisions / resolves queries
3. Engine entity & player setup
4. Spawn + Commands + PowerUp
5. `MonsterInputSystem` (parallel)
6. Waves / input / abilities / query consumers / monsters / collectibles / progression / magnetic lock
7. `CharacterMovementSystem` (parallel) — final position integration
