using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.ViewModels
{
    public class GeotechViewModel : Screen
    {
        private ModelDataAccess ModelDAO;
        private GeotechDataAccess GeotechDAO;
        private FieldDataAccess FieldDAO;
        private List<Model> Models;
        public BindableCollection<Field> Fields { get; set; }
        public BindableCollection<Geotech> Geotechs { get; set; }
        public GeotechViewModel()
        {
            ModelDAO = new ModelDataAccess();
            GeotechDAO = new GeotechDataAccess();
            FieldDAO = new FieldDataAccess();
            Models = ModelDAO.GetAll(Context.ProjectId);
            Fields = new BindableCollection<Field>(FieldDAO.GetAll(Context.ProjectId));
            CreateGeotechForMissingModels();
        }

        private void CreateGeotechForMissingModels()
        {
            List<Geotech> existingGeotechs = GeotechDAO.GetAll(Context.ProjectId);
            foreach (Model model in Models)
            {
                //Geotech geotech = GeotechDAO.Get(model.Id);
                Boolean isEntryPresent = false;
                foreach (Geotech geotech in existingGeotechs)
                {
                    if (geotech.ModelId == model.Id)
                    {
                        isEntryPresent = true;
                        break;
                    }
                }
                if (!isEntryPresent)
                {
                    //creating default geotech against model, type field and first field selected by default
                    Geotech NewGeotech = new Geotech();
                    NewGeotech.ModelId = model.Id;
                    NewGeotech.Script = "";
                    NewGeotech.Type = 1;
                    NewGeotech.FieldId = Fields.First().Id;
                    NewGeotech.ProjectId = Context.ProjectId;
                    GeotechDAO.Insert(NewGeotech);
                } 
            }
            Geotechs = new BindableCollection<Geotech>(GeotechDAO.GetAll(Context.ProjectId));
        }
    }
}
