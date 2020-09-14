using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiskAnalyser
{
    public class DiskAnalyser
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task _analyseTask;

        public DiskAnalyser()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public string[] GetDrives()
        {
            return DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed).Select(d => d.Name).ToArray();
        }

        public void Analyse(string drive)
        {
            var token = _cancellationTokenSource.Token;

            _analyseTask = Task.Run(() => DoAnalysis(drive), token);
        }

        private void DoAnalysis(string drive)
        {
        }
    }
}