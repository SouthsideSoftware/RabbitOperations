Function Invoke-Migrations
{
[CmdletBinding()]
    Param(
    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [String]
    $MigrateExe,
    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [String]
    $MigrationsAssembly,
    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [String]
    $ConnectionString,
    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [String]
    $EnvironmentName,
	[Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [String]
    $DatabaseName
    )
    # Pre-Migrations
    Confirm-ExitCode { & $MigrateExe --connectionString $ConnectionString --context $DatabaseName --assembly $MigrationsAssembly --provider sqlserver2008 --profile PreMigration --tag $EnvironmentName --namespace Autobahn.DataMigrations.PreMigration }

    # Migrations
    Confirm-ExitCode { & $MigrateExe --timeout 90 --connectionString $ConnectionString --context $DatabaseName --assembly $MigrationsAssembly --provider sqlserver2008 --tag $EnvironmentName --namespace Autobahn.DataMigrations.Migrations }

    # Post-Migrations
    Confirm-ExitCode { & $MigrateExe --connectionString $ConnectionString --context $DatabaseName --assembly $MigrationsAssembly --provider sqlserver2008 --profile PostMigration --tag $EnvironmentName --namespace Autobahn.DataMigrations.PostMigration }
}

Function Confirm-ExitCode
{
[CmdletBinding()]
    Param(
        [Parameter(Position=0,Mandatory=1)]
        [ScriptBlock]$cmd
    )
        & $cmd
        if ($lastexitcode -ne 0) 
        {
            throw ("Exec: something bad happened")
        }
}
