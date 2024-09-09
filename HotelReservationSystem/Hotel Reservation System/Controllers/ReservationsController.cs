using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Reservation_System.Models;

namespace Hotel_Reservation_System.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ModelContext _context;

        public ReservationsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Reservations.Include(r => r.Room).Include(r => r.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Reservationid == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create

        // GET: Reservations/Create
        [HttpGet]
        public IActionResult Create(int roomId)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.Roomid == roomId);

            if (room == null)
            {
                return NotFound("Room not found.");
            }

            var reservation = new Reservation
            {
                Roomid = room.Roomid,
                Checkindate = DateTime.Now,
                Checkoutdate = DateTime.Now.AddDays(1),
                Status = "Pending"
            };

            return View(reservation);
        }


        // POST: Reservations/Create
        [HttpPost]
        public IActionResult Create(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Reservations.Add(reservation);
                _context.SaveChanges();
                return RedirectToAction("Payment", new { reservationId = reservation.Reservationid });
            }

            // إذا كان هناك أخطاء في النموذج، أعد عرض النموذج مع الأخطاء
            return View(reservation);
        }












        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            ViewData["Roomid"] = new SelectList(_context.Rooms, "Roomid", "Roomnumber", reservation.Roomid);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Reservationid,Roomid,Checkindate,Checkoutdate,Status")] Reservation reservation)
        {
            if (id != reservation.Reservationid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Reservationid))
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

            ViewData["Roomid"] = new SelectList(_context.Rooms, "Roomid", "Roomnumber", reservation.Roomid);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Reservationid == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Reservations == null)
            {
                return Problem("Entity set 'ModelContext.Reservations' is null.");
            }
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(decimal id)
        {
            return (_context.Reservations?.Any(e => e.Reservationid == id)).GetValueOrDefault();
        }
    }
}