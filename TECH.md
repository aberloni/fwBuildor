# buildor : tech layout

## assemblies

- `Runtime/` : `ab.fwp.buildor`, all platforms
  - app version data (`DataVersion`, `DataBuildSettingVersion` & per platform), `VersionManager` (display in app)
  - enums (`TargetPublish`, `TargetDebug`, `TargetSdks`, `TargetFeatures`), configs (`ConfigDemo`), log levels
- `Editor/` : `ab.fwp.buildor.editor`, editor only
  - everything related to profiles, building, bodules, symbols, windows
  - optional : `com.unity.addressables` (>= 1.20), referenced by name, `BUILDOR_ADDRESSABLES` version define, compiles without it

## folders

| folder | content |
|---|---|
| `Runtime/Versioning/` | `DataVersion` (X.Y.Z + build number), `DataBuildSettingVersion` & platform variants (Internal, Windows, Osx, Switch) |
| `Runtime/Configs/` | `ConfigBase`, `ConfigDemo` : runtime configs stored in Resources/buildor/ |
| `Editor/ProfilePlatforms/` | `DataBuildSettingProfile` & per platform profiles, `ProfilBuildParameters`, `ProfilDebugParameters` |
| `Editor/Building/` | `BuildExecutor`, `BuildPreprocess`, `BuildPostprocess`, `BuildProcess` (editor coroutine base) |
| `Editor/BuildModules/` | `BuildModule`, `BuildContext`, all bodules |
| `Editor/ScriptableSymbols/` | scripting define symbols window & helpers |
| `Editor/Versioning/` | app version window, `VersionIncrementor` |
| `Editor/Window/` | buildor window (`WinEdBuildor`) & sub sections |
| `Editor/Systems/` | specific paths per machine/user, system detection |
| `Editor/NpmIncrementor.cs` | package.json version incrementor |
| `Editor/HelperGit.cs` | git calls in a given folder (add all & commit) |

## profile selection

- `DataBuildSettingsBridge` : one asset in project, lists profiles per platform
- active profile = bridge profile matching
  - unity active build target (platform)
  - `TargetPublish` (release, demo, festival, custom) & `TargetSdks` (none, STEAM), set in buildor window
- `TargetDebug` (release, debug) is not part of selection : it picks `release` or `debug` parameters of the active profile (`profile.Level`)
- window selections are stored in EditorPrefs, per project & per platform (`BuildorVars`)
- no matching profile : window "+profil" button creates one (platform type, publish & sdk set), saved next to other profiles of the same platform (or next to bridge), named `[platform]_[publish](_[sdk])`, added to bridge
- opening buildor window (menu) applies active profile to PlayerSettings (`applyProfilToEditor`), not on domain reload, not in play mode or while building

## profile content

`DataBuildSettingProfile`

- `versionInternal` / `versionPublish` : app version assets (publish is used when present)
- `publish`, `sdk` : identify the profile within the bridge
- `compagny_name`, `product_name` : applied to PlayerSettings
- `build` (`ProfilBuildParameters`) : all levels : features flags, build prefix (app name), icon, merger, `modules`
- `release` (`ProfilReleaseParameters`) : release level only : `modules`
- `debug` (`ProfilDebugParameters`) : debug level only : dev build, script debugging, profiling, `modules`
- `Level` : `release` or `debug`, depending on active debug level
- `specifics` : export path per machine/user, per debug level

## build flow

`WinEdBuildor` BUILD button → `BuildExecutor.launch()`

1. `BuildPreprocess` (editor coroutine, cancelable progress bar)
   - requirements, build is cancelled if not met (before anything is modified)
     - scripts compile check (if "compile.check" toggle), see below
     - switch : `NINTENDO_SDK_ROOT` env var is set & points to a folder containing `Tools/`
   - clear export folder (`profile.BuildPath`)
   - increment version (if "version.incr" toggle)
   - timestamp versions
   - pre bodules : `build.modules` then `release.modules` or `debug.modules`
   - `build.merger`
   - apply profile to PlayerSettings (`applyProfilToEditor`)
   - scenes from build settings, output path, options (autorun, dev build, debugging)
   - wait 2s
   - `BuildPipeline.BuildPlayer`
2. `BuildPostprocess`
   - failure : logs only
   - success : logs, post bodules (`build.modules` then `release.modules` or `debug.modules`), then "open folder" window toggle

## compile check

`BuildCompileCheck` : compiles player scripts only (no scenes, no assets) with `PlayerBuildInterface.CompilePlayerScripts`, seconds instead of a full build

- active build target, `profile.Symbols` as extra defines (no injection), development option at debug level
- errors in console, editor assemblies untouched (output in `Temp/buildor_compile_check`)
- buildor window : "check compile" button (above BUILD), "compile.check" toggle to run it as build requirement
- menu `Window/Buildor/check compile` : same, active profile
- needs target platform module installed

## export path

`profile.BuildPath`

- "specific" toggle on & path set for this machine/user & debug level : that path
- otherwise : `[project]/builds/` + optional parts joined by `__` : prefix, platform, date, version, suffix (+ flags like `_dbuild`)

`profile.FullPath` = `BuildPath/[build_prefix].[extension]`

## bodules

a Bodule (`BuildModule`) is a scriptable object action executed pre or post build

- inherits `BuildModule`, implements `doApply(BuildContext ctx)`
- each type declares its `Phase` (`pre` by default, override for `post`)
- listed in profile, 3 lists
  - `build.modules` : all builds
  - `release.modules` : release level only
  - `debug.modules` : debug level only
- in each phase, `release.modules` / `debug.modules` are always executed after `build.modules`
- within a list, executed in list order
- post bodules are only executed after a successful build
- can be applied manually (context menu "apply" or buildor window), with a context built from active profile
- `askBeforeApply()` : override to show a confirm dialog before applying
- a pre bodule can cancel the build by throwing (ie: `BuildFailedException`), export folder is already cleared & version already incremented at that point

### BuildContext

- `profile` : active profile
- `phase` : pre or post
- `summary` : unity `BuildSummary` (result, outputPath, platform, ...), null in pre & manual
- `Target` : `BuildTarget` being built (summary platform in post, active build target otherwise)

### available bodules

| bodule | phase | what |
|---|---|---|
| `BoduleSymbols` | pre | list of scripting define symbols, gathered by `profile.Symbols` (no action) |
| `BoduleStreamingAssetsCopy` | pre | copy an external folder into StreamingAssets/, extension & path filters |
| `DataBuildorScenesMerger` | pre | replace build settings scenes with scenes of its `DataBuildorScenesFilter` sets |
| `BoduleAddressables` | pre | addressables content : purge build cache, clean, build (see addressables) |
| `BoduleDoNoShip` | post | remove unity folders not meant to ship (`*_DoNotShip`, `*_ButDontShipItWithYourGame`), root of export folder only, can plug a `BodulePostClearFolders` |
| `BodulePostClearFolders` | post | remove specific folders, paths relative to export folder |
| `BodulePostClearFiles` | post | remove specific files at root of export folder (file names only, no sub path) |
| `BodulePostClearFilePaths` | post | remove specific files, paths relative to export folder, `*` `?` allowed in file name (ie: `MyGame_Data/StreamingAssets/*.log`) |
| `BoduleSteam` | post | remove `steam_appid.txt` from root of export folder, if present |
| `BodulePostDropVersion` | post | write version file (default `version.txt`) in export folder |
| `BodulePostZip` | post | zip export folder next to it |
| `BodulePostOpenFolder` | post | open export folder |

recommended post order : do not ship, clear folders, clear files, clear file paths, steam, drop version, zip, open folder

### clear folders paths

relative to export folder, exact folder, `/` or `\`

| platform | StreamingAssets location |
|---|---|
| windows / linux | `MyGame_Data/StreamingAssets/` |
| osx | `MyGame.app/Contents/Resources/Data/StreamingAssets/` |

skipped : empty, absolute, outside export folder (`../`), export folder itself

### clear file paths

same rules as clear folders for the folder part, then a file name or pattern (`*`, `?`) within that single folder (not recursive)

| path | removes |
|---|---|
| `MyGame_Data/StreamingAssets/config.json` | that file |
| `MyGame_Data/StreamingAssets/logs/*.log` | all .log files of `logs/` |
| `MyGame_Data/StreamingAssets/test_??.json` | `test_01.json`, `test_ab.json`... |
| `*.pdb` | all .pdb files at root of export folder |

skipped : wildcards in folder part, path ending with `/` (folder), catch-all pattern at root (`*`, `*.*`)

### addressables

`BoduleAddressables`, needs `com.unity.addressables`, logs a warning and does nothing without it

executed in order, each step toggled

| step | what |
|---|---|
| `purgeBuildCache` | delete scriptable build pipeline cache (`Library/BuildCache`), next content build is a full rebuild |
| `cleanContent` | delete content built by all addressables builders |
| `rebuildAddressable` | mark `saveBeforeBuild` assets dirty, save assets, build content of active build target |

- no "content is outdated" check : content build is incremental (scriptable build pipeline cache), unchanged assets are not rebuilt
  - recommended : `rebuildAddressable` only, `purgeBuildCache` / `cleanContent` to force a full rebuild (ie: release, broken cache)
- content build failure : throws, build is cancelled
- addressables settings "Build Addressables on Player Build" (or its preference value, default on) : unity builds content during `BuildPlayer`
  - during a build, the bodule skips its own content build (no double build), clean steps still apply
  - manual apply : always builds
- content built by the bodule happens before the profile is applied to PlayerSettings (symbols), let unity build content with player if symbols change serialized data

### writing a bodule

```csharp
[CreateAssetMenu(menuName = BuildorHelpers._menuItem_basepath + "modules/+my bodule")]
public class BoduleMine : BuildModule
{
    public override BodulePhase Phase => BodulePhase.post;

    protected override void doApply(BuildContext ctx)
    {
        // ctx.profile, ctx.summary (post), ctx.Target
    }
}
```

## not bodules

- autorun : unity build option (`AutoRunPlayer`), buildor window toggle
- "open folder" window toggle : same as `BodulePostOpenFolder`, executed after post bodules (opens twice if both are used)
- version increment : "version.incr" window toggle, pre build

## app version

two layers, so that multiple platforms can share the same version

- `DataVersion` : X.Y.Z + build number, timestamps (last increment, last build), platform agnostic
  - inspector : MAJOR++ / MINOR++ / PATCH++ buttons (+ build number, refresh increment timestamp), not applied to PlayerSettings
  - `VersionIncrementor.incrementPatch()` (editor) : patch++ & save a `DataVersion`, logs name, new version & count found (ie: `-executeMethod fwp.version.editor.VersionIncrementor.incrementPatch`)
    - single `DataVersion` in project : that one
    - multiple : `DataVersion` of active profile version (publish, or internal), no active profile : warning, nothing incremented
- `DataBuildSettingVersion` : platform version (`DataVersion[Platform]`), references a `DataVersion` (`Data`)
  - increments are forwarded to its `DataVersion`, then applied to PlayerSettings
  - `applyVersionToEditor()` : per platform injection into PlayerSettings
  - subclasses add platform specific content (ie: switch `release`)
  - inspector shows the shared version on top

| platform version | PlayerSettings |
|---|---|
| `DataVersionInternal` | `bundleVersion`, `iOS.buildNumber`, `Android.bundleVersionCode` (build number) |
| `DataVersionWindows` / `DataVersionOsx` | `bundleVersion` |
| `DataVersionSwitch` | `bundleVersion`, before u6 : `Switch.releaseVersion` (`release`), `Switch.displayVersion` |

- `DataVersionSwitch` inspector : RELEASE++ button, increments `release`, adds an entry (release, timestamp) to `release_history`, applies to PlayerSettings
- `DataVersionSwitch` inspector & `Window/Buildor/nmeta version injector (win)` share the same nmeta section (`SwitchNmeta`)
  - .nmeta file path (EditorPrefs, per machine & project)
  - nmeta `ReleaseVersion` / `DisplayVersion` vs switch version (`release` / X.Y.Z), mismatch in red
  - inject version into nmeta, open file, open Authoring tool (`NINTENDO_SDK_ROOT/Tools/Authoring*.exe`)
  - `NINTENDO_SDK_ROOT` status, button to open system env vars when invalid
- `SwitchNmeta.checkSdk()` is also the switch build requirement (preprocess)
- profile `versionInternal` & `versionPublish` can share the same `DataVersion` : "version.incr" increments it once
- `Window/Buildor/version (win)` (`WinEdVersion`), refreshed on focus
  - active profile (bridge, active build target, publish & sdk) : its `versionInternal` & `versionPublish`, MAJOR / MINOR / FIX buttons
  - no bridge or no active profile : lists all `DataVersion` assets, radio selector, focused one gets the buttons (not applied to PlayerSettings)
- `DataVersionSwitch` : `release` field (rom 0, patches 1,2,3...), `InjectVersionToRom` to patch .nmeta

## symbols

- `profile.Symbols` : publish & sdk (if not default) + features + `BoduleSymbols` of build + `BoduleSymbols` of release or debug (+ `debug;` at debug level)
- buildor window "inject" : replaces unity symbols of profile target group with `profile.Symbols`
- `WinEdScriptSymbols` : toggles per known symbols enums, per target group

## npm incrementor

`NpmIncrementor` : package.json version, unrelated to app version

- project window, right click any asset/folder of a package > Package version > MAJOR++ / MINOR++ / PATCH++
- also in `Window/Buildor/package version/` : selected package(s), or buildor package itself if nothing of a package is selected
- embedded or local (`file:`) packages only
- rewrites only `"version"` value (X.Y.Z), refreshes package manager, selects updated package.json
- `PATCH++ & commit` : save assets, bump patch, then per package, in package folder : `git add -A .`, `git commit -m "X.Y.Z"` (new version)
  - `HelperGit`, git in PATH, no commit if add fails
  - `.` : package folder only (own repo : whole repo, embedded in project repo : package only)
