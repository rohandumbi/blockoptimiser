using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
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
    /// Interaction logic for GradeLimitView.xaml
    /// </summary>
    public partial class GradeLimitView : UserControl
    {
        ProductDataAccess ProductDAO;
        ProductJoinDataAccess ProductJoinDAO;
        List<Product> Products;
        List<ProductJoin> ProductJoins;
        public GradeLimitView()
        {
            ProductDAO = new ProductDataAccess();
            ProductJoinDAO = new ProductJoinDataAccess();
            Products = ProductDAO.GetAll(Context.ProjectId);
            ProductJoins = new List<ProductJoin>();
            InitializeComponent();
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
                    GradeCombo.ItemsSource = SelectedProduct.GradeNames;
                }
            } else if (SelectedItem.UnitType == GradeLimit.ITEM_TYPE_PRODUCT_JOIN)
            {
                GradeCombo.ItemsSource = ProductJoinDAO.GetGradeAliasNames(SelectedItem.Name, Context.ProjectId);
            }
        }

        private Product GetProductById(int id)
        {
            Product returnedProduct = null;
            foreach (Product product in Products)
            {
                if (product.Id == id) {
                    returnedProduct = product;
                    break;
                }
            }
            return returnedProduct;
        }

    }
}
