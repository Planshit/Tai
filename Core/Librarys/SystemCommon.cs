using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Librarys
{
    public class SystemCommon
    {
        /// <summary>
        /// 设置开机启动
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static bool SetStartup(bool startup = true)
        {
            string TaskName = "Tai task";
            var logonUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string tai = Path.Combine(
                  AppDomain.CurrentDomain.BaseDirectory,
                   "Tai.exe");
            string taskDescription = "Tai开机自启服务";

            using (var taskService = new TaskService())
            {
                var tasks = taskService.RootFolder.GetTasks(new System.Text.RegularExpressions.Regex(TaskName));
                foreach (var t in tasks)
                {
                    taskService.RootFolder.DeleteTask(t.Name);
                }

                if (startup)
                {
                    var task = taskService.NewTask();
                    task.RegistrationInfo.Description = taskDescription;
                    task.Triggers.Add(new LogonTrigger { UserId = logonUser });
                    task.Principal.RunLevel = TaskRunLevel.Highest;
                    task.Actions.Add(new ExecAction(tai, "--selfStart", AppDomain.CurrentDomain.BaseDirectory));
                    task.Settings.StopIfGoingOnBatteries = false;
                    task.Settings.DisallowStartIfOnBatteries = false;
                    taskService.RootFolder.RegisterTaskDefinition(TaskName, task);
                }
            }
            return false;
        }


    }
}
