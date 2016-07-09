Function Find-NugetTool
{
[CmdletBinding()]
Param(
    [String]
    $ToolName,
    [String]
    $PackagesFolder
)
    return  (Get-ChildItem $PackagesFolder -Include *$ToolName -Recurse).FullName | Sort-Object | Select-Object -last 1
}