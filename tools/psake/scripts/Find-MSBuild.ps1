<#
.SYNOPSIS
Find the Latest version of MSBuild

.DESCRIPTION
Explores the registry for the latest version on msbuild on the local machine.  Once Found
the folder the absolute path will be returned to the caller.

.EXAMPLE
$MSBuildExe = Find-MsBuild

#>

Function Find-MsBuild
{
   $MSBuildHighestVersion =
        (Get-ChildItem HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions) |
        Split-Path -Leaf |
        % {$_ -as [double]} |
        Sort-Object  -Descending  |
        Select-Object -First 1 |
        % {$_ -as [string]}

    if (!($MSBuildHighestVersion -contains ".")) {
        $MSBuildHighestVersion = "$MSBuildHighestVersion.0"
    }

    $MSBuildFolder =
        (Get-Item "HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\$MSBuildHighestVersion").GetValue('MSBuildToolsPath')

   return "$MSBuildFolder\MsBuild.exe"
}
