using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.UI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Windows.Forms;
using LiveSplit.MoHA.UI;

namespace LiveSplit.MoHA
{
    class MOHAComponent : LogicComponent
    {
        public override string ComponentName => "MoHA";

		public MoHASettings Settings { get; set; }

		private MoHAUIComponent UI
		{
			get { return _state.Layout.Components.FirstOrDefault(c => c.GetType() == typeof(MoHAUIComponent)) as MoHAUIComponent; }
		}

        private TimerModel _timer;
		private LiveSplitState _state;
        private GameMemory _gameMemory;
        private Timer _updateTimer;

        public MOHAComponent(LiveSplitState state)
        {
#if DEBUG
            Debug.Listeners.Clear();
            Debug.Listeners.Add(TimedTraceListener.Instance);
#endif
			_state = state;

			this.Settings = new MoHASettings();

            _timer = new TimerModel { CurrentState = state };
            _timer.CurrentState.OnStart += timer_OnStart;

            _updateTimer = new Timer() { Interval = 15, Enabled = true };
            _updateTimer.Tick += updateTimer_Tick;

            _gameMemory = new GameMemory();
            _gameMemory.OnFirstLevelLoaded += gameMemory_OnFirstLevelLoading;
            _gameMemory.OnFadeIn += gameMemory_OnFadeIn;
            _gameMemory.OnLoadStarted += gameMemory_OnLoadStarted;
            _gameMemory.OnLoadFinished += gameMemory_OnLoadFinished;
			//_gameMemory.OnLastTrigger += gameMemory_OnLastTrigger;
			_gameMemory.OnPlayerLostControl += gameMemory_OnLevelCompleted;
			_gameMemory.OnLevelChanged += gameMemory_OnLevelCompleted;
			_gameMemory.OnActualLevelStart += gameMemory_OnActualLevelStart;
			_gameMemory.OnPlayerDeath += gameMemory_OnPlayerDeath;
        }

        public override void Dispose()
        {
            _timer.CurrentState.OnStart -= timer_OnStart;
            _updateTimer?.Dispose();
        }

        void updateTimer_Tick(object sender, EventArgs eventArgs)
        {
            try
            {
                _gameMemory.Update();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        void timer_OnStart(object sender, EventArgs e)
        {
            _timer.InitializeGameTime();
			_timer.CurrentState.CurrentTimingMethod = TimingMethod.GameTime;
		}

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {

        }

        void gameMemory_OnFirstLevelLoading(object sender, EventArgs e)
        {
            _timer.Reset();
        }

        void gameMemory_OnFadeIn(object sender, EventArgs e)
        {
            _timer.Start();
        }

        void gameMemory_OnLoadStarted(object sender, EventArgs e)
        {
            _timer.CurrentState.IsGameTimePaused = true;
        }

        void gameMemory_OnLoadFinished(object sender, EventArgs e)
        {
            _timer.CurrentState.IsGameTimePaused = false;
        }

        //void gameMemory_OnLastTrigger(object sender, EventArgs e)
        //{
		//	_timer.Split();
        //}

        void gameMemory_OnLevelCompleted(object sender, EventArgs e)
        {
			_timer.Split();
        }

		void gameMemory_OnActualLevelStart(object sender, EventArgs e)
		{
			if (this.Settings.AutoSplitBriefings)
				_timer.Split();
			else
				_timer.Start();
		}

		void gameMemory_OnPlayerDeath(object sender, EventArgs e)
		{
			if (_state.CurrentPhase != TimerPhase.NotRunning && _state.CurrentPhase != TimerPhase.Ended)
			{
				if (this.UI != null)
					this.UI.AddDeath();
			}
		}

        public override XmlNode GetSettings(XmlDocument document)
        {
			return this.Settings.GetSettings(document);
        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
			return this.Settings;
        }

        public override void SetSettings(XmlNode settings)
        {
           this.Settings.SetSettings(settings);
        }
    }

    public class TimedTraceListener : DefaultTraceListener
    {
        private static TimedTraceListener _instance;
        public static TimedTraceListener Instance => _instance ?? (_instance = new TimedTraceListener());

        private TimedTraceListener() { }

        public int UpdateCount
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public override void WriteLine(string message)
        {
            base.WriteLine("MoHA: " + this.UpdateCount + " " + message);
        }
    }
}
