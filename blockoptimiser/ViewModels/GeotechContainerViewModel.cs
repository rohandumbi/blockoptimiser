using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace blockoptimiser.ViewModels
{
    class GeotechContainerViewModel : Conductor<Object>
    {
        private String _geotechButtonForeground;
        private String _processButtonForeground;
        private String _expressionButtonForeground;

        public GeotechContainerViewModel()
        {
            SetDisabledButtonForegrounds();
            GeotechButtonForeground = "#FF189AD3";
            ActivateItem(new GeotechViewModel());
        }

        public string GeotechButtonForeground
        {
            get { return _geotechButtonForeground; }
            set
            {
                _geotechButtonForeground = value;
                NotifyOfPropertyChange(() => GeotechButtonForeground);
            }
        }

        public string ProcessButtonForeground
        {
            get { return _processButtonForeground; }
            set
            {
                _processButtonForeground = value;
                NotifyOfPropertyChange(() => ProcessButtonForeground);
            }
        }

        public string ExpressionButtonForeground
        {
            get { return _expressionButtonForeground; }
            set
            {
                _expressionButtonForeground = value;
                NotifyOfPropertyChange(() => ExpressionButtonForeground);
            }
        }

        public void SetDefaultButtonForegrounds()
        {
            GeotechButtonForeground = "#FF0E1A1F";
            ProcessButtonForeground = "#FF0E1A1F";
            ExpressionButtonForeground = "#FF0E1A1F";
        }

        public void SetDisabledButtonForegrounds()
        {
            GeotechButtonForeground = "#D3D3D5";
            ProcessButtonForeground = "#D3D3D5";
            ExpressionButtonForeground = "#D3D3D5";
        }

        public void ClickTab(object sender)
        {
            var selectedButton = sender as Button;
            SetDisabledButtonForegrounds();
            if (selectedButton != null)
            {
                String keyword = selectedButton.Content.ToString();
                switch (keyword)
                {
                    case "Geotech":
                        GeotechButtonForeground = "#FF189AD3";
                        NotifyOfPropertyChange(() => GeotechButtonForeground);
                        ActivateItem(new GeotechViewModel());
                        break;
                    case "Process":
                        ProcessButtonForeground = "#FF189AD3";
                        NotifyOfPropertyChange(() => ProcessButtonForeground);
                        ActivateItem(new ProcessViewModel());
                        break;
                    case "Expression":
                        ExpressionButtonForeground = "#FF189AD3";
                        NotifyOfPropertyChange(() => ExpressionButtonForeground);
                        ActivateItem(new ExpressionViewModel());
                        break;
                    default:
                        GeotechButtonForeground = "#FF189AD3";
                        NotifyOfPropertyChange(() => GeotechButtonForeground);
                        ActivateItem(new GeotechViewModel());
                        break;
                }

            }
        }
    }
}

