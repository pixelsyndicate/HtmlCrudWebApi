using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;

#pragma warning disable 1591

namespace webdemo.DAL
{
    /// <summary>
    /// This extension allows me to add additional functionality to the DbContext concrete model generated earlier.
    /// Override the ValidateEntity method to add my own customer validations.
    /// </summary>
    public partial class EfProducts
    {

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {

            // add my own custom stuff here

            // review the Type
            if (entityEntry.Entity is ProductEntity)
            {
                ProductEntity entity = entityEntry.Entity as ProductEntity;

                var list = ValidateProduct(entity);

                if (list.Any())
                {
                    return new DbEntityValidationResult(entityEntry, list);
                }
            }

            return base.ValidateEntity(entityEntry, items);
        }

        protected List<DbValidationError> ValidateProduct(ProductEntity entity)
        {
            List<DbValidationError> list = new List<DbValidationError>();

            // put in here all of the previous controller Validate() method processes

            if (string.IsNullOrEmpty(entity.ProductName))
                list.Add(new DbValidationError("ProductName", "Product Name must be filled in."));
            else
            {
                if (entity.ProductName.ToLower() == entity.ProductName)
                    list.Add(new DbValidationError("ProductName", "Product Name must not be all lower case."));

                if (entity.ProductName.Length < 4 || entity.ProductName.Length > 150)
                    list.Add(new DbValidationError("ProductName", "Product Name must have between 4 and 150 characters"));
            }



            // introduction date must be within last 5 years.
            int ageY = 5;
            if (entity.IntroductionDate < DateTime.Now.AddYears(-ageY))
                list.Add(new DbValidationError("Introduction Date", $"Introduction Date Must be within the last {ageY} years"));


            // check price
            if (!entity.Price.HasValue || entity.Price.Value <= Convert.ToDouble(0) || entity.Price.Value > Convert.ToDouble(9999.99))
                list.Add(new DbValidationError("Price", $"Price Must be between $0 and $10,000."));


            // check url
            if (string.IsNullOrEmpty(entity.Url))
                list.Add(new DbValidationError("Url", "Url must be filled in."));
            else if (entity.Url.Length < 5 || entity.Url.Length > 255)
                list.Add(new DbValidationError("Url", "Url must be between 5 and 255 characters."));



            return list;
        }


    }
}