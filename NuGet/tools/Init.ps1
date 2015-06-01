param($installPath, $toolsPath, $package, $project) 

function Gac-Remove
{
    param (
        [parameter(Mandatory = $true)][string] $assembly
    )

    try
    {
        $Error.Clear()

        [Reflection.Assembly]::LoadWithPartialName("System.EnterpriseServices") | Out-Null
        [System.EnterpriseServices.Internal.Publish] $publish = New-Object System.EnterpriseServices.Internal.Publish

        if (!(Test-Path $assembly -type Leaf) ) 
            { throw "The assembly $assembly does not exist" }

        $publish.GacRemove($assembly)

        Write-Host "`t`t$($MyInvocation.InvocationName): Assembly $assembly removed from GAC"
    }

    catch
    {
        Write-Host "`t`t$($MyInvocation.InvocationName): $_"
    }
}

function Gac-Install
{
    param (
        [parameter(Mandatory = $true)][string] $assembly,
        [bool] $force
    )

    try
    {
        $Error.Clear()

        [Reflection.Assembly]::LoadWithPartialName("System.EnterpriseServices") | Out-Null
        [System.EnterpriseServices.Internal.Publish] $publish = New-Object System.EnterpriseServices.Internal.Publish

        if (!(Test-Path $assembly -type Leaf) ) 
            { throw "The assembly $assembly does not exist" }

        if ($force)
            { Gac-Remove $assembly }

        if ([System.Reflection.Assembly]::LoadFile($assembly).GetName().GetPublicKey().Length -eq 0 ) 
            { throw "The assembly $assembly must be strongly signed" }

        $publish.GacInstall($assembly)

        Write-Host "`t`t$($MyInvocation.InvocationName): Assembly $assembly installed in GAC"
    }

    catch
    {
        Write-Host "`t`t$($MyInvocation.InvocationName): $_"
    }
}

function PipelineComponent-Register
{
    param (
        [parameter(Mandatory = $true)][string] $assemblyPath,
        [parameter(Mandatory = $true)][string] $assemblyName,
        [bool] $force
    )

    try
    {
        $Error.Clear()

        $path = "C:\Program Files (x86)\Microsoft BizTalk Server 2013\Pipeline Components"

        if((Test-Path -Path $path))
        { 
            if ($force)
            { 
                if (Test-Path $path\$assemblyName -type Leaf)
                {
                    Remove-Item $path\$assemblyName
                }
            }

            Copy-Item $assemblyPath\$assemblyName $path
            Write-Host "`t`t$($MyInvocation.InvocationName): Pipeline component $assemblyPath registered"
        } 
        else
        {
            $missingBtsFolderMsg = "Pipeline component $assemblyPath could not be registered.  $path does not exist."
            Write-Host "`t`t$($MyInvocation.InvocationName): $missingBtsFolderMsg" -foregroundcolor "red"
        }
    }

    catch
    {
        Write-Host "`t`t$($MyInvocation.InvocationName): $_"
    }
}

Gac-Install $installPath\lib\net40\SolidsoftReply.Esb.Libraries.Resolution.dll $True
Gac-Install $installPath\lib\net40\SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.dll $True
Gac-Install $installPath\gac\SolidsoftReply.Esb.Libraries.Facts.dll $True
Gac-Install $installPath\gac\SolidsoftReply.Esb.Libraries.Uddi.dll $True

PipelineComponent-Register $installPath\lib\net40 SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.dll $True
