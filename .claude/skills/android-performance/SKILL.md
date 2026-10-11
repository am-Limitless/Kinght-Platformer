---
name: android-performance
description: Android performance rules and profiling steps for Knight's Edge. Use when optimizing, profiling, or reviewing frame rate, draw calls, GC allocations, memory, textures, audio, physics or build size for the Android port.
---

# Android performance — Knight's Edge

Never claim a system is optimized without a measurement (Unity Profiler, ideally on a real mid-range Android phone). Editor numbers are only a rough guide: the Editor is several times faster than a phone.

## Rules

- Draw calls: sprite atlases, shared materials, sprite batching. Levels are currently 22-37 draw calls (fine) — do not regress.
- Lights/shadows/post-processing: 0 lights today. Keep it that way; prefer baked/cheap 2D alternatives.
- Object pooling for frequently spawned/destroyed objects (pickups, particles, projectiles). No repeated Instantiate/Destroy in hot paths.
- No per-frame allocations, string work, `Find*`/`GetComponent` in `Update`/`FixedUpdate`. Cache references in `Awake`/`Start`. Avoid `Update` loops that can be events or coroutines.
- Physics: only the player has a Rigidbody2D; many static colliders are fine. No continuous collision unless needed. Fixed timestep is 0.02 (50 Hz) — check smoothness on 90/120 Hz screens (Rigidbody interpolation).
- Textures: sensible max sizes, ASTC compression on Android. Audio: music streamed or compressed in memory, short SFX decompress-on-load.
- URP asset: HDR is still ON (not needed for 2D sprites) — candidate to turn off; measure before/after.
- Particles: confetti uses the light pack prefabs only (Simple/Regular). Do not use the 2,800-9,000 particle bursts.
- Avoid `Debug.Log` in hot paths.

## Profiling steps

1. Build a development APK with the profiler connected (see `android-build-release`).
2. Phone on USB, USB debugging on. Unity: Window > Analysis > Profiler, target "AndroidPlayer".
3. Play Level 15 and Level 8 first (most sprites/colliders), then Level 1 for comparison. Cover jumping, dying/respawn, switches, pause, win panel + confetti.
4. Look at: CPU frame time (target steady 16 ms = 60 fps), GC Alloc per frame (should be near 0), draw calls/batches, memory.
5. Record numbers before and after any optimization.

## Last Editor baseline (not device data)

Levels 1/5/8/10/15: 22/26/34/37/36 draw calls, 14 set-pass calls, 43-112 sprite renderers, 19-103 colliders, 1 Rigidbody2D, 0 lights, 2 particle systems, main-thread script time 1.4-2.3 ms, no GC collections in 8 s windows.
