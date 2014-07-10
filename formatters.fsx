#I "packages/Deedle/lib/net40"
#I "packages/FSharp.Charting/lib/net40"
#I "packages/FSharp.Data/lib/net40"
#I "packages/MathNet.Numerics/lib/net40"
#I "packages/MathNet.Numerics.FSharp/lib/net40"
#I "packages/FSharp.Formatting/lib/net40"
#r "Deedle.dll"
#r "System.Windows.Forms.DataVisualization.dll"
#r "FSharp.Charting.dll"
#r "FSharp.Data.dll"
#r "MathNet.Numerics.dll"
#r "MathNet.Numerics.FSharp.dll"
#r "FSharp.Literate.dll"
#r "FSharp.Markdown.dll"

open System.IO
open Deedle
open Deedle.Internal
open FSharp.Literate
open FSharp.Markdown
open FSharp.Charting

// --------------------------------------------------------------------------------------
// Implements Markdown formatters for common FsLab things - including Deedle series
// and frames, F# Charting charts and System.Image values
// --------------------------------------------------------------------------------------

// How many columns and rows from frame should be rendered
let startColumnCount = 3
let endColumnCount = 3

let startRowCount = 8
let endRowCount = 4

// How many items from a series should be rendered
let startItemCount = 5
let endItemCount = 3

// --------------------------------------------------------------------------------------
// Helper functions etc.
// --------------------------------------------------------------------------------------

open System.Windows.Forms
open FSharp.Charting.ChartTypes

/// Extract values from any series using reflection
let (|SeriesValues|_|) (value:obj) = 
  let iser = value.GetType().GetInterface("ISeries`1")
  if iser <> null then
    let keys = value.GetType().GetProperty("Keys").GetValue(value) :?> System.Collections.IEnumerable
    let vector = value.GetType().GetProperty("Vector").GetValue(value) :?> IVector
    Some(Seq.zip (Seq.cast<obj> keys) vector.ObjectSequence)
  else None

/// Format value as a single-literal paragraph
let formatValue def = function
  | Some v -> [ Paragraph [Literal (v.ToString()) ]] 
  | _ -> [ Paragraph [Literal def] ]

/// Format body of a single table cell
let td v = [ Paragraph [Literal v] ]

/// Use 'f' to transform all values, then call 'g' with Some for 
/// values to show and None for "..." in the middle
let mapSteps (startCount, endCount) f g input = 
  input 
  |> Seq.map f |> Seq.startAndEnd startCount endCount
  |> Seq.map (function Choice1Of3 v | Choice3Of3 v -> g (Some v) | _ -> g None)
  |> List.ofSeq

// Tuples with the counts, for easy use later on
let fcols = startColumnCount, endColumnCount
let frows = startRowCount, endRowCount
let sitms = startItemCount, endItemCount

/// Reasonably nice default style for charts
let chartStyle ch =
  let grid = ChartTypes.Grid(LineColor=System.Drawing.Color.LightGray)
  ch 
  |> Chart.WithYAxis(MajorGrid=grid)
  |> Chart.WithXAxis(MajorGrid=grid)

/// Checks if the given directory exists. If not then this functions creates the directory.
let ensureDirectory dir =
  let di = new DirectoryInfo(dir)
  if not di.Exists then di.Create()

/// Combine two paths
let (@@) a b = Path.Combine(a, b)

// --------------------------------------------------------------------------------------
// Build FSI evaluator
// --------------------------------------------------------------------------------------

let mutable currentOutputKind = OutputKind.Html
let InlineMultiformatBlock(html, latex) = 
  let block =
    { new MarkdownEmbedParagraphs with
        member x.Render() = 
          if currentOutputKind = OutputKind.Html then [ InlineBlock html ]
          else [ InlineBlock latex ] }
  EmbedParagraphs(block)

/// Builds FSI evaluator that can render System.Image, F# Charts, series & frames
let createFsiEvaluator root output =

  /// Counter for saving files
  let imageCounter = 
    let count = ref 0
    (fun () -> incr count; !count)

  let transformation (value:obj, typ:System.Type) =
    match value with 
    | :? System.Drawing.Image as img ->
        // Pretty print image - save the image to the "images" directory 
        // and return a DirectImage reference to the appropriate location
        let id = imageCounter().ToString()
        let file = "chart" + id + ".png"
        ensureDirectory (output @@ "images")
        img.Save(output @@ "images" @@ file, System.Drawing.Imaging.ImageFormat.Png) 
        Some [ Paragraph [DirectImage ("", (root + "/images/" + file, None))]  ]

    | :? ChartTypes.GenericChart as ch ->
        // Pretty print F# Chart - save the chart to the "images" directory 
        // and return a DirectImage reference to the appropriate location
        let id = imageCounter().ToString()
        let file = "chart" + id + ".png"
        ensureDirectory (output @@ "images")
      
        // We need to reate host control, but it does not have to be visible
        ( use ctl = new ChartControl(chartStyle ch, Dock = DockStyle.Fill, Width=500, Height=300)
          ch.CopyAsBitmap().Save(output @@ "images" @@ file, System.Drawing.Imaging.ImageFormat.Png) )
        Some [ Paragraph [DirectImage ("", (root + "/images/" + file, None))]  ]

    | SeriesValues s ->
        // Pretty print series!
        let heads  = s |> mapSteps sitms fst (function Some k -> td (k.ToString()) | _ -> td " ... ")
        let row    = s |> mapSteps sitms snd (function Some v -> formatValue "N/A" (OptionalValue.asOption v) | _ -> td " ... ")
        let aligns = s |> mapSteps sitms id (fun _ -> AlignDefault)
        [ InlineMultiformatBlock("<div class=\"deedleseries\">", "\\vspace{1em}")
          TableBlock(Some ((td "Keys")::heads), AlignDefault::aligns, [ (td "Values")::row ]) 
          InlineMultiformatBlock("</div>","\\vspace{1em}") ] |> Some

    | :? IFrame as f ->
      // Pretty print frame!
      {new IFrameOperation<_> with
        member x.Invoke(f) = 
          let heads  = f.ColumnKeys |> mapSteps fcols id (function Some k -> td (k.ToString()) | _ -> td " ... ")
          let aligns = f.ColumnKeys |> mapSteps fcols id (fun _ -> AlignDefault)
          let rows = 
            f.Rows |> Series.observationsAll |> mapSteps frows id (fun item ->
              let def, k, data = 
                match item with 
                | Some(k, Some d) -> "N/A", k.ToString(), Series.observationsAll d |> Seq.map snd 
                | Some(k, _) -> "N/A", k.ToString(), f.ColumnKeys |> Seq.map (fun _ -> None)
                | None -> " ... ", " ... ", f.ColumnKeys |> Seq.map (fun _ -> None)
              let row = data |> mapSteps fcols id (function Some v -> formatValue def v | _ -> td " ... ")
              (td k)::row )
          Some [ 
            InlineMultiformatBlock("<div class=\"deedleframe\">","\\vspace{1em}")
            TableBlock(Some ([]::heads), AlignDefault::aligns, rows) 
            InlineMultiformatBlock("</div>","\\vspace{1em}")
          ] }
      |> f.Apply
    | _ -> None 
    
  // Create FSI evaluator, register transformations & return
  let fsiEvaluator = FsiEvaluator() 
  fsiEvaluator.RegisterTransformation(transformation)
  fsiEvaluator
//  let fsiEvaluator = fsiEvaluator :> IFsiEvaluator
//  { new IFsiEvaluator with
//      member x.Evaluate(text, asExpr, file) = 
//        captureDevice (fun () -> 
//          fsiEvaluator.Evaluate(text, asExpr, file))
//
//      member x.Format(res, kind) = 
//        let res = res :?> ExtraEvaluationResult
//        match kind, res.CapturedImage with
//        | FsiEmbedKind.Output, Some img -> 
//            [ match (res.Results :?> FsiEvaluationResult).Output with
//              | Some s  when not (String.IsNullOrWhiteSpace(s)) ->
//                  yield! fsiEvaluator.Format(res.Results, kind)
//              | _ -> ()
//              yield! transformation(img, typeof<Image>).Value ]
//        | _ -> fsiEvaluator.Format(res.Results, kind) }