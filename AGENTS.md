# AGENTS.md

## 1. Project Identity

- Game name: `Knight's Edge` in Project Settings. The repo/readme still contains older naming like `Cube-Boy-2D-platformer` / `Kinght-Platformer`.
- Genre: 2D platformer.
- Unity version: `6000.3.21f1`.
- Render pipeline: Universal Render Pipeline (URP), based on the Universal 2D template.
- Current gameplay concept: a simple level-based platformer where the player moves through handcrafted stages, avoids hazards, collects hearts, reaches a finish flag, and advances linearly through the game.
- Current playable build scenes: 17 enabled build scenes.
  - `MainMenu`
  - `Level 1` through `Level 15`
  - `GameOver`

## 2. Core Gameplay

Only document behavior that is currently implemented:

- Left/right movement: implemented with Rigidbody2D horizontal velocity steering in `Assets/Scripts/Capabilities/Move.cs`.
- Jump: implemented in `Assets/Scripts/Capabilities/Jump.cs`.
- Double jump: currently implemented as one extra air jump via `_maxAirJumps = 1` by default.
- Coyote time: implemented in `Jump.cs`.
- Jump buffering: implemented in `Jump.cs`.
- Variable jump / gravity behavior: implemented in `Jump.cs` by changing `gravityScale` depending on vertical motion and jump-hold state. The exact feel/logic should be preserved unless a task explicitly changes it.
- Lives and death: player has persistent lives managed by `Assets/Scripts/Game Manager/PlayerLifeManager.cs`.
- Respawning: on losing a life, the player respawns at the scene's `StartPosition` object.
- Finish / level completion: the finish flag triggers `WinTrigger`, then `GameManager` shows the win panel and the next level can be loaded.

## 3. Project Structure

Important folders:

- `Assets/Scripts`
  - `Capabilities`: player movement/jump behavior
  - `Controllers`: input abstraction, door/switch, particles, volume
  - `Checks`: triggers, ground detection, win/death helpers
  - `Game Manager`: lives, menu, pause, scene loading, win flow
  - `Platforms`: moving platform logic
  - `Background`: parallax background logic
- `Assets/Scenes`: main menu, gameplay levels, game over, test scene
- `Assets/Prefabs`: player, managers, UI, finish flag, traps, platforms, doors/switches
- `ProjectSettings`: Unity/editor/input/build configuration
- `Packages`: package manifest and lockfile

Important scenes:

- `Assets/Scenes/MainMenu.unity`
- `Assets/Scenes/Level 1.unity` through `Assets/Scenes/Level 15.unity`
- `Assets/Scenes/GameOver.unity`
- `Assets/Scenes/TEST.unity` is present but not part of the main build flow

Important prefabs:

- `Assets/Prefabs/Player/Player.prefab`
- `Assets/Prefabs/Managers/Level Manager.prefab`
- `Assets/Prefabs/Managers/Pause Manager.prefab`
- `Assets/Prefabs/UI/UI Panel.prefab`
- `Assets/Prefabs/UI/Pause Panel.prefab`
- `Assets/Prefabs/UI/Win Panel.prefab`
- `Assets/Prefabs/Loading Level/Loading Panel.prefab`
- `Assets/Prefabs/Win Flag/Finsh Flag.prefab`
- `Assets/Prefabs/Platforms/Moving Platforms.prefab`
- `Assets/Prefabs/Door & Switch/Door.prefab`
- `Assets/Prefabs/Door & Switch/Switch.prefab`
- `Assets/Prefabs/Traps/*`

Where to look for major systems:

- Player movement: `Assets/Scripts/Capabilities/Move.cs`
- Jumping: `Assets/Scripts/Capabilities/Jump.cs`
- Input abstraction: `Assets/Scripts/Controllers/InputController.cs`
- Current player input implementation: `Assets/Scripts/Controllers/PlayerController.cs`
- Player lives/death/respawn: `Assets/Scripts/Game Manager/PlayerLifeManager.cs` and `Assets/Scripts/Checks/PlayerTrigger.cs`
- Level loading: `Assets/Scripts/Game Manager/LevelLoader.cs`
- Game flow / win flow: `Assets/Scripts/Game Manager/GameManager.cs`, `Assets/Scripts/Checks/WinTrigger.cs`, `Assets/Scripts/Game Manager/Menu Manager.cs`, `Assets/Scripts/Game Manager/PauseMenu.cs`
- UI / HUD / volume controls: `Assets/Scripts/Controllers/VolumeSettings.cs` plus UI prefabs under `Assets/Prefabs/UI`
- Hazards / obstacle interaction: `Assets/Scripts/Checks/PlayerTrigger.cs`, `Assets/Scripts/Checks/Obstacles.cs`, trap prefabs in `Assets/Prefabs/Traps`
- Moving platforms: `Assets/Scripts/Platforms/PlatformController.cs`
- Switches / doors: `Assets/Scripts/Controllers/SwitchController.cs`, `Assets/Scripts/Controllers/DoorController.cs`

## 4. Current Input System

- The project currently uses the legacy Unity Input Manager.
- Evidence:
  - `ProjectSettings/ProjectSettings.asset` has `activeInputHandler: 0`
  - `ProjectSettings/InputManager.asset` defines axes/buttons like `Horizontal`, `Jump`, `Submit`, and `Cancel`
- Current gameplay input handling:
  - `Assets/Scripts/Controllers/PlayerController.cs`
    - `Input.GetAxisRaw("Horizontal")`
    - `Input.GetButtonDown("Jump")`
    - `Input.GetButton("Jump")`
  - `Assets/Scripts/Game Manager/PauseMenu.cs`
    - `Input.GetKeyDown(KeyCode.Escape)`
  - `Assets/Scripts/Controllers/SwitchController.cs`
    - `Input.GetKeyDown(KeyCode.E)`
- UI currently uses legacy-style uGUI event handling and `StandaloneInputModule`.

Planned future note:

- Android touch controls are planned for the future, but should not be implemented unless explicitly requested.
- If the project is later migrated to the new Input System, preserve existing keyboard controls during development and avoid breaking current gameplay behavior.

## 5. Development Rules

- Prefer the smallest safe change that solves the requested problem.
- Do not add new gameplay mechanics unless explicitly requested.
- Do not redesign existing systems without a clear reason.
- Do not refactor large portions of the project unnecessarily.
- Preserve existing gameplay behavior unless the task explicitly asks to change it.
- Preserve serialized fields, prefab references, scene references, and inspector configuration whenever possible.
- Do not rename scripts, GameObjects, scenes, tags, layers, or serialized fields unless absolutely necessary.
- Before making significant changes, inspect the existing implementation and explain the planned change.
- Avoid introducing new packages or third-party dependencies unless explicitly requested.
- Do not replace working systems merely because another architecture is theoretically cleaner.

## 6. Scope Rules

This is a small, simple platformer intended to be accessible to a broad age range.

Current release priorities:

1. Stability
2. Bug fixing
3. Input reliability
4. Android compatibility
5. Performance
6. Maintainability where it directly helps release stability
7. Small quality-of-life improvements

Do not prioritize:

- New enemies
- New player abilities
- New gameplay systems
- Large architectural rewrites
- Unnecessary visual systems
- Feature creep
- Expanding the game's scope before release

## 7. Before Editing

For any future task:

1. Inspect the relevant existing implementation first.
2. Identify the smallest set of files that actually needs to change.
3. Explain what you intend to change before making significant edits.
4. Avoid unrelated modifications.
5. After making changes, verify that the affected system still works.

For larger or potentially risky changes, ask for confirmation before implementation.

## 8. Known Issues — Not Yet Fixed

These were identified during the initial project audit. Document them, but do not treat this file as permission to rewrite systems broadly.

- Win coroutine may be started repeatedly because `GameManager.Update()` can call `StartCoroutine(WinMenuTimer())` every frame after win is triggered.
- Next-scene progression uses hardcoded assumptions instead of deriving safely from build scenes.
- Jump gravity / jump-hold behavior needs verification before changing feel.
- Movement has a potential stale acceleration calculation path in `Move.cs`.
- Persistent player/life handling is tightly coupled and scene-flow-sensitive.
- Checkpoint implementation is unfinished. Respawn currently depends on `StartPosition`.
- There are duplicate / legacy life-damage systems in the project (`PlayerLifeManager`, `PlayerLives`, `Obstacles`, `PlayerTrigger` overlap).
- Excessive per-frame `Debug.Log` calls exist, especially in player movement.
- Some code uses obsolete Unity APIs such as `FindObjectOfType`.
- Several systems depend on exact scene object names like `LivesText` and `StartPosition`.

## 9. Release Philosophy

The goal is to finish and publish the existing game rather than endlessly polish it.

Future work should favor:

- Small changes
- Low risk
- Easy testing
- Measurable improvements
- Shipping

Do not turn cleanup work into an excuse for a major rewrite.

## 10. Android Plan

Planned Android work to document only:

- Add simple left/right touch controls or a virtual joystick on the left side.
- Add a jump button on the right side.
- Preserve the current double-jump behavior.
- Preserve keyboard controls during development.
- Test touch controls on a real Android device.
- Check multiple screen sizes and aspect ratios.
- Avoid introducing unnecessary mobile-specific systems or scope creep.

Do not implement any Android touch-control system unless explicitly requested in a future task.
