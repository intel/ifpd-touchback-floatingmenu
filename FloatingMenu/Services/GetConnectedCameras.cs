using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Text;

namespace InteractiveDisplayCapture.Services
{
    public class GetConnectedCameras
    {
        public static async Task<List<string>> GetConnectedCameraList()
        {
            var result = new List<string>();

            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c pnputil /enum-devices /class Camera /connected";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();

            var lines = output.Split('\n');

            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("Friendly Name"))
                {
                    var name = line.Split(':')[1].Trim();
                    result.Add(name);
                }
            }

            return result;
        }
    }
}
