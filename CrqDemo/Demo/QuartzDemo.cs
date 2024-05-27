using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using Quartz.Impl;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace CrqDemo.Demo
{
    public class QuartzDemo
    {
        public static void Init()
        {
            QuartzService.Init();
            StartTaskJob();
        }

        public static void StartTaskJob()
        {
            var task = new object();
            var job = new QuartzJob()
            {
                JobGroup = $"newgroup",
                JobName = "newJobName",
                BeginTime = DateTime.Now.AddDays(-1),
                EndTime = DateTime.Now.AddDays(1),
                Cron = DateTime.Now.AddMinutes(1).ToCronMonthDay(),
                Description = $"[执行]",
                CreateTime = DateTime.Now,
            };
            job.JobDataMap.Add("task", task);
            job.JobDataMap.Add("Action", new Action(
                () =>
                {
                    Console.WriteLine($"定时触发：{DateTime.Now}");
                }
            ));
            var status = QuartzService.Instance.AddOrUpdateScheduleJob<Common_ExJob>(job);
            Console.WriteLine($"创建作业计划:{DateTime.Now}");
            if (!status.Result)
            {
                string errorMsg = $"失败。";
                ComUse.LogHelper.WriteLog(ComUse.Enumerate.CodeType.Code003, ComUse.Enumerate.NLogType.Error, errorMsg);

            }
        }
    }

    /// <summary>
    /// 触发通用JOB
    /// </summary>
    [DisallowConcurrentExecution]
    public class Common_ExJob : QuartzBaseJob
    {
        protected override bool CanExecute(IJobExecutionContext context)
        {
            return true;
        }

        protected override string GetLogScheduleJobConnStr()
        {
            throw new NotImplementedException();
        }
        protected override void InnerExecute(IJobExecutionContext context)
        {
            var fun = (Action)context.Trigger.JobDataMap["Action"];
            Task.Run(() => {
                fun();
            });
        }

        protected override void WriteDebugLog(string logMsg)
        {

        }
    }



    /// <summary>
    /// 每日4点清除时间
    /// </summary>
    [DisallowConcurrentExecution]
    public class ClearDB_ExJob : QuartzBaseJob
    {
        protected override bool CanExecute(IJobExecutionContext context)
        {
            return true;
        }

        protected override string GetLogScheduleJobConnStr()
        {
            throw new NotImplementedException();
        }
        protected override void InnerExecute(IJobExecutionContext context)
        {
            Console.WriteLine($"开始清除:{DateTime.Now}");
        }

        protected override void WriteDebugLog(string logMsg)
        {

        }
    }
    #region Service
    public class QuartzService : QuartzBaseService
    {
        public static QuartzService Instance { get; private set; }

        static QuartzService()
        {
            Instance = new QuartzService();
            Instance.Init("10001", "Server").ConfigureAwait(false);
        }

        public static void Init()
        {
            Instance.InitSchedule().ConfigureAwait(false);
        }

        protected override async Task InitSchedule(IScheduler scheduler)
        {
            await AddScheduleJob<ClearDB_ExJob>($"0 32 16 * * ?", "清除数据");//每天 04:00:00 执行
        }
    }
    public abstract class QuartzBaseService
    {
        public string Executor { get; private set; } = "DEFAULT";
        public string GroupName { get; private set; } = "DEFAULT";
        public IScheduler Scheduler { get; private set; }

        public async Task Init(string executor, string defaultGroupName, int threaPoolSize = 20)
        {
            Quartz.Logging.LogProvider.IsDisabled = true;
            Executor = executor;
            GroupName = defaultGroupName;
            var factory = new StdSchedulerFactory(QuartzBaseConfig.GetProperties(threaPoolSize));
            Scheduler = await factory.GetScheduler();
            await Scheduler.Start();
        }

        public async Task InitSchedule() => await InitSchedule(Scheduler);

        public async Task AddScheduleJob<T>(string cronExpression, string jobDesc, string specGroupName = null) where T : IJob
        {
            if (!CronExpression.IsValidExpression(cronExpression))
            {
                var log = $"计划任务{jobDesc}添加失败，cron表达式{cronExpression}异常";
                ComUse.StrHelper.ConsoleWrite(log);
                ComUse.LogHelper.WriteLog(ComUse.Enumerate.CodeType.Code003, ComUse.Enumerate.NLogType.Error, log);
                return;
            }
            var groupName = string.IsNullOrEmpty(specGroupName) ? GroupName : specGroupName;
            var jobName = typeof(T).Name;
            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity(jobName, groupName)
                .WithDescription(jobDesc)
                .UsingJobData("executor", Executor)
                .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(jobName, groupName)
                .WithCronSchedule(cronExpression)
                .ForJob(job)
                .Build();
            var nextFireTime = await Scheduler.ScheduleJob(job, trigger);
            System.Console.WriteLine($"{jobDesc}下次执行时间：{nextFireTime.ToLocalTime()}");
        }

        protected abstract Task InitSchedule(IScheduler scheduler);

        /// <summary>
        /// 判断作业是否已存在
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckExistsJob(string jobName, string jobGroup)
        {
            var jobKey = new JobKey(jobName, jobGroup);
            var stat = await Scheduler.CheckExists(jobKey);
            return stat;
        }

        public bool CheckCron(QuartzJob entity)
        {
            if (!CronExpression.IsValidExpression(entity.Cron))
            {
                var log = $"计划任务{entity.JobGroup}_{entity.JobName}添加失败，cron表达式{entity.Cron}异常";
                ComUse.StrHelper.ConsoleWrite(log);
                ComUse.LogHelper.WriteLog(ComUse.Enumerate.CodeType.Code003, ComUse.Enumerate.NLogType.Error, log);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 创建类型Cron的触发器
        /// </summary>
        /// <returns></returns>
        public ITrigger CreateCronTrigger(QuartzJob entity)
        {
            return TriggerBuilder.Create()
                   .WithIdentity(entity.JobName, entity.JobGroup)
                   .StartAt(entity.BeginTime)
                   .EndAt(entity.EndTime)
                   .WithCronSchedule(entity.Cron)
                   .UsingJobData(entity.JobDataMap)
                   .ForJob(entity.JobName, entity.JobGroup)
                   .Build();
        }

        /// <summary>
        /// 新增作业
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddScheduleJob<T>(QuartzJob entity) where T : IJob
        {
            try
            {
                if (!CheckCron(entity))
                    return false;
                //检查任务是否已存在
                var jobKey = new JobKey(entity.JobName, entity.JobGroup);
                if (await Scheduler.CheckExists(jobKey))
                {
                    return false;
                }
                IJobDetail jobDetail = JobBuilder.Create<T>()
                    .SetJobData(entity.JobDataMap)
                    .WithDescription(entity.Description)
                    .WithIdentity(entity.JobName, entity.JobGroup)
                    .Build();

                ITrigger trigger = CreateCronTrigger(entity);

                await Scheduler.ScheduleJob(jobDetail, trigger);
            }
            catch (Exception ex)
            {
                string log = $"作业添加失败:{entity.JobGroup}_{entity.JobName} => {ex}";
                ComUse.LogHelper.WriteLog(ComUse.Enumerate.CodeType.Code003, ComUse.Enumerate.NLogType.Error, log);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 更新作业触发器
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateTriggerJob<T>(QuartzJob entity) where T : IJob
        {
            try
            {
                //检查任务是否已存在
                var jobKey = new JobKey(entity.JobName, entity.JobGroup);
                if (!await Scheduler.CheckExists(jobKey))
                {
                    return false;
                }
                ITrigger newTrigger = CreateCronTrigger(entity);
                await Scheduler.RescheduleJob(newTrigger.Key, newTrigger);
            }
            catch (Exception ex)
            {
                string log = $"作业更新失败:{entity.JobGroup}_{entity.JobName} => {ex}";
                ComUse.LogHelper.WriteLog(ComUse.Enumerate.CodeType.Code003, ComUse.Enumerate.NLogType.Error, log);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 添加或者更新作业
        /// </summary>
        public async Task<bool> AddOrUpdateScheduleJob<T>(QuartzJob entity) where T : IJob
        {
            try
            {
                if (!CheckCron(entity))
                    return false;
                //检查任务是否已存在
                var jobKey = new JobKey(entity.JobName, entity.JobGroup);
                if (await Scheduler.CheckExists(jobKey))
                {
                    return await UpdateTriggerJob<T>(entity);
                }
                return await AddScheduleJob<T>(entity);
            }
            catch (Exception ex)
            {
                string log = $"作业添加或更新失败:{entity.JobGroup}_{entity.JobName} => {ex}";
                ComUse.LogHelper.WriteLog(ComUse.Enumerate.CodeType.Code003, ComUse.Enumerate.NLogType.Error, log);
                return false;
            }
        }

        /// <summary>
        /// 获取所有作业信息
        /// </summary>
        public async Task<List<QuartzJob_Details>> GetAllJobDetailsAsync()
        {
            var jobDetails = new List<QuartzJob_Details>();
            var jboKeyList = new List<JobKey>();
            var groupNames = await Scheduler.GetJobGroupNames();
            foreach (var groupName in groupNames.OrderBy(t => t))
            {
                jboKeyList.AddRange(await Scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)));
            }
            foreach (var jobKey in jboKeyList.OrderBy(t => t.Name))
            {
                var jobDetail = await Scheduler.GetJobDetail(jobKey);
                var triggersList = await Scheduler.GetTriggersOfJob(jobKey);
                var triggers = triggersList.AsEnumerable().FirstOrDefault();

                jobDetails.Add(new QuartzJob_Details()
                {
                    GroupName = jobKey.Group,
                    JobName = jobKey.Name,
                    Cron = (triggers as CronTriggerImpl)?.CronExpressionString,
                    BeginTime = triggers.StartTimeUtc.LocalDateTime,
                    EndTime = triggers.EndTimeUtc?.LocalDateTime,
                    PreviousFireTime = triggers.GetPreviousFireTimeUtc()?.LocalDateTime,
                    NextFireTime = triggers.GetNextFireTimeUtc()?.LocalDateTime,
                    TriggerState = await Scheduler.GetTriggerState(triggers.Key),
                    Description = jobDetail.Description,
                    DataMap = jobDetail.JobDataMap
                });
            }
            return jobDetails;
        }

        /// <summary>
        /// 暂停/删除 指定的计划
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StopOrDelJobAsync(string jobGroup, string jobName, bool isDelete = false)
        {
            try
            {
                await Scheduler.PauseJob(new JobKey(jobName, jobGroup));
                if (isDelete)
                {
                    await Scheduler.DeleteJob(new JobKey(jobName, jobGroup));
                }
            }
            catch (Exception ex)
            {
                string log = $"暂停/删除 指定的计划:{jobGroup},{jobName},{isDelete} => {ex}";
                ComUse.LogHelper.WriteLog(ComUse.Enumerate.CodeType.Code003, ComUse.Enumerate.NLogType.Error, log);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 立即执行作业
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TriggerJobAsync(string jobGroup, string jobName)
        {
            try
            {
                if (await CheckExistsJob(jobGroup, jobName))
                {
                    return false;
                }
                var jobKey = new JobKey(jobName, jobGroup);
                await Scheduler.TriggerJob(jobKey);
            }
            catch (Exception ex)
            {
                string log = $"执行任务失败:{jobGroup}_{jobName} => {ex}";
                ComUse.LogHelper.WriteLog(ComUse.Enumerate.CodeType.Code003, ComUse.Enumerate.NLogType.Error, log);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 移除所有任务分组下的任务
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DelAllScheduleJobAsync()
        {
            try
            {
                var groupNames = await Scheduler.GetJobGroupNames();
                GroupMatcher<JobKey> group;
                List<JobKey> jboKeyList = new List<JobKey>();
                foreach (var groupName in groupNames.OrderBy(t => t))
                {
                    jboKeyList.AddRange(await Scheduler.GetJobKeys(group = GroupMatcher<JobKey>.GroupEquals(groupName)));
                    await Scheduler.DeleteJobs(jboKeyList);
                    jboKeyList.Clear();
                }
            }
            catch (Exception ex)
            {
                ComUse.LogHelper.WriteLog(ComUse.Enumerate.CodeType.Code003, ComUse.Enumerate.NLogType.Error, $"删除所有任务计划失败: {ex}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 移除指定任务分组下的任务
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DelScheduleJobAsync(string groupName)
        {
            try
            {
                var groupNames = await Scheduler.GetJobGroupNames();
                if (!groupNames.Contains(groupName))
                {
                    return false;
                }
                List<JobKey> jboKeyList = new List<JobKey>();
                GroupMatcher<JobKey> group;
                jboKeyList.AddRange(await Scheduler.GetJobKeys(group = GroupMatcher<JobKey>.GroupEquals(groupName)));
                await Scheduler.DeleteJobs(jboKeyList);
                jboKeyList.Clear();
            }
            catch (Exception ex)
            {
                string log = $"移除指定任务分组下的任务失败:{groupName} => {ex}";
                ComUse.LogHelper.WriteLog(ComUse.Enumerate.CodeType.Code003, ComUse.Enumerate.NLogType.Error, log);
                return false;
            }
            return true;
        }
    }

    #endregion
    #region Job

    [DisallowConcurrentExecution]
    public abstract class QuartzBaseJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var description = context.JobDetail.Description;
            try
            {
                var canExecute = CanExecute(context);
                if (!canExecute)
                    return;
                WriteDebugLog($"计划任务 {description} 开始");
                await InnerExecuteAsync(context);
                WriteDebugLog($"计划任务 {description} 完成");
            }
            catch (Exception ex)
            {
                WriteErrorLog($"执行计划任务{description}异常", ex);
            }
        }

        protected virtual bool CanExecute(IJobExecutionContext context)
        {
            return true;
        }

        protected virtual string GetScheduleKey()
        {
            return DateTime.Today.ToString("yyyyMMdd");
        }

        protected virtual Task InnerExecuteAsync(IJobExecutionContext context)
        {
            InnerExecute(context);
            return Task.CompletedTask;
        }

        protected virtual void WriteDebugLog(string logMsg)
        {
            ComUse.LogHelper.WriteLog(ComUse.Enumerate.CodeType.Code003, ComUse.Enumerate.NLogType.Debug, logMsg);
        }

        protected virtual void WriteErrorLog(string logMsg, Exception ex)
        {
            ComUse.LogHelper.WriteLog(ComUse.Enumerate.CodeType.Code003, ComUse.Enumerate.NLogType.Error, logMsg, ex);
        }

        protected abstract string GetLogScheduleJobConnStr();

        protected abstract void InnerExecute(IJobExecutionContext context);
    }

    public static class CronExtension
    {
        /// <summary>
        /// 每天每小时 hour:minute:second 执行
        /// </summary>
        /// <returns></returns>
        public static string ToCronDayHour(this DateTime dt)
        {
            return $"{dt.Second} {dt.Minute} {dt.Hour} * * ?";
        }

        /// <summary>
        /// 每年每月 hour:minute:second 执行
        /// </summary>
        /// <returns></returns>
        public static string ToCronMonthDay(this DateTime dt)
        {
            return $"{dt.Second} {dt.Minute} {dt.Hour} {dt.Day} {dt.Month} ?";
        }

        /// <summary>
        /// 每隔N分钟执行一次
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static string IntervalMinutes(int minutes)
        {
            return $"0 */{minutes} * * * ?";
        }

        /// <summary>
        /// 每隔N秒执行一次
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string IntervalSeconds(int seconds)
        {
            return $"0/{seconds} * * * * ?";
        }
    }

    #endregion

    #region Config

    class QuartzBaseConfig
    {
        public static NameValueCollection GetProperties(int threadCount)
        {
            return new NameValueCollection
            {
                ["quartz.threadPool.threadCount"] = $"{threadCount}",
            };
        }
    }

    #endregion

    #region Model

    public class QuartzJob
    {
        /// <summary>
        /// 任务分组
        /// </summary>
        public string JobGroup { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// 作业开始时间
        /// </summary>
        public DateTimeOffset BeginTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 作业结束时间
        /// </summary>
        public DateTimeOffset? EndTime { get; set; }
        /// <summary>
        /// Cron表达式
        /// </summary>
        public string Cron { get; set; }
        /// <summary>
        /// 任务数据
        /// </summary>
        public JobDataMap JobDataMap { get; set; } = new JobDataMap();
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// job创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    public class QuartzJob_Details
    {
        /// <summary>
        /// 作业组名
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// 下次执行时间
        /// </summary>
        public DateTime? NextFireTime { get; set; }
        /// <summary>
        /// 上次执行时间
        /// </summary>
        public DateTime? PreviousFireTime { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public TriggerState TriggerState { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 显示状态
        /// </summary>
        public string DisplayState
        {
            get
            {
                var state = string.Empty;
                switch (TriggerState)
                {
                    case TriggerState.Normal:
                        state = "正常";
                        break;
                    case TriggerState.Paused:
                        state = "暂停";
                        break;
                    case TriggerState.Complete:
                        state = "完成";
                        break;
                    case TriggerState.Error:
                        state = "异常";
                        break;
                    case TriggerState.Blocked:
                        state = "阻塞";
                        break;
                    case TriggerState.None:
                        state = "不存在";
                        break;
                    default:
                        state = "未知";
                        break;
                }
                return state;
            }
        }
        /// <summary>
        /// 作业数据
        /// </summary>
        public JobDataMap DataMap { get; set; }

        public string JobDataMapStr
        {
            get
            {
                if (DataMap.Count == 0)
                    return "作业字典空";
                return DataMap.ToResString(t => $"[{t.Key}_{t.Value.ToString()}],");
            }
        }

        /// <summary>
        /// Cron
        /// </summary>
        public string Cron { get; set; }

        public override string ToString()
        {
            //作业分组,作业名称,Cron,开始时间,结束时间,下一次执行时间,上一次执行时间,作业状态,作业数据
            return $"[ 分组：{GroupName},作业：{JobName},Cron：{Cron},开始时间：{BeginTime},结束时间：{EndTime},下一次执行时间：{NextFireTime},上一次执行时间：{PreviousFireTime},状态：{DisplayState},数据字典：{JobDataMapStr} ]";
        }
    }
    #endregion

}
