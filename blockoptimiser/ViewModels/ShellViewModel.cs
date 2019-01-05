using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace blockoptimiser.ViewModels
{
    class ShellViewModel : Conductor<Object>, IHandle<object>
    {
        private readonly IEventAggregator _eventAggregator;
        public ShellViewModel()
        {
            _eventAggregator = new EventAggregator();
            _eventAggregator.Subscribe(this);
            ActivateItem(new ProjectsViewModel(_eventAggregator));
        }

        public void Handle(object message)
        {
            String EventName = message as String;
            MessageBox.Show(EventName);
        }
    }
}
