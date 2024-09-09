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
    public class RoomsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;


        public RoomsController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Rooms.Include(r => r.Hotel);
            return View(await modelContext.ToListAsync());
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Rooms == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(m => m.Roomid == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // GET: Rooms/Create
        public IActionResult Create()
        {
            ViewData["Hotelid"] = new SelectList(_context.Hotels, "Hotelid", "Hotelid");
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Roomid,Roomnumber,Roomtype,Price,Isavailable,Hotelid,ImageFile")] Room room)
        {
            if (ModelState.IsValid)
            {
                if (room.ImageFile != null)
                {
                    // 1- Get the root path
                    string wwwRootPath = _webHostEnviroment.WebRootPath;

                    // 2- Generate a unique filename
                    string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(room.ImageFile.FileName);

                    // 3- Combine paths for the final path
                    string path = Path.Combine(wwwRootPath, "image", fileName);

                    // 4- Upload image to the "image" folder
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await room.ImageFile.CopyToAsync(fileStream);
                    }

                    // Set the image path to be saved in the database
                    room.Imagepath = fileName;
                }

                // Add room to the database
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(room);
        }


        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Rooms == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            ViewData["Hotelid"] = new SelectList(_context.Hotels, "Hotelid", "Hotelid", room.Hotelid);
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Roomid,Roomnumber,Roomtype,Price,Isavailable,Hotelid,Imagepath,ImageFile")] Room room)
        {
            if (id != room.Roomid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // If a new image is uploaded
                    if (room.ImageFile != null)
                    {
                        // 1- Get the root path
                        string wwwRootPath = _webHostEnviroment.WebRootPath;

                        // 2- Generate a unique filename
                        string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(room.ImageFile.FileName);

                        // 3- Combine paths for the final path
                        string path = Path.Combine(wwwRootPath, "image", fileName);

                        // 4- Upload image to the "image" folder
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await room.ImageFile.CopyToAsync(fileStream);
                        }

                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(room.Imagepath))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath, "image", room.Imagepath);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Update the image path to the new file name
                        room.Imagepath = fileName;
                    }

                    // Update room details in the database
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.Roomid))
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

            ViewData["Hotelid"] = new SelectList(_context.Hotels, "Hotelid", "Hotelid", room.Hotelid);
            return View(room);
        }


        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Rooms == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(m => m.Roomid == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Rooms == null)
            {
                return Problem("Entity set 'ModelContext.Rooms'  is null.");
            }
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(decimal id)
        {
            return (_context.Rooms?.Any(e => e.Roomid == id)).GetValueOrDefault();
        }
    }
}