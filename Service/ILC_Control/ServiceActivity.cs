using System;

namespace ILC_ControlPanel
{
    class ServiceActivity
    {
        private bool isSucceeded;
        public bool IsSucceeded
        {
            get { return isSucceeded; }
            set { isSucceeded = value; }
        }
        private bool workingState;
        public bool WorkingState
        {
            get { return workingState; }
            set { workingState = value; }
        }
        private DateTime lastCheck;
        public DateTime LastCheck
        {
            get { return lastCheck; }
            set { lastCheck = value; }
        }
        private int totalProgress;
        public int TotalProgress
        {
            get { return totalProgress; }
            set { totalProgress = value; }
        }
        private int passedProgress;
        public int PassedProgress
        {
            get { return passedProgress; }
            set { passedProgress = value; }
        }
    }
}
