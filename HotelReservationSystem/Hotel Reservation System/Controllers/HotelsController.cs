using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Reservation_System.Models;
using Microsoft.AspNetCore.Hosting;

namespace Hotel_Reservation_System.Controllers
{
    public class HotelsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public HotelsController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }

        // GET: Hotels
        public async Task<IActionResult> Index()
        {
              return _context.Hotels != null ? 
                          View(await _context.Hotels.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Hotels'  is null.");
        }

        // GET: Hotels/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Hotels == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(m => m.Hotelid == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // GET: Hotels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hotels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Hotelid,Name,Location,Numberofrooms,Description,Imagepath,ImageFile")] Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                if (hotel.ImageFile != null)
                {
                    // 1- Get the root path
                    string wwwRootPath = _webHostEnviroment.WebRootPath;

                    // 2- Generate a unique filename
                    string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(hotel.ImageFile.FileName);

                    // 3- Combine paths for the final path
                    string path = Path.Combine(wwwRootPath, "image", fileName);

                    // 4- Upload image to the "image" folder
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await hotel.ImageFile.CopyToAsync(fileStream);
                    }

                    // Set the image path to be saved in the database
                    hotel.Imagepath = fileName;
                }

                // Add hotel to the database
                _context.Add(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(hotel);
        }



        // GET: Hotels/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Hotels == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel);
        }

        // POST: Hotels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Hotelid,Name,Location,Numberofrooms,Description,Imagepath,ImageFile")] Hotel hotel)
        {
            if (id != hotel.Hotelid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (hotel.ImageFile != null)
                    {
                        // 1- Get the root path
                        string wwwRootPath = _webHostEnviroment.WebRootPath;

                        // 2- Generate a unique filename
                        string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(hotel.ImageFile.FileName);

                        // 3- Combine paths for the final path
                        string path = Path.Combine(wwwRootPath, "image", fileName);

                        // 4- Upload image to the "image" folder
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await hotel.ImageFile.CopyToAsync(fileStream);
                        }

                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(hotel.Imagepath))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath, "image", hotel.Imagepath);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Update the image path to the new file name
                        hotel.Imagepath = fileName;
                    }

                    // Update hotel details in the database
                    _context.Update(hotel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Hotels.Any(e => e.Hotelid == id))
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

            return View(hotel);
        }

        // GET: Hotels/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Hotels == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(m => m.Hotelid == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Hotels == null)
            {
                return Problem("Entity set 'ModelContext.Hotels'  is null.");
            }
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel != null)
            {
                _context.Hotels.Remove(hotel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HotelExists(decimal id)
        {
          return (_context.Hotels?.Any(e => e.Hotelid == id)).GetValueOrDefault();
        }
    }
}
