using System;
using System.Linq;
using System.Text;

namespace Lopushok.LopushokDataBase.Entities;

partial class Product
{
    public string TitleForView
    {
        get => $"{ProductType?.Title} | {Title}";
    }

    public string MaterialForView
    {
        get
        {
            var materailBuilder = new StringBuilder("Материалы: ");

            foreach (var productMaterial in ProductMaterials)
            {
                materailBuilder.Append(productMaterial.Material.Title + ", ");
            }

            var lastIndex = materailBuilder.ToString().LastIndexOf(',');
            return lastIndex == -1
                ? ""
                : materailBuilder.Remove(lastIndex, 2).ToString();
        }
    }

    public decimal CostForView
    {
        get
        {
            var cost = 0m;

            foreach (var productMaterial in ProductMaterials)
            {
                cost += productMaterial.Material.Cost * (int)productMaterial.Count!;
            }

            return cost;
        }
    }

    public string ImageForView
    {
        get => !string.IsNullOrEmpty(Image)
            ? $"./Pictures/{Image}"
            : "./Pictures/picture.png";
    }

    public string ColorSalesForView
    {
        get => ProductSales.Any(ps => ps.SaleDate.AddMonths(1) > DateTime.Now)
            ? "#ff4040"
            : "#00CC76";
    }
}