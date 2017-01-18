using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using LiveSplit.ComponentUtil;

namespace LiveSplit.MoHA
{
    class GameData : MemoryWatcherList
    {
        public MemoryWatcher<bool> IsLoading { get; }
		public StringWatcher CurrentLevel { get; }
		//public MemoryWatcher<int> EndTrigger { get; }
		public MemoryWatcher<bool> EndCutscene { get; }

		public GameData()
        {
			this.CurrentLevel = new StringWatcher(new DeepPointer(0x3A4FB8, 0x4), 16);
			this.EndCutscene = new MemoryWatcher<bool>(new DeepPointer(0xE16EAC, 0x8));
            this.IsLoading = new MemoryWatcher<bool>(new DeepPointer(0xC180, 0x4));
			//this.EndTrigger = new MemoryWatcher<int>(new DeepPointer(0xDAE19C, 0x10, 0x4, 0x7F0));

			this.AddRange(this.GetType().GetProperties()
                .Where(p => !p.GetIndexParameters().Any())
                .Select(p => p.GetValue(this, null) as MemoryWatcher)
                .Where(p => p != null));
        }
    }

    class GameMemory
    {
		public event EventHandler OnFirstLevelLoaded;
		public event EventHandler OnLevelChanged;
		public event EventHandler OnActualLevelStart;
        public event EventHandler OnLoadStarted;
        public event EventHandler OnLoadFinished;
        public event EventHandler OnFadeIn;
        //public event EventHandler OnLastTrigger;
		public event EventHandler OnPlayerLostControl;

        private List<int> _ignorePIDs;

        private GameData _data;
        private Process _process;
        private bool _loadingStarted;
		private DateTime _runStart;
		private bool _delayedStart;


        public GameMemory()
        {
            _ignorePIDs = new List<int>();
        }

        public void Update()
        {
            if (_process == null || _process.HasExited)
            {
                if (!this.TryGetGameProcess())
                    return;
			}

			TimedTraceListener.Instance.UpdateCount++;

            _data.UpdateAll(_process);

			if (_data.CurrentLevel.Changed)
            {
				if (_data.CurrentLevel.Current == "Tra_Jmp_P")
					this.OnFirstLevelLoaded?.Invoke(this, EventArgs.Empty);
				else if (_data.CurrentLevel.Current.Contains("Briefing"))
					this.OnLevelChanged?.Invoke(this, EventArgs.Empty);
				else if (_data.CurrentLevel.Current.Contains("_P") && !_data.CurrentLevel.Current.Contains("MP_"))
					this.OnActualLevelStart?.Invoke(this, EventArgs.Empty);
            }

            if (_data.IsLoading.Changed)
            {
				if (_data.IsLoading.Current)
				{
					_loadingStarted = true;
					this.OnLoadStarted?.Invoke(this, EventArgs.Empty);
				}
				else
				{
					if (_loadingStarted)
					{
						_loadingStarted = false;
						this.OnLoadFinished?.Invoke(this, EventArgs.Empty);
					}

					if (_data.CurrentLevel.Current == "Tra_Jmp_P")
					{
						_runStart = DateTime.Now;
						_delayedStart = true;
					}
				}
            }

			if (_data.EndCutscene.Changed)
			{
				if (_data.EndCutscene.Current)
					this.OnPlayerLostControl?.Invoke(this, EventArgs.Empty);
			}

			//if (_data.EndTrigger.Changed && _data.CurrentLevel.Current.Contains("Flk"))
			//{
			//	if (_data.EndTrigger.Current == 2)
			//		this.OnLastTrigger?.Invoke(this, EventArgs.Empty);
			//}

			if (_delayedStart && DateTime.Now.Subtract(_runStart) >= TimeSpan.FromSeconds(2))
			{
				_delayedStart = false;
				this.OnFadeIn?.Invoke(this, EventArgs.Empty);
			}
        }

        bool TryGetGameProcess()
        {
            Process game = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.ToLower() == "moha"
                && !p.HasExited && !_ignorePIDs.Contains(p.Id));
            if (game == null)
                return false;

            _data = new GameData();
            _process = game;

            return true;
        }
    }

    class FakeMemoryWatcher<T>
    {
        public T Current { get; set; }
        public T Old { get; set; }

        public FakeMemoryWatcher(T old, T current)
        {
            this.Old = old;
            this.Current = current;
        }
    }
}
