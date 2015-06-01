rd /s /q lib
md .\lib\net40

copy ..\Solidsoft.Esb.Resolution\bin\Release\Solidsoft.Esb.Resolution.dll lib\net40
copy ..\Solidsoft.Esb.BizTalk.PipelineComponents\bin\Release\Solidsoft.Esb.BizTalk.PipelineComponents.dll lib\net40

rd /s /q gac
md .\gac

copy ..\Solidsoft.Esb.Facts\bin\Release\Solidsoft.Esb.Facts.dll gac
copy ..\Solidsoft.Esb.Uddi\bin\Release\Solidsoft.Esb.Uddi.dll gac

nuget pack Solidsoft.Esb.nuspec