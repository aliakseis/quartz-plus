using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;

namespace LineCheckerSrv
{
    /// <summary>
    /// Misfire listener
    /// </summary>
    public class MisfiredListener : ITriggerListener
    {
        #region ITriggerListener Members

        /// <summary>
        /// Get the name of the Quartz.ITriggerListener.
        /// </summary>
        public string Name
        {
            get { return "MisfiredListener"; }
        }

        /// <summary>
        /// Called by the Quartz.IScheduler when a Quartz.Trigger has fired, it's associated
        /// Quartz.JobDetail has been executed, and it's Quartz.Trigger.Triggered(Quartz.ICalendar)
        /// method has been called.
        /// </summary>
        /// <param name="trigger">The Quartz.Trigger that was fired</param>
        /// <param name="context">The Quartz.JobExecutionContext that was passed to the 
        /// Quartz.IJob'sQuartz.IJob.Execute(Quartz.JobExecutionContext) method</param>
        /// <param name="triggerInstructionCode">The result of the call on the 
        /// Quartz.Trigger'sQuartz.Trigger.Triggered(Quartz.ICalendar) method</param>
        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context,
                                    SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Called by the Quartz.IScheduler when a Quartz.Trigger has fired, and it's 
        /// associated Quartz.JobDetail is about to be executed.   It is called before 
        /// the Quartz.ITriggerListener.VetoJobExecution(Quartz.Trigger,Quartz.JobExecutionContext) 
        /// method of this interface.
        /// </summary>
        /// <param name="trigger">The Quartz.Trigger that has fired</param>
        /// <param name="context">The Quartz.JobExecutionContext that will be passed to the 
        /// Quartz.IJob'sQuartz.IJob.Execute(Quartz.JobExecutionContext)method</param>
        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Called by the Quartz.IScheduler when a Quartz.Trigger has misfired.   Consideration
        /// should be given to how much time is spent in this method, as it will affect
        /// all triggers that are misfiring.  If you have lots of triggers misfiring
        /// at once, it could be an issue it this method does a lot.
        /// </summary>
        /// <param name="trigger">The Quartz.Trigger that has misfired</param>
        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default(CancellationToken))
        {
            ILog log = LogManager.GetLogger(AppSettings.GetCommonLoggerName());
            DateTimeOffset? fireTime = trigger.GetNextFireTimeUtc();
            log.Info("Trigger " + trigger.Key.Name + " was misfired; scheduled time: " + 
                (fireTime != null? ((DateTimeOffset)fireTime).ToString() : "null"));
            trigger.JobDataMap[IVRSLineChecker.misfireNameTemplate] = true;

            return Task.FromResult(true);
        }

        /// <summary>
        /// Called by the Quartz.IScheduler when a Quartz.Trigger has fired, and it's
        /// associated Quartz.JobDetail is about to be executed.   It is called after
        /// the Quartz.ITriggerListener.TriggerFired(Quartz.Trigger,Quartz.JobExecutionContext)
        /// method of this interface.
        /// </summary>
        /// <param name="trigger">The Quartz.Trigger that has fired</param>
        /// <param name="context">The Quartz.JobExecutionContext that will be passed to the 
        /// Quartz.IJob'sQuartz.IJob.Execute(Quartz.JobExecutionContext) method.</param>
        /// <returns>Returns true if job execution should be vetoed, false otherwise</returns>
        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(false);
        }

        #endregion
    }
}