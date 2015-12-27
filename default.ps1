properties {
    #can override these from command line with invoke-psake -properties @{}
    #for example, invoke-psake -properties @{configuration="debug2";platform="all"}
    #would override both configuration and platform
    $revision =  if ($env:APPVEYOR_BUILD_NUMBER -ne $NULL) { "$env:APPVEYOR_BUILD_NUMBER" } else { "0" }
    $inBuildServer = if ($env:APPVEYOR_BUILD_NUMBER -ne $NULL) { $true } else { $false }
    $version = "0.9.0"
    $configuration = "Debug"
    $platform = "Any CPU"
    $artifactsDir = "./artifacts"
    $nugetOutputDir = Join-Path $artifactsDir "nuget"
    $testAssemblies = @("tests\RabbitOperations.Tests.Unit/bin/$configuration/RabbitOperations.Tests.Unit.dll",
    "tests\RabbitOperations.Collector.Tests.Unit/bin/$configuration/RabbitOperations.Collector.Tests.Unit.dll")
    $sourceDir = "src"
    $testDir = "test"
}

task validateProperties -Description "Validate the build script properties." -action {
    assert( "Debug","Release" -contains $configuration ) `
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
  [string]$nunitVersion = Get-NunitVersion
  if ($inTeamCity) {
    Write-Host "Running Tests In TeamCity"
    [string] $nunit = "NUnit-" + $nunitVersion
    Write-Host "Running " $env:NUNIT_LAUNCHER v4.0 x64 $nunit $testAssemblies
    & $env:NUNIT_LAUNCHER v4.5 x64 $nunit $testAssemblies
  } else {
    Write-Host "Running Tests Outside TeamCity"
    [string] $nunitPath = Get-NunitPath
    & $nunitPath $testAssemblies "/framework:net-4.5"
  }

  if ($LastExitCode -ne 0) { throw "Tests failed"}
}

Task compile -Description "Build application only" {
  Push-Location $PSScriptRoot
  try {

  } finally {
    Pop-Location
  }
}

task prepDevEnvironment -Description "Installs npm dependencies etc. used in the development environment (assumes node is installed)"{
    Get-SourceDirectories($sourceDir) | % { Invoke-CommandInDirectory (Join-Path $sourceDir $_) "npm install" }
    Get-SourceDirectories($testDir) | % { Invoke-CommandInDirectory (Join-Path $testDir $_) "npm install" }
}

task pullCurrentAndBuild -Description "Does a git pull of the current branch followed by build" -depends pullCurrent, build

task pullCurrent -Description "Does a git pull" {
    git pull
}

task buildDist -Description "Update version. Build appication. Runs tests.  Builds Nuget packages" -depends version, build, publish {
}

task cleanBuildOutput -Description "Cleans the BuildOutput folder" {
  if (Test-Path $artifactsDir) {
    Remove-Item -Recurse -Force $artifactsDir
  }
  New-Item -ItemType directory -Path $artifactsDir
}

task startRaven -Description "Starts RavenDB." {
  Start-Raven
}

Task version -Description "Version the assemblies" {
  $env:DNX_BUILD_VERSION = $revision
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
	$dnxVersion = Get-DnxVersion
	Push-Location
	try {
		Set-Location "src/RabbitOperations.Collector"
		StartApp "dnvm use $dnxVersion -r CLR -arch x64;dnx --configuration $configuration web ASPNET_ENV=Development" "dnx"
	} finally {
		Pop-Location
	}
}

task ? -Description "Helper to display task info" {
  WriteDocumentation
}

function StartApp($appPath, $appName) {
    $processActive = Get-Process $appName -ErrorAction SilentlyContinue
		if ($processActive){
			$startAnyway = Read-Host -Prompt "Process $appName is running. Input Y to start a new process anyway"
		}
		Write-Host "'$startAnyway'"
    if(!$processActive -or $startAnyway -eq "Y")
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
  [string] $destPath = Join-Path $artifactsDir $projectName
  Copy-Item $path $destPath -recurse
}

function Create-NugetPackage ([string] $projectName) {
  Version-Nuspec $projectName
  [string] $input = Join-Path -Path "nuspec" -ChildPath ($projectName + ".nuspec")
  & .nuget\nuget pack $input -OutputDirectory $nugetOutputDir
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

function Get-NunitVersion {
  #Find the correct version of NUnit by looking at the referenced
  #package in the packages.config file of the solution
  [xml]$packages = Get-Content ".\.nuget\Packages.config"
  $server = $packages.SelectSingleNode("//package[@id='NUnit.Console']")
  $version = $server.GetAttribute("version")

  return $version
}

function Get-NunitPath {
  [string] $version = Get-NunitVersion
  [string] $path = ".\packages\Nunit.Console.$version\tools\nunit3-console.exe"

  return $path
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

function Get-DnxVersion
{
    $globalJson = join-path $PSScriptRoot "global.json"
    $jsonData = Get-Content -Path $globalJson -Raw | ConvertFrom-JSON
    return $jsonData.sdk.version
}

function Restore-Packages
{
    Get-ChildItem -Path . -Filter *.xproj -Recurse | ForEach-Object { & dnu restore ("""" + $_.DirectoryName + """") }
}

function Build-Projects
{
  Get-ChildItem -Path .\src -Filter *.xproj -Recurse | ForEach-Object {
    & dnu build ("""" + $_.DirectoryName + """") --configuration $configuration --out $artifactsDir
    if($LASTEXITCODE -ne 0) {
      throw "Build failed"
    }
  }
}

function Package-Project([string] $projectDirectoryName)
{
    & dnu pack ("""" + $projectDirectoryName + """") --configuration $configuration --out .\artifacts\packages; if($LASTEXITCODE -ne 0) { exit 1 }
}

function Publish-TestProject([string] $projectDirectoryName, [int]$index)
{
    # Publish to a numbered/indexed folder rather than the full test project name
    # because the package paths get long and start exceeding OS limitations.
    & dnu publish ("""" + $projectDirectoryName + """") --configuration $configuration --no-source --out .\artifacts\tests\$index; if($LASTEXITCODE -ne 0) { exit 2 }
}

function Invoke-Tests
{
    Get-ChildItem .\artifacts\tests -Filter test.cmd -Recurse | ForEach-Object { & $_.FullName; if($LASTEXITCODE -ne 0) { exit 3 } }
}

function Setup-Dnvm {
	$dnxVersion = Get-DnxVersion
  # Remove the installed DNVM from the path and force use of
  # per-user DNVM (which we can upgrade as needed without admin permissions)
  Remove-PathVariable "*Program Files\Microsoft DNX\DNVM*"
  Install-Dnvm

  # Install DNX
  & dnvm install $dnxVersion -r CoreCLR -NoNative #-Unstable
  & dnvm install $dnxVersion -r CLR -NoNative #-Unstable
  & dnvm use $dnxVersion -r CLR -arch x64
}

function Use-Dnvm {
	$dnxVersion = Get-DnxVersion
  & dnvm use $dnxVersion -r CLR
}

function Install-Dnvm
{
    & where.exe dnvm 2>&1 | Out-Null
    if(($LASTEXITCODE -ne 0) -Or ((Test-Path Env:\APPVEYOR) -eq $true))
    {
        Write-Host "DNVM not found"
        &{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}

        # Normally this happens automatically during install but AppVeyor has
        # an issue where you may need to manually re-run setup from within this process.
        if($env:DNX_HOME -eq $NULL)
        {
            Write-Host "Initial DNVM environment setup failed; running manual setup"
            $tempDnvmPath = Join-Path $env:TEMP "dnvminstall"
            $dnvmSetupCmdPath = Join-Path $tempDnvmPath "dnvm.ps1"
            & $dnvmSetupCmdPath setup
        }
    }
}

function Remove-PathVariable
{
    param([string] $VariableToRemove)
    $path = [Environment]::GetEnvironmentVariable("PATH", "User")
    $newItems = $path.Split(';') | Where-Object { $_.ToString() -inotlike $VariableToRemove }
    [Environment]::SetEnvironmentVariable("PATH", [System.String]::Join(';', $newItems), "User")
    $path = [Environment]::GetEnvironmentVariable("PATH", "Process")
    $newItems = $path.Split(';') | Where-Object { $_.ToString() -inotlike $VariableToRemove }
    [Environment]::SetEnvironmentVariable("PATH", [System.String]::Join(';', $newItems), "Process")
}

function Copy-ReleaseNotes([string]$version) {
  $fileVersion = $version.Replace(".", "_")
  $fileName = "Rabbit_Operations_$fileVersion.markdown"
  $item = "node_modules/trello-releasenotes/export/$fileName"
  Move-Item $item -Destination ReleaseNotes -Force
}

function Get-SourceDirectories($dir){
  return Get-ChildItem $dir
}

function Invoke-CommandInDirectory($dir, $cmd) {
  Write-Host "Running $cmd in $dir"
  Push-Location $dir
  try {
    iex $cmd
  } finally {
    Pop-Location
  }
}
