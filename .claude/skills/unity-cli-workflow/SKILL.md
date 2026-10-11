---
name: unity-cli-workflow
description: How to drive the Knight's Edge Unity project through the Unity CLI (unity command eval, play-mode tests, screenshots, builds). Use when editing scenes/prefabs/assets, play-testing a change, capturing game view, running a build or compile check, or when a CLI command times out or misbehaves.
---

# Unity CLI workflow — Knight's Edge

Prefer the Unity CLI (`unity command ...`) over hand-editing `.unity`/`.prefab` files.

## Commands that work

- Run C#: `unity command eval --result-only "<C# statements ending with return>"`. For long or quote-heavy code write a `.cs` snippet file and use `unity command eval_file --result-only --file <path>`.
- Play mode: `editor_play`, `editor_stop`. After `editor_play`, poll with `eval` for `UnityEditor.EditorApplication.isPlaying` (takes a few seconds). Always `editor_stop` when done.
- Screenshot: `capture_game_view --result-only --save_path "Assets/_t_name.png"`, then Read the PNG. Delete `Assets/_t_*.png` and their `.meta` afterwards.
- Compile: `recompile`, then wait ~12 s; check `console` for `error CS`. `editor_status` shows `compiling`.
- Build validation: `unity command build --dry_run --result-only` (expect `"valid": true`).
- Long jobs: `eval_file --detach --file <path>` returns a job id. `--result-only` cannot be combined with `--detach`, and there is no `job` command — have the script write a report file and poll for it.
- Console: `console --result-only`, `clear_console`.

## Gotchas

- Editor main-thread limit is **5 s per command**. Batch work per scene/level; never scan all assets in one call.
- In play mode each command can take ~4 s of real time. A 1-second effect cannot be sampled across commands — slow it down temporarily via serialized fields (runtime only) or `Time.timeScale`.
- Fully qualify types (`UnityEngine.UI.Image`) in snippets. Apostrophes inside bash double quotes break the command — use a snippet file.
- Git Bash rewrites a leading `/` in hierarchy paths; avoid passing paths that start with `/`.
- Do not start a build or other job while scripts are compiling ("Error building Player because scripts are compiling"). Wait for `compiling:false`.
- Builds can create temp files (e.g. `Assets/Resources/PerformanceTestRun*.json`); check `git status` afterwards and delete strays.
- If Unity stops answering: look for a modal dialog in the Editor window, wait, then retry.

## Testing patterns

- UI taps: `EventSystem.RaycastAll` at the button's screen point, then `ExecuteEvents.ExecuteHierarchy(top, ev, ExecuteEvents.pointerClickHandler)`. Check what is hit first (proves nothing covers the button). Raycasts in the same frame a panel is activated can be stale.
- Triggers: call the private `OnTriggerEnter2D` via reflection with the player's collider (e.g. `WinTrigger`, `PlayerTrigger` with an `Obstacle`-tagged collider).
- Camera/parallax tests: disable `CinemachineBrain` and move `Camera.main` directly; Cinemachine otherwise overrides it.
- Verify after every change: no new console errors, `build --dry_run` valid, play mode stopped, scene not dirty, test PlayerPrefs reset (`CurrentLives`, `HighestUnlockedLevel`).
