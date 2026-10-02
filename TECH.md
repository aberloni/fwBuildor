# buildor : tech layout

## assemblies

- `Runtime/` : `ab.fwp.buildor`, all platforms
  - app version data (`DataVersion`, `DataBuildSettingVersion` & per platform), `VersionManager` (display in app)
  - enums (`TargetPublish`, `TargetDebug`, `TargetSdks`, `TargetFeatures`), configs (`ConfigDemo`), log levels
- `Editor/` : `ab.fwp.buildor.editor`, editor only
  - everything related to profiles, building, bodules, symbols, windows

## folders

| folder | content |
|---|---|
| `Runtime/Versioning/` | `DataVersion` (X.Y.Z + build number), `DataBuildSettingVersion` & platform variants (Internal, Windows, Osx, Switch) |
| `Runtime/Configs/` | `ConfigBase`, `ConfigDemo` : runtime configs stored in Resources/buildor/ |
| `Editor/ProfilePlatforms/` | `DataBuildSettingProfile` & per platform profiles, `ProfilBuildParameters`, `ProfilDebugParameters` |
| `Editor/Building/` | `BuildExecutor`, `BuildPreprocess`, `BuildPostprocess`, `BuildProcess` (editor coroutine base) |
| `Editor/BuildModules/` | `BuildModule`, `BuildContext`, all bodules |
| `Editor/ScriptableSymbols/` | scripting define symbols window & helpers |
| `Editor/Versioning/` | app version incrementor & window |
| `Editor/Window/` | buildor window (`WinEdBuildor`) & sub sections |
| `Editor/Systems/` | specific paths per machine/user, system detection |
| `Editor/NpmIncrementor.cs` | package.json version incrementor |

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
   - platform requirements, build is cancelled if not met (before anything is modified)
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

- profile `versionInternal` & `versionPublish` can share the same `DataVersion` : "version.incr" increments it once
- menus `Version/Internal/*` & `Version/Publish/*` : increment active profile versions
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
