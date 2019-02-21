using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using LiveSplit.ComponentUtil;
using System.Threading.Tasks;

namespace LiveSplit.MoHA
{
    class GameData : MemoryWatcherList
    {
        public MemoryWatcher<bool> IsLoading { get; }
		public StringWatcher Level { get; }
		public MemoryWatcher<bool> EndCutscene { get; }
		public MemoryWatcher<bool> Death { get; }

		public GameData()
        {
			this.Level = new StringWatcher(new DeepPointer(0x3A4FB8, 0x4), 16);
			this.EndCutscene = new MemoryWatcher<bool>(new DeepPointer(0xE16EAC, 0x8));
            this.IsLoading = new MemoryWatcher<bool>(new DeepPointer(0xC180, 0x4));
			this.Death = new MemoryWatcher<bool>(new DeepPointer(0xDDE144, 0x10, 0x1CC));


			this.AddRange(this.GetType().GetProperties()
                .Where(p => !p.GetIndexParameters().Any())
                .Select(p => p.GetValue(this, null) as MemoryWatcher)
                .Where(p => p != null));
        }
    }

    class GameMemory
    {
		public event Action OnFirstLevelLoading;
		public event Action OnLevelChanged;
		public event Action OnActualLevelStart;
        public event Action OnLoadStarted;
        public event Action OnLoadFinished;
        public event Action OnFadeIn;
		public event Action OnPlayerLostControl;
		public event Action OnPlayerDeath;

		private List<int> ignorePIDs;

        private GameData data;
        private Process process;

		private bool freshLoad;
		
        public GameMemory()
        {
            ignorePIDs = new List<int>();
        }

        public void Update()
        {
            if (process == null || process.HasExited)
            {
                if (!this.TryGetGameProcess())
                    return;
			}

			TimedTraceListener.Instance.UpdateCount++;

            data.UpdateAll(process);

			if (data.Level.Changed)
            {
				if (data.Level.Current == "Tra_Jmp_P")
					this.OnFirstLevelLoading?.Invoke();
				else if (data.Level.Current.Contains("Briefing"))
					this.OnLevelChanged?.Invoke();
				else if (data.Level.Current.Contains("_P") && !data.Level.Current.Contains("MP_") && data.Level.Old != "shell")
					freshLoad = true;
            }

            if (data.IsLoading.Changed)
            {
				if (data.IsLoading.Current)
				{
					this.OnLoadStarted?.Invoke();
				}
				else
				{
					this.OnLoadFinished?.Invoke();
					if (data.Level.Current == "Tra_Jmp_P")
						Task.Delay(2000).ContinueWith(_ => this.OnFadeIn?.Invoke());
					else if (data.Level.Current.Contains("_P") && !data.Level.Current.Contains("MP_") && freshLoad)
					{
						this.OnActualLevelStart?.Invoke();
						freshLoad = false;
					}
				}
            }

			if (data.EndCutscene.Changed && data.EndCutscene.Current)
			{
				this.OnPlayerLostControl?.Invoke();
			}

			if (data.Death.Changed && data.Death.Current)
			{
				this.OnPlayerDeath?.Invoke();
			}
        }

        bool TryGetGameProcess()
        {
            Process game = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.ToLower() == "moha"
                && !p.HasExited && !ignorePIDs.Contains(p.Id));
            if (game == null)
                return false;

            data = new GameData();
            process = game;

            return true;
        }
    }
}
