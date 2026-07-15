# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

This is a personal study project for learning **FishNet: Networking Evolved** (v4.7.2) in **Unity 6000.3.10f1** (URP). It is not a shipping product — it's a sandbox for experimenting with FishNet's networking primitives (`NetworkBehaviour`, `SyncVar<T>`, `[ServerRpc]`, `[ObserversRpc]`, `NetworkTransform`).

There is no build/test/lint tooling outside the Unity Editor itself — this is a GUI-driven Unity project, not a CLI-buildable one. Compilation happens when the project is opened/recompiled in the Editor (Rider is configured as the external script editor via `com.unity.ide.rider`).

## Project structure

- `Assets/_Project/Scripts/FishNetTest/` — all first-party gameplay code lives here. Currently:
  - `MyPlayerController.cs` — owner-only client movement via `CharacterController` + the new Input System (polls `Keyboard.current` directly, no `PlayerInput`/action-map asset wiring yet).
  - `MyPlayerIdProvider.cs` — assigns a synced player id (`SyncVar<string>`) on `OnStartClient`, set via a `[ServerRpc]`.
  - `MyPlayerColorChanger.cs` — demonstrates a client → server → observers RPC round trip (`[ServerRpc]` then `[ObserversRpc]`) gated by comparing `MyPlayerIdProvider` identity so only the intended object's renderer changes color.
  - Namespace is inconsistent today: `MyPlayerController` has no namespace; `MyPlayerIdProvider` and `MyPlayerColorChanger` use `namespace _Project`. Match whichever convention the file you're editing already uses rather than "fixing" this project-wide unless asked.
- `Assets/_Project/Prefabs/MyPlayer.prefab` — the networked player prefab. Component order: `NetworkObject` → `NetworkTransform` → `MyPlayerIdProvider` → `MyPlayerController` → `MyPlayerColorChanger`. All three custom scripts are `NetworkBehaviour`s on the same prefab, so they resolve each other via `GetComponent<T>()` on the same GameObject (no cross-prefab references needed).
- `Assets/_Project/Scenes/SampleScene.unity` — the only scene.
- `Assets/FishNet/` — the FishNet package source, vendored directly into `Assets` (not via Package Manager/UPM). Treat it as third-party/vendor code: don't modify it to fix a gameplay bug — fix the call site in `_Project` instead. Docs: `Assets/FishNet/DOCUMENTATION.txt`, `Assets/FishNet/Upgrading/`.
- Single default assembly (`Assembly-CSharp`) — no `.asmdef` split between project code and FishNet, and no test assemblies exist yet despite `com.unity.test-framework` being installed.

## FishNet lifecycle patterns already in use (follow these, don't reinvent)

- **Owner-gating**: every `NetworkBehaviour` here disables itself for non-owners inside `OnStartClient()` via `enabled = IsOwner;` (always calling `base.OnStartClient()` first). Follow this pattern for any new owner-only behaviour rather than checking `IsOwner` inline in `Update()`.
- **Host vs client-only detection**: use `IsHostInitialized` / `IsClientOnlyInitialized` (see `MyPlayerIdProvider.BuildPlayerId()`), not `IsHost`/`IsClient` alone, when the distinction matters.
- **State sync**: use `SyncVar<T>` (FishNet 4.x generic syncvar API, e.g. `private readonly SyncVar<string> _playerId = new();`) with an `OnChange` handler subscribed in `Awake()` and unsubscribed in `OnDestroy()`. This project does not use the older non-generic `SyncVar` attribute style.
- **RPC direction**: client input triggers a `[ServerRpc]`, which — if other clients need to observe the effect — forwards to an `[ObserversRpc]`. See `MyPlayerColorChanger` for the full pattern, including passing an identity token (`MyPlayerIdProvider`) through the RPC so observers can filter for the correct target object.

## Notes for future work

- Input is currently read via direct `Keyboard.current` polls, not the generated `InputSystem_Actions` asset at `Assets/_Project/Inputs/InputSystem_Actions.inputactions` — that asset exists but isn't wired up yet.
- No NetworkManager configuration, connection/transport setup, or UI has been described in the code reviewed here; check the scene in the Editor for current wiring before assuming how sessions are started.
