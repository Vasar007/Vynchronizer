module Vynchronizer.Core.Operator


open System
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Source
open Vynchronizer.Core.Target

let copyData<'TData> (source: ISource<'TData>) (target: ITarget<'TData>) =
    // TODO: compare metadata.
    let dataSource = source.GetData()
    target.WriteData(dataSource)
