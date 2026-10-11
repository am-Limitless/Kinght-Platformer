# CLAUDE.md — Knight's Edge (Knight Platformer)

2D Mario-style platformer. Unity 6000.3.25f1, URP 2D renderer, legacy Input Manager (`activeInputHandler: 0`; no new Input System). Published on itch.io (keyboard); being ported to Android (touch) for the Play Store.
Unity project name is "Knight's Edge"; the repo is `Kinght-Platformer` (typo is real, do not "fix" it).

This is a port, not a rewrite. Keep the feel, levels and game logic. The itch.io/keyboard build must keep working.

## Scope

- Priorities: stability, bug fixes, input reliability, Android compatibility, performance, shipping.
- Do NOT add: new mechanics, enemies, abilities, levels, IAP, ads, analytics, online services, other platforms — unless asked.
- Do not turn cleanup into a rewrite. Smallest safe change that solves the request.
- Do not add packages unless there is a clear need. Check `Packages/manifest.json` first.

## Working rules

1. Inspect the existing code/scene/prefab before editing. Explain larger changes first; ask before risky ones.
2. One small, testable change at a time.
3. Preserve serialized fields, prefab/scene references and Inspector values. Do not rename scripts, objects, scenes, tags, layers or serialized fields without a strong reason. Several systems look objects up by name (`LivesText`, `StartPosition`, `Main Camera`).
4. Do not claim done without verifying. Run the game (play mode via Unity CLI), report what was verified and what was not. Never claim performance gains without a Profiler measurement.
5. After testing: stop play mode, delete temp screenshots, reset test PlayerPrefs (`CurrentLives`, `HighestUnlockedLevel`).
6. Explain trade-offs if a change could hurt Android performance, maintainability or the itch.io build.

## Code style

- Classes/properties `PascalCase`; fields/params `camelCase`; descriptive names, no cryptic abbreviations.
- `[SerializeField] private` for Inspector values; private by default. Group with `[Header]`, `[Tooltip]`, `[Min]`/`[Range]`.
- Gameplay tuning values (speeds, jump, timings, damage, scores) must be Inspector fields, not magic numbers. True implementation details stay `const`.
- Comments explain *why*, not *what*. Keep input, UI, audio and gameplay logic in separate classes. Direct calls over event architecture unless events remove real coupling.
- Fully qualify UI types in scripts (`UnityEngine.UI.Image`) — other namespaces cause ambiguity.

## Project layout (`Assets/_Project/`)

- `Scripts/Player` — `Move`, `Jump`, `Ground`, `PlayerLifeManager` (the live lives system), `PlayerTrigger`, `ParticleController`.
- `Scripts/Input` — `InputController` (abstract) + keyboard (`PlayerController`), touch (`TouchInputController`, `TouchInputBinder`, `TouchJumpZone`), `CombinedInputController`, `InteractButton`, `JumpZoneFlash`.
- `Scripts/Managers` — `GameManager` (win flow), `LevelLoader`, `LevelProgress`, `LevelSelectMenu`, `MenuManager`, `PauseMenu`, `PauseButton`, `WinPanel`, `VolumeSettings`, `HeartHudSpin`.
- `Scripts/World` — `BackgroundController` (parallax), `PlatformController`, `SwitchController`, `DoorController`, `WinTrigger`, `WorldTrigger`, `Obstacles`, `RespawnPoint`.
- `Scripts/Effects` — `ConfettiCelebration`.
- `Prefabs/{Player,UI,Managers,Environment,Hazards,Interactables,Pickups,Goal,Effects,Camera}`; `Scenes/Menus` (MainMenu, GameOver, LevelSelect) and `Scenes/Levels` (Level 1–15); `Animation`, `Art`, `Audio`. Third-party packs live in `Assets/ThirdParty`.
- Build scene order: MainMenu 0, Level 1–15 = 1–15, GameOver 16, LevelSelect 17. Keep indices stable.
- `Touch Controls.prefab` (joystick, jump zone, pause button, interact button) and `UI Panel.prefab` are shared by levels — edit the prefab, not each scene.

## Gameplay facts to preserve

- Movement: Rigidbody2D velocity steering (`Move.cs`). Jump (`Jump.cs`): coyote time, buffering, variable jump via gravityScale, one air jump (double jump). Keep top speed and jump height unless asked.
- Lives persist (`PlayerLifeManager`, PlayerPrefs `CurrentLives`). Death shows the death sprite ~1 s, then respawns at the scene's `StartPosition` (no checkpoints yet).
- Finish flag -> `WinTrigger` -> `LevelProgress.CompleteCurrentLevel()` -> `GameManager` shows Win Panel after `DelayTimer`. Level 15 uses its own `Game Win` panel.
- Level progress: PlayerPrefs `HighestUnlockedLevel`; scenes are named "Level N".
- `PlayerLives`, `Obstacles` are legacy/duplicate life code — not used by the Player prefab. Do not extend them.

## Android port rules

- Input reading stays in the input layer; movement code must not know if input is keyboard or touch. Keep keyboard working (editor/itch.io).
- Touch targets large and spaced; handle multi-touch, dropped touches, finger sliding off a button. Overlay UI must not block gameplay taps; keep the jump zone below buttons in sibling order.
- Landscape only. Canvas: Scale With Screen Size, 800x600 reference, match height. Safe-area handling is NOT done yet.
- Android back button = `Input.GetKeyDown(KeyCode.Escape)` (pause).
- Player Settings: IL2CPP, ARM64, package `com.GokulSurendran.KnightsEdge`, min API 25. Details and release steps: use the `android-build-release` skill. Performance work: `android-performance` skill. Running/testing in the Editor: `unity-cli-workflow` skill.

## Current status

Done: touch controls, pause, level select + progress, win panel + confetti, death sprite, parallax fix, switch button, debug APK tested on a phone.
Next: safe-area script, HDR off + profile on device, release keystore, Play Store assets, re-test itch.io build.
