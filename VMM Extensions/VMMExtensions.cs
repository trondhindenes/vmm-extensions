
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns;
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.ContextTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.AddIn;

//using Microsoft.SystemCenter.VirtualMachineManager;
//using System.Management.Automation;
//using System.Management.Automation.Runspaces;
//using System.Windows.Forms;


    [AddIn("Copy VM Name")]
    public class VMMExtensionsCopyVMName : ActionAddInBase
    {
        public override void PerformAction(IList<ContextObject> contextObjects)
        {
            var contextObject = contextObjects[0];

            string CopyVmNameScript = @"
                $vm = Get-SCVirtualMachine -ID VMID
                $vmname = $vm.name
                $vmname | clip.exe
            ";

            string CopyVmNameScriptFormatted =
                    CopyVmNameScript.Replace("VMID", contextObject.ID.ToString());
            //string.Format(UpdateScript,contextObject.ID);

            this.PowerShellContext.ExecuteScript(CopyVmNameScriptFormatted);

        }
    }

    [AddIn("Get VM Details")]
    public class VMMExtensionsGetVMDetails : ActionAddInBase
    {

        public override void PerformAction(IList<ContextObject> contextObjects)
        {

            string CustomPropScript = @"
                if (!(Get-SCCustomProperty -Name VMPath))
                    {New-SCCustomProperty -Name VMPath -Description 'Virtual Machine Configuration Path' -AddMember @('VM')}
                if (!(Get-SCCustomProperty -Name 'Mounted ISO'))
                    {New-SCCustomProperty -Name 'Mounted ISO' -Description 'Virtual Machine Mounted ISO Path Path' -AddMember @('VM')}
                if (!(Get-SCCustomProperty -Name 'IP Address'))
                    {New-SCCustomProperty -Name 'IP Address' -Description 'First IP address of first adapter' -AddMember @('VM')}
                ";

            this.PowerShellContext.ExecuteScript(CustomPropScript);

            foreach (var contextObject in contextObjects)
            {
                Guid jobguid = System.Guid.NewGuid();

                string UpdateScript = @"
                    if (!(Get-SCCustomProperty -Name VMPath))
                    {New-SCCustomProperty -Name VMPath -Description 'Virtual Machine Configuration Path' -AddMember @('VM')}
                    if (!(Get-SCCustomProperty -Name 'Mounted ISO'))
                    {New-SCCustomProperty -Name 'Mounted ISO' -Description 'Virtual Machine Mounted ISO Path Path' -AddMember @('VM')}
                    if (!(Get-SCCustomProperty -Name 'IP Address'))
                    {New-SCCustomProperty -Name 'IP Address' -Description 'First IP address of first adapter' -AddMember @('VM')}

                    $vm = Get-SCVirtualMachine -ID VMID
                    $jobguid = [system.guid]::newguid()
                    $prop = Get-SCCustomProperty -Name 'VMPath'
                    $location = $vm.Location
                    $customProp = $vm | Get-SCCustomPropertyValue -CustomProperty $prop
                    if ($customProp.Value -ne $location)
                    {
                        $vm | Set-SCCustomPropertyValue -CustomProperty $prop -Value $location -RunAsynchronously | out-null
                    }
                    
                    $prop = Get-SCCustomProperty -Name 'Mounted ISO'
                    $dvd = $vm | Get-SCVirtualDVDDrive
                    $customProp = $vm | Get-SCCustomPropertyValue -CustomProperty $prop
                    if ($dvd.Iso)
                    {
                        if ($customProp.Value -ne ($dvd.ISO.Name))
                        {
                            $vm | Set-SCCustomPropertyValue -CustomProperty $prop -Value ($dvd.ISO.Name) -RunAsynchronously  | out-null
                        }
	                    
                    }
                    Else
                    {
	                    
                        if ($customProp)
                        {
                            Remove-SCCustomPropertyValue -CustomPropertyValue $customProp | out-null
                        }
	                    
                    }


                    $prop = Get-SCCustomProperty -Name 'IP Address'
                    $nic = $vm | Get-SCVirtualNetworkAdapter
                    $customProp = $vm | Get-SCCustomPropertyValue -CustomProperty $prop
                    if ($nic.IPv4Addresses)
                    {
                        if ($customProp.Value -ne ($nic[0].IPv4Addresses[0]))
                        {
                            $vm | Set-SCCustomPropertyValue -CustomProperty $prop -Value ($nic[0].IPv4Addresses[0]) -RunAsynchronously  | out-null
                        }
	                    
                    }
                    Else
                    {
	                    
                        if ($customProp)
                        {
                            Remove-SCCustomPropertyValue -CustomPropertyValue $customProp | out-null
                        }
	                    
                    }
                ";

                string UpdateScriptFormatted =
                    UpdateScript.Replace("VMID", contextObject.ID.ToString());
                    //string.Format(UpdateScript,contextObject.ID);

                this.PowerShellContext.ExecuteScript(UpdateScriptFormatted);

                //Runspace runspace = RunspaceFactory.CreateRunspace();
                //runspace.Open();
                //Pipeline pipeline = runspace.CreatePipeline();
                //pipeline.Commands.AddScript(UpdateScriptFormatted);
                //var result = pipeline.Invoke();

            }
        }
    }


