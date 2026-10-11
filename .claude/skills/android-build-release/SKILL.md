---
name: android-build-release
description: Android build settings, APK vs AAB, debug/profiling builds, signing, and Play Store prep for Knight's Edge. Use when building or installing an Android build, changing Player Settings, preparing a release, or fixing build errors.
---

# Android build and release — Knight's Edge

## Current settings

- Package `com.GokulSurendran.KnightsEdge`, product "Knight's Edge", version 1.0, bundle code 1.
- IL2CPP, ARM64, min API 25, target API Auto, landscape only (LandscapeLeft/Right), Vulkan + OpenGL ES 3.
- Default Android quality level: Medium (index 2).
- Scripting defines (Android + Standalone): `UNITY_POST_PROCESSING_STACK_V2;URP_COMPATIBILITY_MODE`. **`URP_COMPATIBILITY_MODE` is required** — Unity 6.3 refuses to build otherwise because the project uses URP Compatibility Mode (deprecated; migrating to Render Graph is a separate, careful task).
- Android SDK/JDK are the ones bundled with the Editor install. `adb` is at `<Unity install>\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe`.

## APK vs AAB

- Testing on a phone -> **APK** (`EditorUserBuildSettings.buildAppBundle = false`). An AAB cannot be installed on a phone.
- Play Store release -> **AAB** (`buildAppBundle = true`), signed with the release keystore.
- Currently left at APK (false) for testing. Flip to true before the release build.

## Debug / profiling APK

`BuildPipeline.BuildPlayer` with `BuildOptions.Development | ConnectWithProfiler`, target Android, output `Builds/KnightsEdge-debug.apk`, run as a detached `eval_file` job (see `unity-cli-workflow`). First build takes ~11 minutes; the APK is ~59 MB. `Builds/`, `*.apk` and `*.aab` must not be committed (`.gitignore` covers apk/aab).

Install: `adb install -r Builds/KnightsEdge-debug.apk` (phone: developer options + USB debugging on), or copy the file to the phone.

## Release checklist (not done yet)

1. Commit all work first.
2. Create a release keystore; store it and its passwords safely and back them up — losing it blocks updates. Never commit it.
3. Safe-area handling for notches/gesture bars.
4. HDR off in the URP asset (after profiling); profile on a real mid-range phone.
5. Splash screen / app icon check on a launcher; bump the version code for each upload.
6. Release AAB build (App Bundle on, no development flags).
7. Re-test the itch.io/desktop build (keyboard) still works.
8. Play Store listing: icon (512), feature graphic (1024x500), phone screenshots, short/full description, privacy policy URL, content rating, target audience (all ages — check the Families policy).
