﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace blockoptimiser.ViewModels
{
    public class LimitsViewModel: Conductor<Object>
    {
        public String PeriodButtonForeground { get; set; }
        public String FinanceButtonForeground { get; set; }
        public String ProcessButtonForeground { get; set; }
        public String GradeButtonForeground { get; set; }
        public Boolean IsPeriodSelected { get; set; }

        public LimitsViewModel()
        {
            SetDisabledButtonForegrounds();
            PeriodButtonForeground = "#FF189AD3";
            IsPeriodSelected = false;
            ActivateItem(new PeriodViewModel());
        }

        public void SetDefaultButtonForegrounds()
        {
            PeriodButtonForeground = "#FF0E1A1F";
            FinanceButtonForeground = "#FF0E1A1F";
            ProcessButtonForeground = "#FF0E1A1F";
            GradeButtonForeground = "#FF0E1A1F";
        }

        public void SetDisabledButtonForegrounds()
        {
            FinanceButtonForeground = "#D3D3D3";
            ProcessButtonForeground = "#D3D3D3";
            GradeButtonForeground = "#D3D3D3";
        }

        private void NotifyButtonforegroundChanges()
        {
            NotifyOfPropertyChange(() => PeriodButtonForeground);
            NotifyOfPropertyChange(() => FinanceButtonForeground);
            NotifyOfPropertyChange(() => ProcessButtonForeground);
            NotifyOfPropertyChange(() => GradeButtonForeground);
        }

        public void ClickTab(object sender)
        {
            var selectedButton = sender as Button;
            SetDefaultButtonForegrounds();
            NotifyButtonforegroundChanges();
            //IsPeriodSelected = true;
            //NotifyOfPropertyChange(() => IsPeriodSelected);
            if (selectedButton != null)
            {
                String keyword = selectedButton.Content.ToString();
                switch (keyword)
                {
                    case "Period":
                        PeriodButtonForeground = "#FF189AD3";
                        NotifyOfPropertyChange(() => PeriodButtonForeground);
                        ActivateItem(new PeriodViewModel());
                        IsPeriodSelected = true;
                        NotifyOfPropertyChange(() => IsPeriodSelected);
                        break;
                    case "Finance":
                        FinanceButtonForeground = "#FF189AD3";
                        NotifyOfPropertyChange(() => FinanceButtonForeground);
                        ActivateItem(new FinanceLimitViewModel());
                        break;
                    case "Process":
                        ProcessButtonForeground = "#FF189AD3";
                        NotifyOfPropertyChange(() => ProcessButtonForeground);
                        ActivateItem(new ProcessLimitViewModel());
                        break;
                    case "Grade":
                        GradeButtonForeground = "#FF189AD3";
                        NotifyOfPropertyChange(() => GradeButtonForeground);
                        ActivateItem(new GradeLimitViewModel());
                        break;
                    default:
                        PeriodButtonForeground = "#FF189AD3";
                        ActivateItem(new PeriodViewModel());
                        break;
                }
            }
        }
    }
}