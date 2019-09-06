using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using WindowsInput;
using FF_Fishing.Annotations;
using FF_Fishing.Core;
using FF_Fishing.Core.Helpers;
using FF_Fishing.Core.Settings;

namespace FF_Fishing.Controller
{
    public class MainFishingLogic : INotifyPropertyChanged, IDisposable
    {
        private float _averageSoundLevel;
        private float _lastDetectedSoundLevel;
        private readonly Listener _soundListener;
        private readonly CancellationTokenSource _ctsUpdater;
        private CancellationTokenSource _ctsMain;
        private bool _running;
        private readonly InputSimulator _inputSimulator;
        private int _cycles;
        private readonly FishingSettings _settings;
        private readonly Dispatcher _dispatcher;
        private ObservableCollection<FishLogEntry> _fishLog;

        public MainFishingLogic()
        {
            _soundListener = new Listener();
            _ctsUpdater = new CancellationTokenSource();
            StartBackgroundTasks();
            _inputSimulator = new InputSimulator();

        }

        public MainFishingLogic(FishingSettings settings, Dispatcher dispatcher) : this()
        {
            _settings = settings;
            _dispatcher = dispatcher;
        }

        public bool WindowTopMost
        {
            get => _settings.WindowTopMost;
            set
            {
                _settings.WindowTopMost = value;
                OnPropertyChanged();
            }
        }


        #region PropChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public event EventHandler NewLogEntry;

        public ObservableCollection<FishLogEntry> FishLog
        {
            get => _fishLog;
            set
            {
                if (Equals(value, _fishLog)) return;
                _fishLog = value;
                OnPropertyChanged();
            }
        }

        public float AverageSoundLevel
        {
            get => _averageSoundLevel;
            private set
            {
                if (Math.Abs(value - _averageSoundLevel) < 0.0001) return;
                _averageSoundLevel = value;
                OnPropertyChanged();
            }
        }

        public int Cycles
        {
            get => _cycles;
            private set
            {
                if (value == _cycles) return;
                _cycles = value;
                OnPropertyChanged();
            }
        }

        public bool Running
        {
            get => _running;
            private set
            {
                if (value == _running) return;
                _running = value;
                OnPropertyChanged();
            }
        }

        public float LastDetectedSoundLevel
        {
            get => _lastDetectedSoundLevel;
            private set
            {
                if (Math.Abs(value - _lastDetectedSoundLevel) < 0.0001) return;
                _lastDetectedSoundLevel = value;
                OnPropertyChanged();
            }
        }

        private void StartBackgroundTasks()
        {
           Task.Run(() => UpdateSoundLevels(_ctsUpdater.Token));
        }

        private async Task UpdateSoundLevels(CancellationToken ctx)
        {
            try
            {

                while (true)
                {
                    try
                    {
                        LastDetectedSoundLevel = _soundListener.VolumeQueue.Latest;
                        AverageSoundLevel = _soundListener.GetAverageVolume();
                        await Task.Delay(10, ctx);
                    }
                    catch (InvalidOperationException)
                    {
                        //ignore
                    }
                }
            }
            catch (TaskCanceledException)
            {
                //ignore
            }
        }

        private void AddLogMessage(TimeSpan elapsed, string message)
        {
            _dispatcher.Invoke(() =>
            {
                FishLog.Add(new FishLogEntry {Message = message, Stamp = elapsed});
                OnNewLogEntry();
            });

        }

        public void Dispose()
        {
            _ctsUpdater.Cancel();
        }

        public void StartFishing()
        {
            if (Running)
            {
                return;
            }
            Running = true;
            FishLog = new ObservableCollection<FishLogEntry>();
            _ctsMain = new CancellationTokenSource();
            Task.Run(() => MainLogic(_ctsMain.Token));
        }

        public async Task MainLogic(CancellationToken ctx)
        {
            var watch = Stopwatch.StartNew();
            AddLogMessage(watch.Elapsed, "Starting fishing session...");
            try
            {
                while (true)
                {
                    await Task.Delay(_settings.TimersSettings.BeforeCast, ctx);
                    AddLogMessage(watch.Elapsed, "Casting rod");
                    _inputSimulator.Keyboard.KeyPress(_settings.BindingSettings.CastKey);

                    await Task.Delay(_settings.TimersSettings.AfterCast, ctx);

                    AddLogMessage(watch.Elapsed, "Listening for a catch...");
                    var fishHeard = await _soundListener.Listen(_settings.TimersSettings.HookWait, _settings.BiteSoundMultiplier, ctx);

                    if (fishHeard)
                    {
                        await Task.Delay(50, ctx); // waiting sound levels info to update

                        AddLogMessage(watch.Elapsed, $"I think I've heard something! (Volume: {LastDetectedSoundLevel:N2}, C: {(LastDetectedSoundLevel / AverageSoundLevel):N2})");
                        _inputSimulator.Keyboard.KeyPress(_settings.BindingSettings.HookKey);

                        AddLogMessage(watch.Elapsed, "Waiting for luring procedure to finish...");
                        await Task.Delay(_settings.UseLure ? _settings.TimersSettings.TackleHook : _settings.TimersSettings.BaitHook, ctx);

                        await Task.Delay(_settings.TimersSettings.Gather, ctx);

                        if (_settings.Mooch)
                        {
                            await Task.Delay(_settings.TimersSettings.Gather, ctx);
                            AddLogMessage(watch.Elapsed, "Trying to mooch a fish...");
                            _inputSimulator.Keyboard.KeyPress(_settings.BindingSettings.MoochKey);
                        }
                        else if (_settings.ReleaseFish)
                        {
                            AddLogMessage(watch.Elapsed, "Trying to release a fish...");
                            _inputSimulator.Keyboard.KeyPress(_settings.BindingSettings.ReleaseKey);
                        }

                        if (_settings.UsePatience && FishingSkillsInfo.TryUseSkill(FishingSkill.Patience))
                        {
                            AddLogMessage(watch.Elapsed, "Trying to cast Patience...");
                            _inputSimulator.Keyboard.KeyPress(_settings.BindingSettings.PatienceKey);
                            await Task.Delay(1500, ctx);
                        }

                        AddLogMessage(watch.Elapsed, "Finishing up an attempt...");
                    }
                    else
                    {
                        AddLogMessage(watch.Elapsed, "Failed to detect anything, re-initializing fishing!");
                        _inputSimulator.Keyboard.KeyPress(_settings.BindingSettings.QuitKey);
                        await Task.Delay(_settings.TimersSettings.FailRetry, ctx);
                    }

                    await Task.Delay(_settings.TimersSettings.Finish, ctx);
                    Cycles++;
                }
            }
            catch (TaskCanceledException)
            {
                watch.Stop();
                //ignore
            }

            watch.Stop();
        }

        public void StopFishing()
        {
            Cycles = 0;
            Running = false;
            _ctsMain.Cancel();
        }

        protected virtual void OnNewLogEntry()
        {
            NewLogEntry?.Invoke(this, EventArgs.Empty);
        }
    }
}