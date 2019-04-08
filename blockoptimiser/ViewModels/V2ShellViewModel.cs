using blockoptimiser.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace blockoptimiser.ViewModels
{
    class V2ShellViewModel : Conductor<Object>, IHandle<object>
    {
        private readonly IEventAggregator _eventAggregator;
        public Boolean IsAddFlyoutOpen { get; set; }
        public Boolean IsShowFlyoutOpen { get; set; }
        private V2ProjectsViewModel ProjectsView;

        public String NewProjectName { get; set; }
        public String NewProjectDescription { get; set; }
        public Boolean ShouldOpenNewProject { get; set; }
        public V2ShellViewModel()
        {
            _eventAggregator = new EventAggregator();
            _eventAggregator.Subscribe(this);
            IsAddFlyoutOpen = false;
            ShouldOpenNewProject = false;
            //ActivateItem(new ProjectsViewModel(_eventAggregator));
            ProjectsView = new V2ProjectsViewModel(_eventAggregator);
            ActivateItem(ProjectsView);
        }

        public void ClickTab(object sender)
        {
            var selectedButton = sender as Button;
            //SetDefaultButtonForegrounds();
            if (selectedButton != null)
            {
                String keyword = selectedButton.Content.ToString();
                if (keyword == "Projects")
                {
                    ShowProjectsViewScreen();
                } else if (keyword == "New App")
                {
                    ShowNewApp();
                }
            }
        }

        private void ShowProjectsViewScreen()
        {
            ActivateItem(new ProjectsViewModel(_eventAggregator));
        }

        private void ShowNewApp()
        {
            ProjectsView = new V2ProjectsViewModel(_eventAggregator);
            ActivateItem(ProjectsView);
        }

        public void CreateProject(object e, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (NewProjectName == "" || NewProjectName == null)
            {
                MessageBox.Show("Please enter mandatory field Name");
                return;
            }
            Project newProject = ProjectsView.CreateProject(NewProjectName, NewProjectDescription);
            if (ShouldOpenNewProject == true)
            {
                Context.ProjectId = newProject.Id;
                ActivateItem(new AppViewModel());
            }
            
        }

        public void Handle(object message)
        {
            String EventName = message as String;
            if (EventName == "loaded:project")
            {
                //ActivateItem(new MainViewModel());
                ActivateItem(new AppViewModel());
            }
            else if (EventName == "load:addProjectFlyout")
            {
                IsAddFlyoutOpen = !IsAddFlyoutOpen;
                NotifyOfPropertyChange("IsAddFlyoutOpen");
            }
            else if (EventName == "load:projectDetailsFlyout")
            {
                IsShowFlyoutOpen = !IsShowFlyoutOpen;
                NotifyOfPropertyChange("IsShowFlyoutOpen");
            }
        }
    }
}
