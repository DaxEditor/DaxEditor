#// The project released under MS-PL license https://daxeditor.codeplex.com/license

set-strictmode -version 2.0

# Gets list of functions from SSAS and generates all files that use the functions
$pathToAdomdClinent = (Get-Location).ToString() + "\Microsoft.AnalysisServices.AdomdClient.dll"
if(![System.IO.File]::Exists($pathToAdomdClinent)) {
   throw "$pathToAdomdClinent does not exist.  Fix the path and re-run the script."
}
$pathToGeneratedFile = (Get-Location).ToString() + "\DaxFunctions_Gen.cs"
if(![System.IO.File]::Exists($pathToGeneratedFile)) {
   throw "$pathToGeneratedFile does not exist.  The script needs to run from the location of the file."
}

Add-Type -Path $pathToAdomdClinent

$conn = New-Object Microsoft.AnalysisServices.AdomdClient.AdomdConnection "Data Source=."
$conn.Open()

# The first table contains functions, the second table parameters
$resultSet = $conn.GetSchemaDataSet("MDSCHEMA_FUNCTIONS", $null)

# Iterate over the functions table and select DAX functions
$generatedCode = ""
$resultSet.Tables[0] | Where-Object {$_.Origin -ge 3} | %{

   if($_.Item("Library_Name") -eq "Scalar"){
         $libraryName = "LibraryName.Scalar"
   }
   else {
         $libraryName = "LibraryName.Unknown"
   }
   $interfaceName = $_.Item("Interface_Name")
   if($interfaceName -eq "DateTime") {
      $interfaceName = "InterfaceName.DateTime"
   } elseif ($interfaceName -eq "Info") {
      $interfaceName = "InterfaceName.Info"
   } elseif ($interfaceName -eq "Logical") {
      $interfaceName = "InterfaceName.Logical"
   } elseif ($interfaceName -eq "MathTrig") {
      $interfaceName = "InterfaceName.MathTrig"
   } elseif ($interfaceName -eq "Text") {
      $interfaceName = "InterfaceName.Text"
   } elseif ($interfaceName -eq "Filter") {
      $interfaceName = "InterfaceName.Filter"
   } elseif ($interfaceName -eq "ParentChild") {
      $interfaceName = "InterfaceName.ParentChild"
   } elseif ($interfaceName -eq "Statistical") {
      $interfaceName = "InterfaceName.Statistical"
   } else {
      $interfaceName = "InterfaceName.Unknown"
   }



   $generatedCode += "

      // " + $_.Item("Function_Name") + "
      parameters = new List<Babel.Parameter>();"

    $currentParameterInfo = $_.ParameterInfo;
    $resultSet.Tables[1] | Where-Object {$_.rowsetTablePARAMETERINFO -eq $currentParameterInfo} | %{
        $generatedCode +=  [string]::Format("
      parameter = new Babel.Parameter();
      parameter.Name = @""{0}"";
      parameter.Display = @""{1}"";
      parameter.Description = @""{2}"";
      parameters.Add(parameter);",
        $_.Item("Name"),
        $_.Item("Name"),
        $_.Item("Description"))
   }

   $generatedCode += [string]::Format("
      this.Add(new DaxFunctionDescription() {{ Name = @""{0}"", Description = @""{1}"", LibraryName = {2}, InterfaceName = {3}, Parameters = parameters }});",
      $_.Item("Function_Name"),
      $_.Item("Description"),
      $libraryName,
      $interfaceName
      )
}



$tokenBeginReplace = "// Beggining of generated code"
$tokenEndReplace = "// End of generated code"
$inputText = Get-Content $pathToGeneratedFile | Out-String
[string]$resultText = $inputText.Substring(0, $inputText.IndexOf($tokenBeginReplace) + $tokenBeginReplace.Length)
$resultText += $generatedCode;
$resultText += $inputText.Substring($inputText.IndexOf($tokenEndReplace) - 2);
$resultText | Out-File -FilePath $pathToGeneratedFile -Encoding UTF8


