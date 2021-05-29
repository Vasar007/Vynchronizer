module Vynchronizer.ConsoleApp.Options.RuleOptions

open CommandLine
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Target


[<Struct>]
type public ResourceStorageArg =
    | UnknownStorage = 0
    | LocalFileSystem = 1
    | GoogleDrive = 2

[<Struct>]
type public ConflictResolutionPolicyArg =
    | ReplaceAll = 0
    | ReplaceIfNewer = 1

[<Verb("execute", HelpText = "Executes synchronization rule.")>]
type ExecuteRuleOptions = {
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
