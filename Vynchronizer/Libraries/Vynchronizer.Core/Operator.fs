module Vynchronizer.Core.Operator


open System
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Source
open Vynchronizer.Core.Target

let private getSourceData (sourceSpec: SourceSpec) =
    match sourceSpec.StorageType with
        | ResourceStorage.UnknownStorage -> failwith "Unknown source resource storage."
        | ResourceStorage.LocalFileSystem -> processDataFromLocalFile
        | ResourceStorage.GoogleDrive -> raise (NotSupportedException("Google Drive storage is not supported."))

let private getTargetWriter (targetSpec: TargetSpec) =
    match targetSpec.StorageType with
        | ResourceStorage.UnknownStorage -> failwith "Unknown target resource storage."
        | ResourceStorage.LocalFileSystem -> writeDataToLocalFile
        | ResourceStorage.GoogleDrive -> raise (NotSupportedException("Google Drive storage is not supported."))

// TODO: add rule to specify when we should copy data.
let public processSpecs sourceSpec targetSpec =
    let source = getSourceData sourceSpec
    let target = getTargetWriter targetSpec

    // TODO: compare metadata at first.
    source sourceSpec
        |> Result.bind (target targetSpec)
