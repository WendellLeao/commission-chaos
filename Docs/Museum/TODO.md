# Museum Night Shift — TODO

Source: `Docs/Multiplayer Chaos Game Ideas.pdf` — idea #4 "Museum Night Shift".

## Premise

Players transport valuable artifacts out of the museum before it opens. The artifacts are haunted:
statues watch the players, paintings change room layouts, dinosaur skeletons collapse, ancient masks
possess players, doors disappear, rooms rotate.

Scope for this first slice (a few days, agent experimentation): prove the multiplayer *core loop*
(carry/deliver an artifact) + 1-2 chaos systems, not the full game.

## Project conventions (follow, don't reinvent)

- Owner-gating via `enabled = IsOwner;` in `OnStartClient()` (see `MyPlayerController`).
- Generic `SyncVar<T>` (FishNet 4.x), `OnChange` subscribed in `Awake()`, unsubscribed in `OnDestroy()`.
- RPC flow: client input → `[ServerRpc]` → (if other clients need to observe) → `[ObserversRpc]`.
- New scripts go in `Assets/_Project/Scripts/FishNetTest/` (or a new `Museum/` subfolder inside it —
  decide when implementation starts), following the `_Project` namespace already used by
  `MyPlayerIdProvider`/`MyPlayerColorChanger`.
- New prefabs go in `Assets/_Project/Prefabs/`.
- Do not edit `Assets/FishNet/` (vendor).

## Phase 0 — Session foundation (blocks everything)

- [ ] Confirm/configure `NetworkManager` in the scene (transport, player spawning) — not done yet
- [ ] Confirm `MyPlayer` spawns correctly for host + client
- [ ] `MuseumArtifact` (`NetworkObject` + `NetworkBehaviour`):
  (`SyncVar<bool> IsHeld`, `SyncVar<NetworkObject>` holder)
- [ ] Pickup/drop: `[ServerRpc]` validated on the server (client-side `NetworkObject`
  parenting/ownership via FishNet's API)
- [ ] `DropOffZone`: when a player carrying an artifact enters the zone, the server validates and
  marks the artifact as "delivered" (`SyncVar<bool>`), adding to the match score.
- [ ] Match-wide score/objective `SyncVar` (artifacts delivered / total).
- [ ] Verify: two clients can pick up different artifacts simultaneously without conflict; a player
  cannot steal an artifact another player is already holding (server-side validation).

## Phase 2 — Chaos Mode: items that spawn chaotic (the game's differentiator)

Core idea: common pickups have a random (not too rare) chance of spawning "chaotic" — gaining a
special behavior that creates extra interaction/challenge with the player. Each behavior is an
independent, pluggable component, following the "build systems, not content" principle (see Notes).

- [x] Chaotic spawn flag/roll on the existing `PickupSpawner`: when spawning a pickup, roll whether
  it spawns chaotic and, if so, which chaotic behavior it gets (weighted roll, not uniform). Done
  only for the fleeing item so far (`chaoticFleeingPrefab` + `chaoticSpawnChance` on
  `PickupSpawner`, simple uniform roll); becomes a weighted roll across behaviors once more than
  one exists.
- [ ] Define an enum/list of available chaotic behaviors and one component per behavior (its own
  `NetworkBehaviour`, attached/enabled only when the item spawns chaotic with that type).

### Behavior 1 — Fleeing item (hops away)
- [x] Detects player proximity (radius/distance) and flees in a hopping motion (zigzag)
- [x] If the player holds it too long without delivering it, auto-drops (auto-drop
  animation/position via `NetworkTransform`)
- [x] `ChaoticFleeingItem.cs` + `PickupItem_Fleeing.prefab`, wired into the spawner's roll/spawn flow
- [x] Hops are gated on ground contact

### Behavior 2 — Tall and fragile item (sways and breaks)
- [ ] While the player carries the item, aggressive turns (sudden changes in direction/angular
  velocity) make the item sway to the side opposite the turn.
- [ ] If the turn is too aggressive (above a threshold), the item topples, falls to the ground, and
  breaks permanently ("broken" state — can no longer be delivered).
- [ ] Verify: turn-aggressiveness calculation runs on the server (authority over whether it breaks);
  the visual sway can be purely cosmetic on the client, with no gameplay depending on it.

### Behavior 3 — Trap item (grabs the player)
- [ ] On pickup, chance that the item "captures" the player: grabs them and launches them away
  (impulse/knockback) instead of being collected normally.
- [ ] Verify: outcome (capture vs. normal pickup) decided on the server at interaction time;
  knockback synchronized without a perceptible jarring teleport for other clients.

- [ ] Verify (general): chaotic items still count toward the score objective like normal pickups when
  successfully delivered (except the broken fragile item, which is lost).

## Phase 3 — Chaos #1: Statue that stalks (small scope, high visual impact)

- [ ] `HauntedStatue`: statue with a `NavMeshAgent` (or simple movement) that freezes when observed
  by a player (`OnStartClient`/server tick computes each player's view angle vs. the statue) and
  advances slowly when nobody is looking. `SyncVar<bool> IsBeingWatched` drives position sync via
  `NetworkTransform`.
- [ ] Verify: movement is smooth and synchronized across clients; no visible flicker/pop when the
  watched state changes.

## Phase 4 — Chaos #2: Rotating room

- [ ] Platform/periodic rotation (`SyncVar` for angle, or `NetworkTransform` on the room's root
  object) that reorganizes the path to the exit.
- [ ] Verify: rotation is smooth and synchronized across clients (no perceptible desync).

## Phase 5 — Minimal match loop

- [ ] Match timer (`SyncVar<float>` counting down, updated only on the server).
- [ ] Simple win/lose condition: all artifacts delivered before the timer runs out.
- [ ] End-of-match screen/state (can be just a minimal log/UI — not the focus of this slice).

## Out of scope (for now)

- Multiple rooms/maps, fleeing furniture, fridges, pianos, etc. (ideas from other minigames in the GDD).
- Random match modifiers (low gravity, blackout, etc.) — a general GDD idea, not museum-specific;
  evaluate after the core loop is stable.
- Final art/assets — use placeholders (cubes/capsules) while validating mechanics.
- Polished UI.

## Notes

- Always test with host + at least 1 client build/ParrelSync (or 2 Editor instances, if configured) —
  client/server authority bugs don't show up when running solo as host.
- Keep chaos systems as independent, pluggable components (each its own `NetworkBehaviour`), so they
  can be toggled and combined later — aligned with the GDD principle of "build systems, not content."
