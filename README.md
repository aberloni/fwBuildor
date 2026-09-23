# buildor

various tools to manage building and app:version-ing

# version

meant to store app versioning numbers
and give tools to manage it

# pre/post process

will manage building after applying a bunch of properties

# Bodule

a Bodule (BuildModule) is an action to be done pre or post building the app

- scriptable object, inherits `BuildModule`, implements `doApply(BuildContext ctx)`
- listed in a Build Setting Profile : `build.modules` (always) & `debug.modules` (debug level only)
- in each phase, `debug.modules` are always executed after `build.modules`
- each type declares its `Phase` (`pre` by default, override for `post`)
- executed at the appropriate time during the build process
  - pre : in `BuildPreprocess`, before building
  - post : in `BuildPostprocess`, after a successful build
- can also be applied manually (context menu "apply" or buildor window)

BuildContext, given to each bodule when applied

- `profile` : active Build Setting Profile
- `phase` : pre or post
- `summary` : unity BuildSummary (result, outputPath, ...), null in pre
- `Target` : BuildTarget being built (summary platform in post, active build target otherwise)

available bodules

pre

- `BoduleSymbols` : list of scripting define symbols, gathered by the profile when injecting symbols (no action)
- `BoduleStreamingAssetsCopy` : copy an external folder into StreamingAssets/, with extension & path filters
- `DataBuildorScenesMerger` : replace build settings scenes with scenes from a set of `DataBuildorScenesFilter`

post

- `BodulePostClearFolders` : remove specific folders, paths relative to build folder (ie: `MyGame_Data/StreamingAssets/something/`)
- `BodulePostDropVersion` : write a text file with version number(s) in build folder
- `BodulePostZip` : zip build folder next to it
- `BodulePostOpenFolder` : open build folder in explorer/finder

post bodules are executed in list order : clear folders & drop version before zip

autorun is not a bodule : it's a unity build option (`AutoRunPlayer`), toggled in buildor window

# npm incrementor

increment version of a package (package.json), unrelated to app version

right click on any asset/folder of a package (project window) > Package version > MAJOR++ / MINOR++ / PATCH++
only for embedded or local (file:) packages