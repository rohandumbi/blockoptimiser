using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Services.LP
{
    public class ExecutionContext
    {
        public int ProjectId { get; set; }
        public int ScenarioId { get; set; }
        public int Year { get; set; }
        public int Period { get; set; }
        public int DiscountFactor { get; set; }

        private List<Model> models;
        private List<Field> fields;
        private List<Expression> expressions;
        private List<Process> processes;
        private List<Product> products;
        private List<String> processJoins;
        private List<String> productJoins;
        private List<Opex> opexList;
        private Dictionary<String, String> requiredFields;
        public Dictionary<int, List<int>> processBlockMapping;


        public ExecutionContext(int ProjectId, int ScenarioId, int DiscountFactor)
        {
            this.ProjectId = ProjectId;
            this.ScenarioId = ScenarioId;
            this.DiscountFactor = DiscountFactor/100;

            List<RequiredFieldMapping> requiredFieldMappings = new RequiredFieldMappingDataAccess().GetAll(ProjectId);
            requiredFields = new Dictionary<string, string>();
            foreach(RequiredFieldMapping mapping in requiredFieldMappings)
            {
                requiredFields.Add(mapping.RequiredFieldName, mapping.MappedColumnName);
            }
            processBlockMapping = new Dictionary<int, List<int>>();
            LoadData();
        }

        private void LoadData()
        {
            models = new ModelDataAccess().GetAll(ProjectId);
            fields = new FieldDataAccess().GetAll(ProjectId);
            expressions = new ExpressionDataAccess().GetAll(ProjectId);
            processes = new ProcessDataAccess().GetAll(ProjectId);
            products = new ProductDataAccess().GetAll(ProjectId);
            processJoins = new ProcessJoinDataAccess().GetProcessJoins(ProjectId);
            productJoins = new ProductJoinDataAccess().GetProductJoins(ProjectId);
            opexList = new OpexDataAccess().GetAll(ScenarioId);
        }

        public List<Model> GetModels()
        {
            return models;
        }

        public List<Process> GetProcessList()
        {
            return processes;
        }

        public String GetColumnNameById(int fieldId, int modelId)
        {
            List<CsvColumnMapping> mappings = new CsvColumnMappingDataAccess().GetAll(modelId);
            foreach(var mapping in mappings)
            {
                if(mapping.FieldId == fieldId)
                {
                    return mapping.ColumnName;
                }
            }
            return null;
        }

        public int GetIRA(String selectstr, int modelId)
        {
            String angle = new BlockDataAccess().GetAngle(ProjectId, modelId, selectstr);
            try
            {
                return Int32.Parse(angle);
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
            
        }
        public List<ModelDimension> GetModelDimension(int ModelId)
        {
            return new ModelDimensionDataAccess().GetAll(ModelId);
        }
        public Geotech GetGeotechByModel(int ModelId)
        {
            return new GeotechDataAccess().Get(ModelId);
        }
        public List<Block> GetBlocks(int modelId, String condition)
        {
            return new BlockDataAccess().GetBlocks(ProjectId, modelId, condition);
        }

        public List<Opex> getOpexList()
        {
            return opexList;
        }



        public Boolean IsProductJoinAssociatedToProcess(String productJoinName, int processId)
        {
            List<String> productNames = new ProductJoinDataAccess().GetProductsInJoin(productJoinName);
            foreach(var productName in productNames)
            {
                foreach(var product in products)
                {
                    if(product.Name.Equals(productName))
                    {
                        if(product.ProcessIds.Contains(processId))
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            return false;
        }


        public Boolean IsProductAssociatedToProcess(String productName, int processId)
        {
            foreach (var product in products)
            {
                if (product.Name.Equals(productName))
                {
                    if (product.ProcessIds.Contains(processId))
                    {
                        return true;
                    }
                    break;
                }
            }
            return false;
        }

        public Boolean IsProcessInProcessJoin(String processJoinName, String processName)
        {
            return new ProcessJoinDataAccess().GetProcessesInJoin(processJoinName).Contains(processName);
        }
        public Decimal GetUnitValueforBlock(Block b, byte unitType, int unitId)
        {
            if (unitType == 1)
            { // 1- Field, 2 - Expression
                foreach(Field field in fields)
                {
                    if(field.Id == unitId)
                    {

                        String value = (String)b.data[field.Name];
                        if (value == null)
                        {
                            return 0;
                        }
                        else
                        {
                            return Decimal.Parse(value);
                        }
                    }
                }
                
            }
            else
            {
                foreach (Expression expression in expressions)
                {
                    if (expression.Id == unitId)
                    {

                        String value = (String)b.data[expression.Name];
                        if (value == null)
                        {
                            return 0;
                        }
                        else
                        {
                            return Decimal.Parse(value);
                        }
                    }
                }
            }
            return 0;
        }

        public Decimal GetTonnesWtForBlock(Block b)
        {
            String tonnesColumnName = requiredFields["tonnage"];
            String value = (String)b.data[tonnesColumnName];
            if(value == null)
            {
                return 0;
            } else
            {
                return Decimal.Parse(value);
            }
        }
    }
}
