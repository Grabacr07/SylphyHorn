function Main
{
    begin
    {
        $target = 'Release (AppX)'
        $bin = '..\source\SylphyHorn\bin'
 
        $appxManifest = 'AppxManifest.xml'
        $appxManifestSource = 'app.appxmanifest'
        $versionSource = 'SylphyHorn.exe'

        $targetKeywords = '*.exe','*.dll','*.exe.config','*.txt','*.VisualElementsManifest.xml', '*.png', $appxManifest
        $ignoreKeyword  = '*.vshost.*'

        $appx = 'SylphyHorn.appx'
        $appxBundle = 'SylphyHorn.appxbundle'
        $storekey = 'SylphyHorn.StoreKey.pfx'
        $publisher = 'CN=33C1D2CA-4B3F-4CCA-8103-6D02939C6477'
        
        $appxMapping = '_SylphyHorn.appx.map'
        $appxBundleMapping = '_SylphyHorn.appxbundle.map'

        #$tempCer = '_SylphyHorn.cer'
        #$tempPvk = '_SylphyHorn.pvk'
        #$tempPfx = '_SylphyHorn.pfx'

        $win10Sdk = 'C:\Program Files (x86)\Windows Kits\10\bin\x64' 
 
        if (-not(Test-Path $bin))
        {
            throw 'Script detected as locate in invalid path exception!! Make sure exist in <SylphyHorn repository root>\packaging\'
        }
    }
 
    end
    {
        try
        {
            # clean up current
            Get-ChildItem -Directory | Remove-item -Recurse
            Get-ChildItem | where { $_.Name -eq $appx -or $_.Name -eq $appxBundle } | Remove-Item
            Get-ChildItem | where Name -Like "_*" | Remove-Item
 
            $source = Join-Path $bin $target -Resolve
            $version = $(Get-ChildItem (Join-Path $source $versionSource)).VersionInfo.FileVersion

            # replace version in appxmanifest with assembly version
            $(Get-Content (Join-Path $source $appxManifestSource -Resolve)) -replace 'Version="0.0.0.0"', (' Version="{0}"' -f $version) | Out-File (Join-Path $source $appxManifest) -Encoding UTF8

            New-MappingFile -SourcePath $source -Filename $appxMapping -Targets $targetKeywords -Exclude $ignoreKeyword
            
            Start-SdkTool -SdkPath $win10Sdk -ToolName 'makeappx' -Arguments "pack /l /f $appxMapping /p $appx"
            Start-SdkTool -SdkPath $win10Sdk -ToolName 'signtool' -Arguments "sign /f $storekey /fd SHA256 /v $appx"            
            
            # if you don't have pfx file, can create that by following commands (Administrator authority required).
            #Start-SdkTool -SdkPath $win10Sdk -ToolName 'makecert' -Arguments "/r /h 0 /n $publisher /eku `"1.3.6.1.5.5.7.3.3`" /pe /sv $tempPvk $tempCer"
            #Start-SdkTool -SdkPath $win10Sdk -ToolName 'pvk2pfx'  -Arguments "/pvk $tempPvk /spc $tempCer /pfx $tempPfx"
            #Start-SdkTool -SdkPath $win10Sdk -ToolName 'signtool' -Arguments "sign /f $tempPfx /fd SHA256 /v $appx"
            #Certutil -addStore TrustedPeople $tempCer

            New-MappingFile -SourcePath '.\' -Filename $appxBundleMapping -Targets $appx

            Start-SdkTool -SdkPath $win10Sdk -ToolName 'makeappx' -Arguments "bundle /f $appxBundleMapping /p $appxBundle /bv $version"
        }
        catch
        {
            throw $_
        }
    }
}

function New-MappingFile
{
    [CmdletBinding()]
    param
    (
        [parameter(
            mandatory = 1,
            position  = 0,
            ValueFromPipeline = 1,
            ValueFromPipelineByPropertyName = 1)]
        [string]
        $SourcePath,
 
        [parameter(
            mandatory = 1,
            position  = 1,
            ValueFromPipelineByPropertyName = 1)]
        [string]
        $Filename,
 
        [parameter(
            mandatory = 1,
            position  = 2,
            ValueFromPipelineByPropertyName = 1)]
        [string[]]
        $Targets,
 
        [parameter(
            mandatory = 0,
            position  = 3,
            ValueFromPipelineByPropertyName = 1)]
        [string]
        $Exclude
    )
 
    begin
    {
        $files = New-Object 'System.Collections.Generic.List[String]'
        $mapping = "[Files]`n"
    }
 
    end
    {
        Foreach ($target in $Targets)
        {
            $files += Get-ChildItem -Path $SourcePath -Recurse -Filter $target | where Name -NotLike $Exclude
        }

        Foreach ($file in $files | sort)
        {
            $mapping += "`"{0}`" `"{1}`"`n" -f $file.FullName, $file.FullName.Replace((Convert-Path $SourcePath) + "\", "")
        }        

        Write-Output $mapping
        Write-Output $mapping | Out-File $Filename
    }
}

function Start-SdkTool
{
    [CmdletBinding()]
    param
    (
        [parameter(
            mandatory = 1,
            position  = 0,
            ValueFromPipeline = 1,
            ValueFromPipelineByPropertyName = 1)]
        [string]
        $SdkPath,
 
        [parameter(
            mandatory = 1,
            position  = 1,
            ValueFromPipelineByPropertyName = 1)]
        [string]
        $ToolName,
 
        [parameter(
            mandatory = 1,
            position  = 2,
            ValueFromPipelineByPropertyName = 1)]
        [string]
        $Arguments
    )
    
    begin
    {
        $exe = (Join-Path $SdkPath ($ToolName + '.exe'))
    }

    end
    {
        Write-Host $ToolName $Arguments
        Write-Host '----------'
        & $exe ($Arguments -split " ")
        Write-Host ''
        Write-Host ''
    }
}

Main
