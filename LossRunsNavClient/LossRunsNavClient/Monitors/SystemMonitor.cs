using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LossRunsNavClient.Monitors
{
    class SystemMonitor
    {
        const int minTimeout = 1000;
        const float minThreshold = 25.0F;
        const int minSampleSz = 20;

        private float sysCpuThreshold;
        private float sysMemThreshold;
        private float procCpuThreshold;
        private float procMemThreshold;

        private List<float> sysCpuList;
        private List<float> procCpuList;
        private List<float> sysMemList;
        private List<float> procMemList;

        private int sampleSize;
        private float? sysCpuAverage;
        private float? sysMemAverage;
        private float? procCpuAverage;
        private float? procMemAverage;

        private int GlobalTimeout;


        // System counters
        private PerformanceCounter cpuSysCounter =
            new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private PerformanceCounter memSysCounter =
            new PerformanceCounter("Memory", "Available MBytes");

        // Process counters
        private PerformanceCounter cpuProcCounter =
            new PerformanceCounter("Process", "% Processor Time",
                Process.GetCurrentProcess().ProcessName);
        private PerformanceCounter memProcCounter =
            new PerformanceCounter("Process", "Working Set",
                Process.GetCurrentProcess().ProcessName);

        // Constructor
        public SystemMonitor()
        {
            sysCpuList = new List<float>();
            procCpuList = new List<float>();
            sysMemList = new List<float>();
            procMemList = new List<float>();
        }

        public SystemMonitor(int sampleSz, float sysCpuThresh, float procCpuThresh) 
            : this()
        {
            sampleSize = sampleSz;
            sysCpuThreshold = sysCpuThresh;
            procCpuThreshold = procCpuThresh;
        }

        public SystemMonitor(int sampleSz, float sysCpuThresh, float procCpuThresh, float sysMemThresh, float procMemThresh, int timeout) 
            : this(sampleSz, sysCpuThresh, procCpuThresh)
        {
            sysMemThreshold = sysMemThresh;
            procMemThreshold = procMemThresh;
            GlobalTimeout = timeout;
        }

        // System accessors
        public float GetSysCpuUsage()
        {
            return cpuSysCounter.NextValue();
        }

        public float GetAvgSysCpuUsage()
        {
            float avg;
            if (sysCpuAverage != null)
                avg = (float)sysCpuAverage;
            else
                avg = 0.0F;
            return avg;
        }

        public float GetSysMemUsage()
        {
            return memSysCounter.NextValue();
        }

        // Process accessors
        public float GetProcCpuUsage()
        {
            return cpuProcCounter.NextValue();
        }

        public float GetAvgProcCpuUsage()
        {
            float avg;
            if (procCpuAverage != null)
                 avg = (float)procCpuAverage;
            else
                avg = 0.0F;
            return avg;
        }

        public float GetProcMemUsage()
        {
            return memProcCounter.NextValue();
        }

        // System mutators - Sets minimum available resources to throw event
        public void SetCpuMinThreshold(float t)
        {
            sysCpuThreshold = t;
        }

        public void SetMemMinThreshold(float t)
        {
            sysMemThreshold = t;
        }

        public void SetSampleSize(int s)
        {
            sampleSize = s;
        }

        // Process mutators - Sets maximum amount resources to use
        public void SetCpuMaxThreshold(float t)
        {
            procCpuThreshold = t;
        }

        public void SetMemMaxThreshold(float t)
        {
            procMemThreshold = t;
        }

        // Monitors
        public async Task<bool> CheckCpuUsage()
        {
            bool isOk = false;
            bool isSysOk = false;
            bool isProcOk = false;

            bool isSysReady = await FindAverage(true);
            bool isProcReady = await FindAverage(false);
            Debug.WriteLine("CPU sample ready");

            if (sysCpuThreshold < minThreshold)
                sysCpuThreshold = minThreshold;
            if (procCpuThreshold < minThreshold)
                procCpuThreshold = minThreshold;

            if (isSysReady)
            {
                sysCpuAverage = sysCpuList.Sum() / sysCpuList.Count();
                var remainingAvgCpu = 100 - sysCpuAverage;
                var remainingThreshCpu = 100 - sysCpuThreshold;
                if (remainingAvgCpu > remainingThreshCpu)
                {
                    isSysOk = true;
                }
                sysCpuList.Clear();
            }

            if (isProcReady)
            {
                procCpuAverage = procCpuList.Sum() / procCpuList.Count();
                if (procCpuAverage < procCpuThreshold)
                {
                    isProcOk = true;
                }
                procCpuList.Clear();
            }

            if (isSysOk && isProcOk)
                isOk = true;

            Debug.WriteLine("CPU sample " + isOk.ToString());
            return isOk;
        }

        public async Task<bool> CheckMemUsage()
        {
            bool isOk = false;
            bool isSysOk = false;
            bool isProcOk = false;

            sysMemList.Add(GetSysMemUsage());
            procMemList.Add(GetProcMemUsage());

            if (sysMemThreshold < minThreshold)
                sysMemThreshold = minThreshold;
            if (procMemThreshold < minThreshold)
                procMemThreshold = minThreshold;

            if (await isMemSampleSizeMet(true))
            {
                sysMemAverage = sysMemList.Sum() / sysMemList.Count();
                var remainingAvgMem = 100 - sysMemAverage;
                var remainingThreshMem = 100 - sysMemThreshold;
                if (remainingAvgMem > remainingThreshMem)
                {
                    isSysOk = true;
                }
                sysMemList.Clear();
            }

            if (await isMemSampleSizeMet(false))
            {
                procMemAverage = procMemList.Sum() / procMemList.Count();
                if (procMemAverage < procMemThreshold)
                {
                    isProcOk = true;
                }
                procMemList.Clear();
            }

            if (isSysOk && isProcOk)
                isOk = true;

            return isOk;
        }

        private async Task<bool> isSampleSizeMet(bool isSys)
        {
            if (GlobalTimeout > minTimeout)
                await Task.Delay(GlobalTimeout * 10);
            else
                await Task.Delay(minTimeout * 10);

            if (sampleSize < minSampleSz)
                sampleSize = minSampleSz;

            if (isSys)
            {
                if (sysCpuList.Count() > sampleSize)
                    return true;
                else
                    return false;
            }
            else
            {
                if (procCpuList.Count() > sampleSize)
                    return true;
                else
                    return false;
            }
        }

        private async Task<bool> isMemSampleSizeMet(bool isSys)
        {
            if (GlobalTimeout > minTimeout)
                await Task.Delay(GlobalTimeout * 10);
            else
                await Task.Delay(minTimeout * 10);

            if (sampleSize < minSampleSz)
                sampleSize = minSampleSz;

            if (isSys)
            {
                if (sysMemList.Count() > sampleSize)
                    return true;
                else
                    return false;
            }
            else
            {
                if (procMemList.Count() > sampleSize)
                    return true;
                else
                    return false;
            }
        }

        private async Task<bool> FindAverage(bool isSys)
        {
            if (isSys)
            {
                bool isReady = false;
                while (!isReady)
                {
                    sysCpuList.Add(GetSysMemUsage());
                    sysMemList.Add(GetSysMemUsage());
                    isReady = await isSampleSizeMet(true);
                }
                return true;
            }
            else
            {
                bool isReady = false;
                while (!isReady)
                {
                    procCpuList.Add(GetProcMemUsage());
                    procMemList.Add(GetProcMemUsage());
                    isReady = await isSampleSizeMet(false);
                }
                return true;
            }
        }

        // Events
        protected virtual void OnThesholdReached(ThresholdReachedEventArgs e)
        {
            EventHandler<ThresholdReachedEventArgs> handler = ThresholdReached;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<ThresholdReachedEventArgs> ThresholdReached;

    }

    public class ThresholdReachedEventArgs : EventArgs
    {
        public bool isCpu { get; set; }
        public bool isMem { get; set; }
        public float SysThresholdRemain { get; set; }
        public float ProcThresholdRemain { get; set; }
    }
}
