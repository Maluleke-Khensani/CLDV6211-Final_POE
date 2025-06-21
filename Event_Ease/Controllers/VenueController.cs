using System.Net;
using Azure.Storage.Blobs;
using Event_Ease.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class VenueController : Controller
{
    // Portions of the code in this file were written with the assistance of GitHub Copilot.
    //My CRUD methods were written with the assistace of lecturer videos
    private readonly ApplicationDbContext _context;

    public VenueController(ApplicationDbContext context)
    {
        _context = context;
    }

    // This method will return a list of all venues
    public async Task<IActionResult> Index()
    {
        var venues = await _context.Venue.ToListAsync();
        return View(venues);
    }

    // This method will return a form to create a new venue
    public IActionResult Create()
    {
        return View();
    }

   

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Venue venue)
    {
        if (ModelState.IsValid)
        {
            // Handle image upload to Azure Blob Storage if an image is provided
            if(venue.ImageFile != null)
            {
                // Upload the image to Blog Storage and get the URL
                var blobURL = await UploadImageToBlobAsync(venue.ImageFile);//method to upload image to blob storage

                venue.ImageUrl = blobURL; // Set the image URL in the venue object
            }

            _context.Add(venue);
            await _context.SaveChangesAsync();
           
            TempData["Success"] = "Venue created successfully!";
            return RedirectToAction("Index");
        } 
        return View(venue);
    }

    // This method will return a form to delete a venue
    public async Task<IActionResult> Delete(int id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var venue = await _context.Venue
            .FirstOrDefaultAsync(v => v.VenueId == id);
        if (venue == null)
        {
            return NotFound();
        }
        return View(venue);
    }


    // This method will handle the deletion of the venue
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var venue = await _context.Venue.FindAsync(id);
        if (venue == null)
        {
            return NotFound();
        }
        var hasBooking = await _context.Booking.AnyAsync(b => b.VenueId == id);
        
        if (hasBooking)
        {
            TempData["Error"] = "Cannot delete venue with existing bookings.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            // Delete the venue from the database
            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Venue deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "An error occurred while deleting the venue: ";    
        }
        
        return RedirectToAction(nameof(Index));
     }

    // GET: Venue/Edit/{id}
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var venue = await _context.Venue.FindAsync(id);
        if (venue == null)
        {
            return NotFound();
        }
        return View(venue);
    }

    // POST: Venue/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Venue venue)
    {
        if (id != venue.VenueId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existingVenue = await _context.Venue.AsNoTracking().FirstOrDefaultAsync(v => v.VenueId == id);

                if (existingVenue == null)
                    return NotFound();

                if (venue.ImageFile != null)
                {
                    // Upload new image
                    var blobURL = await UploadImageToBlobAsync(venue.ImageFile);
                    venue.ImageUrl = blobURL;
                }
                else
                {
                    // Preserve existing image URL
                    venue.ImageUrl = existingVenue.ImageUrl;
                }

                _context.Update(venue);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Venue updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VenueExists(venue.VenueId))
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
        return View(venue);
    }




    // This method will return the details of a venue
    // '?' means that the parameter is nullable, which means it can be null or have a value of type int.

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();  // it will return a 404 Not Found error if the id is null
        }

        //This will  find the venue with the specified id and include the events associated with the venue
        var venue = await _context.Venue
            .Include(v => v.Events)
            .FirstOrDefaultAsync(v => v.VenueId == id);

        if (venue == null)
        {
            return NotFound();  // it will return a 404 Not Found error if the venue is null
        }  

        return View(venue);
    }

    // Check if the venue exists in the database
    private bool VenueExists(int id)
    {
        return _context.Venue.Any(e => e.VenueId == id);
    }
    // This method will upload the image to Azure Blob Storage and return the URL
    // It will take the image file as a parameter and return the URL of the uploaded image  
    private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
    {
        var connectionString = "connection";
        var containerName = "cldve-container";

        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

        var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
        {
            ContentType = imageFile.ContentType
        };

        using (var stream = imageFile.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
            });
        }

        return blobClient.Uri.ToString();
    }
}
