﻿using blockoptimiser.DataAccessClasses;
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
using System.Windows.Shapes;

namespace blockoptimiser.Views
{
    /// <summary>
    /// Interaction logic for ProductJoinDefinitionView.xaml
    /// </summary>
    public partial class ProductJoinDefinitionView : Window
    {
        List<Product> Products;
        ProductDataAccess ProductDAO;
        ProductJoinDataAccess ProductJoinDAO;
        public ProductJoinDefinitionView()
        {
            InitializeComponent();
            ProductDAO = new ProductDataAccess();
            ProductJoinDAO = new ProductJoinDataAccess();
            Products = ProductDAO.GetAll(Context.ProjectId);
            BindDropDown();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            String ProductJoinName = Name.Text;
            if (ProductJoinName == null)
            {
                MessageBox.Show("Enter manadatory field NAME");
                return;
            }
            if (testListbox.Items.Count == 0)
            {
                MessageBox.Show("Select atleast one Product");
                return;
            }

            ProductJoin NewProductJoin = new ProductJoin();
            NewProductJoin.Name = ProductJoinName;
            NewProductJoin.ProjectId = Context.ProjectId;
            NewProductJoin.ProductNames = new List<string>();
            foreach (String ChildProductName in testListbox.Items)
            {
                NewProductJoin.ProductNames.Add(ChildProductName);
            }
            ProductJoinDAO.Insert(NewProductJoin);
            this.Close();
        }

        private void BindDropDown()
        {
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
            BindListBOX();
        }

        private void BindListBOX()
        {
            testListbox.Items.Clear();
            foreach (var product in Products)
            {
                if (product.Check_Status == true)
                {
                    testListbox.Items.Add(product.Name);
                }
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
