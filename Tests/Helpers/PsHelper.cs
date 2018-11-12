using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

namespace Tests.Helpers
{
    public static class PsHelper
    {
        public static Dictionary<string, int> Run(string script)
        {
            Console.WriteLine("Running script:");
            Console.WriteLine(script);

            Runspace runspace = RunspaceFactory.CreateRunspace();

            // open it
            runspace.Open();

            // create a pipeline and feed it the script text
            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(script);

            // execute the script
            Collection<PSObject> results = pipeline.Invoke();

            // close the runspace
            runspace.Close();

            // convert the script result into a single string
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }

            string t = stringBuilder.ToString();

            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (string record in stringBuilder.ToString().Trim().Replace("\r\n", ";").Split(';'))
            {
                result.Add(record.Split(',')[0], int.Parse(record.Split(',')[1]));
            }

            return result;
        }
    }
}
