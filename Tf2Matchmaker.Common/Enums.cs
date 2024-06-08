namespace Tf2Matchmaker;

public enum ServerType
{
	Unknown,
	Dedicated,
	NonDedicated,
	SourceTv
}

public enum ServerEnvironment
{
	Unknown,
	Windows,
	Linux,
	MacOs
}

public enum ServerVisibility
{
	Public = 0,
	Private = 1
}

public enum VacSecurityLevel
{
	Unsecured = 0,
	Secured = 1
}