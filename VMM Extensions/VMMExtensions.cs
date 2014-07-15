using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.AddIn;
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.ContextTypes;
using Microsoft.SystemCenter.VirtualMachineManager;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Windows.Forms;



namespace VMM_Extensions
{
    [AddIn("Get VM Paths")]
    public class VMMExtensions : ActionAddInBase
    {
        public override void PerformAction(IList<ContextObject> contextObjects)
        {
            //string CustomPropScript = 
            //    string.Format(
            //"if (!(Get-SCCustomProperty -Name VMPath)) {New-SCCustomProperty -Name VMPath -Description 'Virtual Machine Configuration Path' -AddMember @('VM')}"
            //);

            //this.PowerShellContext.ExecuteScript(CustomPropScript);

            foreach (var contextObject in contextObjects)
            {

                string getScript =
                    string.Format(
                        "$prop = Get-SCCustomProperty -Name 'VMPath';" +
                        "$vm = Get-SCVirtualMachine -ID {0};" +
                        "$location = $vm.Location;" +
                        "$vm | Set-SCCustomPropertyValue -CustomProperty $prop -Value $location | out-null",
                        contextObject.ID);

                Runspace runspace = RunspaceFactory.CreateRunspace();
                runspace.Open();
                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(getScript);
                pipeline.Invoke();

            }
        }
    }
}
