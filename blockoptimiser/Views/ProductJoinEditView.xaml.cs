using blockoptimiser.DataAccessClasses;
using blockoptimiser.Models;
using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace blockoptimiser.Views
{
    /// <summary>
    /// Interaction logic for ProductJoinDefinitionView.xaml
    /// </summary>
    public partial class ProductJoinEditView : MetroWindow
    {
        List<Product> Products;
        ProductDataAccess ProductDAO;
        ProductJoinDataAccess ProductJoinDAO;
        public List<String> ProductJoinGradeAliasNames;
        private ProductJoin UpdatedProductJoin;
        private ProductJoinDataAccess productJoinDAO;
        public ProductJoinEditView(String productJoinName)
        {
            productJoinDAO = new ProductJoinDataAccess();
            UpdatedProductJoin = new ProductJoin();
            UpdatedProductJoin.ProjectId = Context.ProjectId;
            UpdatedProductJoin.Name = productJoinName;
            UpdatedProductJoin.ProductNames = productJoinDAO.GetProductsInJoin(productJoinName);
            UpdatedProductJoin.ProductJoinGradeAliasings = productJoinDAO.GetGradeAliases(productJoinName, Context.ProjectId);
            InitializeComponent();
            ProductDAO = new ProductDataAccess();
            ProductJoinDAO = new ProductJoinDataAccess();
            Products = ProductDAO.GetAll(Context.ProjectId);
            ProductJoinGradeAliasNames = ProductJoinDAO.GetGradeAliasNames(productJoinName, Context.ProjectId);
            BindDropDown();
            BindListBox();
            BindGradeListBox();
            Name.Text = UpdatedProductJoin.Name;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            String ProductJoinName = Name.Text;
            if (ProductJoinName == null || ProductJoinName == "")
            {
                MessageBox.Show("Enter manadatory field NAME");
                return;
            }
            if (productListbox.Items.Count == 0)
            {
                MessageBox.Show("Select atleast one Product");
                return;
            }
            try {
                ProductJoinDAO.Delete(ProductJoinName);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            UpdatedProductJoin.Name = ProductJoinName;
            UpdatedProductJoin.ProjectId = Context.ProjectId;
            UpdatedProductJoin.ProductNames = new List<string>();
            foreach (String ChildProductName in productListbox.Items)
            {
                UpdatedProductJoin.ProductNames.Add(ChildProductName);
            }
            List<ProductJoinGradeAliasing> ProductJoinGradeAliasings = new List<ProductJoinGradeAliasing>();
            for (int i = 0; i < gradeListbox.Items.Count; i++)
            {
                ProductJoinGradeAliasing NewProductJoinGradeAliasing = new ProductJoinGradeAliasing();
                NewProductJoinGradeAliasing.ProjectId = Context.ProjectId;
                NewProductJoinGradeAliasing.ProductJoinName = UpdatedProductJoin.Name;
                NewProductJoinGradeAliasing.GradeAliasName = gradeListbox.Items[i].ToString();
                NewProductJoinGradeAliasing.GradeAliasNameIndex = (i + 1);
                ProductJoinGradeAliasings.Add(NewProductJoinGradeAliasing);
            }
            UpdatedProductJoin.ProductJoinGradeAliasings = ProductJoinGradeAliasings;
            try
            {
                ProductJoinDAO.Insert(UpdatedProductJoin);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.Close();
        }

        private void button_grade_Click(object sender, RoutedEventArgs e)
        {
            String GradeName = GradeAliasName.Text;
            if (GradeName == null || GradeName == "")
            {
                MessageBox.Show("Enter manadatory field Grade Name");
                return;
            }
            ProductJoinGradeAliasNames.Add(GradeName);

            BindGradeListBox();
        }

        private void button_grade_remove_Click(object sender, RoutedEventArgs e)
        {
            if (gradeListbox.SelectedItem == null)
            {
                MessageBox.Show("Select atleast one grade to remove.");
                return;
            }
            String removedGradeName = gradeListbox.SelectedItem.ToString();
            int i = ProductJoinGradeAliasNames.IndexOf(removedGradeName);
            ProductJoinGradeAliasNames.RemoveAt(i);
            gradeListbox.Items.Remove(gradeListbox.SelectedItem);
            BindGradeListBox();
        }

        private void BindDropDown()
        {
            foreach (Product product in Products)
            {
                foreach (String productName in UpdatedProductJoin.ProductNames)
                {
                    if (product.Name == productName)
                    {
                        product.CheckStatus = true;
                    }
                }
            }
            productCombo.ItemsSource = Products;
        }
        private void Product_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Product_TextChanged(object sender, TextChangedEventArgs e)
        {
            productCombo.ItemsSource = Products.Where(x => x.Name.StartsWith(productCombo.Text.Trim()));
        }

        private void AllCheckbocx_CheckedAndUnchecked(object sender, RoutedEventArgs e)
        {
            BindListBox();
        }

        private void BindListBox()
        {
            productListbox.Items.Clear();
            foreach (var product in Products)
            {
                if (product.CheckStatus == true)
                {
                    productListbox.Items.Add(product.Name);
                }
            }
        }

        private void BindGradeListBox()
        {
            gradeListbox.Items.Clear();
            foreach (String aliasName in ProductJoinGradeAliasNames)
            {
                gradeListbox.Items.Add(aliasName);
            }
        }

        private Product GetProductByName(String name)
        {
            Product returnedProduct = Products.First();
            foreach (Product product in Products)
            {
                if (product.Name == name)
                {
                    returnedProduct = product;
                    break;
                }
            }
            return returnedProduct;
        }
    }
}
