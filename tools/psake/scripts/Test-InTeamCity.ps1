Function Test-InTeamCity {
Param(
)
    return Test-Path env:TEAMCITY_VERSION
}