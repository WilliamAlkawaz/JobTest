using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesApiController : ControllerBase
    {
        private readonly JobTestDB _context;

        public CategoriesApiController(JobTestDB context)
        {
            _context = context;
        }

        // GET: api/CategoriesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = (await _context.Categories.ToListAsync()).OrderBy(o => o.Min).ToList(); 
            return categories;
        }

        // GET: api/CategoriesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/CategoriesApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id)
        {
            Category origianCategory = _context.Categories.Find(id);
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            IFormFile file;
            byte[] photo;
            string ImageMimeType; 
            if (Request.Form.Files.Count !=0)
            {
                file = Request.Form.Files[0];
                photo = await GetByteArrayFromImageAsync(Request.Form.Files[0]);
                ImageMimeType = file.ContentType;
            }
            else
            {
                photo = origianCategory.PhotoFile;
                ImageMimeType = origianCategory.ImageMimeType; 
            }            
            
            var name = dict["Name"];
            
            
            Category category = new Category()
            {
                Category_ID = int.Parse(dict["Category_ID"]),
                Name = dict["Name"],
                Min = int.Parse(dict["Min"]),
                Max = int.Parse(dict["Max"]), 
                PhotoFile = photo, 
                ImageMimeType = ImageMimeType
            };
            if (id != category.Category_ID)
            {
                return BadRequest();
            }

            //_context.Entry(category).State = EntityState.Modified;
            _context.Entry(origianCategory).CurrentValues.SetValues(category);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CategoriesApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory()
        {
            // To create a category: 
            // 1. Adjust ranges based on the algoirhtm we already specified. 
            // 2. Add new category we the ranges given from the algorithm we already specified. 
            // 3. Update vehiles categories. 

            // 1. Adjust ranges: 
            List<Category> categories = (await _context.Categories.ToListAsync()).OrderBy(o => o.Min).ToList();
            // Fetch the last category: 
            var lastCategory = categories[categories.Count - 1];
            int newCategoryMax = lastCategory.Max; 
            int common = lastCategory.Min + (lastCategory.Max - lastCategory.Min) / 2;
            await UpdateCategoryRange(lastCategory.Category_ID, lastCategory.Min, common); 
            // 2. Add new category: 
            var file = Request.Form.Files[0];
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            var name = dict["Name"];
            var photo = await GetByteArrayFromImageAsync(Request.Form.Files[0]);
            var ImageMimeType = file.ContentType;
            Category category = new Category()
            {
                Name = dict["Name"],
                Min = common,
                Max = newCategoryMax,
                PhotoFile = photo,
                ImageMimeType = ImageMimeType
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // 3. Update vehicles category
            await UpdateVehicles();

            // To get rid of the 500 error, comment this line and return NoContent(). 
            //return CreatedAtAction("GetCategory", new { id = category.Category_ID }, category);
            return NoContent();
        }

        // DELETE: api/CategoriesApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            // To delete a category:
            // 1. Fetch all the vehicles under this category. 
            // 2. Adjust the ranges first based on the algorithm we already specified.
            // 3. Delete the category. 
            // 4. Assign the vehicles fetched in (1) to new categories based on their weight.
            // 5. Since categories ranges have been modified, we need to loop through all
            //    the categories and make sure that the have the right vehicles under them 
            //    based on the vehicles weight. 
            
            // 1. Fetch the vehicles: 
            var vehicles = _context.Vehicles.Where(x => x.Category_ID == id).ToList();

            // 2. Adjust the ranges: 
            List<Category> categories = (await _context.Categories.ToListAsync()).OrderBy(o=>o.Min).ToList(); 
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].Category_ID == id)
                {
                    if (i == 0)
                    {
                        await UpdateCategoryRange(categories[i + 1].Category_ID, 0, categories[i + 1].Max);
                    }
                    else if (i == (categories.Count - 1))
                    {
                        await UpdateCategoryRange(categories[i - 1].Category_ID, categories[i - 1].Min, categories[i].Max);
                    }
                    else
                    {
                        int common = categories[i - 1].Max + (int)((categories[i].Max - categories[i].Min) / 2);
                        await UpdateCategoryRange(categories[i - 1].Category_ID, categories[i - 1].Min, common);
                        await UpdateCategoryRange(categories[i + 1].Category_ID, common, categories[i + 1].Max);
                    }
                    break;
                }
            }

            // 3. Delete the category: 
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            // 4. Assign the vehicles fetched in (1) to diffrent categories:
            foreach(var v in vehicles)
            {
                var cat = await GetCategoryByWeight(v.Weight); 
                // I know it's better to check if cat is null but at this stage I'd prefer to 
                // see exceptions. 
                v.Category = null; 
                v.Category = cat;
                v.Category_ID = cat.Category_ID;

                if (await TryUpdateModelAsync<Vehicle>(v,"",
                    c => c.Category, c => c.Category_ID))
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        ModelState.AddModelError("", "Unable to save Changes.");
                    }
                }
                    
            }

            // 5. Update vehicles category
            await UpdateVehicles(); 
            return NoContent();
        }

        // This function is to receive new updated ranges from the front end and updates 
        // categories ranges. 
        [HttpPost()]
        [Route("PostNewRanges")]
        public async Task<bool> PostNewRanges()
        {
            // To update ranges: 
            // 1. Update each category ranges. 
            // 2. Update vehicle range. 

            //1. Update each category ranges:
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            var str = dict["Ranges"];
            var ranges = ((str ?? "").Split(',').Select<string, int>(int.Parse)).ToList();
            List<Category> categories = (await _context.Categories.ToListAsync()).OrderBy(o=>o.Min).ToList(); 
            for(int i = 0; i<categories.Count(); i++)
            {
                await UpdateCategoryRange(categories[i].Category_ID, ranges[i], ranges[i + 1]);
            }

            // 2. Update vehicle range: 
            await UpdateVehicles(); 
            return true; 
        }
        // This function is to update a category range. It takes id, min, and max. ID is used 
        // to fetch the category and min & max is used to adjust the ranges. 
        private async Task<bool> UpdateCategoryRange(int id, int min, int max)
        {
            var category = await _context.Categories.FindAsync(id);
            category.Min = min;
            category.Max = max;
            _context.Entry(category).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }
        private async Task<byte[]> GetByteArrayFromImageAsync(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                await file.CopyToAsync(target);
                return target.ToArray();
            }
        }

        // This function is to make sure that all the vehicles are assigned to the correct 
        // category. It loops through all the categories rather than the vehicles as categories
        // table is unlikely to grow large. 
        //[HttpGet("UpdateVehicles")]
        private async Task<bool> UpdateVehicles()
        {
            var categories = await _context.Categories.ToListAsync(); 
            foreach(var cat in categories)
            {
                await UpdateVehiclesCategory(cat);
            }
            return true; 
        }

        // This function is to check if a vehicles under a category of Category_ID = id are 
        // assigned correctly. If vehicles are not assigned correctly, it reassign them. 
        private async Task<bool> UpdateVehiclesCategory(Category category)
        {
            List<Vehicle> vehicles = await _context.Vehicles.Where(vehicle => vehicle.Category_ID == category.Category_ID).ToListAsync(); 
            foreach(var vehicle in vehicles)
            {
                var cat = await GetCategoryByWeight(vehicle.Weight);
                if(cat==null)
                {
                    throw new ArgumentNullException(); 
                }
                vehicle.Category = null;
                vehicle.Category = cat;
                vehicle.Category_ID = cat.Category_ID;

                if (await TryUpdateModelAsync<Vehicle>(vehicle, "",
                c => c.Category, c => c.Category_ID))
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        ModelState.AddModelError("", "Unable to save Changes.");
                    }
                }
            }
            return true; 
        }

        // This function returs a category based on vehicles weight 
        public async Task<Category> GetCategoryByWeight(decimal weight)
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            foreach (var category in categories)
            {
                if (category.Min < weight && weight <= category.Max)
                    return category;
            }
            return null;
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Category_ID == id);
        }
    }
}
