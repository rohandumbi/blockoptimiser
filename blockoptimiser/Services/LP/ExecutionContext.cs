using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using System;
using System.Collections.Generic;
using System.IO;
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


        private List<Model> models;
        private List<Field> fields;
        private List<Expression> expressions;
        private List<Process> processes;
        private List<Product> products;
        private List<String> processJoins;
        private List<Opex> opexList;
        private List<ProcessLimit> processLimits;
        private List<GradeLimit> gradeLimits;
        private List<BenchLimit> benchLimits;
        private Dictionary<int, BenchLimit> modelBenchLimitMapping;
        private Dictionary<String, String> requiredFields;
        private Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Block>>>> Blocks { get; set; }
        private Dictionary<String, int> BenchesMinedinPeriod;
        private Dictionary<int, List<long>> LastPeriodMinedBlocks;
        public ExecutionContext(RunConfig runconfig)
        {
            this.ProjectId = runconfig.ProjectId;
            this.ScenarioId = runconfig.ScenarioId;
            this.DiscountFactor = (decimal)runconfig.DiscountFactor / 100;
            SchedulerResultDataAccess schedulerResultDataAccess = new SchedulerResultDataAccess();

            List<RequiredFieldMapping> requiredFieldMappings = new RequiredFieldMappingDataAccess().GetAll(ProjectId);
            requiredFields = new Dictionary<string, string>();
            foreach(RequiredFieldMapping mapping in requiredFieldMappings)
            {
                requiredFields.Add(mapping.RequiredFieldName, mapping.MappedColumnName);
            }
            BenchesMinedinPeriod = new Dictionary<String, int>();
            LoadConfigurations();
            schedulerResultDataAccess.Create(ProjectId);
            LoadValidBlocks();
        }
        private void LoadConfigurations()
        {
            models = new ModelDataAccess().GetAll(ProjectId);
            fields = new FieldDataAccess().GetAll(ProjectId);
            expressions = new ExpressionDataAccess().GetAll(ProjectId);
            processes = new ProcessDataAccess().GetAll(ProjectId);
            products = new ProductDataAccess().GetAll(ProjectId);
            processJoins = new ProcessJoinDataAccess().GetProcessJoins(ProjectId);
            opexList = new OpexDataAccess().GetAll(ScenarioId);
            processLimits = new ProcessLimitDataAccess().GetAll(ScenarioId);
            gradeLimits = new GradeLimitDataAccess().GetAll(ScenarioId);
            benchLimits = new BenchLimitDataAccess().GetAll(ScenarioId);
            modelBenchLimitMapping = new Dictionary<int, BenchLimit>();
            foreach(BenchLimit benchLimit in benchLimits)
            {
                if(!modelBenchLimitMapping.ContainsKey(benchLimit.ModelId))
                {
                    modelBenchLimitMapping.Add(benchLimit.ModelId, benchLimit);
                }
            }
        }

        private void LoadValidBlocks()
        {
            Blocks = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Block>>>>();
            BlockDataAccess blockDataAccess = new BlockDataAccess();
            foreach (Model model in models)
            {
                Dictionary<int, Dictionary<int, Dictionary<int, Block>>> validBlocks = new Dictionary<int, Dictionary<int, Dictionary<int, Block>>>();
                Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocksInModel = blockDataAccess.GetBlocks(this.ProjectId, model.Id);
                List<BlockPosition> processBlockPositions = GetProcessBlocksPositions(model.Id);

                // Add All process blocks to valid list of blocks
                foreach(BlockPosition bp in processBlockPositions)
                {
                    Block b = blocksInModel[bp.K][bp.I][bp.J];
                    b.Processes = bp.Processes;
                    
                    AddBlockToDictionary(validBlocks, bp.I, bp.J, bp.K, b);
                }

                decimal xinc = 0, yinc = 0, zinc = 0, max_dim = 0;
                decimal xm = 0, ym = 0, zm = 0;
                List<ModelDimension> modelDimensions = GetModelDimension(model.Id);
                foreach (ModelDimension dimension in modelDimensions)
                {
                    if (dimension.Type.Equals("Origin"))
                    {
                        xm = dimension.XDim;
                        ym = dimension.YDim;
                        zm = dimension.ZDim;
                    }
                    else if (dimension.Type.Equals("Dimensions"))
                    {
                        xinc = dimension.XDim;
                        yinc = dimension.YDim;
                        zinc = dimension.ZDim;
                        if (xinc > yinc)
                        {
                            max_dim = xinc;
                        }
                        else
                        {
                            max_dim = yinc;
                        }
                    }
                }

                Geotech geotech = GetGeotechByModel(model.Id);
                String selectstr = "max ( ";
                if (!geotech.UseScript)
                {
                    String columnName = GetColumnNameById(geotech.Id, model.Id);
                    if (columnName == null)
                    {
                        throw new Exception("Please check your geotech configuration.");
                    }
                    selectstr = selectstr + GetColumnNameById(geotech.Id, model.Id) + ") as angle";
                }
                else
                {
                    selectstr = selectstr + geotech.Script + ") as angle";
                }

                double max_ira = GetIRA(selectstr, model.Id) * Math.PI / 180; ;

                int nbenches = (int)Math.Ceiling((max_dim / 2) / (zinc / (decimal)Math.Tan(max_ira)));

                foreach (int kk in GetSortedList(validBlocks.Keys.ToList()))
                {
                    foreach (int ii in validBlocks[kk].Keys.ToList())
                    {
                        foreach (int jj in validBlocks[kk][ii].Keys.ToList())
                        {
                            Block b = validBlocks[kk][ii][jj];
                            decimal xorth = (decimal)b.data["Xortho"];
                            decimal yorth = (decimal)b.data["Yortho"];


                            for (int k = 1; k <= nbenches; k++)
                            {
                                decimal dist = k * zinc / (decimal)Math.Tan(max_ira);
                                int imin = (int)((xorth - dist + xinc - xm) / xinc);
                                int imax = (int)((xorth + dist + xinc - xm) / xinc);
                                int jmin = (int)((yorth - dist + yinc - ym) / yinc);
                                int jmax = (int)((yorth + dist + yinc - ym) / yinc);
                                if (imin <= 0) imin = 1;
                                if (jmin <= 0) jmin = 1;
                                //Console.WriteLine("Block :" + b.Id + " imin: " + imin + " imax: " + imax + " jmin: " + jmin + " jmax: " + jmax + " nbneches:" + nbenches);
                                for (int i = imin; i <= imax; i++)
                                {
                                    for (int j = jmin; j <= jmax; j++)
                                    {
                                        if (!blocksInModel.ContainsKey(kk + k) || !blocksInModel[kk + k].ContainsKey(i) || !blocksInModel[kk + k][i].ContainsKey(j))
                                        {
                                            continue;
                                        }
                                        Block ub = blocksInModel[kk + k][i][j];
                                        AddBlockToDictionary(validBlocks, i, j, kk + k, ub);
                                        if(b.DependentBlocks == null)
                                        {
                                            b.DependentBlocks = new List<Block>();
                                        }
                                        b.DependentBlocks.Add(ub);
                                    }
                                }
                            }
                        }
                    }
                }
                // Change way of accessing blocks
                Dictionary<int, Dictionary<int, Dictionary<int, Block>>> UpdatedBlocks = new Dictionary<int, Dictionary<int, Dictionary<int, Block>>>();
                foreach (int kk in validBlocks.Keys.ToList())
                {
                    foreach (int ii in validBlocks[kk].Keys.ToList())
                    {
                        foreach (int jj in validBlocks[kk][ii].Keys.ToList())
                        {
                            Block b = validBlocks[kk][ii][jj];
                            AddBlockToDictionaryByX(UpdatedBlocks, ii, jj, kk, b);
                        }
                    }
                }

                Blocks.Add(model.Id, UpdatedBlocks);
            }
            //PrintBlocks();
        }

        /*
         * This function will iterate trough all the blocks and mark them as mined or ready for mining
         */
        public void UpdateBlocks() 
        {
            
            foreach (Model model in models)
            {
                List<long> minedBlocks = null;
                if (LastPeriodMinedBlocks != null && LastPeriodMinedBlocks.ContainsKey(model.Id))
                {
                    minedBlocks = LastPeriodMinedBlocks[model.Id];
                }
                else
                {
                    minedBlocks = new List<long>();
                }

                int benchConstraint = -1;
                if (modelBenchLimitMapping.ContainsKey(model.Id))
                {
                    BenchLimit benchLimit = modelBenchLimitMapping[model.Id];
                    if (benchLimit.IsUsed)
                    {
                        benchConstraint = benchLimit.Value;
                    }
                }
                Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocksinModel = Blocks[model.Id];
                foreach (int i in blocksinModel.Keys)
                {
                    foreach (int j in blocksinModel[i].Keys)
                    {
                        Dictionary<int, Block> zblocks = blocksinModel[i][j];
                        List<int> keys = zblocks.Keys.ToList();
                        keys.Sort();
                        keys.Reverse();
                        int includedBlockCount = 0;
                        int countFromFirstProcessBlock = 0;
                        int periodMinedLast = 0; 
                        if(BenchesMinedinPeriod.ContainsKey(model.Id + "-" + i + "-" + j))
                        {
                            periodMinedLast = BenchesMinedinPeriod[model.Id + "-" + i + "-" + j];
                        }
                        if(this.Period > 1 && periodMinedLast == 0)
                        {
                            periodMinedLast = 1;
                        }
                        foreach(int k in keys)
                        {
                            Block b = zblocks[k];
                            if (b.IsMined) continue;
                            if(minedBlocks.Contains(b.Id))
                            {
                                b.IsMined = true;
                                continue;
                            }
                            if(benchConstraint > 0 && ((countFromFirstProcessBlock == benchConstraint) || (includedBlockCount == (Period - periodMinedLast) * benchConstraint)))
                            {
                                break;
                            }
                            b.IsIncluded = true;
                            includedBlockCount++;
                            if (b.IsProcessBlock && countFromFirstProcessBlock == 0)
                            {
                                countFromFirstProcessBlock++;
                            }
                            if (countFromFirstProcessBlock > 0) countFromFirstProcessBlock++;
                        }
                    }
                }
                foreach (int i in blocksinModel.Keys)
                {
                    foreach (int j in blocksinModel[i].Keys)
                    {
                        foreach (int k in blocksinModel[i][j].Keys)
                        {
                            Block b = blocksinModel[i][j][k];
                            if (b.IsMined || !b.IsIncluded || b.DependentBlocks == null ) continue;
                            foreach(Block db in b.DependentBlocks)
                            {
                                if(!db.IsIncluded && !db.IsMined)
                                {
                                    b.IsIncluded = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddBlockToDictionary(Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks, int i, int j, int k, Block block)
        {
            if (!blocks.ContainsKey(k))
            {
                Dictionary<int, Block> yblocks = new Dictionary<int, Block>();
                Dictionary<int, Dictionary<int, Block>> xblocks = new Dictionary<int, Dictionary<int, Block>>();
                yblocks.Add(j, block);
                xblocks.Add(i, yblocks);
                blocks.Add(k, xblocks);
            }
            else
            {
                Dictionary<int, Dictionary<int, Block>> xblocks = blocks[k];
                if (!xblocks.ContainsKey(i))
                {
                    Dictionary<int, Block> yblocks = new Dictionary<int, Block>();
                    yblocks.Add(j, block);
                    xblocks.Add(i, yblocks);
                }
                else
                {
                    Dictionary<int, Block> yblocks = xblocks[i];
                    if (!yblocks.ContainsKey(j))
                    {
                        yblocks.Add(j, block);
                    }
                }
            }
        }

        private void AddBlockToDictionaryByX(Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks, int i, int j, int k, Block block)
        {
            if (!blocks.ContainsKey(i))
            {
                Dictionary<int, Block> zblocks = new Dictionary<int, Block>();
                Dictionary<int, Dictionary<int, Block>> yblocks = new Dictionary<int, Dictionary<int, Block>>();
                zblocks.Add(k, block);
                yblocks.Add(j, zblocks);
                blocks.Add(i, yblocks);
            }
            else
            {
                Dictionary<int, Dictionary<int, Block>> yblocks = blocks[i];
                if (!yblocks.ContainsKey(j))
                {
                    Dictionary<int, Block> zblocks = new Dictionary<int, Block>();
                    zblocks.Add(k, block);
                    yblocks.Add(j, zblocks);
                }
                else
                {
                    Dictionary<int, Block> zblocks = yblocks[j];
                    if (!zblocks.ContainsKey(k))
                    {
                        zblocks.Add(k, block);
                    }
                }
            }
        }
        private List<BlockPosition> GetProcessBlocksPositions(int modelId)
        {
            List<BlockPosition> blockPositions = new List<BlockPosition>();
            BlockDataAccess blockDataAccess = new BlockDataAccess();
            foreach (Process process in processes)
            {
                foreach (ProcessModelMapping mapping in process.Mapping)
                {
                    if(mapping.ModelId == modelId)
                    {
                        List<BlockPosition> procesBlockPositions = blockDataAccess.GetBlockPositions(ProjectId, mapping.ModelId, mapping.FilterString);
                        foreach(BlockPosition blockPosition in procesBlockPositions)
                        {
                            Boolean exists = false;
                            foreach(BlockPosition bp in blockPositions)
                            {
                                if(bp.Bid == blockPosition.Bid)
                                {
                                    exists = true;
                                    bp.Processes.Add(process);
                                    break;
                                }
                            }
                            if(!exists)
                            {
                                blockPosition.Processes = new List<Process> { process };
                                blockPositions.Add(blockPosition);
                            }
                        }
                    }
                }
            }

            return blockPositions;
        }

        private void PrintBlocks()
        {
            Directory.CreateDirectory(@"C:\\blockoptimiser");
            FileStream fs = File.Create("C:\\blockoptimiser\\blockoptimiser-blocks.csv");
            using (StreamWriter sw = new StreamWriter(fs))
            {
                foreach (int key in Blocks.Keys)
                {
                    Dictionary<int, Dictionary<int, Dictionary<int, Block>>> validBlocks = Blocks[key];

                    foreach (int kk in validBlocks.Keys)
                    {
                        foreach (int ii in validBlocks[kk].Keys)
                        {
                            foreach (int jj in validBlocks[kk][ii].Keys)
                            {
                                Block b = validBlocks[kk][ii][jj];
                                int i = (int)b.data["I"];
                                int j = (int)b.data["J"];
                                int k = (int)b.data["K"];
                                String xcentre = (String)b.data["xcentre"];
                                String ycentre = (String)b.data["ycentre"];
                                String zcentre = (String)b.data["zcentre"];
                                Boolean isProcessBlock = ((b.Processes != null) && (b.Processes.Count > 0));
                                String line = b.Id + "," + i + "," + j + "," + k + "," + xcentre + "," + ycentre + "," + zcentre+","+isProcessBlock;
                                sw.WriteLine(line);
                            }
                        }
                    }
                }
            }
        }
        public void ProcessMinedBlocks()
        {
            LastPeriodMinedBlocks = new Dictionary<int, List<long>>();
            SchedulerResultDataAccess schedulerResultDataAccess = new SchedulerResultDataAccess();
            foreach(Model model in models)
            {
                int benchConstraint = -1;
                if (modelBenchLimitMapping.ContainsKey(model.Id))
                {
                    BenchLimit benchLimit = modelBenchLimitMapping[model.Id];
                    if (benchLimit.IsUsed)
                    {
                        benchConstraint = benchLimit.Value;
                    }
                }
                List<long> lastYearMinedBlockIds = new List<long>();
                Dictionary<int, Dictionary<int, Dictionary<int, MinedBlock>>> minedBlockMap = schedulerResultDataAccess.GetMinedBlocks(ProjectId, model.Id, this.Year);
                List<MinedBlock> updatedBlocks = new List<MinedBlock>();
                foreach(int i in minedBlockMap.Keys)
                {
                    foreach (int j in minedBlockMap[i].Keys)
                    {
                        Dictionary<int, MinedBlock> zblocks = minedBlockMap[i][j];
                        List<int> keys = GetSortedList(zblocks.Keys.ToList());
                        //keys.Reverse();
                        for(int count = 0; count < keys.Count; count ++ )
                        {
                            int k = keys.ElementAt(count);
                            MinedBlock minedBlock = zblocks[k];
                            lastYearMinedBlockIds.Add(minedBlock.Bid);
                            String key = model.Id + "-" + minedBlock.I + "-" + minedBlock.J;
                            if (BenchesMinedinPeriod.ContainsKey(key))
                            {
                                BenchesMinedinPeriod[key] = Period;
                            }
                            else
                            {
                                BenchesMinedinPeriod.Add(key, Period);
                            }
                            if(benchConstraint > 0 )
                            {
                                int factor = count / benchConstraint;
                                minedBlock.Year = minedBlock.Year - factor;
                                updatedBlocks.Add(minedBlock);
                            }
                            
                        }                      
                    }
                }
                schedulerResultDataAccess.UpdateYear(ProjectId, updatedBlocks);
                LastPeriodMinedBlocks.Add(model.Id, lastYearMinedBlockIds);
            }          
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

        public Boolean IsValid(Block b, int modelId)
        {
            /*if (modelBenchLimitMapping.ContainsKey(modelId))
            {
                BenchLimit benchLimit = modelBenchLimitMapping[modelId];
                if (benchLimit.IsUsed)
                {
                    int i = (int)b.data["I"];
                    int j = (int)b.data["J"];
                    int k = (int)b.data["K"];

                    Dictionary<int, Dictionary<int, Dictionary<int, Block>>> blocks = GetBlocks(modelId);
                    Dictionary<int, Block> verticalBlocks = blocks[i][j];
                    int maxKValue = verticalBlocks.Keys.Max();
                    if (k <= (maxKValue - benchLimit.Value))
                    {
                        isValid = false;
                    }
                }
            }*/
            return !b.IsMined && b.IsIncluded;
        }

        private List<int> GetSortedList(List<int> list)
        {
            list.Sort();
            return list;
        }
    }
}
