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
		#region Common
		public StringWatcher Level { get; }
		public MemoryWatcher<bool> IsLoading { get; }
		public MemoryWatcher<bool> Death { get; }
		#endregion Common

		#region Training
		#endregion Training

		#region Husky
        //public MemoryWatcher<bool> HillHousesAADestoryed { get; }
        //public MemoryWatcher<bool> TownHallNorthAADestoryed { get; }
        //public MemoryWatcher<bool> TownHallSouthAADestoryed { get; }
        //public MemoryWatcher<bool> NortheastGateAADestoryed { get; }
        public MemoryWatcher<bool> HuskyLowerOfficerKilled { get; }
		public MemoryWatcher<bool> HuskyMidOfficerKilled { get; }
		public MemoryWatcher<bool> HuskyUpperOfficerKilled { get; }
		#endregion Husky

		#region Avalanche
		#endregion Avalanche

		#region Neptune
		#endregion Neptune

		#region Market Garden
		#endregion Market Garden

		#region Varsity
		#endregion Varsity

		#region Der Flakturm
		public MemoryWatcher<bool> HellboxActivation { get; }
		//public MemoryWatcher<bool> EndCutscene { get; }
		#endregion Der Flakturm

		public GameData()
		{
			this.Level = new StringWatcher(new DeepPointer(0x3A4FB8, 0x4), 16);
			this.IsLoading = new MemoryWatcher<bool>(new DeepPointer(0xC180, 0x4));
			this.Death = new MemoryWatcher<bool>(new DeepPointer(0xDDE144, 0x10, 0x1CC));

			this.HuskyLowerOfficerKilled = new MemoryWatcher<bool>(new DeepPointer(0xE0F07C, 0x8, 0x228, 0xC4, 0x80, 0xC4, 0x38, 0x90));
			this.HuskyMidOfficerKilled = new MemoryWatcher<bool>(new DeepPointer(0xE0F07C, 0x8, 0x228, 0xC4, 0x78, 0xC4, 0x38, 0x90));
			this.HuskyUpperOfficerKilled = new MemoryWatcher<bool>(new DeepPointer(0xE0F07C, 0x8, 0x228, 0xC4, 0x70, 0xC4, 0x38, 0x90));

			this.HellboxActivation = new MemoryWatcher<bool>(new DeepPointer(0xDF7060, 0x8, 0xC4, 0xA4, 0x1C8, 0x0, 0x38, 0x2CC));
			//this.EndCutscene = new MemoryWatcher<bool>(new DeepPointer(0xE16EAC, 0x8));

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

		#region Husky
		public event Action OnHuskyLowerOfficerKilled;
		public event Action OnHuskyMidOfficerKilled;
		public event Action OnHuskyUpperOfficerKilled;
		#endregion Husky

		//public event Action OnPlayerLostControl;
		public event Action OnPlayerDeath;
		public event Action OnFinalInput;

		private List<int> ignorePIDs;

		private GameData data;
		private Process process;

		private bool firstLoad;
		
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

			var oldLevel = data.Level.Old;
			var currentLevel = data.Level.Current;

			if (data.Level.Changed)
			{
				if (Level.Advanced(oldLevel, currentLevel))
				{
					if (currentLevel == Level.Training)
						this.OnFirstLevelLoading?.Invoke();
					if (Level.IsBriefing(currentLevel))
						this.OnLevelChanged?.Invoke();
					else if (Level.IsPlayable(currentLevel))
						firstLoad = true;
				}
			}

			if (data.IsLoading.Changed)
			{
				if (data.IsLoading.Current)
					this.OnLoadStarted?.Invoke();
				else
				{
					this.OnLoadFinished?.Invoke();
					if (currentLevel == Level.Training)
						Task.Delay(2000).ContinueWith(_ => this.OnFadeIn?.Invoke());
					else if (Level.IsPlayable(currentLevel) && firstLoad)
					{
						this.OnActualLevelStart?.Invoke();
						firstLoad = false;
					}
				}
			}

			// Husky
			if (data.Level.Current == Level.Husky)
			{
				if (data.HuskyLowerOfficerKilled.Changed && data.HuskyLowerOfficerKilled.Current)
					this.OnHuskyLowerOfficerKilled?.Invoke();
				else if (data.HuskyMidOfficerKilled.Changed && data.HuskyMidOfficerKilled.Current)
					this.OnHuskyMidOfficerKilled?.Invoke();
				else if (data.HuskyUpperOfficerKilled.Changed && data.HuskyUpperOfficerKilled.Current)
					this.OnHuskyUpperOfficerKilled?.Invoke();
			}

			if (data.Level.Current == Level.DerFlakturm)
			{
				if (data.HellboxActivation.Changed && data.HellboxActivation.Current)
					this.OnFinalInput?.Invoke();
			}

			//if (data.EndCutscene.Changed && data.EndCutscene.Current)
			//{
			//	this.OnPlayerLostControl?.Invoke();
			//}

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
