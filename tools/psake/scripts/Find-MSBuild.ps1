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
		(Get-ChildItem HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions).GetSubkeyNames() | 
		Sort-Object -Descending  | 
		Select-Object -First 1
    
	$MSBuildFolder = 
		(Get-Item HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\$MSBuildHighestVersion).GetValue('MSBuildToolsPath')

    return "$MSBuildFolder\MsBuild.exe"
}

