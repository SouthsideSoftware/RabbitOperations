. .\tools\psake\scripts\Find-MSBuild.ps1
. .\tools\psake\scripts\Find-Tool.ps1
. .\tools\psake\scripts\MSSQLDB-Helper.ps1
. .\tools\psake\scripts\Find-NugetTool.ps1
. .\tools\psake\scripts\Invoke-Migrations.ps1
. .\tools\psake\scripts\Test-InTeamCity.ps1

properties {
    #can override these from command line with invoke-psake -properties @{}
    #for example, invoke-psake -properties @{configuration="debug2";platform="all"}
    #would override both configuration and platform
    $BuildRoot = Resolve-Path .
    $revision =  if ("$env:BUILD_NUMBER".length -gt 0) { "$env:BUILD_NUMBER" } else { "0" }
    $inTeamCity = if ("$env:BUILD_NUMBER".length -gt 0) { $true } else { $false }
    $version = "0.15.0"
    $configuration = "Debug"
    $platform = "Any CPU"
    $buildOutputDir = "./BuildOutput"
    $nugetOutputDir = Join-Path $buildOutputDir "nuget"
    $NugetPackagesDirectory = Join-Path $BuildRoot "packages"
    $testAssemblies = @("tests\RabbitOperations.Tests.Unit/bin/$configuration/RabbitOperations.Tests.Unit.dll",
    "tests\RabbitOperations.Collector.Tests.Unit/bin/$configuration/RabbitOperations.Collector.Tests.Unit.dll")
}

task validateProperties -Description "Validate the build script properties." -action {
    assert( "debug","release" -contains $configuration ) `
        "Invalid Configuration: $configuration : valid values are debug and release"

    assert( "Any Cpu" -contains $platform ) `
        "Invalid Platform: $platform : valid values are `"Any Cpu`""
}

task default -depends Build

task build -Description "Build application.  Runs tests" -depends validateProperties, cleanBuildOutput, version, compile, test {
}

task quickBuild -Description "Build application no tests" -depends cleanBuildOutput, version, compile {
}

task test -Description "Runs tests" {
    $NUnitConsoleExe = Find-Tool -ToolName nunit3-console.exe -PackagesFolder $NugetPackagesDirectory

   	if ($NUnitConsoleExe -eq $null) {
   		throw "Nunit test runner (nunit3-console.exe) cannot be found in packages folder. "
   	}

    ForEach($testAssembly in $TestAssemblies)
    {
    	Exec {
    		&  $NUnitConsoleExe $testAssembly --noresult --labels=On $(if (Test-InTeamcity -eq $True) { "--teamcity" } else { "" })
    	}
    }
}

Task compile -Description "Build application only" {
    # programFilesDir = ProgramFiles(x86) ?? ProgramFiles
    $programFilesDir = (${env:ProgramFiles(x86)}, ${env:ProgramFiles} -ne $null)[0]
    # exec {.nuget\nuget restore}
    & msbuild $sln_file /t:rebuild /m:1 /p:VisualStudioVersion=15.0 "/p:Configuration=$configuration" "/p:Platform=$platform"
}

task pullCurrentAndBuild -Description "Does a git pull of the current branch followed by build" -depends pullCurrent, build

task pullCurrent -Description "Does a git pull" {
    git pull
}

task buildDist -Description "Update version. Build appication. Runs tests.  Builds Nuget packages" -depends version, build, publish {
}

task cleanBuildOutput -Description "Cleans the BuildOutput folder" {
  if (Test-Path $buildOutputDir) {
    Remove-Item -Recurse -Force $buildOutputDir
  }
  New-Item -ItemType directory -Path $buildOutputDir
  New-Item -ItemType directory -Path $nugetOutputDir
}

task startRaven -Description "Starts RavenDB." {
  Start-Raven
}

Task version -Description "Version the assemblies" {
	Update-CommonAssemblyInfoFile $version $revision
}

Task versionReset -Description "Returns the version of the assemblies to 0.1.0.0" {
  Reset-CommonAssemblyInfoFile
}

Task publish -Description "Publish artifacts" {
  Generate-ReleaseNotes $version
  Copy-ReleaseNotes $version
}

task startCollector -Description "Starts the collector host" {
    StartApp "app/RabbitOperations.Collector/bin/$configuration/RabbitOperations.Collector.exe" "RabbitOperations.Collector"
}

task ? -Description "Helper to display task info" {
  WriteDocumentation
}

function StartApp($appPath, $appName) {
    $processActive = Get-Process $appName -ErrorAction SilentlyContinue
    if(!$processActive)
    {
        Write-Host $appPath
        if (test-path env:ConEmuDir) {
            & ConEmu -reuse -cmd "$appPath"
        } else {
            Start-Process -FilePath $path
        }
    } else {
        Write-Host "$appName already running."
    }
}

function Update-CommonAssemblyInfoFile ([string] $version, [string]$revision) {
  if ($version -notmatch "[0-9]+(\.([0-9]+|\*)){1,3}") {
    Write-Error "Version number incorrect format: $version"
  }

  $versionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,2}"\)'
  $versionAssembly = 'AssemblyVersion("' + $version + '")';
  $versionFilePattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
  $versionAssemblyFile = 'AssemblyFileVersion("' + $version + "." + $revision + '")';

  Get-ChildItem -filter .\Common\CommonAssemblyInfo.cs | % {
    $filename = $_.fullname

    if ($make_writable) { Writeable-File($filename) }

    $tmp = ($file + ".tmp")
    if (test-path ($tmp)) { remove-item $tmp }

    (get-content $filename) | % {$_ -replace $versionFilePattern, $versionAssemblyFile } | % {$_ -replace $versionPattern, $versionAssembly }  > $tmp
    write-host Updating file AssemblyInfo and AssemblyFileInfo: $filename --> $versionAssembly / $versionAssemblyFile

    if (test-path ($filename)) { remove-item $filename }
    move-item $tmp $filename -force

    if ($make_writable) { ReadOnly-File($filename) }

  }
}

function Version-Nuspec ([string]$project) {
  [string] $nuspecFilePath = Join-Path -Path ".\nuspec" -ChildPath ($project + ".nuspec") -Resolve
  Write-Host $nuspecFilePath
  [xml]$nuspecFile = Get-Content $nuspecFilePath
  $ns = New-Object System.Xml.XmlNamespaceManager($nuspecFile.NameTable)
  $ns.AddNamespace("ns", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd")
  $versionNode = $nuspecFile.SelectSingleNode("//ns:version", $ns)
  if ($versionNode -eq $null) {
    throw "Cannot find version node in nuspec package for $nuspecFile"
  }
  $versionNode.innerText = $version + "." + $revision

  $nuspecFile.Save($nuspecFilePath)
}

function Copy-DllOutputs ([string] $projectName) {
  [string] $path = Join-Path (Join-Path "app/$projectName" "bin") $configuration
  [string] $destPath = Join-Path $buildOutputDir $projectName
  Copy-Item $path $destPath -recurse
}

function Create-NugetPackage ([string] $projectName) {
  Version-Nuspec $projectName
  $fn = $projectName + ".nuspec"
  [string] $source = Join-Path -Path "nuspec" -ChildPath $fn
  & .nuget\nuget pack $source -OutputDirectory $nugetOutputDir
  if ($LastExitCode -ne 0) { throw "Failed to create nuget package for $projectName"}
}

function Reset-CommonAssemblyInfoFile(){
  Update-CommonAssemblyInfoFile "0.1.0" "0"
}

function Writeable-File($filename){
	sp $filename IsReadOnly $false
}

function ReadOnly-File($filename){
	sp $filename IsReadOnly $true
}

function Start-Raven {
  $processActive = Get-Process Raven.Server -ErrorAction SilentlyContinue
  if (!$processActive)
  {
    #Find the correct version of RavenDB by looking at the referenced
    #package in the packages.config file of the solution
    [xml]$packages = Get-Content ".\.nuget\Packages.config"
    $server = $packages.SelectSingleNode("//package[@id='RavenDB.Server']")
    $version = $server.GetAttribute("version")
    [string] $path = ".\packages\RavenDB.Server.$version\tools\Raven.Server.exe"

	  #Start it up
    Write-Host "Starting Raven at: " $path
    if (test-path env:ConEmuDir) {
      & ConEmu -reuse -cmd "$path"
    } else {
      Start-Process -FilePath $path
    }
    Start-Sleep 5
    Exit 0
  }
  else
  {
    Write-Host "RavenDB already running"
    Exit 0
  }
}

function Get-NunitPath {
    return Find-Tool -ToolName nunit3-console.exe -PackagesFolder $NugetPackagesDirectory
}

function Generate-ReleaseNotes([string]$version) {
  Copy-Item ReleaseNotes/Configuration/settings.json -Destination node_modules/trello-releasenotes -Force
  Copy-Item ReleaseNotes/Configuration/default.template -Destination node_modules/trello-releasenotes/templates -Force
  $lists = "Ready to Release $version,Released $version"
  Push-Location node_modules\trello-releasenotes
  try {
    & node index.js -g "$version" -v $version
  } finally {
    Pop-Location
  }
}

function Copy-ReleaseNotes([string]$version) {
  $fileVersion = $version.Replace(".", "_")
  $fileName = "Rabbit_Operations_$fileVersion.markdown"
  $item = "node_modules/trello-releasenotes/export/$fileName"
  Move-Item $item -Destination ReleaseNotes -Force
}
