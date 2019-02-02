using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace blockoptimiser.ViewModels
{
    public class GeotechViewModel : Screen
    {
        private ModelDataAccess ModelDAO;
        private GeotechDataAccess GeotechDAO;
        private FieldDataAccess FieldDAO;
        private List<Model> Models;
        private List<Field> Fields { get; set; }
        public BindableCollection<Geotech> Geotechs { get; set; }
        public GeotechViewModel()
        {
            ModelDAO = new ModelDataAccess();
            GeotechDAO = new GeotechDataAccess();
            FieldDAO = new FieldDataAccess();
            Models = ModelDAO.GetAll(Context.ProjectId);
            Fields = FieldDAO.GetAll(Context.ProjectId);
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
                    NewGeotech.UseScript = false;
                    NewGeotech.FieldId = Fields.First().Id;
                    NewGeotech.ProjectId = Context.ProjectId;
                    GeotechDAO.Insert(NewGeotech);
                } 
            }
            Geotechs = new BindableCollection<Geotech>(GeotechDAO.GetAll(Context.ProjectId));
            foreach (Geotech geotech in Geotechs)
            {
                geotech.AvailableFields = new List<String>();
                foreach (Field field in Fields)
                {
                    geotech.AvailableFields.Add(field.Name);
                }
                geotech.FieldName = GetFieldById(geotech.FieldId).Name;
                geotech.ModelName = GetModelById(geotech.ModelId).Name;
            }
        }

        private Field GetFieldById(int fieldId)
        {
            Field returnedField = Fields.First();
            foreach (Field field in Fields)
            {
                if (field.Id == fieldId)
                {
                    returnedField = field;
                }
            }
            return returnedField;
        }

        private Field GetFieldByName(string fieldName)
        {
            Field returnedField = Fields.First();
            foreach (Field field in Fields)
            {
                if (field.Name == fieldName)
                {
                    returnedField = field;
                }
            }
            return returnedField;
        }

        private Model GetModelById(int modelId)
        {
            Model returnedModel = Models.First();
            foreach (Model model in Models)
            {
                if (model.Id == modelId)
                {
                    returnedModel = model;
                }
            }
            return returnedModel;
        }

        public void UpdateGeotech(Geotech child)
        {
            child.FieldId = GetFieldByName(child.FieldName).Id;
            GeotechDAO.Update(child);
        }
    }
}
