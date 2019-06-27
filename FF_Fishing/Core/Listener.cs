using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace FF_Fishing.Core
{
    internal class Listener
    {
        private MMDevice _sndDevice;
        public LimitedCollection<float> VolumeQueue { get; }
        public int TickRate { get; }

        private const int MaxVolumeQueueLength = 40;

        public Listener(int tickRate = 50)
        {
            TickRate = tickRate;
            VolumeQueue = new LimitedCollection<float>(MaxVolumeQueueLength) { 0 };
        }

        public async Task<bool> Listen(int millisecondsToListen, double soundLimitMultiplier, CancellationToken cancellationToken)
        {
            VolumeQueue.Clear();
            var stopwatch = new Stopwatch();
            var sndDevEnum = new MMDeviceEnumerator();
            _sndDevice = sndDevEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds <= millisecondsToListen)
            {
                await Task.Delay(TickRate, cancellationToken);
                if (SpikeDetected(soundLimitMultiplier))
                {
                    return true;
                }
            }
            return false;
        }

        private bool SpikeDetected(double soundLimitMultiplier)
        {
            var hear = false;
            var level = _sndDevice.AudioMeterInformation.MasterPeakValue * 100;
            if(VolumeQueue.Count() == MaxVolumeQueueLength && level > 1)
            {
                var avgVol = GetAverageVolume();
                hear = level / avgVol >= soundLimitMultiplier;
            }

            // Keep a running queue of the last X sounds as a reference point
            VolumeQueue.Add(level);
            return hear;
        }

        internal float GetAverageVolume()
        {
            return VolumeQueue.Average();
        }
    }
}
