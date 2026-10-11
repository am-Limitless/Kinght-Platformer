# CLAUDE.md --- Knight Platformer

## Project Overview

Knight Platformer is a 2D platformer inspired by classic-era Mario-style
games, designed to be approachable for players of all ages. It was
originally built and published as a PC/browser game on itch.io. The
current goal is to **port and re-optimize the game for Android**, then
publish it on the Google Play Store.

This is a port effort, not a rewrite: preserve the existing feel, level
design, and game logic. Add only what is genuinely required to make the
game work well as a touch-controlled Android release.

## Technology and Art Direction

- **Engine:** Unity 6
- **Render pipeline:** URP (2D renderer)
- **Original platform:** itch.io (desktop/browser, keyboard input)
- **Target platform:** Android (new), existing itch.io build must keep
  working
- **Visual style:** 2D sprite-based, Mario-inspired platforming
- **Audience:** All ages — family-friendly, readable UI, forgiving but
  skill-based controls
- **Current packages already in the project:** URP, 2D Animation, 2D
  Pixel Perfect, 2D PSD Importer, 2D Sprite, 2D SpriteShape, 2D Tilemap,
  Cinemachine, Post Processing, TextMesh Pro, Visual Scripting

Check `Packages/manifest.json` and installed packages before assuming a
system (e.g. the new Input System) is available — add packages only when
there is a clear need.

## Core Gameplay (Existing, Preserve This)

1. Player runs, jumps, and platforms through 2D levels.
2. Enemies, hazards, collectibles, and checkpoints follow classic
   platformer conventions.
3. Levels are completed by reaching an end point; player health/lives
   and scoring follow the existing itch.io design.

Do not redesign core movement, physics feel, or level logic as part of
the Android port unless a change is explicitly requested. The Android
version should feel like the same game, played with touch instead of a
keyboard.

## Android Port Scope

This port requires work across input, UI, and performance. Keep each
piece of work small and testable.

### 1. Touch Input

- Replace or supplement keyboard input with on-screen touch controls:
  a movement control (D-pad or virtual joystick) and action buttons
  (jump, and any existing actions like attack/dash if present).
- Controls must have large, clearly spaced touch targets and give clear
  visual feedback on press.
- Keep input reading in dedicated input-handling code, separate from
  movement/physics logic, so the same gameplay code can be driven by
  keyboard (editor/itch.io) or touch (Android) without duplication.
- Do not remove existing keyboard support if the itch.io build still
  needs to work from the same project.

### 2. UI and Screen Support

- Ensure UI (HUD, menus, pause screen) scales correctly across common
  Android aspect ratios and resolutions (use Canvas Scaler with
  screen-match settings suited to a landscape or portrait platformer,
  matching the existing game's orientation).
- Respect safe areas on notched/gesture-navigation devices for any UI
  near screen edges.
- Keep touch targets large enough for fingers, not just mouse pointers.

### 3. Android Build Requirements

- Configure Player Settings for Android: package name, minimum API
  level, target API level, orientation, icons, and splash screen.
- Verify URP render settings are suitable for mobile (avoid desktop-only
  quality defaults).
- Confirm input, UI, and save/progress systems behave correctly with no
  keyboard or mouse connected.

### 4. Do Not Add Unless Asked

- In-app purchases, ads, analytics, or online services
- New gameplay systems, levels, or mechanics beyond what exists
- Platform features outside Android (iOS, console, etc.)

## Android Performance Requirements

**Android optimization is a core requirement of this port, not a final
step.** Measure performance on real mid-range Android hardware before
calling any optimization complete.

- Keep draw calls low: use sprite atlases, shared materials, and sprite
  batching wherever possible.
- Limit the number of active Lights, real-time shadows, and
  post-processing effects; prefer lightweight or baked alternatives for
  2D scenes.
- Use object pooling for frequently spawned/destroyed objects (enemies,
  projectiles, particles, pickups) instead of repeated
  Instantiate/Destroy.
- Avoid unnecessary `Update` loops, per-frame allocations, string
  operations, and repeated `GetComponent`/scene searches in hot paths.
- Keep texture sizes and compression settings appropriate for mobile;
  use Android-appropriate texture compression formats.
- Keep physics simple: minimal Rigidbody2D/Collider2D counts, sensible
  fixed timestep, no unnecessary continuous collision detection.
- Test builds on actual Android hardware, not only in the Unity Editor
  or a desktop build.
- Use the Unity Profiler and Android-specific build analysis tools
  before and after optimization work. Do not claim a system is
  optimized without measurement.

## Code Quality and Engineering Principles

Write code that is easy to understand, maintain, debug, and safely
modify. Code that merely compiles is not enough; prioritize clarity and
maintainability over cleverness or premature optimization.

### Naming and Consistency

- Use explicit, descriptive names. Prefer `movementSpeed` over `s` and
  `reachTargetDistance` over `d`.
- Avoid obscure abbreviations and single-letter names, except
  conventional loop iterators such as `i` and `j`.
- Follow consistent naming conventions:
  - Classes and public properties: `PascalCase`,
    e.g. `PlayerController`, `HealthPoints`.
  - Fields and parameters: `camelCase`, e.g. `movementSpeed`,
    `targetDistance`.
  - Private fields: use a consistent convention throughout the project.
    Prefer `[SerializeField] private float movementSpeed;` for
    Inspector-exposed fields.
- Make intent explicit so readers do not need to trace several methods
  just to understand what code does.

### Separation of Concerns and Single Responsibility

- Keep gameplay logic separate from UI, rendering, animation, audio,
  and input where practical. This matters especially for the Android
  port: input handling (touch vs. keyboard) must be swappable without
  touching movement/physics code.
- Give each class and method one clear primary responsibility.
- If a method name naturally needs "and" to describe its work, consider
  splitting it into focused methods.
- Use events, callbacks, or delegates when they reduce direct
  dependencies. Do not introduce an event architecture when a simple
  direct call is clearer.

### Encapsulation and Single Source of Truth

- Default to private fields and methods. Expose only the interface
  other systems need.
- Use `[SerializeField] private` for values editable in the Unity
  Inspector without exposing them publicly.
- Prefer read-only properties or controlled methods over publicly
  mutable fields.
- Keep each piece of state in one authoritative place. UI, audio, and
  other systems should read or subscribe to that source instead of
  maintaining unsynchronized copies.

### Comments, Constants, and Types

- Use clear names and straightforward control flow so code explains
  itself.
- Comments should explain why a non-obvious decision was made, not
  repeat what the code already says.
- Replace unexplained magic numbers with named constants or serialized
  configuration fields.
- Avoid fragile string-based lookups when a serialized reference, enum,
  component, or other strongly typed option is appropriate. Use tags
  and string identifiers only when they are the right Unity tool for
  the job.

### Composition and Refactoring

- Prefer small reusable components and composition over deep
  inheritance hierarchies.
- First make the feature work, then clean it up without changing its
  behavior.
- Refactor incrementally: clarify names, extract complex logic into
  focused methods, remove dead code, and simplify over-engineered
  abstractions.
- Leave the codebase at least as clear and maintainable as you found
  it. Avoid broad refactors unrelated to the requested task.

## Inspector-Configurable Gameplay Values

Gameplay tuning must be easy to adjust directly in the Unity Inspector
without editing code.

- Expose appropriate gameplay settings as `[SerializeField] private`
  fields, with clear names and sensible defaults.
- This includes values such as movement speed, jump height/force,
  gravity scale, coyote time, interaction range, enemy speed/damage,
  spawn intervals, scoring values, and other balance-related values.
- Group related fields in the Inspector using `[Header]`, `[Tooltip]`,
  and suitable ranges such as `[Min]` or `[Range]` where they improve
  usability.
- Use ScriptableObjects for shared or data-heavy configuration (e.g.
  level data, enemy stats) when genuinely useful; do not create
  unnecessary configuration assets for every small value.
- Avoid hardcoding gameplay-tuning values throughout scripts. Keep each
  value in one authoritative place and have systems read from that
  source.
- Use constants for values that are true implementation details and
  should not be changed during normal game balancing. Do not expose
  every technical constant just for the sake of it.
- Validate Inspector values where needed and handle invalid or missing
  references safely.
- After adding or touching a gameplay feature, identify which values
  should be adjustable by a designer and expose them in the Inspector.
- The goal is to let the developer tune gameplay quickly in Unity
  without repeatedly modifying scripts, while keeping code maintainable
  and avoiding duplicated settings.

## Development Rules for Claude

1. **Inspect before editing.** Check the existing Unity project
   structure, relevant scripts, scenes, packages, and settings before
   making changes.
2. **Work incrementally.** Implement one small, testable feature at a
   time (e.g. touch movement first, then touch jump, then UI scaling,
   then build settings).
3. **Avoid unnecessary complexity.** Prefer the simplest reliable
   solution for a small 2D mobile platformer.
4. **Preserve existing work.** Do not overwrite or delete files,
   scenes, assets, levels, or systems without a clear need. The itch.io
   build's behavior should not regress.
5. **Use compatible APIs.** Check the project's actual Unity version and
   installed packages before using APIs or adding dependencies (e.g.
   confirm whether the new Input System is installed before relying on
   it).
6. **Keep code maintainable.** Follow the principles above. Avoid giant
   manager scripts, deep inheritance trees, or abstractions without a
   clear need.
7. **Consider mobile performance throughout development.** Do not
   postpone Android optimization until the end.
8. **Handle interactions carefully.** Consider multi-touch, rapid
   taps/holds, dropped touches, and interrupted actions (e.g. finger
   sliding off a button).
9. **Do not fake completion.** Run available checks or tests and report
   what changed, what was verified, and what remains untested.
10. **Keep scope under control.** Do not implement features beyond the
    Android port (new mechanics, monetization, online features) unless
    specifically asked.
11. **Explain trade-offs.** If a solution may hurt Android performance,
    maintainability, or the itch.io build, explain why and suggest a
    simpler alternative.
12. **Refactor carefully.** Avoid changing unrelated systems while
    implementing a feature.

## Current Priority Order

1. Audit existing input handling and identify everywhere keyboard/mouse
   input is read directly in gameplay code.
2. Introduce touch-friendly input (on-screen joystick/buttons) behind a
   clean input layer, without breaking existing keyboard input.
3. Verify and fix UI scaling, safe areas, and touch target sizes across
   target Android aspect ratios.
4. Configure Android Player Settings (package name, API levels,
   orientation, icons, splash screen) and produce a first working
   Android build.
5. Profile on real Android hardware; fix draw call, memory, and
   frame-rate issues using the performance guidelines above.
6. Re-test the itch.io build to confirm it still works after the port
   changes.
7. Prepare Play Store listing requirements (icon sizes, screenshots,
   store assets) as a separate, later task.

## Guiding Principle

This is a porting and optimization project, not a redesign. Keep the
game that already shipped on itch.io intact, make it feel natural on
touchscreens, and make it run well on mid-range Android hardware before
adding anything new.
