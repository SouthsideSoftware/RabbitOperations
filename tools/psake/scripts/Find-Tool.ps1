Function Find-Tool
{
[CmdletBinding()]
Param(
	[string]
	$ToolName,
	[string]
	$PackagesFolder
)
	return (Get-ChildItem $PackagesFolder -Include $ToolName -Recurse).FullName | Sort-Object | Select-Object -last 1
}