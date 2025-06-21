using Event_Ease.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

namespace Event_Ease.Controllers
{
    public class BookingController : Controller
    {
        // Portions of the code in this file were written with the assistance of GitHub Copilot.
        //My CRUD methods were written with the assistace of lecturer videos
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        //This method will inclue the event and venue in the booking and return the view
        public async Task<IActionResult> Index( string searchString)
        {
            // Get all bookings and include related Event and Venue data from the database
            var bookings = _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable(); // Make it a query so we can filter it later

            // If the user typed something in the search box, filter the bookings
            if (!string.IsNullOrEmpty(searchString))
            {
                // Search by Booking ID or Event Name
                bookings = bookings.Where(b =>
                    b.BookingId.ToString().Contains(searchString) ||
                    b.Event.EventName.Contains(searchString));
            }

            return View(await bookings.ToListAsync());
        }

        public IActionResult Create()
        {
            // Populate ViewBag with Event and Venue data
            ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName");
            ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "VenueName");

            return View();
        }

            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,EventId,VenueId,BookingDate")] Booking booking)
        {
            ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName");
            ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "VenueName");
            // Get the event linked to this booking
            var selectedEvent = await _context.Event
                    .FirstOrDefaultAsync(e => e.EventId == booking.EventId);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event not found.");
                // Populate ViewBag with Event and Venue data
               return View(booking);
            }

            // Extract the date part only (ignore time)
            //  var eventDateOnly = selectedEvent.EventDate.Date;
            if (ModelState.IsValid)
            {
                // Check for existing bookings on the same date and venue
                var isVenueAlreadyBooked = await _context.Booking

            .AnyAsync(b =>
                b.VenueId == booking.VenueId &&

                b.BookingDate.Date == booking.BookingDate.Date);
            
            // bool isVenueAlreadyBooked = existingBookings.Any(b =>
            //  b.Event != null && b.Event.EventDate.Date == eventDateOnly);

            if (isVenueAlreadyBooked)
                {
                    ModelState.AddModelError("", "This venue is already booked on the selected date.");

                // Repopulate dropdowns
                // Populate ViewBag with Event and Venue data
               // ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName");
               // ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "VenueName");

                return View(booking);
            }

            // booking.BookingDate = DateTime.Now;

            
                
                    _context.Add(booking);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Booking successfully created!";
                    return RedirectToAction(nameof(Index));
               
            }
            // Populate ViewBag with Event and Venue data
            ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName");
            ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "VenueName");

            return View(booking);

        }







        // This method will delete the booking and return the view
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _context.Booking
       .Include(b => b.Event)
       .Include(b => b.Venue)
       .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }


        // The hhtp post is responsible for deleting the booking
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.Booking
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", booking.VenueId);
            ViewBag.CurrentVenueName = booking.Venue?.VenueName;

            return View(booking);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,VenueId,BookingDate")] Booking booking)
        {
           
            if (id != booking.BookingId)
            {
                return NotFound();
            }
               

            //var selectedEvent = await _context.Event.FirstOrDefaultAsync(e => e.EventId == booking.EventId);


            if (ModelState.IsValid)
            {
                

            try
            {
                _context.Update(booking);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(booking.BookingId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction("Index");
        }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }



        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _context.Booking
    .Include(b => b.Event)
    .Include(b => b.Venue)
    .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }


        private bool BookingExists(int id)
        {
            return _context.Booking.Any(b => b.BookingId == id);
        }


    }
}
