$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
$version = $env:APPVEYOR_BUILD_VERSION
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\source\Contrib.sln\Thinktecture.IdentityServer.Services.Contrib\ThinkTecture.IdentityServer.Services.Contrib.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\source\Contrib.sln\Thinktecture.IdentityServer.Services.Contrib\ThinkTecture.IdentityServer.Services.Contrib.nuspec
