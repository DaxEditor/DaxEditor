#// The project released under MS-PL license https://daxeditor.codeplex.com/license

# Generates code with DAX keywords

[array]$keyWords = @()
$keyWords += New-Object object | add-member NoteProperty Name "evaluate" -passThru  | add-member NoteProperty Description "Evaluate keyword" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "define" -passThru  | add-member NoteProperty Description "Define keyword" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "measure" -passThru  | add-member NoteProperty Description "Measure keyword" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "Order" -passThru  | add-member NoteProperty Description "Order keyword" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "by" -passThru  | add-member NoteProperty Description "BY keyword" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "TRUE" -passThru  | add-member NoteProperty Description "True constant" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "FALSE" -passThru  | add-member NoteProperty Description "False constant" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "ASC" -passThru  | add-member NoteProperty Description "Ascending sort order" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "DESC" -passThru  | add-member NoteProperty Description "Descending sort order" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "DAY" -passThru  | add-member NoteProperty Description "Period equal to 1 day in time intelligence functions" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "MONTH" -passThru  | add-member NoteProperty Description "Period equal to 1 month in time intelligence functions" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "YEAR" -passThru  | add-member NoteProperty Description "Period equal to 1 year in time intelligence functions" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "Create" -passThru  | add-member NoteProperty Description "CREATE keyword" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "CALCULATE" -passThru  | add-member NoteProperty Description "CALCULATE keyword" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "CALCULATION" -passThru  | add-member NoteProperty Description "CALCULATION keyword" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "PROPERTY" -passThru  | add-member NoteProperty Description "PROPERTY keyword" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "General" -passThru  | add-member NoteProperty Description "General calculation property format type" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "NumberDecimal" -passThru  | add-member NoteProperty Description "NumberDecimal calculation property format type" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "NumberWhole" -passThru  | add-member NoteProperty Description "NumberWhole calculation property format type" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "Percentage" -passThru  | add-member NoteProperty Description "Percentage calculation property format type" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "Scientific" -passThru  | add-member NoteProperty Description "Scientific calculation property format type" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "Currency" -passThru  | add-member NoteProperty Description "Currency calculation property format type" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "DateTimeCustom" -passThru  | add-member NoteProperty Description "DateTimeCustom calculation property format type" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "DateTimeShortDatePattern" -passThru  | add-member NoteProperty Description "DateTimeShortDatePattern calculation property format type" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "DateTimeGeneral" -passThru  | add-member NoteProperty Description "DateTimeGeneral calculation property format type" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "Text" -passThru  | add-member NoteProperty Description "Text format type" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "Accuracy" -passThru  | add-member NoteProperty Description "Accuracy keyword" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "ThousandSeparator" -passThru  | add-member NoteProperty Description "ThousandSeparator keyword" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "Format" -passThru  | add-member NoteProperty Description "Format keyword" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "AdditionalInfo" -passThru  | add-member NoteProperty Description "AdditionalInfo keyword" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "KPI" -passThru  | add-member NoteProperty Description "Key Performance Indicator" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "Visible" -passThru  | add-member NoteProperty Description "Measure Visible Property" -passThru  
$keyWords += New-Object object | add-member NoteProperty Name "Description" -passThru  | add-member NoteProperty Description "Measure Description Property" -passThru    
$keyWords += New-Object object | add-member NoteProperty Name "DisplayFolder" -passThru  | add-member NoteProperty Description "Measure DisplayFolder Property" -passThru    
$keyWords += New-Object object | add-member NoteProperty Name "Var" -passThru  | add-member NoteProperty Description "Var keyword" -passThru   
$keyWords += New-Object object | add-member NoteProperty Name "Return" -passThru  | add-member NoteProperty Description "Return keyword" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "DataTable" -passThru  | add-member NoteProperty Description "DATATABLE Function" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "Boolean" -passThru  | add-member NoteProperty Description "DataTable Boolean Column Type" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "DateTime" -passThru  | add-member NoteProperty Description "DataTable DateTime Column Type" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "Double" -passThru  | add-member NoteProperty Description "DataTable Double Column Type" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "Integer" -passThru  | add-member NoteProperty Description "DataTable Integer Column Type" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "String" -passThru  | add-member NoteProperty Description "DataTable String Column Type" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "RankX" -passThru  | add-member NoteProperty Description "RANKX Function" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "Skip" -passThru  | add-member NoteProperty Description "RANKX Ties Enum Element" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "Dense" -passThru  | add-member NoteProperty Description "RANKX Ties Enum Element" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "Not" -passThru  | add-member NoteProperty Description "NOT operator" -passThru
# KPI Xml support
$keyWords += New-Object object | add-member NoteProperty Name "AS" -passThru  | add-member NoteProperty Description "AS keyword" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "ASSOCIATED_MEASURE_GROUP" -passThru  | add-member NoteProperty Description "ASSOCIATED_MEASURE_GROUP Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "GOAL" -passThru  | add-member NoteProperty Description "GOAL Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "STATUS" -passThru  | add-member NoteProperty Description "STATUS Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "STATUS_GRAPHIC" -passThru  | add-member NoteProperty Description "STATUS_GRAPHIC Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "TREND" -passThru  | add-member NoteProperty Description "TREND Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "TREND_GRAPHIC" -passThru  | add-member NoteProperty Description "TREND_GRAPHIC Property" -passThru
# KPI Json support
$keyWords += New-Object object | add-member NoteProperty Name "KpiDescription" -passThru  | add-member NoteProperty Description "Measure KpiDescription Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "KpiTargetFormatString" -passThru  | add-member NoteProperty Description "Measure KpiTargetFormatString Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "KpiTargetDescription" -passThru  | add-member NoteProperty Description "Measure KpiTargetDescription Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "KpiTargetExpression" -passThru  | add-member NoteProperty Description "Measure KpiTargetExpression Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "KpiStatusGraphic" -passThru  | add-member NoteProperty Description "Measure KpiStatusGraphic Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "KpiStatusDescription" -passThru  | add-member NoteProperty Description "Measure KpiStatusDescription Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "KpiStatusExpression" -passThru  | add-member NoteProperty Description "Measure KpiStatusExpression Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "KpiTrendGraphic" -passThru  | add-member NoteProperty Description "Measure KpiTrendGraphic Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "KpiTrendDescription" -passThru  | add-member NoteProperty Description "Measure KpiTrendDescription Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "KpiTrendExpression" -passThru  | add-member NoteProperty Description "Measure KpiTrendExpression Property" -passThru
$keyWords += New-Object object | add-member NoteProperty Name "KpiAnnotations" -passThru  | add-member NoteProperty Description "Measure KpiAnnotations Property" -passThru

# DaxKeywords_Gen.cs
$pathToGeneratedFile = (Get-Location).ToString() + "\DaxKeywords_Gen.cs"
if(![System.IO.File]::Exists($pathToGeneratedFile)) {
   throw "$pathToGeneratedFile does not exist.  The script needs to run from the location of the file."
}
$generatedCode = ""
$keyWords | %{
   $generatedCode += [string]::Format("
      this.Add(new DaxKeywordDescription() {{ Name = @""{0}"", Description = @""{1}""}});",
      $_.Name.ToUpperInvariant(),
      $_.Description
      )
}

$tokenBeginReplace = "/* Begin generated code */"
$tokenEndReplace = "/* End generated code */"
$inputText = Get-Content $pathToGeneratedFile | Out-String
[string]$resultText = $inputText.Substring(0, $inputText.IndexOf($tokenBeginReplace) + $tokenBeginReplace.Length)
$resultText += $generatedCode;
$resultText += $inputText.Substring($inputText.IndexOf($tokenEndReplace) - 2);
$resultText | Out-File -FilePath $pathToGeneratedFile -Encoding UTF8


# lexer.lex
$pathToGeneratedFile = (Get-Location).ToString() + "\lexer.lex"
if(![System.IO.File]::Exists($pathToGeneratedFile)) {
   throw "$pathToGeneratedFile does not exist.  The script needs to run from the location of the file."
}

$fieldSize = (($keyWords | Sort-Object -property @{Expression={$_.Name.Length}; Ascending=$false} | Select-Object -First 1).Name.Length + 2) * 4

$generatedCode = ""
$listOfTokensString = "%token"

$keyWords | %{
$currentKeyword = $_.Name.ToUpperInvariant()
$listOfTokensString += " KW$currentKeyword"
$currentKeyword.ToCharArray() | %{ $generatedCode += "[" + $_ + [char]::ToLowerInvariant($_) + "]"}
$spacesNeeded = $fieldSize - $currentKeyword.Length * 4;
$generatedCode += [string]::Format("{0,$spacesNeeded}", " ");
$generatedCode += "{ return (int)Tokens.KW$currentKeyword; }" + [Environment]::NewLine;
}
$inputText = Get-Content $pathToGeneratedFile | Out-String
[string]$resultText = $inputText.Substring(0, $inputText.IndexOf($tokenBeginReplace) + $tokenBeginReplace.Length + 2)
$resultText += $generatedCode;
$resultText += $inputText.Substring($inputText.IndexOf($tokenEndReplace));
$resultText | Out-File -FilePath $pathToGeneratedFile -Encoding ASCII

# parser.y
$pathToGeneratedFile = (Get-Location).ToString() + "\parser.y"
$inputText = Get-Content $pathToGeneratedFile | Out-String
$tokenBeginReplace = "/* Begin generated list of tokens */"
$tokenEndReplace = "/* End generated list of tokens */"
[string]$resultText = $inputText.Substring(0, $inputText.IndexOf($tokenBeginReplace) + $tokenBeginReplace.Length + 2)
$resultText += $listOfTokensString;
$resultText += $inputText.Substring($inputText.IndexOf($tokenEndReplace) - 2);
$resultText | Out-File -FilePath $pathToGeneratedFile -Encoding ASCII
