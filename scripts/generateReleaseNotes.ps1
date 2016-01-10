param(
  [Parameter(Mandatory=$true)]
  [string]$version
)

[string] $scriptDirectory = Split-Path $MyInvocation.MyCommand.Path

Push-Location $scriptDirectory\..\node_modules\trello-releasenotes
try {
  & node index.js -g "$version" -v $version
} finally {
  Pop-Location
}
