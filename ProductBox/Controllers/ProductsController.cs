using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ProductBox.Models;
using ProductBox.Models.Media;
using ProductBox.Models.ViewModels;
using ProductBox.Services;

namespace ProductBox.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private AppDbContext _context;
        private IWebHostEnvironment _hostingEnvironment;
        private IImageManager _imageManager;

        public ProductsController(AppDbContext context, IWebHostEnvironment hostingEnvironment, IImageManager imageManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _imageManager = imageManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var products = _context.Products
                .Include(p => p.Images)
                .Select(i => new {
                i.Id,
                i.Images,
                i.Name,
                i.DateCreated,
                i.Description,
                i.Price,
                i.Quantity
            }).OrderByDescending(i => i.DateCreated);

            return Json(await DataSourceLoader.LoadAsync(products, loadOptions));
        }

        [HttpPost("add")]
        public async Task<IActionResult> Post([FromForm] AddProductViewModel model, [FromForm] IList<IFormFile> files) {
            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var product = new Product();

            var images = await _imageManager.UploadImages(files, "images/products");
            product.AddImages(images);

            product.SetName(model.Name);
            product.SetDescription(model.Description);
            product.SetPrice(model.Price.Value);
            product.SetQuantity(model.Quantity.Value);

            var result = _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromForm] EditProductViewModel model, [FromForm] IList<IFormFile> files) {
            if (!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var product = await _context.Products.FirstOrDefaultAsync(item => item.Id == id);
            if (product == null)
                return NotFound();

            var addedImages = await _imageManager.UploadImages(files, "images/products");
            var deletedImages = await _imageManager.DeleteImages(model.DeletedImageIds, "images/products");

            product.AddImages(addedImages);
            product.RemoveImages(deletedImages);

            if (model.Name != null)
                product.SetName(model.Name);

            if (model.Description != null)
                product.SetDescription(model.Description);

            if (model.Price != null)
                product.SetPrice(model.Price.Value);

            if (model.Quantity != null)
                product.SetQuantity(model.Quantity.Value);

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task Delete(int id) {
            var model = await _context.Products.FirstOrDefaultAsync(item => item.Id == id);

            _context.Products.Remove(model);
            await _context.SaveChangesAsync();
        }


        /*private void PopulateModel(Product model, IDictionary values) {
            string NAME = nameof(Product.Name);
            string DATE_CREATED = nameof(Product.DateCreated);
            string DESCRIPTION = nameof(Product.Description);
            string PRICE = nameof(Product.Price);
            string QUANTITY = nameof(Product.Quantity);

            if(values.Contains(NAME)) {
                model.Name = Convert.ToString(values[NAME]);
            }

            if(values.Contains(DATE_CREATED)) {
                model.DateCreated = Convert.ToDateTime(values[DATE_CREATED]);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }

            if(values.Contains(PRICE)) {
                model.Price = Convert.ToSingle(values[PRICE], CultureInfo.InvariantCulture);
            }

            if(values.Contains(QUANTITY)) {
                model.Quantity = Convert.ToInt32(values[QUANTITY]);
            }
        }*/

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}