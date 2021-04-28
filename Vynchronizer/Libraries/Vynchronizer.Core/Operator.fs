module Vynchronizer.Core.Operator


open System
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Source
open Vynchronizer.Core.Target

// TODO: add rule to specify when we should copy data.
let copyData<'TData> (source: ISource<'TData>) (target: ITarget<'TData>) =
    // TODO: compare metadata at first.
    let dataSource = source.GetData()
    target.WriteData(dataSource)
