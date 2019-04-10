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
        public decimal DiscountFactor { get; set; }

        private Boolean _benchConstraintEnabled = true;
        private int _benchConstraint = 4;


        private List<Model> models;
        private List<Field> fields;
        private List<Expression> expressions;
        private List<Process> processes;
        private List<Product> products;
        private List<String> processJoins;
        private List<Opex> opexList;
        private List<ProcessLimit> processLimits;
        private List<GradeLimit> gradeLimits;
        private Dictionary<String, String> requiredFields;
        private Dictionary<long, List<int>> blockProcessMapping;
        private List<long> minedBlocks;
        private Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Block>>>> Blocks { get; set; }

        public ExecutionContext(int ProjectId, int ScenarioId, int DiscountFactor)
        {
            this.ProjectId = ProjectId;
            this.ScenarioId = ScenarioId;
            this.DiscountFactor = (decimal)DiscountFactor/100;
            SchedulerResultDataAccess schedulerResultDataAccess = new SchedulerResultDataAccess();

            List<RequiredFieldMapping> requiredFieldMappings = new RequiredFieldMappingDataAccess().GetAll(ProjectId);
            blockProcessMapping = new Dictionary<long, List<int>>();
            requiredFields = new Dictionary<string, string>();
            foreach(RequiredFieldMapping mapping in requiredFieldMappings)
            {
                requiredFields.Add(mapping.RequiredFieldName, mapping.MappedColumnName);
            }
            LoadData();
            schedulerResultDataAccess.Create(ProjectId);
        }

        public void Reset()
        {
            blockProcessMapping = new Dictionary<long, List<int>>();
            LoadBlocks();
        }
        private void LoadData()
        {
            models = new ModelDataAccess().GetAll(ProjectId);
            fields = new FieldDataAccess().GetAll(ProjectId);
            expressions = new ExpressionDataAccess().GetAll(ProjectId);
            processes = new ProcessDataAccess().GetAll(ProjectId);
            products = new ProductDataAccess().GetAll(ProjectId);
            processJoins = new ProcessJoinDataAccess().GetProcessJoins(ProjectId);
            opexList = new OpexDataAccess().GetAll(ScenarioId);
            processLimits = new ProcessLimitDataAccess().GetProcessLimits();
            gradeLimits = new GradeLimitDataAccess().GetGradeLimits();
        }

        private void LoadBlocks()
        {
            Blocks = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Block>>>>();
            BlockDataAccess blockDataAccess = new BlockDataAccess();
            foreach (Model model in models)
            {
                Dictionary<int, Dictionary<int, Dictionary<int, Block>>>  blocksInModel = blockDataAccess.GetBlocks(this.ProjectId, model.Id);
                Blocks.Add(model.Id, blocksInModel);
            }
        }

        public void LoadMinedBlockList()
        {
            SchedulerResultDataAccess schedulerResultDataAccess = new SchedulerResultDataAccess();
            minedBlocks = schedulerResultDataAccess.GetMinedBlocks(ProjectId);
        } 
        public Boolean IsMined(long BId)
        {
            return minedBlocks.Contains(BId);
        }
        public Dictionary<int, Dictionary<int, Dictionary<int, Block>>> GetBlocks(int modelId)
        {
            return Blocks[modelId];
        }
        public List<Model> GetModels()
        {
            return models;
        }

        public List<Process> GetProcessList()
        {
            return processes;
        }
        public Process GetProcessById(int Id)
        {
            foreach(Process process in processes)
            {
                if(process.Id == Id)
                {
                    return process;
                }
            }
            return null;
        }

        public List<String> GetGradeFieldsByAssociatedFieldName(String AssociatedFieldName)
        {
            List<String> gradeFieldNames = new List<string>();
            int AssociatedFieldId = -1;
            foreach(Field f in fields)
            {
                if(f.DataType == Field.DATA_TYPE_ADDITIVE && f.Name.Equals(AssociatedFieldName))
                {
                    AssociatedFieldId = f.Id;
                    break;
                }
            }
            foreach (Field f in fields)
            {
                if (f.DataType == Field.DATA_TYPE_GRADE && f.AssociatedField == AssociatedFieldId)
                {
                    gradeFieldNames.Add(f.Name);
                }
            }
            return gradeFieldNames;
        }

        public Product GetProductById(int Id)
        {
            foreach (Product product in products)
            {
                if (product.Id == Id)
                {
                    return product;
                }
            }
            return null;
        }

        public Product GetProductByName(String name)
        {
            foreach (Product product in products)
            {
                if (product.Name.Equals(name))
                {
                    return product;
                }
            }
            return null;
        }

        public List<ProductJoinGradeAliasing> GetGradeAlisesByProductJoinName(String Name)
        {          
            return new ProductJoinDataAccess().GetGradeAliases(Name, this.ProjectId);
        }

        public List<String> GetProductsInProductJoin(String productJoinName)
        {       
            return new ProductJoinDataAccess().GetProductsInJoin(productJoinName);
        }

        public List<ProcessLimit> GetProcessLimtis()
        {
            return processLimits;
        }

        public List<GradeLimit> GetGradeLimtis()
        {
            return gradeLimits;
        }

        public Dictionary<long, List<int>> GetBlockProcessMapping() {
            return blockProcessMapping;
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
        public List<BlockPosition> GetBlockPositions(int modelId, String condition)
        {
            return new BlockDataAccess().GetBlockPositions(ProjectId, modelId, condition);
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

                        return (Decimal)b.data[expression.Name];                      
                    }
                }
            }
            return 0;
        }

        public Decimal GetFieldValueforBlock(Block b, String fieldName)
        {
            String value = (String)b.data[fieldName];
            if (value != null)
            {
                return Decimal.Parse(value);
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

        public Boolean IsValid(BlockPosition bp, int modelId)
        {
            Boolean isValid = true;
            if(_benchConstraintEnabled)
            {
                Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks = GetBlocks(modelId);
                Dictionary<int, Block> verticalBlocks = blocks[bp.I][bp.J];
                int maxKValue = verticalBlocks.Keys.Max();
                if (bp.K <= (maxKValue - _benchConstraint))
                {
                    isValid = false;
                }
            }
            return isValid;
        }
        public Boolean IsValid(Block b, int modelId)
        {
            Boolean isValid = true;
            if (_benchConstraintEnabled)
            {
                int i = (int)b.data["I"];
                int j = (int)b.data["J"];
                int k = (int)b.data["K"];

                Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks = GetBlocks(modelId);
                Dictionary<int, Block> verticalBlocks = blocks[i][j];
                int maxKValue = verticalBlocks.Keys.Max();
                if (k <= (maxKValue - _benchConstraint))
                {
                    isValid = false;
                }
            }
            return isValid;
        }
    }
}
