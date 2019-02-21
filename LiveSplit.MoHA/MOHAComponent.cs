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
			get { return state.Layout.Components.FirstOrDefault(c => c.GetType() == typeof(MoHAUIComponent)) as MoHAUIComponent; }
		}

        private TimerModel timer;
		private LiveSplitState state;
        private GameMemory gameMemory;
        private Timer updateTimer;

        public MOHAComponent(LiveSplitState state)
        {
#if DEBUG
            Debug.Listeners.Clear();
            Debug.Listeners.Add(TimedTraceListener.Instance);
#endif
			this.state = state;

			this.Settings = new MoHASettings();

            timer = new TimerModel { CurrentState = this.state };
            timer.CurrentState.OnStart += timer_OnStart;

            updateTimer = new Timer() { Interval = 15, Enabled = true };
            updateTimer.Tick += updateTimer_Tick;

            gameMemory = new GameMemory();
            gameMemory.OnFirstLevelLoading += gameMemory_OnFirstLevelLoading;
            gameMemory.OnFadeIn += gameMemory_OnFadeIn;
            gameMemory.OnLoadStarted += gameMemory_OnLoadStarted;
            gameMemory.OnLoadFinished += gameMemory_OnLoadFinished;
			gameMemory.OnPlayerLostControl += gameMemory_OnLevelCompleted;
			gameMemory.OnLevelChanged += gameMemory_OnLevelCompleted;
			gameMemory.OnActualLevelStart += gameMemory_OnActualLevelStart;
			gameMemory.OnPlayerDeath += gameMemory_OnPlayerDeath;
        }

		public override void Dispose()
        {
            timer.CurrentState.OnStart -= timer_OnStart;
            updateTimer?.Dispose();
        }

        void updateTimer_Tick(object sender, EventArgs eventArgs)
        {
            try
            {
                gameMemory.Update();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        void timer_OnStart(object sender, EventArgs e)
        {
            timer.InitializeGameTime();
			timer.CurrentState.CurrentTimingMethod = TimingMethod.GameTime;
		}

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {

        }

        void gameMemory_OnFirstLevelLoading()
        {
            timer.Reset();
        }

        void gameMemory_OnFadeIn()
        {
            timer.Start();
        }

        void gameMemory_OnLoadStarted()
        {
            timer.CurrentState.IsGameTimePaused = true;
        }

        void gameMemory_OnLoadFinished()
        {
            timer.CurrentState.IsGameTimePaused = false;
        }

		void gameMemory_OnActualLevelStart()
		{
			if (this.Settings.AutoSplitBriefings)
				timer.Split();
			else
				timer.Start();
		}

		void gameMemory_OnLevelCompleted()
		{
			timer.Split();
		}

		private void gameMemory_OnPlayerDeath()
		{
			if (timer.CurrentState.CurrentPhase == TimerPhase.Running)
				UI?.AddDeath();
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
