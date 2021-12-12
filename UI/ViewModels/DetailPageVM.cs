using Core.Librarys;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controls.Charts.Model;
using UI.Librays;
using UI.Models;

namespace UI.ViewModels
{
    public class DetailPageVM : DetailPageModel
    {
        private readonly IData data;
        private readonly MainViewModel main;
        public DetailPageVM(
            IData data, MainViewModel main)
        {
            this.data = data;
            this.main = main;
            Init();
        }

        private async void Init()
        {
            Process = main.Data as ChartsDataModel;

            Date = DateTime.Now;

            PropertyChanged += DetailPageVM_PropertyChanged;

            await LoadData();

            await LoadInfo();
        }

        private async void DetailPageVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Date))
            {
                await LoadData();
            }
        }

        private async Task LoadInfo()
        {
            await Task.Run(() =>
            {
                if (Process != null)
                {
                    var info = data.GetLast(Process.PopupText);
                    if (info != null)
                    {
                        ProcessName = info.ProcessName;

                    }
                }
            }
            );
        }

        private async Task LoadData()
        {
            await Task.Run(() =>
            {
                if (Process != null)
                {
                    var monthData = data.GetProcessMonthLogList(Process.PopupText, Date);
                    int monthTotal = monthData.Sum(m => m.Time);
                    Total = Timer.Fromat(monthTotal);

                    DateTime start = new DateTime(Date.Year, Date.Month, 1);
                    DateTime end = new DateTime(Date.Year, Date.Month, DateTime.DaysInMonth(Date.Year, Date.Month));
                    var monthAllData = data.GetDateRangelogList(start, end);

                    LongDay = "暂无数据";
                    if (monthData.Count > 0)
                    {
                        var longDayData = monthData.OrderByDescending(m => m.Time).FirstOrDefault();
                        LongDay = longDayData.Date.ToString("最长一天是在 dd 日，使用了 " + Timer.Fromat(longDayData.Time));

                    }


                    int monthAllTotal = monthAllData.Sum(m => m.Time);
                    if (monthAllTotal > 0)
                    {
                        Ratio = (((double)monthTotal / (double)monthAllTotal)).ToString("P");
                    }
                    else
                    {
                        Ratio = "暂无数据";
                    }
                    Data = MapToChartsData(monthData);
                    if (Data.Count == 0)
                    {
                        Data.Add(new ChartsDataModel()
                        {
                            DateTime = Date,
                        });
                    }
                }
            });
        }

        private List<ChartsDataModel> MapToChartsData(List<Core.Models.DailyLogModel> list)
        {
            var resData = new List<ChartsDataModel>();

            foreach (var item in list)
            {
                var bindModel = new ChartsDataModel();
                bindModel.Name = string.IsNullOrEmpty(item.ProcessDescription) ? item.ProcessName : item.ProcessDescription;
                bindModel.Value = item.Time;
                bindModel.Tag = Timer.Fromat(item.Time);
                bindModel.PopupText = item.File;
                bindModel.Icon = Iconer.Get(item.ProcessName, item.ProcessDescription);
                bindModel.DateTime = item.Date;
                resData.Add(bindModel);
            }

            return resData;
        }
    }
}
