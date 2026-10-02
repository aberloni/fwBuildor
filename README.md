# buildor

various tools to manage building and app:version-ing

see [TECH.md](TECH.md) for tech layout

# version

meant to store app versioning numbers
and give tools to manage it

`DataVersion` : X.Y.Z + build number, can be shared by multiple platforms
`DataVersion[Platform]` : how a `DataVersion` applies to a platform

# build

buildor window : `Window/Buildor/buildor (win)`
select a profile (publish, sdk, debug) and build

# Bodule

a Bodule is an action to be done pre or post building the app
listed in a Build Setting Profile, executed at the appropriate time

# npm incrementor

increment version of a package (package.json)
right click on any asset/folder of a package > Package version
