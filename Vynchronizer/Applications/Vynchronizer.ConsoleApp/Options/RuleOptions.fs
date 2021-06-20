module Vynchronizer.ConsoleApp.Options.RuleOptions

open CommandLine
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Target


[<Struct>]
type internal ResourceStorageArg =
    | UnknownStorage = 0
    | LocalFileSystem = 1
    | GoogleDrive = 2

[<Struct>]
type internal ConflictResolutionPolicyArg =
    | ReplaceAll = 0
    | ReplaceIfNewer = 1

[<Struct>]
type internal ScheduleTypeArg =
    | Second = 0
    | Minute = 1
    | Hour = 2
    | Day = 3
    | Week = 4
    | Month = 5
    | Year = 6

[<Struct>]
type internal ScheduleType =
    | Second
    | Minute
    | Hour
    | Day
    | Week
    | Month
    | Year

[<Literal>]
let private scheduleNumberGroup = "ScheduleNumberGroup"

[<Verb("execute", HelpText = "Executes synchronization rule.")>]
type internal ExecuteRuleOptions = {
    [<Option("source-storage", Required = true, HelpText = "Source storage.")>]
    SourceStorageType: ResourceStorageArg

    [<Option("source-path", Required = true, HelpText = "Resource path on source storage.")>]
    SourcePath: string

    [<Option("source-block-size", Required = false, Default = 1024, HelpText = "Block size. Synchronization will copy data from source to target by blocks. Valid values: LocalFileSystem (1), GoogleDrive (2).")>]
    SourceBlockSize: int

    [<Option("target-storage", Required = true, HelpText = "Target storage.")>]
    TargetStorageType: ResourceStorageArg

    [<Option("target-path", Required = true, HelpText = "Resource path on target storage.")>]
    TargetPath: string

    [<Option("target-resolution", Required = false, Default = ConflictResolutionPolicyArg.ReplaceAll, HelpText = "Policy to resolve conflicts during synchronization. Valid values: ReplaceAll (0), ReplaceIfNewer (1).")>]
    TargetConflictResolution: ConflictResolutionPolicyArg
}

[<Verb("execute", HelpText = "Adds synchronization rule and schedules processing.")>]
type internal AddRuleOptions = {
    [<Option("source-storage", Required = true, HelpText = "Source storage.")>]
    SourceStorageType: ResourceStorageArg

    [<Option("source-path", Required = true, HelpText = "Resource path on source storage.")>]
    SourcePath: string

    [<Option("source-block-size", Required = false, Default = 1024, HelpText = "Block size. Synchronization will copy data from source to target by blocks. Valid values: LocalFileSystem (1), GoogleDrive (2).")>]
    SourceBlockSize: int

    [<Option("target-storage", Required = true, HelpText = "Target storage.")>]
    TargetStorageType: ResourceStorageArg

    [<Option("target-path", Required = true, HelpText = "Resource path on target storage.")>]
    TargetPath: string

    [<Option("target-resolution", Required = false, Default = ConflictResolutionPolicyArg.ReplaceAll, HelpText = "Policy to resolve conflicts during synchronization. Valid values: ReplaceAll (0), ReplaceIfNewer (1).")>]
    TargetConflictResolution: ConflictResolutionPolicyArg

    [<Option("schedule-number", Group = scheduleNumberGroup, Required = false, Default = 1,  HelpText = "Schedule number. Meaning depends on the other schedule options.")>]
    ScheduleNumber: int

    [<Option("schedule-type", Group = scheduleNumberGroup, Required = false, Default = ScheduleTypeArg.Second, HelpText = "Schedule type.")>]
    ScheduleType: ScheduleTypeArg
}

let internal convertResourceStorage resourceStorage =
    match resourceStorage with
        | ResourceStorageArg.UnknownStorage -> ResourceStorage.UnknownStorage
        | ResourceStorageArg.LocalFileSystem -> ResourceStorage.LocalFileSystem
        | ResourceStorageArg.GoogleDrive -> ResourceStorage.GoogleDrive
        | _ -> invalidArg (nameof resourceStorage) ($"Resource storage is out of range: \"{resourceStorage.ToString()}\".")

let internal convertConflictResolutionPolicy conflictResolutionPolicy =
    match conflictResolutionPolicy with
        | ConflictResolutionPolicyArg.ReplaceAll -> ConflictResolutionPolicy.ReplaceAll
        | ConflictResolutionPolicyArg.ReplaceIfNewer -> ConflictResolutionPolicy.ReplaceIfNewer
        | _ -> invalidArg (nameof conflictResolutionPolicy) ($"Conflict resolution policy is out of range: \"{conflictResolutionPolicy.ToString()}\".")

let internal convertScheduleType scheduleType =
    match scheduleType with
        | ScheduleTypeArg.Second -> ScheduleType.Second
        | ScheduleTypeArg.Minute -> ScheduleType.Minute
        | ScheduleTypeArg.Hour -> ScheduleType.Hour
        | ScheduleTypeArg.Day -> ScheduleType.Day
        | ScheduleTypeArg.Week -> ScheduleType.Week
        | ScheduleTypeArg.Month -> ScheduleType.Month
        | ScheduleTypeArg.Year -> ScheduleType.Year
        | _ -> invalidArg (nameof scheduleType) ($"Schedule type is out of range: \"{scheduleType.ToString()}\".")
