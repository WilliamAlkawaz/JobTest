using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project1.Models;

namespace Project1.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly JobTestDB _context;

        public CategoriesController(JobTestDB context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        //Retrieve a category icon
        private async Task<IActionResult> GetImage(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            var stream = new MemoryStream(category.PhotoFile);

            // Compose a response containing the image and return to the controller
            var result = new HttpResponseMessage();

            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue(category.ImageMimeType);

            return File(category.PhotoFile,
                        category.ImageMimeType);
        }
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Category_ID == id);
        }
    }
}
