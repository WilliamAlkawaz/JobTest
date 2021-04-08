using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;

namespace Project1.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly JobTestDB _context;

        public VehiclesController(JobTestDB context)
        {
            _context = context;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "nameDesc" : "";
            ViewData["ManufacturerSortParam"] = sortOrder == "manufacturer" ? "manDesc" : "manufacturer";
            ViewData["YearSortParam"] = sortOrder == "year" ? "yearDesc" : "year";
            ViewData["WeightSortParam"] = sortOrder == "weight" ? "weightDesc" : "weight"; 

            var vehicles = from v in _context.Vehicles select v; 

            switch(sortOrder)
            {
                case "nameDesc": 
                    vehicles = vehicles.OrderByDescending(s => s.Owners_Name);
                    break;
                case "manufacturer":
                    vehicles = vehicles.OrderBy(s => s.Manufacturer);
                    break;
                case "manDesc":
                    vehicles = vehicles.OrderByDescending(s => s.Manufacturer);
                    break;
                case "year":
                    vehicles = vehicles.OrderBy(s => s.Year_of_Manufacture);
                    break;
                case "yearDesc":
                    vehicles = vehicles.OrderByDescending(s => s.Year_of_Manufacture);
                    break;
                case "weight":
                    vehicles = vehicles.OrderBy(s => s.Weight);
                    break;
                case "weightDesc":
                    vehicles = vehicles.OrderByDescending(s => s.Weight);
                    break;
                default:
                    vehicles = vehicles.OrderBy(s => s.Owners_Name);
                    break;                 
            }
            return View(await vehicles.AsNoTracking().ToListAsync());
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(m => m.Vehicle_ID == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Vehicle_ID,Owners_Name,Manufacturer,Year_of_Manufacture,Weight")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                var cat = await GetCategory(vehicle.Weight);
                vehicle.Category_ID = cat.Category_ID; 
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Vehicle_ID,Owners_Name,Manufacturer,Year_of_Manufacture,Weight")] Vehicle vehicle)
        {
            if (id != vehicle.Vehicle_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var cat = await GetCategory(vehicle.Weight);
                    vehicle.Category_ID = cat.Category_ID;
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Vehicle_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(m => m.Vehicle_ID == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Retrieve a category icon
        private async Task<IActionResult> GetImage(int vehicle_id)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicle_id);
            try
            {
                int category_id = (int) vehicle.Category_ID;

                Category category = await _context.Categories.FindAsync(category_id);

                var stream = new MemoryStream(category.PhotoFile);

                // Compose a response containing the image and return to the user.
                var result = new HttpResponseMessage();

                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType =
                        new MediaTypeHeaderValue(category.ImageMimeType);

                return File(category.PhotoFile,
                            category.ImageMimeType);

            }
            catch (Exception e)
            {
                return NotFound();
            }            
        }

        //Get vehicle's category 
        public async Task<Category> GetCategory(decimal weight)
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            foreach (var category in categories)
            {
                if (category.Min < weight && weight <= category.Max)
                    return category;
            }
            return null;
        }
        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.Vehicle_ID == id);
        }
    }
}
