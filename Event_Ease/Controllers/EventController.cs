using Event_Ease.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Event_Ease.Controllers
{
    public class EventController : Controller
    {
        // Portions of the code in this file were written with the assistance of GitHub Copilot.
        //My CRUD methods were written with the assistace of lecturer videos
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchType, int? VenueId, DateTime? StartDate, DateTime? endDate)

        {
            var events =  _context.Event
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

           if(!string.IsNullOrEmpty(searchType))
            {
                // Filter events by EventType
                events = events.Where(e => e.EventType.Name == searchType);
            }
           if(VenueId.HasValue)
            {
                // Filter events by VenueId
                events = events.Where(e => e.VenueId == VenueId);
            }

            if (StartDate.HasValue && endDate.HasValue)
            {
                // Filter events by date range
                events = events.Where(e => e.EventDate >= StartDate && e.EventDate <= endDate);
            }

            //This will return the list of events to the view
            ViewData["EventTypeId"] = new SelectList(_context.EventType, "Name", "Name", searchType);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Location", VenueId);

            return View(await events.ToListAsync());
        }


        //this method will return a form to create a new event
        public IActionResult Create()
        {
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Location");
            ViewData["EventTypeId"] = new SelectList(_context.EventType, "EventTypeId", "Name");
           
            return View();
        }


//this method will create a new event
//It will handle how the form data is submitted
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event events)
        {
            if (ModelState.IsValid)   // Check if form data is valid
            {
                _context.Add(events);    // Add the new event to the database    
                await _context.SaveChangesAsync();   // Save the changes
                return RedirectToAction("Index");    // Redirect to the Index action
            }

            // If the form is invalid, re-populate the dropdown list
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", events.VenueId);

            //This ViewData is used to populate the dropdown list for EventTypeId in the form
            //ViewData["EventTypeId"] = new SelectList(_context.EventType, "EventTypeId", "EventTypeName", events.EventTypeId);

            return View(events);   // Return the form back to the user with the data they entered
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.EventId == id);
           
            if (events == null)
            {
                return NotFound();
            }

            return View(events);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @events = await _context.Event.FindAsync(id);

            //In detail this part of the code checks if the event exists in the database.
            if (@events == null) return NotFound();
            
               
           

            var validBooking = await _context.Booking.AnyAsync(b => b.EventId == id);
            if (validBooking)
            {
                TempData["Error"] = "This event cannot be deleted because it has associated bookings.";
                return RedirectToAction(nameof(Index));
            }

            try
            {


                _context.Event.Remove(@events);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Event deleted successfully!";
            }
            catch(Exception ex) 
               
            {
                TempData["Error"] = "An error occurred while deleting the venue: ";
            }

            // Redirect to the Index action after deletion
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Event.FindAsync(id);
            if (events == null)
            {
                return NotFound();
            }
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", events.VenueId);

            //This ViewData is used to populate the dropdown list for EventTypeId in the form
            ViewData["EventTypeId"] = new SelectList(_context.EventType, "EventTypeId", "Name", events.EventTypeId);

            return View(events);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event events)
        {
            if (id != events.EventId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(events);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(events.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", events.VenueId);
            return View(events);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (events == null)
            {
                return NotFound();
            }

            return View(events);
        }

        // This method checks if the event exists in the database by its EventId
        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventId == id);
        }



    }
}
