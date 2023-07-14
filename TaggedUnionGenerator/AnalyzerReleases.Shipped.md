
## Release 1.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
UG0001  |  Usage   |  Error   | "Failed to generate union struct" - Generic failure to generate union struct due to an internal exception being thrown
UG0002  |  Usage   |  Error   | "Union option name cannot me empty" - Missing or empty name of the option
UG0003  |  Usage   |  Error   | "Union option names are not unique" - Duplicate names of options prevent proper generation of tagged union
UG0004  |  Usage   |  Error   | "Union option name does not start with capital letter" - Option name does not use PascalCase naming format
UG0005  |  Usage   |  Warning | "Union with single option serves no purpose" - A Tagged union with single named option
UG0006  |  Usage   |  Error   | "Type is not an union" - Failure to generate union json converter due to invalid type provided
UG0007  |  Usage   |  Error   | "Failed to generate union json serializer" - Generic failure to generate union json converter due to an internal exception being thrown
