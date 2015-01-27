$root = $env:APPVEYOR_BUILD_FOLDER
$version = $env:APPVEYOR_BUILD_VERSION
$nuspecPath = $root \source\Contrib.sln\Thinktecture.IdentityServer.Services.Contrib\ThinkTecture.IdentityServer.Services.Contrib.nuspec
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host $nuspecPath
Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\source\Contrib.sln\Thinktecture.IdentityServer.Services.Contrib\ThinkTecture.IdentityServer.Services.Contrib.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\source\Contrib.sln\Thinktecture.IdentityServer.Services.Contrib\ThinkTecture.IdentityServer.Services.Contrib.nuspec
