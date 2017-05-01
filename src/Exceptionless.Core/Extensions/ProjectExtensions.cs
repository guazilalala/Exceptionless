﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exceptionless.Core.Models;
using Exceptionless.DateTimeExtensions;
using Foundatio.Utility;

namespace Exceptionless.Core.Extensions {
    public static class ProjectExtensions {
        public static void AddDefaultOwnerNotificationSettings(this Project project, string userId, NotificationSettings settings = null) {
            if (project.NotificationSettings.ContainsKey(userId))
                return;

            project.NotificationSettings.Add(userId, settings ?? new NotificationSettings {
                ReportNewErrors = true,
                SendDailySummary = true,
                ReportCriticalErrors = true,
                ReportEventRegressions = true
            });
        }

        public static void SetDefaultUserAgentBotPatterns(this Project project) {
            if (project.Configuration.Settings.ContainsKey(SettingsDictionary.KnownKeys.UserAgentBotPatterns))
                return;

            project.Configuration.Settings[SettingsDictionary.KnownKeys.UserAgentBotPatterns] = "*bot*,*crawler*,*spider*,*aolbuild*,*teoma*,*yahoo*";
        }

        public static string BuildFilter(this IList<Project> projects) {
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < projects.Count; index++) {
                if (index > 0)
                    builder.Append(" OR ");

                builder.AppendFormat("project:{0}", projects[index].Id);
            }

            return builder.ToString();
        }

        public static int GetCurrentHourlyTotal(this Project project) {
            var date = SystemClock.UtcNow.Floor(TimeSpan.FromHours(1));
            var usageInfo = project.OverageHours.FirstOrDefault(o => o.Date == date);
            return usageInfo?.Total ?? 0;
        }

        public static int GetCurrentHourlyBlocked(this Project project) {
            var date = SystemClock.UtcNow.Floor(TimeSpan.FromHours(1));
            var usageInfo = project.OverageHours.FirstOrDefault(o => o.Date == date);
            return usageInfo?.Blocked ?? 0;
        }

        public static int GetCurrentHourlyTooBig(this Project project) {
            var date = SystemClock.UtcNow.Floor(TimeSpan.FromHours(1));
            var usageInfo = project.OverageHours.FirstOrDefault(o => o.Date == date);
            return usageInfo?.TooBig ?? 0;
        }

        public static int GetCurrentMonthlyTotal(this Project project) {
            var date = new DateTime(SystemClock.UtcNow.Year, SystemClock.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var usageInfo = project.Usage.FirstOrDefault(o => o.Date == date);
            return usageInfo?.Total ?? 0;
        }

        public static int GetCurrentMonthlyBlocked(this Project project) {
            var date = new DateTime(SystemClock.UtcNow.Year, SystemClock.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var usageInfo = project.Usage.FirstOrDefault(o => o.Date == date);
            return usageInfo?.Blocked ?? 0;
        }

        public static int GetCurrentMonthlyTooBig(this Project project) {
            var date = new DateTime(SystemClock.UtcNow.Year, SystemClock.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var usageInfo = project.Usage.FirstOrDefault(o => o.Date == date);
            return usageInfo?.TooBig ?? 0;
        }

        public static void SetHourlyOverage(this Project project, double total, double blocked, double tooBig, int hourlyLimit) {
            var date = SystemClock.UtcNow.Floor(TimeSpan.FromHours(1));
            project.OverageHours.SetUsage(date, (int)total, (int)blocked, (int)tooBig, hourlyLimit, TimeSpan.FromDays(32));
        }

        public static void SetMonthlyUsage(this Project project, double total, double blocked, double tooBig, int monthlyLimit) {
            var date = new DateTime(SystemClock.UtcNow.Year, SystemClock.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            project.Usage.SetUsage(date, (int)total, (int)blocked, (int)tooBig, monthlyLimit, TimeSpan.FromDays(366));
        }
    }
}