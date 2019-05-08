using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using blockoptimiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace blockoptimiser.Views
{
    /// <summary>
    /// Interaction logic for V2GradeLimitView.xaml
    /// </summary>
    public partial class V2GradeLimitView : UserControl
    {
        ProductDataAccess ProductDAO;
        ProductJoinDataAccess ProductJoinDAO;
        FieldDataAccess FieldDAO;
        List<Product> Products;
        List<ProductJoin> ProductJoins;
        List<Field> Fields;

        public V2GradeLimitView()
        {
            ProductDAO = new ProductDataAccess();
            ProductJoinDAO = new ProductJoinDataAccess();
            FieldDAO = new FieldDataAccess();
            Products = ProductDAO.GetAll(Context.ProjectId);
            Fields = FieldDAO.GetAll(Context.ProjectId);
            ProductJoins = new List<ProductJoin>();
            InitializeComponent();
            //temp hack
            Context.ScenarioId = 1;
            if (!(Context.ScenarioId > 0))
            {
                MessageBox.Show("Please select a period.");
                return;
            }
            this.DataContext = new GradeLimitViewModel();
        }

        private void AddGradeLimit(object sender, RoutedEventArgs e)
        {
            ((GradeLimitViewModel)this.DataContext).CreateGradeLimit();
        }
        private void Item_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox itemCombo = (ComboBox)sender;
            UnitItem SelectedItem = (UnitItem)itemCombo.SelectedItem;
            if (SelectedItem.UnitType == GradeLimit.ITEM_TYPE_PRODUCT)
            {
                Product SelectedProduct = GetProductById(SelectedItem.UnitId);
                if (SelectedProduct != null)
                {
                    GradeCombo.ItemsSource = GetAssociatedGrades(SelectedProduct);
                }
            }
            else if (SelectedItem.UnitType == GradeLimit.ITEM_TYPE_PRODUCT_JOIN)
            {
                GradeCombo.ItemsSource = ProductJoinDAO.GetGradeAliasNames(SelectedItem.Name, Context.ProjectId);
            }
        }

        private List<String> GetAssociatedGrades(Product selectedProduct)
        {
            List<String> associatedGradeNames = new List<string>();
            foreach (Field field in Fields)
            {
                if (field.DataType == Field.DATA_TYPE_GRADE && field.AssociatedField == selectedProduct.UnitId)
                {
                    associatedGradeNames.Add(field.Name);
                }
            }
            return associatedGradeNames;
        }

        private Product GetProductById(int id)
        {
            Product returnedProduct = null;
            foreach (Product product in Products)
            {
                if (product.Id == id)
                {
                    returnedProduct = product;
                    break;
                }
            }
            return returnedProduct;
        }
    }
}
