$isPullRequest = $env:APPVEYOR_PULL_REQUEST_NUMBER -ne $null
$packagesDir = "./packages"
$coverageDir = "./codecoverage"
$xunitConsoleExe = "$packagesDir/xunit.runner.console.2.0.0/tools/xunit.console.exe"
$openCoverExe = "$packagesDir/OpenCover.4.6.166/tools/OpenCover.Console.exe"
$reportGeneratorExe = "$packagesDir/ReportGenerator.2.2.0.0/tools/ReportGenerator.exe"
$testDir = "*Tests/bin/*"
$tests = @("$testDir/*Tests.dll")
$targetArgs = Get-ChildItem $tests -Recurse
$openCoverXml = "opencover_report.xml"
$xunitXml = "xunit_report.xml"
$converallsNetExe = "$packagesDir/coveralls.io.1.3.4/tools/coveralls.net.exe"
$openCoverArgs = @('-register:user', "`"-target:$xunitConsoleExe`"", "`"-targetargs:$targetArgs -appveyor -noshadow -nologo -quiet`"", "`"-filter:+[NModbus4]*`"", "`"-output:$openCoverXml`"", '-coverbytest:*Tests.dll', '-log:All', '-returntargetcode')
& $xunitConsoleExe $targetArgs -xml $xunitXml
$testsFailed = Select-String $xunitXml -pattern "<failure .*>" -Quiet
if (!$testsFailed)
{
    & $openCoverExe $openCoverArgs
    & $reportGeneratorExe -verbosity:Info "`"-reports:$openCoverXml`"" "`"-targetdir:$coverageDir`""
    if (!$isPullRequest)
    {
        & $converallsNetExe --opencover $openCoverXml --full-sources --repo-token gSRzaRcGVrNfpOrbaEwGCEej4KypnMnK1        
    }
}
