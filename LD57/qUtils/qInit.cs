using System.Diagnostics;

namespace LD57.qUtils
{
    public class qInit : SyncScript
    {
        public qInstance Instance { get; private set; }

        public string projectName;
        public string version;

        public bool useConsole = true;

        public override void Start()
        {
            Instance = new qInstance(new RemoteAppInfo()
            {
                projectName = projectName,
                version = version,
                engine = "Stride",
                engineVersion = FileVersionInfo.GetVersionInfo("Stride.dll").FileVersion,
            })
            {
                useNetworkDiscovery = false,
            };

            Services.AddService(Instance);

            Instance.Services.Add(Game as Game);

            Instance.RemoteInspectorServer.StopUpdateLog();

            Instance.Start();
        }

        public override void Update()
        {
            Instance.RemoteInspectorServer.Update();
        }

        public override void Cancel()
        {
            Instance.Stop();
        }
    }
}
