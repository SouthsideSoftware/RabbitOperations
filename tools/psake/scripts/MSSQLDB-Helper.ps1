Function Create-DB
{
[CmdletBinding()]
Param(
	[String]
	$Server,
	[String]
	$DatabaseName,
	[Switch]
	$Force
)	
	if ($Force) {
		DROP-DB -Server $Server -DatabaseName $DatabaseName
	}
	
	$cmd = "
	CREATE DATABASE [$DatabaseName]
	CONTAINMENT = NONE
			
	ALTER DATABASE [$DatabaseName]
    SET RECOVERY SIMPLE WITH NO_WAIT`n"
		
	if ($Force -eq $False) {
		$tSql = "
		IF DB_ID('$DatabaseName') IS NULL
		BEGIN" + 
		$cmd + 
		"END"
	} else {
		$tsql = $cmd
	}
	
	
	Execute-Sql -Server $Server -Database "Master" -cmd $tsql
}

Function Drop-DB
{
[CmdletBinding()]
Param(
	[String]
	$Server,
	[String]
	$DatabaseName
)
	$tsql = "
IF DB_ID('$DatabaseName') IS NOT NULL 
BEGIN
	ALTER DATABASE [$DatabaseName] 
	SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

	DROP DATABASE [$DatabaseName];
END"
	
	Execute-Sql -Server $Server -Database "Master" -cmd $tsql
}

Function Create-DefaultLogin
{
[CmdletBinding()]
Param(
	[String]
	$Server
)
	$tsql = "
USE [master]
GO

IF NOT EXISTS (SELECT * FROM sys.syslogins WHERE Name = 'IIS APPPOOL\ir.local.com')
BEGIN
	CREATE LOGIN [IIS APPPOOL\ir.local.com] FROM WINDOWS WITH DEFAULT_DATABASE=[master]
	ALTER SERVER ROLE [sysadmin] ADD MEMBER [IIS APPPOOL\ir.local.com]
END"

	Execute-Sql -Server $Server -Database "Master" -cmd $tsql
	
}

Function Execute-Sql
{
[CmdletBinding()]
Param(
	[String]
	$Server,
	[String]
	$Database,
	[String]
	$cmd
)
	Push-Location
    Import-Module SQLPS -DisableNameChecking
	Pop-Location
	
	Invoke-Sqlcmd -ServerInstance $Server -Database $Database -Query $cmd -Verbose
}